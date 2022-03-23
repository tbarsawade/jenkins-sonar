<%@ Page Title="" Language="VB" MasterPageFile="~/USR.master" AutoEventWireup="false" CodeFile="Elogbook_Settings.aspx.vb" Inherits="Elogbook_Settings" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="asp" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
     <link href="kendu/styles/kendo.common.min.css" rel="stylesheet" />
    <link href="kendu/styles/kendo.rtl.min.css" rel="stylesheet" />
    <link href="kendu/styles/kendo.default.min.css" rel="stylesheet" />
    <link href="kendu/styles/kendo.dataviz.min.css" rel="stylesheet" />
    <link href="kendu/styles/kendo.dataviz.default.min.css" rel="stylesheet" />
    <script src="kendu/js/jquery.min.js"></script>
    <script src="kendu/js/kendo.all.min.js"></script>
    <script src="kendu/content/shared/js/console.js"></script>
    
 <style>
      .divloader
        {
           position: absolute;
background-color: #FAFAFA;
z-index: 2147483647 !important;
opacity: 0.8;
overflow: hidden;
text-align: center; top: 0; left: 0;
height: 100%;
width: 100%;
padding-top:20%;
          
     
        }
   .AjaxLoaderInner
{
    position: absolute;
    top: 45%;
    left: 45%;
    font-size: 11px;
    font-family: Verdana, Arial, Helvetica, sans-serif;
    font-weight: bold;
    color: Black;
}
.AjaxLoaderOuter
{
    position: fixed;
    vertical-align: middle;
    text-align: center;
    z-index: 1000;
    top: 0px;
    left: 0px;
    background-color: Gray;
    filter: alpha(opacity=70);
    opacity: 0.7;
    margin: 0px 0px 0px 0px;
    width: 100%;
    height: 100%;
    min-height: 100%;
    min-width: 100%;    
}
 </style>
    <h3>   Add/Update E-Logbook Settings </h3>
    <div ><asp:UpdatePanel ID="upPnl" runat="server">
        <ContentTemplate>
         
        <table style="width: 100%;">
            <tr>
                <td align="center" colspan="4">
                    <asp:Label ID="lblMsg" runat="server" CssClass="k-error-colored" Font-Bold="True"></asp:Label>
                </td>
            </tr>
            <tr>
                <td colspan="4">
                    <fieldset><legend><b>Chart Settings</b></legend>
                    <div > 
                    <table style="width:100%;">
                        <tr>
                            <td align="center" colspan="2" width="33%">Chart Vehicle</td>
                            <td align="center" colspan="2" width="33%">Chart Site</td>
                            <td align="center" colspan="2" width="34%">Kms Chart</td>
                        </tr>
                        <tr>
                            <td align="right" style="height: 22px" width="13%">Is Visisble : </td>
                            <td style="height: 22px" width="20%">
                                <asp:CheckBox ID="chkVChartVis" runat="server" />
                            </td>
                            <td align="right" style="height: 22px" width="13%">Is Visible :</td>
                            <td style="height: 22px" width="20%">
                                <asp:CheckBox ID="chkSChartVis" runat="server" />
                            </td>
                            <td align="right" style="height: 22px" width="14%">Is Visible :</td>
                            <td style="height: 22px" width="20%">
                                <asp:CheckBox ID="chkKChartVis" runat="server" />
                            </td>
                        </tr>
                        <tr>
                            <td align="right" style="height: 31px" width="13%">Display Text* :</td>
                            <td style="height: 31px" width="20%">
                                <asp:TextBox ID="txtVChartNm" runat="server" CssClass="k-textbox"></asp:TextBox>
                            </td>
                            <td align="right" style="height: 31px" width="13%">Display Text* :</td>
                            <td style="height: 31px" width="20%">
                                <asp:TextBox ID="txtSChartNm" runat="server" CssClass="k-textbox"></asp:TextBox>
                            </td>
                            <td align="right" style="height: 31px" width="14%">Display Text* :</td>
                            <td style="height: 31px" width="20%">
                                <asp:TextBox ID="txtKChartNm" runat="server" CssClass="k-textbox"></asp:TextBox>
                            </td>
                        </tr>
                        <tr>
                            <td align="center" colspan="6" style="height: 31px">
                                <asp:Label ID="lblChartErr" runat="server" CssClass="k-error-colored" Font-Bold="True"></asp:Label>
                            </td>
                        </tr>
                    </table>
                        </div>
                        </fieldset>
                   </td>
            </tr>
            <tr>
                <td>&nbsp;</td>
                <td>&nbsp;</td>
                <td>&nbsp;</td>
                <td>&nbsp;</td>
            </tr>
            <tr>
                <td colspan="4">
               <fieldset><legend><b>Grid Settings</b></legend>     <div>
                    <table style="width:100%;">
                        <tr>
                            <td width="20%" align="right" colspan="4">
                                <table style="width:100%;">
                                    <tr>
                                        <td width="20%">Select Elogbook Fields to be displayed* :</td>
                                        <td width="20%"><div style="height:200px;overflow:scroll;" align="left">
                                            -<asp:CheckBoxList ID="chkLELogbook" runat="server" CssClass="k-checkbox" AutoPostBack="True">
                                    </asp:CheckBoxList>
                                </div></td>
                                        <td width="10%" align="center">
                                            <asp:Label ID="Label2" runat="server" Text="Add Fields" Font-Bold="True"></asp:Label>
                                            <br />
                                            <asp:ImageButton ID="btnAddElogbookFlds" runat="server" ImageUrl="~/images/arrowhr.png" ToolTip="ADD" style="height: 15px" />
                                            
                                        </td>
                                        <td width="50%">
                                            <asp:GridView ID="gvElogbookflds" runat="server" CssClass="k-grid" Width="100%" AutoGenerateColumns="False" 
                                                EmptyDataText="No record added." ShowHeaderWhenEmpty="True" DataKeyNames="Tid,Sequence">
                                                <Columns>
                                                    <asp:BoundField DataField="SNO" HeaderText="S No.">
                                                    <HeaderStyle HorizontalAlign="Left" />
                                                    <ItemStyle HorizontalAlign="Left" />
                                                    </asp:BoundField>
                                                    <asp:BoundField DataField="FieldNm" HeaderText="Field Name" >
                                                    <HeaderStyle HorizontalAlign="Center" />
                                                    <ItemStyle HorizontalAlign="Left" />
                                                    </asp:BoundField>
                                                    <asp:TemplateField HeaderText="Display Text">
                                                       
                                                        <ItemTemplate>
                                                            <asp:TextBox ID="txtlogbookdis" runat="server" Text='<%# Eval("DisplayText") %>' Enabled='<%# Eval("Enable") %>'></asp:TextBox>
                                                        </ItemTemplate>
                                                        <HeaderStyle HorizontalAlign="Center" />
                                                        <ItemStyle HorizontalAlign="Center" />
                                                    </asp:TemplateField>
                                                    <asp:TemplateField HeaderText="Action">
                                                        <ItemTemplate>
                                                            <asp:ImageButton ID="btnelogbookUp" runat="server" ImageUrl="~/images/up.png" CausesValidation="False" CommandArgument='<%# Eval("Tid") %>'  CommandName="UP" ToolTip="Move Up" />
                                                            <asp:ImageButton ID="btnElogbookdown" runat="server" ImageUrl="~/images/down.png"   CommandArgument='<%# Eval("Tid") %>' CommandName="DOWN" ToolTip="Move Down" />
                                                            <asp:ImageButton ID="btnelogbookDel" runat="server" CommandArgument='<%# Eval("Tid") %>' CommandName="DELETE" ImageUrl="~/images/Delete.gif" ToolTip="Delete " Enabled='<%# Eval("Enable") %>' />
                                                        </ItemTemplate>
                                                        <HeaderStyle HorizontalAlign="Center" />
                                                        <ItemStyle HorizontalAlign="Center" />
                                                    </asp:TemplateField>
                                                </Columns>
                                                <EmptyDataTemplate>
                                                    <asp:Label ID="Label1" runat="server" ForeColor="Red" Text="No Records added!"></asp:Label>
                                                </EmptyDataTemplate>
                                            </asp:GridView>
                                        </td>
                                    </tr>
                                </table>
                            </td>
                        </tr>
                        <tr>
                            <td align="right" width="20%" class="auto-style1"></td>
                            <td colspan="3" width="30%" class="auto-style2">
                                
                            </td>
                           
                        </tr>
                        <tr>
                            <td width="20%" align="right" style="height: 22px">Vehicle Document* :</td>
                            <td width="30%" style="height: 22px">
                                <asp:DropDownList ID="ddlVehicleDoc" runat="server" CssClass="k-dropdown-operator" Width="200px" AutoPostBack="True">
                                </asp:DropDownList>
                            </td>
                            <td align="right" width="20%" style="height: 22px">Vehicle IMEI Field * :</td>
                            <td width="30%" style="height: 22px">
                                <asp:TextBox ID="txtVehIMEIFld" runat="server" CssClass="k-textbox" Enabled="False"></asp:TextBox>
                                <asp:Label ID="lblVehFldMaping" runat="server" Visible="False"></asp:Label>
                            </td>
                        </tr>
                        <tr>
                            <td width="20%" align="right" class="auto-style3">Detials Section (On/Off)* :</td>
                            <td width="30%" class="auto-style3">
                                <asp:RadioButtonList ID="rbOnOff" runat="server" RepeatDirection="Horizontal" AutoPostBack="True">
                                    <asp:ListItem Value="1">On</asp:ListItem>
                                    <asp:ListItem Value="0">Off</asp:ListItem>
                                </asp:RadioButtonList>
                            </td>
                            <td align="right" width="20%" class="auto-style3"></td>
                            <td width="30%" class="auto-style3"></td>
                        </tr>
                        <tr>
                            <td align="center" colspan="4" style="height: 20px">
                                <asp:Label ID="lblGridErr" runat="server" CssClass="k-error-colored" Font-Bold="True"></asp:Label>
                            </td>
                        </tr>
                    </table>
                   </div>
                   </fieldset>

                </td>
            </tr>
            <tr>
                <td>&nbsp;</td>
                <td>&nbsp;</td>
                <td>
                    <div style="text-align:center;">
                        <asp:UpdateProgress ID="upd" runat="server">
    <ProgressTemplate>
        <div class="AjaxLoaderOuter">
            <div class="AjaxLoaderInner" id="LoadingImg">
                <p>
                    Loading Please wait</p>
                <asp:Image ID="imgloading" runat="server" ImageUrl="~/images/loading.gif" /><br />
            </div>
        </div>
    </ProgressTemplate>
</asp:UpdateProgress>
    <%--<asp:updateprogress id="UpdateProgress1" runat="server" associatedupdatepanelid="upPnl" dynamiclayout="true">
                        <progresstemplate >
                            <div id="Layer1" style="position:absolute; z-index:1; left: 50%;top : 50%;">
                                <asp:Label ID="lblWait" runat="server" 
	Text=" Please wait... " />
	<asp:Image ID="imgWait" runat="server" 
	ImageAlign="Middle" ImageUrl="images/loading.gif" />
                         <%--  <img alt="" src="images/loading.gif" >--%>
                               <%-- </div>
                        </progresstemplate>
                    </asp:updateprogress>--%>

                    </div>
                </td>
                <td>&nbsp;</td>
            </tr>
            <tr >
                <td colspan="4" >
             <div id="divDetail" runat="server" visible="false" >    <fieldset><legend><b>Detail Grid Settings</b></legend>
                    <table style="width:100%;">
                                    <tr>
                                        <td width="20%" align="right">Select Elogbook Fields to be displayed on Detail section* :</td>
                                        <td width="20%"><div style="height:200px;overflow:scroll;">
                                    <asp:CheckBoxList ID="chkLELogbookDet" runat="server" CssClass="k-checkbox" AutoPostBack="True">
                                    </asp:CheckBoxList>
                                </div></td>
                                        <td width="10%" align="center">
                                            <asp:Label ID="Label3" runat="server" Text="Add Fields" Font-Bold="True"></asp:Label>
                                            <br />
                                            <asp:ImageButton ID="btnAddElogbookdetflds" runat="server" ImageUrl="~/images/arrowhr.png" ToolTip="ADD" />
                                            
                                        </td>
                                        <td width="50%" rowspan="4" valign="middle">
                                            <asp:GridView ID="gvElogbookDetFlds" runat="server" AutoGenerateColumns="False" CssClass="k-grid"
                                                 DataKeyNames="Tid,Sequence" EmptyDataText="No record added." ShowHeaderWhenEmpty="True" Width="100%">
                                                <Columns>
                                                    <asp:BoundField DataField="SNO" HeaderText="S No.">
                                                    <HeaderStyle HorizontalAlign="Left" />
                                                    <ItemStyle HorizontalAlign="Left" />
                                                    </asp:BoundField>
                                                    <asp:BoundField DataField="FieldNm" HeaderText="Field Name">
                                                    <HeaderStyle HorizontalAlign="Center" />
                                                    <ItemStyle HorizontalAlign="Left" />
                                                    </asp:BoundField>
                                                    <asp:TemplateField HeaderText="Display Text">
                                                        <ItemTemplate>
                                                            <asp:TextBox ID="txtlogbookdetdis" runat="server" Text='<%# Eval("DisplayText") %>' Enabled='<%# Eval("Enable") %>'></asp:TextBox>
                                                        </ItemTemplate>
                                                        <HeaderStyle HorizontalAlign="Center" />
                                                        <ItemStyle HorizontalAlign="Center" />
                                                    </asp:TemplateField>
                                                    <asp:TemplateField HeaderText="Action">
                                                        <ItemTemplate>
                                                            <asp:ImageButton ID="btnelogbookUp0" runat="server" CausesValidation="False" CommandArgument='<%# Eval("Tid") %>' CommandName="UP" ImageUrl="~/images/up.png" ToolTip="Move Up" />
                                                            <asp:ImageButton ID="btnElogbookdown0" runat="server" CommandArgument='<%# Eval("Tid") %>' CommandName="DOWN" ImageUrl="~/images/down.png" ToolTip="Move Down" />
                                                            <asp:ImageButton ID="btnelogbookDel0" runat="server" CommandArgument='<%# Eval("Tid") %>' CommandName="DELETE" ImageUrl="~/images/Delete.gif" ToolTip="Delete " Enabled='<%# Eval("Enable") %>' />
                                                        </ItemTemplate>
                                                        <HeaderStyle HorizontalAlign="Center" />
                                                        <ItemStyle HorizontalAlign="Center" />
                                                    </asp:TemplateField>
                                                </Columns>
                                                <EmptyDataTemplate>
                                                    <asp:Label ID="Label8" runat="server" ForeColor="Red" Text="No Records added!"></asp:Label>
                                                </EmptyDataTemplate>
                                            </asp:GridView>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td width="20%" align="right">Select Vehicle fields to be displayed :</td>
                                        <td width="20%">
                                            <div style="height:200px;overflow:scroll">
                                                <asp:CheckBoxList ID="chkLVehicle" runat="server" CssClass="k-checkbox">
                                                </asp:CheckBoxList>
                                            </div>
                                        </td>
                                        <td align="center" width="10%">
                                            <asp:Label ID="Label4" runat="server" Font-Bold="True" Text="Add Fields"></asp:Label>
                                            <br />
                                            <asp:ImageButton ID="btnAddVehicleFlds" runat="server" ImageUrl="~/images/arrowhr.png" ToolTip="ADD" />
                                        </td>
                                    </tr>
                                    <tr>
                                        <td width="20%" align="right">Select Site Document :</td>
                                        <td width="20%">
                                            <asp:DropDownList ID="ddlSiteDoc" runat="server" AutoPostBack="True" CssClass="k-dropdown-operator" Width="200px">
                                            </asp:DropDownList>
                                        </td>
                                        <td align="center" width="10%">&nbsp;</td>
                                    </tr>
                        <tr>
                                        <td width="20%" align="right">Site Visit (From-To):</td>
                                        <td width="20%">
                                            <asp:RadioButtonList ID="rbSiteOnOff" runat="server" AutoPostBack="True" RepeatDirection="Horizontal">
                                                <asp:ListItem Value="1">On</asp:ListItem>
                                                <asp:ListItem Value="0">Off</asp:ListItem>
                                            </asp:RadioButtonList>
                                        </td>
                                        <td align="center" width="10%">&nbsp;</td>
                                    </tr>
                                    <tr>
                                        <td width="20%" align="right">Select Site fields to be
                                            <br />
                                            displayed :</td>
                                        <td width="20%">
                                            <div style="height:200px;overflow:scroll">
                                                <asp:CheckBoxList ID="chkLSite" runat="server" CssClass="k-checkbox">
                                                </asp:CheckBoxList>
                                            </div>
                                        </td>
                                        <td align="center" width="10%">
                                            <asp:Label ID="Label6" runat="server" Font-Bold="True" Text="Add Fields"></asp:Label>
                                            <br />
                                            <asp:ImageButton ID="btnAddSiteflds" runat="server" ImageUrl="~/images/arrowhr.png" ToolTip="ADD" />
                                        </td>
                                    </tr>
                                    <tr>
                                        <td align="center" colspan="4">
                                            <asp:Label ID="lblDetailErr" runat="server" CssClass="k-error-colored" Font-Bold="True"></asp:Label>
                                        </td>
                                    </tr>
                                </table>
                               
                     </fieldset></div></td>
            </tr>
           
            <tr>
                <td align="center" colspan="4" style="height: 28px">
                    <asp:Button ID="btnSaveSettings" runat="server" Text="Save" style="height: 26px" />
                </td>
            </tr>
        </table>
        </ContentTemplate></asp:UpdatePanel>
    </div>
    <br />
</asp:Content>

