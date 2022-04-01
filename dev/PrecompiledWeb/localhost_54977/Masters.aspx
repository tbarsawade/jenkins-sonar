<%@ page title="" language="VB" masterpagefile="~/usrFullScreenBPM.master" autoeventwireup="false" inherits="Masters, App_Web_qpjniz5y" viewStateEncryptionMode="Always" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="asp" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
    <script type="text/javascript" src="js_child/jquery.min.cache"></script>
    <script src="js_child/jquery-ui.js" type="text/javascript"></script>
    <link href="js_child/jquery-ui[1].css" rel="stylesheet" type="text/css" />
    <script src="js/Utils.js"></script>
        <link href="https://cdn.jsdelivr.net/npm/select2@4.1.0-beta.1/dist/css/select2.min.css" rel="stylesheet" />
    <script src="https://cdn.jsdelivr.net/npm/select2@4.1.0-beta.1/dist/js/select2.min.js"></script>
    <style type="text/css">
        .mg {
            margin: 10px 0px;
        }
    tr.RowStyle td:not(:last-child)
    {
        white-space:pre;
    }
      .select2-dropdown {
            z-index: 999999 !important;
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
            $("select").not('.invisible').select2({
            });
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

    <%--<script type="text/javascript">
          function ace_itemSelected(sender, e) {
              var test = e.get_value();
              var g1 = sender.get_element()
              var t1 = g1.id;
              var hdnID = t1.replace("fld", "HDN");
              aler(hdnID)
              document.getElementById(hdnID).value = test;
              //            alert(document.getElementById(hdnID).value);
          }
            </script>--%>
    <script type="text/javascript">
        function ace_itemSelected(sender, e) {
            debugger;
            var test = e.get_value();
            var g1 = sender.get_element()
            var t1 = g1.id;
            var hdnID = t1.replace("fld", "HDN");
            document.getElementById(hdnID).value = test;
            //            alert(document.getElementById(hdnID).value);
        }

        function onDataShown(sender, args) {
            sender._popupBehavior._element.style.zIndex = 1000001;
            // sender._popupBehavior._element.style.left = "54px"; //set positions according to your requriment.
            // sender._popupBehavior._element.style.top = "50px"; //set top postion accorind to you requirement.

            //you can either use left,top or right,bottom or any combination u want to set ur divlist.            
        }
    </script>
    <%--<script type="text/javascript">
     jQuery(document).ready(function () {
         jQuery("#form1").validationEngine();
     });
</script>--%>



    <script type="text/javascript">
        function OpenWindow(url) {

            var new_window = window.open(url, "List", "scrollbars=yes,resizable=yes,width=800,height=480");
            //new_window.onbeforeunload = function () { document.getElementById('ContentPlaceHolder1_btnRefresh').onclick(); }
            return false;
        }

        $(document).ready(function () {
            //    $('#panel1').slidePanel({
            //	triggerName: '#trigger1',
            //	position: 'fixed',
            //	triggerTopPos: '65px',
            //	panelTopPos: '95px'
            //});

            //$('.detail').each(function() {
            //    var $link = $(this);
            //    var $dialog = $('<div></div>')
            //		.load($link.attr('href'))
            //		.dialog({
            //		    autoOpen: false,
            //		    title: 'Document Detail',
            //		    width: 700,
            //		    height: 550,
            //		    modal: true
            //		});

            //    $link.click(function() {
            //        $dialog.dialog('open');
            //        return false;
            //    });
            //});
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


    <asp:UpdatePanel ID="updPnlGrid" runat="server" UpdateMode="Conditional">
        <Triggers>
            <asp:PostBackTrigger ControlID="btnexport" />
            <asp:PostBackTrigger ControlID="helpexport" />
            <asp:PostBackTrigger ControlID="btnimport" />

        </Triggers>
        <ContentTemplate>
              <asp:UpdateProgress ID="UpdateProgress1" runat="server">
                            <ProgressTemplate>
                                <div id="Layer1" style="position: absolute; z-index: 999999; left: 50%; top: 50%">
                                    <asp:Image ID="Image1" runat="server" Height="25px" ImageUrl="~/Images/prg.gif" />
                                    please wait...
                                </div>
                            </ProgressTemplate>
                        </asp:UpdateProgress>
                        <div class="container-fluid container-fluid-amend">
                             <div class="form">
                <div class="doc_header">
                    <asp:Label ID="lblCaption" runat="server"></asp:Label>
                </div>
                <div class="row">
                    <div class="col-md-12 col-sm-12 mg" style="text-align: center;">
                        <asp:Label ID="lblMsg" runat="server" Font-Bold="True" ForeColor="Red"
                            Width="100%"></asp:Label>
                        <asp:Label ID="lblRecord" runat="server" ForeColor="red"></asp:Label>
                    </div>
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
                    <div class="col-md-3 col-sm-3">
                        <div class="pull-left" style="border: 1px solid #ccc; padding: 4px 4px;">
                            <asp:ImageButton ID="btnSearch" runat="server" Width="20px" Height="20px"
                                ImageUrl="~/Images/search-icon.png" />
                            <asp:ImageButton ID="btnimmm" ToolTip="Import" runat="server" Width="20px" Height="20px" ImageUrl="~/Images/import-icon.png" />
                        </div>
                    </div>
                    <div class="col-md-1 col-sm-1">
                        <div class="pull-left" style="border: 1px solid #ccc; padding: 4px 4px;">
                            <asp:ImageButton ID="btnNew" runat="server" Width="20px" Height="20px" ImageUrl="~/Images/plus-icon.png" OnClick="Add" />
                            <asp:ImageButton ID="btnchangeView" runat="server" ImageUrl="~/images/Viewmapicon.jpg" CssClass="btnNew" Font-Bold="True" Font-Size="X-Small" Width="100px" Height="17px" Visible="false" />
                            <asp:ImageButton ID="btnexport" ToolTip="Export" runat="server" Width="20px" Height="20px" ImageUrl="~/Images/excelexport.png" />
                        </div>
                    </div>
                </div>
                <div class="row">
                    <div class="col-md-12 col-sm-12">
                        <asp:GridView ID="gvData" runat="server" CssClass="table table-bordered table-hover GridView"
                            CellPadding="2" DataKeyNames="tid" DataSourceID="SqlData"
                            AllowSorting="True" AllowPaging="True" PageSize="15" AutoGenerateColumns="true">
                            <FooterStyle CssClass="FooterStyle" />
                            <RowStyle CssClass="RowStyle" />
                            <EditRowStyle CssClass="EmptyDataRowStyle" />
                            <SelectedRowStyle CssClass="SelectedRowStyle" />
                            <PagerStyle CssClass="PagerStyle" />
                            <HeaderStyle CssClass="HeaderStyle" />
                            <AlternatingRowStyle CssClass="AlternatingRowStyle" />
                            <Columns>

                             

                                <asp:TemplateField HeaderText="Action">
                                    <ItemTemplate>

                                        <a class="detail" style="text-decoration: none;" href="#" onclick="OpenWindow('DocDetailMaster.aspx?DOCID=<%# DataBinder.Eval(Container.DataItem, "tid")%>')">
                                            <img alt="Detail" title="SeeAll" src="images/seeall.png" />
                                        </a>

                                        &nbsp;
           
                                <asp:ImageButton ID="btnEdit" runat="server" ImageUrl="~/images/edit1.png" Height="16px" Width="16px" OnClick="EditHit" title="Edit" AlternateText="Edit" />
                                        &nbsp;
                                <asp:ImageButton ID="btnDtl" ImageUrl="~/images/lock.png" runat="server" Height="16px" Width="16px" OnClick="LockHit" title="Lock / Unlock" ToolTip="Lock / Unlock" AlternateText="Lock / Unlock" />
                                        <%--<asp:ImageButton ID="btnDelete" runat="server" ImageUrl="~/images/Cancel.gif" Height="16px" Width="16px" OnClick="DeleteHit" AlternateText="Delete"/>--%>
                                    </ItemTemplate>
                                    <ItemStyle Width="120px" HorizontalAlign="Center" />
                                </asp:TemplateField>
                            </Columns>
                        </asp:GridView>
                        <asp:SqlDataSource ID="SqlData" runat="server"
                            ConnectionString="<%$ ConnectionStrings:conStr %>"
                            SelectCommand="uspGetGridByMasterType1New_USINGROLEASSIGNMENT" SelectCommandType="StoredProcedure">
                            <SelectParameters>
                                <asp:ControlParameter ControlID="ddlField" Name="sField"
                                    PropertyName="SelectedValue" Type="String" />
                               
                                <asp:SessionParameter DefaultValue="%" Name="sValue" SessionField="Value" 
                                    Type="String" />
                                <asp:SessionParameter DefaultValue="0" Name="eid" SessionField="EID"
                                    Type="Int32" />
                                <asp:QueryStringParameter DefaultValue="" Name="documentType"
                                    QueryStringField="SC" Type="String" />
                                <asp:SessionParameter DefaultValue="" Name="MASTERQRY" SessionField="MASTERQRY"
                                    Type="String" />

                            </SelectParameters>
                        </asp:SqlDataSource>
                        <asp:GridView ID="gvexport" Style="display: none;" runat="server" CssClass="GridView"
                            CellPadding="2" DataKeyNames="tid"
                            ForeColor="#333333" Width="100%"
                            AllowSorting="False" AllowPaging="False" DataSourceID="SqlData">
                            <FooterStyle CssClass="FooterStyle" />
                            <RowStyle CssClass="RowStyle" />
                            <EditRowStyle CssClass="EmptyDataRowStyle" />
                            <SelectedRowStyle CssClass="SelectedRowStyle" />
                            <PagerStyle CssClass="PagerStyle" />
                            <HeaderStyle CssClass="HeaderStyle" />
                            <AlternatingRowStyle CssClass="AlternatingRowStyle" />
                            <Columns>
                            </Columns>
                        </asp:GridView>
                    </div>
                </div>
            </div>
                        </div>
         <%--   <table cellpadding="0px" cellspacing="0px" style="text-align: left" width="100%" border="0px">--%>
                <%--<tr style="color: #000000">
                    <td style="text-align: left;"></td>
                </tr>

                <tr>
                    <td style="text-align: center;"></td>
                </tr>--%>

<%--                <tr style="color: #000000">
                    <td style="text-align: left; width: 100%; border: 3px double green">--%>
                        <%--   <table cellpadding="0px" cellspacing="3px" style="text-align: left" width="100%"
                            border="0px">
                            <tr>
                                <td style="width: 90px;">
                                   
                                </td>

                                <td style="width: 170px;"></td>

                                <td style="width: 50px;">
                                    
                                </td>

                                <td style="width: 200px;"></td>

                                <td style="text-align: right; width: 25px"></td>

                                <td style="text-align: right;">&nbsp;
           
                                </td>


                            </tr>
                        </table>--%>
              
<%--                    </td>
                </tr>--%>

              <%--  <tr style="color: #000000">
                    <td style="text-align: left" valign="top">
                        <div style="overflow: auto; width: 1000px">
                        </div>
                    </td>
                </tr>--%>
         <%--   </table>--%>
        </ContentTemplate>
    </asp:UpdatePanel>
    <asp:Button ID="btnstatus" runat="server" Style="display: none" />
    <asp:ModalPopupExtender ID="modalstatus" runat="server"
        TargetControlID="btnstatus" PopupControlID="pnlstatus"
        CancelControlID="ImageButton1" BackgroundCssClass="modalBackground"
        DropShadow="true">
    </asp:ModalPopupExtender>

    <asp:Panel ID="pnlstatus" runat="server" Width="500px" Height="300px" Style="display: none;" BackColor="aqua">
        <div class="box">
            <table cellspacing="2px" cellpadding="2px" width="100%" border="1px">
                <tr>
                    <td>
                        <h3>
                            <asp:Label ID="lblstatus" runat="server" Text="Data Uploading Status" Font-Bold="True"></asp:Label></h3>
                    </td>
                    <td style="width: 20px">
                        <asp:ImageButton ID="ImageButton1"
                            ImageUrl="images/close.png" runat="server" /></td>
                </tr>
                <tr style="border: 1px solid black;">
                    <td>
                        <asp:Label ID="lblstat" runat="server" Font-Bold="True"></asp:Label></td>
                </tr>
                <tr style="border: 1px solid black;">
                    <td>
                        <asp:Label ID="Label2" runat="server" Font-Bold="True"></asp:Label></td>
                </tr>
                <tr>
                    <td>
                        <table border="1px">
                            <tr>
                                <td width="217"><span style="color: Black; font-weight: bold;">Row No</span></td>
                                <td width="217px"><span style="color: Black; font-weight: bold;">Column Where Error Occured</span></td>
                            </tr>
                        </table>
                    </td>
                </tr>
                <tr style="border: 1px solid black;">
                    <td>
                        <div style="overflow: scroll; height: 300px">
                            <asp:Label ID="lblstatus1" runat="server" />
                        </div>
                    </td>
                </tr>
            </table>
        </div>
    </asp:Panel>

    <asp:Button ID="btnShowPopupEdit" runat="server" Style="display: none" />
    <asp:ModalPopupExtender ID="btnEdit_ModalPopupExtender" runat="server"
        TargetControlID="btnShowPopupEdit" PopupControlID="pnlPopupEdit"
        CancelControlID="btnCloseEdit" BackgroundCssClass="modalBackground"
        DropShadow="true">
    </asp:ModalPopupExtender>
    <asp:Panel ID="pnlPopupEdit" runat="server" Width="900px" Style="display: none" BackColor="aqua">
        <div class="box">
            <table cellspacing="2px" cellpadding="2px" width="100%">
                <tr>
                    <td>
                        <asp:UpdatePanel ID="updPanalHeader" runat="server" UpdateMode="Conditional">
                            <ContentTemplate>
                                <h3>
                                    <asp:Label ID="lblHeaderPopUp" runat="server" Font-Bold="True"></asp:Label></h3>
                            </ContentTemplate>
                        </asp:UpdatePanel>
                    </td>
                    <td style="width: 20px">
                        <asp:ImageButton ID="btnCloseEdit" ImageUrl="images/close.png" runat="server" OnClick="popUpClose" /></td>
                </tr>
                <tr>
                    <td colspan="2">
                        <asp:UpdatePanel ID="updatePanelEdit" runat="server" UpdateMode="Conditional">
                            <%--<Triggers > <asp:PostBackTrigger ControlID ="btnActEdit" /></Triggers>--%>
                            <ContentTemplate>
                                <asp:Panel ID="pnlPoupup" Width="900px" runat="server">
                                    <asp:Label ID="lblTab" runat="server" Font-Bold="True" ForeColor="Red"></asp:Label>
                                </asp:Panel>
                                <asp:Panel ID="pnlFields" Width="900px" Height="500px" runat="server" Style="overflow: scroll;">
                                </asp:Panel>
                                <div style="width: 100%; text-align: right">
                                    <asp:Button ID="Button1" runat="server" Text="Calculate" Visible="false" CssClass="btnNew" Font-Bold="True" Font-Size="X-Small" Width="100px" OnClick="CalculateFormulaP" />
                                    <asp:Button ID="btnActEdit" runat="server" Text="Save"
                                        OnClick="EditRecord" CssClass="btnNew" Font-Bold="True" OnClientClick="this.disabled=true;" UseSubmitBehavior="false"
                                        Font-Size="X-Small" Width="100px" />
                                </div>
                            </ContentTemplate>
                        </asp:UpdatePanel>
                    </td>
                </tr>
            </table>
        </div>
    </asp:Panel>


    <asp:Button ID="btnShowPopupDelete" runat="server" Style="display: none" />
    <asp:ModalPopupExtender ID="btnDelete_ModalPopupExtender" runat="server"
        TargetControlID="btnShowPopupDelete" PopupControlID="pnlPopupDelete"
        CancelControlID="btnCloseDelete" BackgroundCssClass="modalBackground"
        DropShadow="true">
    </asp:ModalPopupExtender>
    <asp:Panel ID="pnlPopupDelete" runat="server" Width="500px" Style="display: none" BackColor="Aqua">
        <div class="box">
            <table cellspacing="0px" cellpadding="0px" width="100%">
                <tr>
                    <td>
                        <h3>Project Delete : Confirmation</h3>
                    </td>
                    <td style="width: 20px">
                        <asp:ImageButton ID="btnCloseDelete" ImageUrl="images/close.png" runat="server" /></td>
                </tr>
                <tr>
                    <td colspan="2">
                        <asp:UpdatePanel ID="updatePanelDelete" runat="server" UpdateMode="Conditional">
                            <ContentTemplate>
                                <h2>
                                    <asp:Label ID="lblMsgDelete" runat="server" Font-Bold="True" ForeColor="Red"
                                        Width="97%" Font-Size="X-Small"></asp:Label></h2>
                                <div style="width: 100%; text-align: right">
                                    <asp:Button ID="btnActDelete" runat="server" Text="Yes Delete" Width="90px" OnClientClick="this.disabled=true;" UseSubmitBehavior="false"
                                        OnClick="DeleteRecord" CssClass="btnNew" Font-Size="X-Small" />
                                </div>
                            </ContentTemplate>
                        </asp:UpdatePanel>
                    </td>
                </tr>
            </table>
        </div>
    </asp:Panel>


    <style id="cssStyle" type="text/css" media="all">
        .CS {
            background-color: white;
            color: #99ae46;
            border: 1px solid #99ae46;
            font: Verdana 10px;
            padding: 1px 4px;
            font-family: Palatino Linotype, Arial, Helvetica, sans-serif;
        }
    </style>
    <asp:Button ID="btnim" runat="server" Style="display: none;" />
    <asp:ModalPopupExtender ID="modalpopupimport" runat="server"
        TargetControlID="btnim" PopupControlID="pnlimport"
        CancelControlID="ImageButton2" BackgroundCssClass="modalBackground"
        DropShadow="true">
    </asp:ModalPopupExtender>

    <asp:Panel ID="pnlimport" runat="server" Width="400px" Height="100px" Style="display: none;" BackColor="white">
        <div class="box">
            <table cellspacing="1px" cellpadding="2px" width="100%" border="0px">
                <tr>
                    <td>
                        <h3>
                            <asp:Label ID="lblpophead" runat="server" Text="Import Csv File" Font-Bold="True"></asp:Label></h3>
                    </td>
                    <td style="width: 20px">
                        <asp:ImageButton ID="ImageButton2"
                            ImageUrl="images/close.png" runat="server" /></td>
                </tr>

                <tr>
                    <td colspan="2">
                        <br />
                        <asp:FileUpload ID="impfile" CssClass="CS" runat="server" />
                        &nbsp;
             <asp:ImageButton ID="btnimport" ToolTip="Import" Style="vertical-align: bottom;" runat="server" Width="20px" Height="20px" ImageUrl="~/Images/import.png" />&nbsp;
              <asp:ImageButton ID="helpexport" ToolTip="Download Import Sample & Format" runat="server" Style="vertical-align: bottom;" Width="20px" Height="20px" ImageUrl="~/Images/Helpexp.png" />
                        &nbsp;</td>

                </tr>
            </table>
        </div>
    </asp:Panel>

    <asp:Button ID="btnLock" runat="server" Style="display: none" />
    <asp:ModalPopupExtender ID="ModalPopup_Lock" runat="server"
        TargetControlID="btnLock" PopupControlID="pnlPopupLock"
        CancelControlID="btnCloseLock" BackgroundCssClass="modalBackground"
        DropShadow="true">
    </asp:ModalPopupExtender>
    <asp:Panel ID="pnlPopupLock" runat="server" Width="500px" Style="display: none" BackColor="Aqua">
        <div class="box">
            <table cellspacing="0px" cellpadding="0px" width="100%">
                <tr>
                    <td>
                        <h3>Lock / Unlock : Confirmation</h3>
                    </td>
                    <td style="width: 20px">
                        <asp:ImageButton ID="btnCloseLock" ImageUrl="images/close.png" runat="server" /></td>
                </tr>
                <tr>
                    <td colspan="2">
                        <asp:UpdatePanel ID="updLock" runat="server" UpdateMode="Conditional">
                            <ContentTemplate>
                                <asp:Label ID="lblLock" runat="server" Font-Bold="True" ForeColor="Red"
                                    Width="440px" Font-Size="X-Small"></asp:Label>
                                <div style="width: 100%; text-align: right">
                                    <asp:Button ID="btnLockupdate" runat="server" Text="Yes Lock" Width="90px" OnClientClick="this.disabled=true;" UseSubmitBehavior="false"
                                        OnClick="LockRecord" CssClass="btnNew" Font-Size="X-Small" />
                                </div>
                            </ContentTemplate>
                        </asp:UpdatePanel>
                    </td>
                </tr>
            </table>
        </div>
    </asp:Panel>


    <asp:Button ID="Btnchild" runat="server" Style="display: none" />
    <asp:ModalPopupExtender ID="ModalPopupExtender1" runat="server"
        TargetControlID="Btnchild" PopupControlID="pnlPopupchild"
        CancelControlID="btnClose" BackgroundCssClass="modalBackground"
        DropShadow="true">
    </asp:ModalPopupExtender>

    <asp:Panel ID="pnlPopupchild" runat="server" Width="700px" Style="display: none;" Height="420px" BackColor="aqua">
        <div class="box" style="overflow: scroll;">
            <table cellspacing="2px" cellpadding="2px" width="100%">
                <tr>
                    <td>
                        <h3>
                            <asp:Label ID="Label3" runat="server" Font-Bold="True"></asp:Label></h3>
                    </td>
                    <td style="width: 20px">
                        <asp:ImageButton ID="btnClose" ImageUrl="images/close.png" runat="server" /></td>
                </tr>
                <tr>
                    <td colspan="2">
                        <asp:UpdatePanel ID="updpnlchild" runat="server" UpdateMode="Conditional">
                            <ContentTemplate>
                                <asp:Panel ID="Pnllable" runat="server">
                                    <asp:Label ID="Label4" runat="server" Font-Bold="True" ForeColor="Red"></asp:Label>
                                </asp:Panel>
                                <asp:Panel ID="pnlFields1" Width="100%" ScrollBars="auto" runat="server">
                                </asp:Panel>
                                <div style="width: 100%; text-align: right;">

                                    <asp:Button ID="Button2" runat="server" Text="Save" CssClass="btnNew" Font-Bold="True" OnClick="EditItem" Font-Size="X-Small" Width="100px" OnClientClick="this.disabled=true;" UseSubmitBehavior="false" />

                                </div>
                            </ContentTemplate>
                        </asp:UpdatePanel>
                    </td>
                </tr>
            </table>
        </div>
    </asp:Panel>


</asp:Content>
