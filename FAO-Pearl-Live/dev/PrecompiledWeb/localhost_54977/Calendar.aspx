<%@ page title="" language="VB" masterpagefile="~/USR.master" autoeventwireup="false" inherits="Calendar, App_Web_l4hlb3yz" viewStateEncryptionMode="Always" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
    <script type="text/javascript">
 $(document).ready(function () 
 {
        $('#panel1').slidePanel({
		triggerName: '#trigger1',
		position: 'fixed',
		triggerTopPos: '65px',
		panelTopPos: '95px'
        });
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
 <table width="100%" cellspacing="0px" cellpadding ="0px">
    <tr>
    <td valign="top" align="center" style="width:80%">
<%--<a href="#" id="trigger1" class="trigger left">My Calendar</a>--%>
<div id="panel" >
 <asp:UpdatePanel ID="UpdatePanel1" runat="server">
   <ContentTemplate>
       <asp:Calendar id="Calendar1" runat="server" 
            BackColor="#FFFFCC" BorderColor="#FFCC66" BorderWidth="5px" 
            DayNameFormat="Full" Font-Names="Verdana" Font-Size="8pt" 
            ForeColor="#663399" Height="500px" ShowGridLines="True" Width="100%" 
           NextPrevFormat="FullMonth" >
        <DayHeaderStyle BackColor="#FFCC66" Font-Bold="True" Height="1px" />
        <NextPrevStyle Font-Size="15pt" ForeColor="#FFFFCC" />
        <OtherMonthDayStyle ForeColor="#CC9966" />
        <SelectedDayStyle BackColor="#990000" Font-Bold="True" />
        <SelectorStyle BackColor="#990000" />
        <TitleStyle BackColor="#990000" Font-Bold="True" Font-Size="Medium" 
            ForeColor="#FFFFCC" Height="30px" VerticalAlign="Middle" />
        <TodayDayStyle BackColor="#990000" ForeColor="White" />
        </asp:Calendar>
        </ContentTemplate>
      </asp:UpdatePanel>
   </div>
    </td>
    <td valign="top" align="center" style="width:20%" >
  <%-- <a href="#" id="trigger1" class="trigger right">My TaskList</a>
<div id="panel1" class="panel1 right">--%>
<div>
    <asp:UpdatePanel ID="UpdGrid" runat ="server" UpdateMode ="Conditional" ><ContentTemplate >
    <div style ="background-color :#990000">
    <asp:Label ID="lbtoday" runat ="server" Font-Bold ="true" ForeColor="#FFFFCC"  Text ="" Font-Size ="Medium"   Font-Names ="Verdana"  ></asp:Label>
    </div>
   <asp:GridView ID="grdTasklist" runat="server" AutoGenerateColumns="False" 
             CellPadding="2"  Width="100%"  DataKeyNames ="docid"
              BackColor="LightGoldenrodYellow" BorderColor="Tan" 
                        BorderWidth="1px" ForeColor="Black" GridLines="None">
                    <FooterStyle BackColor="Tan" />
                    <SelectedRowStyle BackColor="DarkSlateBlue" ForeColor="GhostWhite" />
                    <PagerStyle ForeColor="DarkSlateBlue" HorizontalAlign="Center" 
                        BackColor="PaleGoldenrod"  />
                    <HeaderStyle Font-Bold="True" HorizontalAlign ="Left" 
                        CssClass="GridHeader" BackColor="Tan" />
                    <AlternatingRowStyle BackColor="PaleGoldenrod" />
                    <Columns>
                    <asp:TemplateField HeaderText ="Task Name">
                    <ItemTemplate >
                    <a class="detail"  href="#" onclick="ShowDialog('DocDetail.aspx?DOCID=<%# DataBinder.Eval(Container.DataItem, "docid")%>')" ><asp:Label ID="lbTSK" runat="server"  Text ='<%# Bind("taskname") %>'></asp:Label> </a> 
                    </ItemTemplate>
                    <ItemStyle HorizontalAlign ="Left" />
                    </asp:TemplateField>
                        <asp:BoundField DataField="STATUS" HeaderText="STATUS">
                        <ItemStyle HorizontalAlign="Left" />
                        </asp:BoundField>
                     </Columns>
                    <SortedAscendingCellStyle BackColor="#FAFAE7" />
                    <SortedAscendingHeaderStyle BackColor="#DAC09E" />
                    <SortedDescendingCellStyle BackColor="#E1DB9C" />
                    <SortedDescendingHeaderStyle BackColor="#C2A47B" />
                    <SortedAscendingCellStyle BackColor="#EDF6F6" />
                    <SortedAscendingHeaderStyle BackColor="#0D4AC4"  />
                    <SortedDescendingCellStyle BackColor="#D6DFDF" />
                    <SortedDescendingHeaderStyle BackColor="#002876" />
                </asp:GridView>
</ContentTemplate></asp:UpdatePanel>
    </div>
    </td>
    </tr>
    </table> 
</asp:Content>

