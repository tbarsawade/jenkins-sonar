<%@ page title="" language="VB" masterpagefile="~/USR.master" autoeventwireup="false" inherits="dashboard, App_Web_4pl2ohtp" viewStateEncryptionMode="Always" %>

<%@ Register Assembly="System.Web.DataVisualization, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" Namespace="System.Web.UI.DataVisualization.Charting" TagPrefix="asp" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="asp" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
    <%--<script type="text/javascript" src="https://ajax.googleapis.com/ajax/libs/jquery/1.4.4/jquery.min.js"></script>--%>
    <script src="jquery/jquery-3.3.1.js" type="text/javascript"></script>
    <script src="jquery/jquery-3.3.1.min.js" type="text/javascript"></script>
<script type="text/javascript">
    function toggleDiv(divId) {
        $("#" + divId).toggle();
    }
    function HideDiv(divId) {
        $("#" + divId).Hide();
    }
</script>
<script src="http://maps.google.com/maps/api/js?sensor=false" 
          type="text/javascript"></script>
   <script type="text/javascript">

       function OpenWindow(url) {
           var new_window = window.open(url, "List", "scrollbars=yes,resizable=yes,width=800,height=480,location=no");
           //new_window.onbeforeunload = function () { document.getElementById('ContentPlaceHolder1_btnRefresh').onclick(); }
           return false;
       }

    </script>
    <asp:UpdatePanel ID="upData" runat="server">
        <ContentTemplate>
            <table cellpadding="0px" cellspacing="0px" style="text-align: left" width="100%" border="0px">
                <tr>
                    <td>
                        <asp:DataList ID="dlChart" runat="server" OnItemDataBound="Shalini_ItemCommand" RepeatColumns="2" RepeatDirection="Horizontal">

                            <ItemTemplate>
                                <div class="box">
                                    <b><%#Eval("Branch Name")%>
                                        <br />
                                    </b>
                                    <asp:Chart ID="sChart" runat="server" Width="200px" Height="200px">
                                        <Legends>
                                            <asp:Legend Alignment="Center" Docking="Left" IsTextAutoFit="False" Name="Default" LegendStyle="Column" />
                                        </Legends>
                                        <Series>
                                            <asp:Series Name="Series1" ChartType="Pie" ToolTip="#VALX(#PERCENT)"></asp:Series>
                                        </Series>
                                        <ChartAreas>
                                            <asp:ChartArea Name="ChartArea1"></asp:ChartArea>
                                        </ChartAreas>
                                    </asp:Chart>
                                </div>
                            </ItemTemplate>
                         </asp:DataList>
                    </td>
                </tr>
            </table>
            <%--<span><b>Total Number Of Record(s) </b></span>--%>
            <asp:Label runat="server" ID="lblDataCount"></asp:Label>
            <div id="divData" style="width: 998px; border: 0; margin-top: 20px; overflow-x: scroll;">
                <table cellpadding="0px" cellspacing="0px" style="text-align: left" width="100%" border="0px">
                    <tr>
                        <td width="100%">
                            <asp:GridView ID="gvData" runat="server" Width="998px" AllowPaging="false" DataKeyNames="Branch Code" OnRowCreated="gv_OnRowCreated" AutoGenerateColumns="true" CellPadding="2">
                                <FooterStyle CssClass="FooterStyle" />
                                <RowStyle CssClass="RowStyle" HorizontalAlign="Center"  />
                                <EditRowStyle CssClass="EditRowStyle" />
                                <SelectedRowStyle CssClass="SelectedRowStyle" />
                                <PagerStyle CssClass="PagerStyle" />
                                <HeaderStyle CssClass=" HeaderStyle" HorizontalAlign="Center"  />
                                <AlternatingRowStyle CssClass="AlternatingRowStyle" />
                                <EmptyDataRowStyle CssClass="EmptyDataRowStyle" />
                                 <Columns>
                                 <asp:TemplateField HeaderText="Map" ItemStyle-HorizontalAlign="Right"  >
                            <ItemTemplate>
                                   <a class="detail" style="text-decoration:none;"  href="#" onclick="OpenWindow('http://<%#  HttpContext.Current.Request.Url.Authority + HttpContext.Current.Request.ApplicationPath  %>/MapforDashBoard.aspx?branch code=<%#DataBinder.Eval(Container.DataItem, "Branch Code")%>')"   >
                 <img alt="Show On Map" src="images/earth_search.png"  height="16px" width="16px"/></a>                          
                            </ItemTemplate>
                            <ItemStyle Width="60px" HorizontalAlign="center"/>
                        </asp:TemplateField>
                                </Columns>
                            </asp:GridView>
                        </td>
                    </tr>
                </table>
            </div>
        </ContentTemplate>
    </asp:UpdatePanel>
   </asp:Content>

