<%@ page title="" language="VB" masterpagefile="~/usrFullScreenBPM.master" autoeventwireup="false" inherits="UniversalSearch, App_Web_gsdfcjye" enableeventvalidation="true" viewStateEncryptionMode="Always" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="asp" %>


<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">

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
            <asp:PostBackTrigger ControlID="btnAdvanceSearch" />
            <asp:PostBackTrigger ControlID="btnDynamicSearch" />
            <asp:PostBackTrigger ControlID="btnexportxl" />
        </Triggers>
        <ContentTemplate>
        <div class="container-fluid container-fluid-amend">
            <div class="form" style="text-align: left">
                <div class="doc_header">
                    Document Search               
                </div>
                <div class="row mg">
                    <div class="col-md-2 col-sm-2 mg">
                        <label>Document Type</label>
                    </div>
                    <div class="col-md-3 col-sm-3">
                        <asp:DropDownList ID="ddlDocType" CssClass="txtBox" runat="server" AutoPostBack="true">
                        </asp:DropDownList>
                    </div>
                    <div class="col-md-2 col-sm-2 mg">
                        <label>Search By DocID</label>
                    </div>
                    <div class="col-md-2 col-sm-2">
                        <asp:TextBox ID="txtSearch" runat="server" CssClass="txtBox"></asp:TextBox>
                    </div>
                    <div class="col-md-3 col-sm-3">
                        <asp:ImageButton ID="btnSearch" runat="server" Height="25px" ImageUrl="~/Images/search1.png" ToolTip="Search  " />
                        <asp:ImageButton ID="btnAdvanceSearch" runat="server" Height="25px" ImageUrl="~/Images/Asearch.png" ToolTip="Advance Search  " />
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
                            <Columns>
                                <asp:TemplateField HeaderText="Document Detail">
                                    <ItemTemplate>
                                        <a class="detail" style="text-decoration: none;" href="#" onclick="OpenWindow('DocDetail.aspx?DOCID=<%# DataBinder.Eval(Container.DataItem, "tid")%>')">
                                            <%#DataBinder.Eval(Container.DataItem, "tid")%> </a>
                                    </ItemTemplate>
                                    <ItemStyle Width="120px" HorizontalAlign="Center" />
                                </asp:TemplateField>
                            </Columns>
                            <RowStyle BackColor="#F7F6F3" ForeColor="#333333" HorizontalAlign="Center" VerticalAlign="Middle" />
                        </asp:GridView>
                    </div>
                </div>
            </div>
       </div>
            <%-- <div class="gradientBoxesWithOuterShadows" style="margin-top: 10px">--%>
            <%--   <table>
                    &nbsp;&nbsp;
    <tr>
        <td style="text-align: right; width: 150px">
            <asp:Label ID="lblDocType" runat="server" Font-Bold="True" Text="Document Type"></asp:Label></td>
        <td></td>



        <td style="text-align: left; width: 120px">&nbsp;
        <asp:Label ID="lblSearch" runat="server" Font-Bold="True" Text="Search By DocID"></asp:Label></td>

        <td></td>
        &nbsp;&nbsp;   &nbsp;&nbsp;
       <td style="width: 10px"></td>
        <td></td>
        &nbsp;&nbsp;
       
               <td></td>

        <td width="30px"></td>
        <td></td>



    </tr>
                </table>--%>
            <%-- <div align="center">
            </div>--%>
            <%--   <div align="center">--%>

            <%--<asp:SqlDataSource ID="SqlData" runat="server" 
               ConnectionString="<%$ ConnectionStrings:conStr %>" 
               SelectCommand="uspGetSearchDocType" SelectCommandType="StoredProcedure">
               <SelectParameters>
                      <asp:ControlParameter ControlID="txtSearch" Name="val" PropertyName="Text" DefaultValue="%"  
                       Type="String" />
                        <asp:ControlParameter ControlID="ddlDocType" Name="documentType" 
                       PropertyName="SelectedValue" Type="String" />
                   <asp:SessionParameter DefaultValue="0" Name="eid" SessionField="EID" 
                       Type="Int32" />
                         <asp:SessionParameter DefaultValue="0" Name="UID" SessionField="UID" 
                       Type="Int32" />
                   </SelectParameters>
           </asp:SqlDataSource>--%>
            <%--<asp:GridView ID="gvexport" style="display:none;" runat="server" CssClass="GridView" 
             CellPadding="2" DataKeyNames="tid"
                    ForeColor="#333333" Width="100%"  
               AllowSorting="False" AllowPaging="False" DataSourceID="SqlData">
                    <FooterStyle  CssClass="FooterStyle" />
                    <RowStyle CssClass="RowStyle"/>
                    <EditRowStyle  CssClass="EmptyDataRowStyle" />
                    <SelectedRowStyle CssClass="SelectedRowStyle" />
                    <PagerStyle CssClass="PagerStyle" />
                    <HeaderStyle CssClass="HeaderStyle"/>
                    <AlternatingRowStyle CssClass="AlternatingRowStyle" />
                    <Columns>
                      </Columns>
                </asp:GridView>--%>
            <%--     <table cellspacing="0px" cellpadding="0px" width="100%" border="0px">
                    <tr>
                        <td style="width: 34%; text-align: center;">
                            <asp:Label ID="lbltotpending" runat="server" Style="text-align: center; color: #598526;"></asp:Label>
                        </td>
                    </tr>
                </table>
            </div>--%>
            <%--  </div>--%>
        </ContentTemplate>
    </asp:UpdatePanel>
    <asp:Button ID="btnShowPopupForm" runat="server" Style="display: none" />
    <asp:ModalPopupExtender ID="btnForm_ModalPopupExtender" runat="server" PopupControlID="pnlFields" TargetControlID="btnShowPopupForm"
        CancelControlID="btnCloseForm" BackgroundCssClass="modalBackground">
    </asp:ModalPopupExtender>
    <%--<div id="main" >--%>

    <asp:Panel ID="pnlFields" runat="server" Width="750px" Height="430px" BackColor="White" Style="display: none; overflow: scroll">

        <div class="doc_header">
            Advance Search   
            <div class="pull-right">
                <asp:ImageButton ID="btnCloseForm" ImageUrl="images/close.png" runat="server" />
            </div>
        </div>
        <div class="row" style="text-align: center;">
            <div class="col-md-12 col-sm-12">
                &nbsp;
            </div>
            <asp:ImageButton ID="btnDynamicSearch" runat="server" Height="25px" ImageUrl="~/Images/Asearch.png" ToolTip="Search  " />
            <div class="col-md-12 col-sm-12">
                &nbsp;
            </div>
        </div>
        <%--  <table cellspacing="0px" cellpadding="0px" width="100%">
            <tr>
                <td>

                    <asp:UpdatePanel ID="UpdatePanel2" runat="server">
                        <ContentTemplate>
                            <table cellspacing="5px" cellpadding="0px" width="100%" border="0px">
                                <tr>
                                    <td style="width: 100%">
                                        <asp:Label ID="lblForm" runat="server" Text=""></asp:Label>
                                    </td>
                                </tr>



                            </table>

                        </ContentTemplate>
                    </asp:UpdatePanel>

                </td>
            </tr>
        </table>--%>


        <%-- <div style="margin-left: 600px">
          

        </div>--%>
    </asp:Panel>

    <%--<asp:GridView ID="GrdASearch" EmptyDataText="Record does not exists." AllowSorting="true"     ShowFooter="false" AllowPaging ="true"   runat="server" AutoGenerateColumns="true" 
             CellPadding="3" Width="100%"    
                             
             PageSize="15"   BorderStyle="none"  BorderColor="Green" 
                        BorderWidth="1px" Font-Size="Small"  >
                    <RowStyle BackColor="White" CssClass="gridrowhome"  Height="25px" BorderColor="Green" BorderWidth="1px" ForeColor="Black" />
                    <SelectedRowStyle BackColor="green" Font-Bold="True" ForeColor="White"  />
                    <PagerStyle ForeColor="green" HorizontalAlign="Center"  />
                    <HeaderStyle Font-Bold="True"    BorderColor="Green"  BorderWidth="1px"  
                            Height="25px" ForeColor="black" 
                        CssClass="gridheaderhome" BackColor="#d0d0d0" HorizontalAlign="Center"  />
                        <EmptyDataRowStyle CssClass="EmptyDataRowStyle" />
                    <Columns>
                           <asp:TemplateField HeaderText="Document Detail">
                            <ItemTemplate>
                                   <a class="detail" style=" text-decoration:none;"  href="#"  onclick="OpenWindow('DocDetail.aspx?DOCID=<%# DataBinder.Eval(Container.DataItem, "tid")%>')" >
              <%#DataBinder.Eval(Container.DataItem, "tid")%> </a>    
                             </ItemTemplate >
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
                    <RowStyle BackColor="#F7F6F3" ForeColor="#333333" HorizontalAlign="Center" VerticalAlign="Middle" />

                </asp:GridView>--%>
</asp:Content>

