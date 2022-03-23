<%@ Page Title="" Language="VB" MasterPageFile="~/USR.master" AutoEventWireup="false" CodeFile="GPSReportSettings.aspx.vb" Inherits="GPSReportSettings" %>

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
     
     .auto-style1
     {
         height: 20px;
     }
     
     .auto-style2
     {
         height: 22px;
     }
     
     </style>
    <h3>   Add/Update GPS Report Settings</h3>
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
                    <fieldset><legend><b>Report Type</b></legend>
                    <div > 
                    <table style="width:100%; margin-bottom: 0px;">
                        <tr>
                            <td align="right"  width="20%" class="auto-style2">Select Report Type* :</td>
                            <td  width="20%" class="auto-style2">
                                <asp:DropDownList ID="ddlReport" runat="server" AutoPostBack="True">
                                    <asp:ListItem Value="0">Select</asp:ListItem>
                                    <asp:ListItem Value="MileageReport">Mileage Report</asp:ListItem>
                                    <asp:ListItem Value="VehiclePosition">Online Vehicle Position Report</asp:ListItem>
                                    <asp:ListItem Value="VehicleMap">Online Vehicles Map Report</asp:ListItem>
                                </asp:DropDownList>
                            </td>
                            <td align="right"  width="6%" class="auto-style2"></td>
                            <td  width="20%" class="auto-style2">
                                </td>
                            <td align="right"  width="14%" class="auto-style2"></td>
                            <td  width="20%" class="auto-style2">
                                </td>
                        </tr>
                       
                          <tr>
                            <td align="right" width="20%">&nbsp;</td>
                            <td width="20%">&nbsp;</td>
                            <td align="right" width="6%">&nbsp;</td>
                            <td width="20%">&nbsp;</td>
                            <td align="right" width="14%">&nbsp;</td>
                            <td width="20%">&nbsp;</td>
                        </tr>
                        <tr>
                            <td align="right"  width="20%" class="auto-style2">Select Master for IMEI * :</td>
                            <td width="20%" class="auto-style2">
                                <asp:DropDownList ID="ddlVehicle" runat="server" AutoPostBack="True">
                                </asp:DropDownList>
                            </td>
                            <td align="right" width="6%" class="auto-style2"></td>
                            <td  width="20%" class="auto-style2">IMEI Field * :</td>
                            <td align="right"  width="14%" class="auto-style2">
                                <asp:TextBox ID="txtVehIMEIFld" runat="server" Enabled="False" Width="130px"></asp:TextBox>
                            </td>
                            <td  width="20%" class="auto-style2">
                                <asp:Label ID="lblVehFldMaping" runat="server" Visible="False"></asp:Label>
                            </td>
                        </tr>
                         <tr>
                            <td align="right" width="20%">&nbsp;</td>
                            <td width="20%">&nbsp;</td>
                            <td align="right" width="6%">&nbsp;</td>
                            <td width="20%">&nbsp;</td>
                            <td align="right" width="14%">&nbsp;</td>
                            <td width="20%">&nbsp;</td>
                        </tr>
                         <tr id="trVehPermission" runat="server" visible="false" >
                            <td align="right" width="20%">Select Document Type used for Vehicle permissions :</td>
                            <td width="20%">
                                <asp:DropDownList ID="ddlDocForVehPermmision" runat="server" AutoPostBack="True">
                                </asp:DropDownList>
                             </td>
                            <td align="right" width="6%">&nbsp;</td>
                            <td width="20%">Select Vehicle Field used for permissions :</td>
                            <td align="left" width="14%">
                                <asp:DropDownList ID="ddlVehFieldForPermission" runat="server" AutoPostBack="True">
                                </asp:DropDownList>
                             </td>
                            <td width="20%">&nbsp;</td>
                        </tr>
                        <tr>
                            <td align="right" width="20%">&nbsp;</td>
                            <td width="20%">&nbsp;</td>
                            <td align="right" width="6%">&nbsp;</td>
                            <td width="20%">&nbsp;</td>
                            <td align="right" width="14%">&nbsp;</td>
                            <td width="20%">&nbsp;</td>
                        </tr>
                        <tr id="trDocType" runat="server" visible="false" >
                            <td align="right" width="20%" class="auto-style2">Select Document Type* :</td>
                            <td width="20%" class="auto-style2">
                                <asp:DropDownList ID="ddlDocumenttype" runat="server" AutoPostBack="True">
                                </asp:DropDownList>
                            </td>
                            <td align="right" width="6%" class="auto-style2"></td>
                            <td width="22%" class="auto-style2"></td>
                            <td align="left" width="14%" class="auto-style2">
                               
                            </td>
                            <td width="20%" class="auto-style2"></td>
                        </tr>
                         <tr>
                            <td align="right" width="20%" class="auto-style1"></td>
                            <td width="20%" class="auto-style1"></td>
                            <td align="right" width="6%" class="auto-style1"></td>
                            <td width="20%" class="auto-style1"></td>
                            <td align="right" width="14%" class="auto-style1"></td>
                            <td width="20%" class="auto-style1"></td>
                        </tr>
                        <tr id="trDocFields" runat="server" visible="false">
                            <td nowrap="nowrap">Select Document Field for vehicle Mapping * :</td>
                            <td> <asp:DropDownList ID="ddlDocField" runat="server" AutoPostBack="True">
                                </asp:DropDownList></td>
                            <td></td>
                            <td>Select Document Field for User Mapping * :</td>
                            <td><asp:DropDownList ID="ddlDocFieldUser" runat="server" AutoPostBack="True">
                                </asp:DropDownList></td>
                            <td></td>

                        </tr>
                         <tr>
                            <td align="right" width="20%">&nbsp;</td>
                            <td width="20%">&nbsp;</td>
                            <td align="right" width="6%">&nbsp;</td>
                            <td width="20%">&nbsp;</td>
                            <td align="right" width="14%">&nbsp;</td>
                            <td width="20%">&nbsp;</td>
                        </tr>
                         <tr id="trUserName" runat="server" visible="false" >
                            <td align="right"  width="20%">UserName Display Enable* :</td>
                            <td class="auto-style1" width="20%">
                                <asp:CheckBox ID="chkUserName" runat="server"  />
                            </td>
                            <td align="right"  width="6%"></td>
                            <td  width="20%"></td>
                            <td align="left"  width="14%">
                                
                             </td>
                            <td  width="20%"></td>
                        </tr>
                        <tr>
                            <td align="center" colspan="6" >
                                <asp:Label ID="lblError" runat="server" CssClass="k-error-colored" Font-Bold="True"></asp:Label>
                            </td>
                        </tr>
                    </table>
                        </div>
                        </fieldset>
                   </td>
            </tr>
            <tr>
                <td class="auto-style1"></td>
                <td class="auto-style1"></td>
                <td class="auto-style1">
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
    

                    </div>
                </td>
                <td class="auto-style1"></td>
            </tr>
            <tr >
                <td colspan="4" >
             <div id="divDetail" runat="server"  visible="false" >    <fieldset><legend><b>Grid Settings</b></legend>
                    <table style="width:100%;">
                                    <tr>
                                        <td width="20%" align="right">Select GPS fields to be displayed:*</td>
                                        <td width="20%"><div style="height:200px;overflow:scroll; ">
                                    <asp:CheckBoxList ID="chkLGPSCol" runat="server" CssClass="k-checkbox" AutoPostBack="True">
                                        <asp:ListItem Selected="True">IMIENO</asp:ListItem>
                                        <asp:ListItem>cTime</asp:ListItem>
                                        <asp:ListItem>DevDist</asp:ListItem>
                                        <asp:ListItem>speed</asp:ListItem>
                                    </asp:CheckBoxList>
                                            
                                </div>
                                        </td>
                                        <td width="10%" align="center" >
                                            <asp:Label ID="Label3" runat="server" Text="Add Fields" Font-Bold="True"></asp:Label>
                                            <br />
                                            <asp:ImageButton ID="btnAddGPSFields" runat="server" ImageUrl="~/images/arrowhr.png" ToolTip="ADD" style="height: 15px; width: 13px;" />
                                            
                                        </td>
                                        <td width="50%" rowspan="4" valign="middle">
                                            <asp:GridView ID="gvGPS" runat="server" AutoGenerateColumns="False" CssClass="k-grid"
                                                 DataKeyNames="TID,SequenceNo" EmptyDataText="No record added." ShowHeaderWhenEmpty="True" Width="100%">
                                                <Columns>
                                                    <asp:BoundField DataField="SNO" HeaderText="S No.">
                                                    <HeaderStyle HorizontalAlign="Left" />
                                                    <ItemStyle HorizontalAlign="Left" />
                                                    </asp:BoundField>
                                                    <asp:BoundField DataField="DispName" HeaderText="Field Name">
                                                    <HeaderStyle HorizontalAlign="Center" />
                                                    <ItemStyle HorizontalAlign="Left" />
                                                    </asp:BoundField>
                                                    <asp:TemplateField HeaderText="Display Name">
                                                        <ItemTemplate>
                                                            <asp:TextBox ID="txtDispName" runat="server" Text='<%# Eval("DispName")%>' ></asp:TextBox>
                                                        </ItemTemplate>
                                                        <HeaderStyle HorizontalAlign="Center" />
                                                        <ItemStyle HorizontalAlign="Center" />
                                                    </asp:TemplateField>
                                                    <asp:TemplateField HeaderText="Action">
                                                        <ItemTemplate>
                                                            <asp:ImageButton ID="btnelogbookUp0" runat="server" CausesValidation="False" CommandArgument='<%# Eval("Tid") %>' CommandName="UP" ImageUrl="~/images/up.png" ToolTip="Move Up" />
                                                            <asp:ImageButton ID="btnElogbookdown0" runat="server" CommandArgument='<%# Eval("Tid") %>' CommandName="DOWN" ImageUrl="~/images/down.png" ToolTip="Move Down" />
                                                            <asp:ImageButton ID="btnelogbookDel0" runat="server" CommandArgument='<%# Eval("Tid") %>' CommandName="DELETE" ImageUrl="~/images/Delete.gif" ToolTip="Delete "   />
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
                                    <tr id="trSpeed" runat="server" visible="false"  >
                                        <td align="right" width="20%">Select Function for Speed*: </td>
                                        <td width="20%">
                                            <asp:RadioButtonList ID="rbLSpeed" runat="server" RepeatDirection="Horizontal">
                                                <asp:ListItem Selected="True">Sum</asp:ListItem>
                                                <asp:ListItem>Max</asp:ListItem>
                                                <asp:ListItem>Min</asp:ListItem>
                                                <asp:ListItem Value="Avg">Average</asp:ListItem>
                                            </asp:RadioButtonList>
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
                                        <td align="center" width="10%" rowspan="1">
                                            <asp:Label ID="Label4" runat="server" Font-Bold="True" Text="Add Fields"></asp:Label>
                                            <br />
                                            <asp:ImageButton ID="btnAddVehicleFlds" runat="server" ImageUrl="~/images/arrowhr.png" ToolTip="ADD" />
                                        </td>
                                    </tr>
                                    <tr>
                                        <td width="20%" align="right" class="auto-style1"></td>
                                        <td width="20%" class="auto-style1">
                                            </td>
                                        <td align="center" width="10%" class="auto-style1"></td>
                                    </tr>
                                    <tr>
                                        <td align="center" colspan="4" class="auto-style1">
                                            <asp:Label ID="lblGridError" runat="server" CssClass="k-error-colored" Font-Bold="True"></asp:Label>
                                        </td>
                                    </tr>
                                </table>
                               
                     </fieldset></div></td>
            </tr>
           
            <tr>
                <td align="center" colspan="4" style="height: 28px">
                    <asp:Button ID="btnSaveSettings" runat="server" Text="Save" style="height: 26px; width: 42px;" />
                </td>
            </tr>
        </table>
        </ContentTemplate></asp:UpdatePanel>
    </div>
    <br />
</asp:Content>

