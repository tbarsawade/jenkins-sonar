<%@ page title="" language="VB" masterpagefile="~/USR.master" autoeventwireup="false" inherits="MapSettings, App_Web_4ysr50e5" viewStateEncryptionMode="Always" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="asp" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
    <style type="text/css">
        .scroll {
            max-height:400px;
            overflow-y:scroll;
            width:100%;
        }
        .top {
            width: 97%;
            margin: auto;
            height:auto;
            padding: 5px;
        }

        .ddlstyle
        {
            width: 200px;
        }
        option
        {
            padding-left: 30px;
            font-size: 14px;
            height:28px;
        }
         .red {
            background:#f59797;
            border:1px solid #e63c3c;
            border-radius:5px;
            padding:7px;
            color:#000;
            margin-top:15px;
        }
        .green {
            background:#a5f1a9;
            border:1px solid #43ea4c;
            border-radius:5px;
            padding:7px;
            color:#000;
            margin-top:15px;
        }

    </style>

    <style type="text/css">

        .Grid {background-color: #fff; margin: 5px 0 10px 0; border: solid 1px #525252; border-collapse:collapse; font-family:Calibri; color: #474747;}

.Grid td {

      padding: 2px;
      border: solid 1px #c1c1c1; }

.Grid th  {

      padding : 4px 2px;

      color: #fff;

      background: #525252;

      border-left: solid 1px #525252;
      border-right: solid 1px #525252;
      border-top: solid 1px #525252;

      font-size: 0.9em; }

.Grid .alt {

      background: #fcfcfc url(Images/grid-alt.png) repeat-x top; }

.Grid .pgr {background: #363670 url(Images/grid-pgr.png) repeat-x top; }

.Grid .pgr table { margin: 3px 0; }

.Grid .pgr td { border-width: 0; padding: 0 6px; border-left: solid 1px #666; font-weight: bold; color: #fff; line-height: 12px; }  

.Grid .pgr a { color: Gray; text-decoration: none; }

.Grid .pgr a:hover { color: #000; text-decoration: none; }

        .expand:focus {
            height:100px;
            width:250px;
            z-index:20000 !important;
        }

    </style>

    <script type="text/javascript">

        function ChangeImage(ddl)
        {
            var ddlval = $(ddl).val();
            var ddl = $(ddl).next().attr("src", ddlval);
        }

        var ddlval1;
        function iterate()
        {
            $('#<%=grdEdit.ClientID%>').find('tr').each(function (row) {
                $(this).find('select,img')
                   .each(function (col) {
                      
                       $ctl = $(this);
                       if ($ctl.is('select')) {
                           ddlval1 = $(this).val();
                       }
                       else {
                           $(this).attr("src", ddlval1);
                       }
                   });
            });
        }


        function iterate1() {
            $('#<%=GridView1.ClientID%>').find('tr').each(function (row) {
                $(this).find('select,img')
                   .each(function (col) {

                       $ctl = $(this);
                       if ($ctl.is('select')) {
                           ddlval1 = $(this).val();
                       }
                       else {
                           $(this).attr("src", ddlval1);
                       }
                   });
            });
        }


        function pageLoad(sender, args) {
            if (args.get_isPartialLoad()) {
                iterate1();
                iterate();
            }
        };

    </script>

    <div class="top">
        <asp:TabContainer ID="TabContainer1" runat="server"
            Height="500px"
            Width="100%"
            ActiveTabIndex="0"
            OnDemand="true"
            AutoPostBack="false"
            TabStripPlacement="Top"
            ScrollBars="None"
            VerticalStripWidth="120px">
            <asp:TabPanel ID="TabPanel1" runat="server"
                HeaderText="Group Map SETTINGS"
                Enabled="true"
                ScrollBars="Auto"
                OnDemandMode="Once">
                <ContentTemplate>
                    <div style="padding:0px;">
                    
                    <div style="margin-bottom: 15px;">
                        <table>
                            <tr>
                                <td>Group Name<span style="color:red;">*</span> :</td>
                                <td>
                                    <asp:TextBox ID="txtgroup" Width="150px" runat="server" ></asp:TextBox></td>
                                <td>&nbsp;</td>
                            </tr>
                        </table>
                    </div>
                    <div style="margin-bottom: 15px;">
                        <div class="scroll">
                        <asp:GridView ID="GridView1" CssClass="Grid" AutoGenerateColumns="False" DataKeyNames="Tid" runat="server" Width="100%">
                            <Columns>
                                <asp:TemplateField HeaderText="Documents">
                                    <ItemTemplate>
                                        <asp:CheckBox ID="chk1" runat="server"  />
                                        <asp:Label ID="lbl1" runat="server" Text=' <%#Eval("FormName")%>'></asp:Label>
                                    </ItemTemplate>
                                    <ItemStyle HorizontalAlign="Left" VerticalAlign="Middle" Width="250px" />
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Select Icon">
                                    <ItemTemplate>
                                        <asp:DropDownList ID="ddlIcon" runat="server" Width="200px" onchange="ChangeImage(this);"> </asp:DropDownList>
                                        <img src="images/MapIcons/marker1.png" />
                                    </ItemTemplate>
                                    <ItemStyle HorizontalAlign="Center" VerticalAlign="Middle" Width="240px" />
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Info Type"  >
                                    <ItemTemplate>
                                        <asp:CheckBox ID="CheckBox1" runat="server" AutoPostBack="true" OnCheckedChanged="CheckBox1_CheckedChanged" /> <br/> Custom Query
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Info">
                                    <ItemTemplate>
                                        <div style="height:100px; overflow-x:scroll;">
                                        <asp:CheckBoxList ID="chkList" Width="95%" runat="server" ></asp:CheckBoxList>
                                        <asp:TextBox ID="txtQuery" Visible="false" BorderStyle="Dashed" BorderColor="#999999"  TextMode="multiline" Columns="10" Height="85%" Width="95%" Rows="5"  class="expand" runat="server"></asp:TextBox>
                                    </div>
                                            </ItemTemplate>
                                </asp:TemplateField>
                            </Columns>

                        </asp:GridView>
                            </div>
                    </div>
                    <div>
                        <asp:Button ID="btnSaveSettings" CssClass="btnNew" runat="server" Text="Submit" />
                    </div>
                        <div>
                            <div id="divMsg" runat="server" visible="False" style="width:60%;">
                             </div>
                        </div>
                  </div>

                </ContentTemplate>

            </asp:TabPanel>


            <asp:TabPanel ID="TabPanel2" runat="server"
                HeaderText="Edit Group Map SETTINGS"
                Enabled="true"
                ScrollBars="Auto"
                OnDemandMode="Once">
                <ContentTemplate>
                    <div style="padding:0px;">
                        <div style="margin-bottom: 15px;">
                        <table>
                            <tr>
                                <td>Group Name<span style="color:red;">*</span> :</td>
                                <td>
                                    <asp:DropDownList ID="ddlGroup" Width="150px" AutoPostBack="True" runat="server"></asp:DropDownList>
                                    </td>
                                <td></td>
                            </tr>
                        </table>
                    </div>
                     
                    <div id ="divGrid" runat="server" style="margin-bottom: 15px;">
                        <div class="scroll">
                        <asp:GridView ID="grdEdit" CssClass="Grid" AutoGenerateColumns="False" DataKeyNames="Tid" runat="server" Width="100%" EmptyDataText="No data found.">
                            <Columns>
                                <asp:TemplateField HeaderText="Documents">
                                    <ItemTemplate>
                                        <asp:CheckBox ID="chk1" runat="server"  Checked='<%#Eval("Checked")%>' />
                                        <asp:Label ID="lbl1" runat="server" Text=' <%#Eval("FormName")%>'></asp:Label>
                                    </ItemTemplate>
                                    <ItemStyle HorizontalAlign="Left" VerticalAlign="Middle" Width="250px" />
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Select Icon">
                                    <ItemTemplate>
                                        <asp:DropDownList ID="ddlIcon" runat="server" Width="200px" onchange="ChangeImage(this);"> </asp:DropDownList>
                                    <img src="images/MapIcons/marker1.png" />
                                    </ItemTemplate>
                                    <ItemStyle HorizontalAlign="Center" VerticalAlign="Middle" Width="240px" />
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Select Icon" Visible="False">
                                    <ItemTemplate>
                                        <asp:Label ID="lblicon" runat="server" Text=' <%#Eval("IconName")%>'></asp:Label>
                                    </ItemTemplate>
                                    <ItemStyle HorizontalAlign="Center" VerticalAlign="Middle" Width="175px" />
                                </asp:TemplateField>

                                <asp:TemplateField HeaderText="Info Type"  >
                                    <ItemTemplate>
                                        <asp:CheckBox ID="CheckBox1" runat="server" AutoPostBack="true" OnCheckedChanged="CheckBox1_CheckedChanged1" /> <br/> Custom Query
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Info">
                                    <ItemTemplate>
                                        <div style="height:100px; overflow-x:scroll;">
                                        <asp:CheckBoxList ID="chkList" Width="95%" runat="server" ></asp:CheckBoxList>
                                        <asp:TextBox ID="txtQuery" BorderStyle="Dashed" BorderColor="#999999" Visible="false" TextMode="multiline" Columns="10" Height="85%" Width="95%" Rows="5"  class="expand" runat="server"></asp:TextBox>
                                    </div>
                                            </ItemTemplate>
                                </asp:TemplateField>

                            </Columns>
                        </asp:GridView>
                            </div>

                        <div style="margin-top:15px;">
                        <asp:Button ID="btnSaveEdit" CssClass="btnNew" runat="server" Text="Submit" />
                    </div>
                        <div>
                            <div id="DivMsg1" runat="server" visible="False" style="width:60%;">
                             </div>
                        </div>

                    </div>
                        </div>
                </ContentTemplate>

            </asp:TabPanel>

        </asp:TabContainer>
    </div>

    <asp:UpdateProgress ID="UpdateProgress1" runat="server">
        <ProgressTemplate>
            
            <div id="Layer1" style="position: absolute; z-index: 9999999; left: 50%; top: 50%">
                <asp:Image ID="Image1" runat="server" Height="25px" ImageUrl="~/Images/prg.gif" />
                please wait...
            </div>
        </ProgressTemplate>
    </asp:UpdateProgress>
</asp:Content>


