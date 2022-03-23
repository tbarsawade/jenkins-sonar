<%@ page title="" language="VB" masterpagefile="~/usrFullScreenBPM.master" autoeventwireup="false" inherits="UserMaster, App_Web_gsdfcjye" enableeventvalidation="false" viewStateEncryptionMode="Always" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="asp" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
    <script type="text/javascript" src="js_child/jquery.min.cache"></script>
    <script src="js_child/jquery-ui.js" type="text/javascript"></script>
    <link href="js_child/jquery-ui[1].css" rel="stylesheet" type="text/css" />
    <style type="text/css">
        #ContentPlaceHolder1_pnlFields .form {
            min-height: 0px !important;
        }
    </style>
    <script type="text/javascript">
        var selTab;
        $(function () {
            var tabs = $("#tabs").tabs({
                show: function () {

                    //get the selected tab index  
                    selTab = $('#tabs').tabs('option', 'selected');

                }
            });

        });;
        $(function () {
            $(".btnDyn").button()
        });

        function pageLoad(sender, args) {

            if (args.get_isPartialLoad()) {
                $("#tabs").tabs({
                    show: function () {

                        //get the selected tab index on partial postback  
                        selTab = $('#tabs').tabs('option', 'selected');
                    }, selected: selTab
                });

                $(function () {
                    $(".btnDyn").button()
                });
            }

        };

    </script>
    <asp:UpdatePanel ID="updPnlGrid" runat="server" UpdateMode="Conditional">
        <Triggers>
            <asp:PostBackTrigger ControlID="btnActEdit" />
            <asp:PostBackTrigger ControlID="btnexport" />
        </Triggers>
        <ContentTemplate>
            <div class="container-fluid">
                <div class="form">
                    <div class="doc_header">
                        User Master
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
                                ImageUrl="~/Images/search.png" />
                        </div>
                        <div class="col-md-2 col-sm-2">
                            <asp:Label ID="lblRecord" runat="server" style="text-align: right;font-weight: lighter !important;    font-size: 14px;    line-height: 1.8;" ></asp:Label>
                            </div>
                        <div class="col-md-1 col-sm-1" style="text-align: right;">
                            
                            <asp:ImageButton ID="btnNew" runat="server" Width="20px" Height="20px" ImageUrl="~/Images/plus.jpg"
                                ToolTip="Add User" OnClick="Add" />
                            &nbsp;<asp:ImageButton ID="btnexport" runat="server" ToolTip="Export" ImageUrl="~/Images/excel.gif" />
                            
                        </div>
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
                            <asp:GridView ID="gvData" runat="server" AutoGenerateColumns="false" CellPadding="2"
                                DataKeyNames="uid" ForeColor="#333333" Width="100%" AllowSorting="True" AllowPaging="True"
                                DataSourceID="SqlData">
                                <FooterStyle BackColor="#507CD1" Font-Bold="True" ForeColor="White" />
                                <RowStyle BackColor="#EFF3FB" />
                                <EditRowStyle BackColor="#2461BF" />
                                <SelectedRowStyle BackColor="#D1DDF1" Font-Bold="True" ForeColor="#333333" />
                                <PagerStyle BackColor="CornflowerBlue" ForeColor="White" HorizontalAlign="Center" />
                                <HeaderStyle BackColor="CornflowerBlue" Font-Bold="True" ForeColor="White" />
                                <FooterStyle Height="20px" Font-Size="12px" /> 
                                <PagerStyle Font-Size="20px" Font-Bold="true" ForeColor="Black"/> 
                               
                               <PagerSettings Mode="NumericFirstLast" PageButtonCount="8" FirstPageText="First" LastPageText="Last"/>  
                               
                                <AlternatingRowStyle BackColor="White" />
                                <Columns>
                                    <asp:TemplateField HeaderText="S.No">
                                        <ItemTemplate>
                                            <%--<%# CType(Container, GridViewRow).RowIndex + 1%>--%>
                                            &nbsp;&nbsp; <%# Container.DataItemIndex + 1 %>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:BoundField DataField="Username" HeaderText="User Name">
                                        <HeaderStyle HorizontalAlign="Left" />
                                    </asp:BoundField>
                                    <asp:BoundField DataField="emailid" HeaderText="Email ID">
                                        <HeaderStyle HorizontalAlign="Left" />
                                    </asp:BoundField>
                                    <asp:BoundField DataField="userid" HeaderText="User ID">
                                        <HeaderStyle HorizontalAlign="Left" />
                                    </asp:BoundField>
                                    <%--<asp:BoundField DataField="weeklyHoliDay" HeaderText="Weekly Off">
                                        <HeaderStyle HorizontalAlign="Left" />
                                    </asp:BoundField>
                                    <asp:BoundField DataField="WorkingStartTime" HeaderText="From">
                                        <HeaderStyle HorizontalAlign="Left" />
                                    </asp:BoundField>
                                    <asp:BoundField DataField="WorkingEndTime" HeaderText="To">
                                        <HeaderStyle HorizontalAlign="Left" />
                                    </asp:BoundField>
                                    <asp:BoundField DataField="Locationname" Visible="false" HeaderText="Location">
                                        <HeaderStyle HorizontalAlign="Left" />
                                    </asp:BoundField>--%>
                                    <asp:BoundField DataField="status" HeaderText="Status">
                                        <HeaderStyle HorizontalAlign="Left" />
                                    </asp:BoundField>
                                    <asp:BoundField DataField="userrole" HeaderText="User Role">
                                        <HeaderStyle HorizontalAlign="Left" />
                                    </asp:BoundField>

                                    <asp:BoundField DataField="CurrentRoles" HeaderText="Addl. Roles">
                                        <HeaderStyle HorizontalAlign="Left" />
                                    </asp:BoundField>
                                    <asp:BoundField DataField="uid" HeaderText="UID" ItemStyle-Width="0px">
                                        <HeaderStyle HorizontalAlign="Left" />
                                    </asp:BoundField>
                                     <asp:BoundField DataField="CreatedBy" HeaderText="Created By" ItemStyle-Width="0px">
                                        <HeaderStyle HorizontalAlign="Left" />
                                    </asp:BoundField>

                                    <asp:TemplateField HeaderText="Action">
                                        <ItemTemplate>
                                            <asp:ImageButton ID="btnDtl" ImageUrl="~/images/lock.png" runat="server" Height="16px"
                                                Width="16px" OnClick="LockHit" ToolTip="Lock / Unlock User" AlternateText="Lock / Unlock" />
                                            &nbsp;
                                        <asp:ImageButton ID="btnEdit" runat="server" ImageUrl="~/images/edit.png" Height="16px"
                                            Width="16px" OnClick="EditHit" ToolTip="Edit User" AlternateText="Edit" />
                                            &nbsp;
                                        <asp:ImageButton ID="btnRole" runat="server" ImageUrl="~/images/add_group.png" Height="16px"
                                            Width="16px" OnClick="AssignRole" ToolTip="Additional Role Assignment" AlternateText="Additional Role Assignment" />
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
                            <asp:SqlDataSource ID="SqlData" runat="server" ConnectionString="<%$ ConnectionStrings:conStr %>"
                                SelectCommand="uspGetResultUsers" SelectCommandType="StoredProcedure">
                                <SelectParameters>
                                    <asp:ControlParameter ControlID="ddlField" Name="sField" PropertyName="SelectedValue"
                                        Type="String" />
                                    <asp:ControlParameter ControlID="txtValue" Name="sValue" PropertyName="Text" DefaultValue="%"
                                        Type="String" />
                                    <asp:SessionParameter Name="eid" SessionField="EID" Type="String" />
                                </SelectParameters>
                            </asp:SqlDataSource>
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
    <asp:Panel ID="pnlPopupEdit" runat="server" Width="1000px" BackColor="#FFFFFF" Height="500px"
        Style="overflow: scroll">
        <div class="box">
            <div class="container-fluid">
                <div class="doc_header">
                    New / Update Users
                    <div style="float: right;">
                        <asp:ImageButton ID="btnCloseEdit" CssClass="PopUpCloseBtn" AlternateText="close button" runat="server" ImageUrl="images/close.png"  />
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
                        <table width="100%" cellspacing="5px" border="0" cellpadding="0">
                            <tr>
                                <td style="width: 130px; text-align: left">
                                    <label>User Name *</label>

                                </td>
                                <td style="width: 240px; text-align: left">
                                    <asp:TextBox ID="txtProductName" CssClass="txtBox" runat="server" Width="230px"></asp:TextBox>
                                </td>
                                <td style="width: 130px; text-align: left">
                                    <label>Email ID *</label>

                                </td>
                                <td style="width: 240px; text-align: left">
                                    <asp:TextBox ID="txtProductDesc" CssClass="txtBox" runat="server" Width="230px"></asp:TextBox>
                                </td>
                            </tr>
                            <tr>
                                <td style="width: 130px; text-align: left">
                                    <label>User Role *</label>

                                </td>
                                <td style="width: 240px; text-align: left">
                                    <asp:DropDownList ID="ddluserrole" runat="server" CssClass="txtBox" Width="230px" >
                                    </asp:DropDownList>
                                </td>
                                <td style="width: 130px; text-align: left">
                              <%--      <label>OFF</label>--%>
                             <%--    <asp:CheckBoxList ID="chkDocumentType"  runat="server" Visible="false" CssClass="txtBox" Height="0px">
                                </asp:CheckBoxList>--%>
                                </td>
                                <td style="width: 240px; text-align: left">
                                    <div style="width: 230px">
                              <%--          <asp:CheckBox ID="Mon" runat="server" Text="Monday" value="1" />
                                        <asp:CheckBox ID="Tus" runat="server" Text="Tuesday" value="2" />
                                        <asp:CheckBox ID="Wed" runat="server" Text="Wednesday" value="3" />
                                        <asp:CheckBox ID="Thus" runat="server" Text="Thursday" value="4" />
                                        <asp:CheckBox ID="Fri" runat="server" Text="Friday" value="5" />
                                        <asp:CheckBox ID="Sat" runat="server" Text="Saturday" value="6" />
                                        <asp:CheckBox ID="Sun" runat="server" Text="Sunday" value="7" />--%>
                                    </div>

                                </td>
                            </tr>
                            <tr>
                                <td style="width: 130px; text-align: left">
                              <%--      <label>From</label>--%>
                                    
                                </td>
                                <td style="width: 240px; text-align: left">
                              <%--      <asp:DropDownList ID="ddlFrom" runat="server" CssClass="txtBox" Width="230px">
                                    </asp:DropDownList>--%>
                                </td>
                                <td style="width: 130px; text-align: left">
                              <%--      <label>To</label>--%>

                                </td>
                                <td style="width: 240px; text-align: left">
                              <%--      <asp:DropDownList ID="ddlTo" runat="server" CssClass="txtBox" Width="230px">
                                    </asp:DropDownList>--%>
                                </td>
                            </tr>
                        </table>
                        <%--  <div class="row">

                            <div class="col-md-2 col-sm-2">
                                <label>User Name *</label>
                            </div>
                            <div class="col-md-4 col-sm-4">
                                <asp:TextBox ID="txtProductName" CssClass="txtBox" runat="server"></asp:TextBox>
                            </div>
                            <div class="col-md-2 col-sm-2">
                                <label>Email ID *</label>
                            </div>
                            <div class="col-md-4 col-sm-4">
                                <asp:TextBox ID="txtProductDesc" CssClass="txtBox" runat="server" Width="98%"></asp:TextBox>
                            </div>

                        </div>
                        <div class="row">
                            <div class="col-md-2 col-sm-2">
                                <label>User Role *</label>
                            </div>
                            <div class="col-md-4 col-sm-4">
                                <asp:DropDownList ID="ddluserrole" runat="server" CssClass="txtBox">
                                </asp:DropDownList>
                            </div>
                            <div class="col-md-2 col-sm-2">
                                <label>OFF </label>
                            </div>
                            <div class="col-md-4 col-sm-4">
                                <asp:CheckBox ID="Mon" runat="server" Text="Monday" value="1" />
                                <asp:CheckBox ID="Tus" runat="server" Text="Tuesday" value="2" />
                                <asp:CheckBox ID="Wed" runat="server" Text="Wednesday" value="3" />
                                <asp:CheckBox ID="Thus" runat="server" Text="Thursday" value="4" />
                                <asp:CheckBox ID="Fri" runat="server" Text="Friday" value="5" />
                                <asp:CheckBox ID="Sat" runat="server" Text="Saturday" value="6" />
                                <asp:CheckBox ID="Sun" runat="server" Text="Sunday" value="7" />
                            </div>
                        </div>
                        <div class="row">
                            <div class="col-md-2 col-sm-2">
                                <label>From</label>
                            </div>
                            <div class="col-md-4 col-sm-4">
                                <asp:DropDownList ID="ddlFrom" runat="server" CssClass="txtBox">
                                </asp:DropDownList>
                            </div>
                            <div class="col-md-2 col-sm-2">
                                <label>To</label>
                            </div>
                            <div class="col-md-4 col-sm-4">
                                <asp:DropDownList ID="ddlTo" runat="server" CssClass="txtBox">
                                </asp:DropDownList>
                            </div>
                        </div>--%>
                        <div class="row">
                            <asp:Panel ID="pnlFields" Width="100%" runat="server">
                            </asp:Panel>
                        </div>
                        <div class="row">
                            <div class="col-md-12 col-sm-12" style="text-align:right;">
                                <asp:Button ID="btnActEdit" runat="server" Text="Update" OnClick="EditRecord" CssClass="btnNew"
                                    Font-Bold="True" Font-Size="X-Small" />
                            </div>
                        </div>
                    </ContentTemplate>
                </asp:UpdatePanel>

            </div>
        </div>
    </asp:Panel>
    <%--lock and unlock--%>
    <asp:Button ID="btnShowPopuplock" runat="server" Style="display: none" />
    <asp:ModalPopupExtender ID="btnlock_ModalPopupExtender" runat="server" TargetControlID="btnShowPopuplock"
        PopupControlID="pnlPopuplock" CancelControlID="btnCloselock" BackgroundCssClass="modalBackground"
        DropShadow="true">
    </asp:ModalPopupExtender>
    <asp:Panel ID="pnlPopupLock" runat="server" Width="500px" Style="display: none" BackColor="Aqua">
        <div class="box">
            <table cellspacing="0px" cellpadding="0px" width="100%">
                <tr>
                    <td>
                        <h3>User : Lock / Unlock</h3>
                    </td>
                    <td style="width: 20px">
                        <asp:ImageButton ID="btnCloseLock" ImageUrl="images/close.png" runat="server" />
                    </td>
                </tr>
                <tr>
                    <td colspan="2">
                        <asp:UpdatePanel ID="updatePanelLock" runat="server" UpdateMode="Conditional">
                            <ContentTemplate>
                                <h2>
                                    <asp:Label ID="lblMsgLock" runat="server" Font-Bold="True" ForeColor="Red" Width="97%"
                                        Font-Size="X-Small"></asp:Label></h2>
                                <div style="width: 100%; text-align: right">
                                    <asp:Button ID="btnActLock" runat="server" Text="Yes Proceed" Width="90px" OnClick="LockUser"
                                        CssClass="btnNew" Font-Size="X-Small" />
                                </div>
                            </ContentTemplate>
                        </asp:UpdatePanel>
                    </td>
                </tr>
            </table>
        </div>
    </asp:Panel>
    <%--Excel uploader --%>
    <%--<asp:Button id="btnuploadexcel" runat="server" style="display:none" />
<asp:ModalPopupExtender ID="MPExcel" runat="server" 
                                TargetControlID="btnuploadexcel" PopupControlID="pnlPopupexcel" 
                CancelControlID="btnCloseexcel" BackgroundCssClass="modalBackground" 
                                DropShadow="true">
</asp:ModalPopupExtender>
<asp:Panel ID="pnlPopupexcel" runat="server" Width="500px" style="display:none " BackColor="Aqua" >
<div class="box">
<table cellspacing="0px" cellpadding="0px" width="100%">
<tr>
<td><h3>Data : Upload Records</h3></td>
<td style="width:20px"><asp:ImageButton ID="btnCloseexcel" ImageUrl="images/close.png" runat="server"/></td>
</tr>
<tr>
<td colspan="2">
<asp:UpdatePanel ID="updpnlexcel" runat="server" UpdateMode="Conditional">
   <ContentTemplate> 
<h2> <asp:Label ID="lblexc" runat="server" Font-Bold="True" ForeColor="Red" 
        Width="97%" Font-Size="X-Small" ></asp:Label></h2>
       
       <div>
           <asp:FileUpload ID="FileUpload1" runat="server" /> </div>
       <div style="width:100%;text-align:right" >

       
                <asp:Button ID="Button2" runat="server" Text="Yes Proceed"  Width="90px" 
                    OnClick="LockUser" CssClass="btnNew" Font-Size="X-Small" />
       </div> 
   </ContentTemplate> 
   </asp:UpdatePanel> 
</td>
</tr>
</table> 
</div>
</asp:Panel>

    --%>
    <asp:Button ID="Btnchild" runat="server" Style="display: none" />
    <asp:ModalPopupExtender ID="ModalPopupExtender1" runat="server" TargetControlID="Btnchild"
        PopupControlID="pnlPopupchild" CancelControlID="btnClose" BackgroundCssClass="modalBackground"
        DropShadow="true">
    </asp:ModalPopupExtender>
    <asp:Panel ID="pnlPopupchild" runat="server" Width="800px" Height="300px" ScrollBars="Auto"
        BackColor="aqua">
        <div class="box">
            <table cellspacing="2px" cellpadding="2px" width="100%">
                <tr>
                    <td>
                        <h3>
                            <asp:Label ID="Label2" runat="server" Font-Bold="True"></asp:Label></h3>
                    </td>
                    <td style="width: 20px">
                        <asp:ImageButton ID="btnClose" ImageUrl="images/close.png" runat="server" />
                    </td>
                </tr>
                <tr>
                    <td colspan="2">
                        <asp:UpdatePanel ID="updpnlchild" runat="server" UpdateMode="Conditional">
                            <ContentTemplate>
                                <asp:ImageButton ID="btnimmm" align="right" ToolTip="Import" runat="server" Visible="false"
                                    Height="22px" Width="90px" ImageUrl="~/Images/upload.png" />
                                <asp:Panel ID="Pnllable" runat="server">
                                    <asp:Label ID="lblTab1" runat="server" Font-Bold="true" ForeColor="Red" Font-Size="Small"></asp:Label>
                                    <asp:Label ID="Label3" runat="server" Font-Bold="True" ForeColor="Red"></asp:Label>
                                </asp:Panel>
                                <asp:Panel ID="pnlFields1" Width="100%" runat="server">
                                </asp:Panel>
                                <div style="width: 100%; text-align: right">
                                    <asp:Button ID="Button2" runat="server" Text="Save" CssClass="btnNew" Font-Bold="True"
                                        OnClick="EditItem" Font-Size="X-Small" Width="100px" OnClientClick="this.disabled=true;"
                                        UseSubmitBehavior="false" />
                                </div>
                            </ContentTemplate>
                        </asp:UpdatePanel>
                    </td>
                </tr>
            </table>
        </div>
    </asp:Panel>
    <asp:Button ID="btnShowPopupRole" runat="server" Style="display: none" />
    <asp:ModalPopupExtender ID="btnRole_ModalPopupExtender" runat="server" TargetControlID="btnShowPopupRole"
        PopupControlID="pnlPopupRole" CancelControlID="btnCloseRole" BackgroundCssClass="modalBackground"
        DropShadow="true">
    </asp:ModalPopupExtender>
    <asp:Panel ID="pnlPopupRole" runat="server" Width="500px" Style="" BackColor="aqua">
        <div class="box">
            <table cellspacing="2px" cellpadding="2px" width="100%">
                <tr>
                    <td style="width: 580px">
                        <h3>Additional Role Assignment</h3>
                    </td>
                    <td style="width: 20px">
                        <asp:ImageButton ID="btnCloseRole" ImageUrl="images/close.png" runat="server" ToolTip="Close"  OnClientClick="document.location.reload(true)" />
                    </td>
                </tr>
                <tr>
                    <td colspan="2">
                        <div class="form">
                            <asp:UpdatePanel ID="updPnlRole" runat="server" UpdateMode="Conditional">
                                <ContentTemplate>
                                    <table cellspacing="4px" cellpadding="0px" width="100%" border="0px">
                                        <tr>
                                            <td colspan="2" align="left">
                                                <asp:Label ID="lblMessRole" runat="server" Font-Bold="True" ForeColor="Red" Width="100%"
                                                    Font-Size="X-Small"></asp:Label>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td align="left" style="width: 110px; height: 25px;">
                                                <label>
                                                    User Name</label>
                                            </td>
                                            <td align="left" style="height: 25px">
                                                <asp:DropDownList ID="ddlUserList" runat="server" Width="85%" CssClass="Inputform">
                                                </asp:DropDownList>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td align="left" style="width: 110px; height: 25px;">
                                                <label>
                                                    Role Type</label>
                                            </td>
                                            <td align="left" style="height: 25px">                                                
                                                <asp:DropDownList ID="ddlRoleType" AutoPostBack="true" runat="server" Width="85%" CssClass="Inputform">
                                                   <asp:ListItem Text="Select Type" Selected="true" Value="Select Type" ></asp:ListItem>  
                                                     <asp:ListItem Text="Post Type" Value="Post Type" ></asp:ListItem>  
                                                    <asp:ListItem Text="Pre Type" Value="Pre Type" ></asp:ListItem>                                                      
                                                </asp:DropDownList>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td align="left" style="width: 110px">
                                                <label>
                                                    Role Name</label>
                                            </td>
                                            <td align="left">
                                                <asp:DropDownList ID="ddlUserRoleName" runat="server" Width="85%" CssClass="Inputform">
                                                </asp:DropDownList>
                                            </td>
                                            <td align="left">
                                                <asp:Button ID="btnActUserSave" runat="server" CssClass="btnNew" Font-Bold="True"
                                                    Font-Size="X-Small" Text="Save" Width="70px" />
                                            </td>
                                            <tr>
                                                <td>
                                                    <asp:UpdateProgress ID="UpdateProgress2" runat="server" AssociatedUpdatePanelID="updPnlRole">
                                                        <ProgressTemplate>
                                                            <div id="Layer1" style="position: absolute; z-index: 1; left: 50%; top: 50%">
                                                                <asp:Image ID="imguserRole" runat="server" Height="25px" ImageUrl="~/Images/prg.gif" />
                                                                please wait...
                                                            </div>
                                                        </ProgressTemplate>
                                                    </asp:UpdateProgress>
                                                </td>
                                            </tr>
                                    </table>
                                    <table>
                                        <tr>
                                            <td style="text-align: left;" valign="top" colspan="2">
                                                <asp:GridView ID="gvDataRole" runat="server" AutoGenerateColumns="false" CellPadding="2"
                                                    DataKeyNames="tid" ForeColor="#333333" Width="100%" AllowSorting="True" AllowPaging="True">
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
                                                        <asp:BoundField DataField="Username" HeaderText="User Name">
                                                            <HeaderStyle HorizontalAlign="Left" />
                                                        </asp:BoundField>
                                                        <asp:BoundField DataField="RoleType" HeaderText="Role Type">
                                                            <HeaderStyle HorizontalAlign="Left" />
                                                        </asp:BoundField>
                                                        <asp:BoundField DataField="RoleName" HeaderText="Role Name">
                                                            <HeaderStyle HorizontalAlign="Left" />
                                                        </asp:BoundField>
                                                        <asp:TemplateField HeaderText="Action">
                                                            <ItemTemplate>
                                                                <asp:ImageButton ID="btnRoleDelete" ImageUrl="~/images/closered.png" runat="server"
                                                                    Height="16px" Width="16px" OnClick="UserRoleDeleteHit" ToolTip="Delete Record"
                                                                    AlternateText="Delete User Role" />
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
                                    </table>
                                </ContentTemplate>
                            </asp:UpdatePanel>
                        </div>
                    </td>
                </tr>
            </table>
        </div>
    </asp:Panel>
    <asp:Button ID="btnDelroleID" runat="server" Style="display: none" />
    <asp:ModalPopupExtender ID="MP_DelRole" runat="server" TargetControlID="btnDelroleID"
        PopupControlID="pnlPopupUserRole" CancelControlID="closeUserRole" BackgroundCssClass="modalBackground"
        DropShadow="true">
    </asp:ModalPopupExtender>
    <asp:Panel ID="pnlPopupUserRole" runat="server" Width="500px" Style="display: none"
        BackColor="Aqua">
        <div class="box">
            <table cellspacing="0px" cellpadding="0px" width="100%">
                <tr>
                    <td>
                        <h3>Confirmation to Delete User Role</h3>
                    </td>
                    <td style="width: 20px">
                        <asp:ImageButton ID="closeUserRole" ImageUrl="images/close.png" runat="server" />
                    </td>
                </tr>
                <tr>
                    <td colspan="2">
                        <asp:UpdatePanel ID="updDelRole" runat="server" UpdateMode="Conditional">
                            <ContentTemplate>
                                <h2>
                                    <asp:Label ID="lblMessuserRole" runat="server" Font-Bold="True" ForeColor="Red" Width="97%"
                                        Font-Size="X-Small"></asp:Label></h2>
                                <div style="width: 100%; text-align: right">
                                    <asp:Button ID="btnDeluserRole" runat="server" Text="Yes Proceed" Width="90px" OnClick="delUsrRole"
                                        CssClass="btnNew" Font-Size="X-Small" />
                                </div>
                                <asp:UpdateProgress ID="UpdateProgress3" runat="server" AssociatedUpdatePanelID="updDelRole">
                                    <ProgressTemplate>
                                        <div id="Layer1" style="position: absolute; z-index: 1; left: 50%; top: 50%">
                                            <asp:Image ID="imgDelRole" runat="server" Height="25px" ImageUrl="~/Images/prg.gif" />
                                            please wait...
                                        </div>
                                    </ProgressTemplate>
                                </asp:UpdateProgress>
                            </ContentTemplate>
                        </asp:UpdatePanel>
                    </td>
                </tr>
            </table>
        </div>
    </asp:Panel>
</asp:Content>
