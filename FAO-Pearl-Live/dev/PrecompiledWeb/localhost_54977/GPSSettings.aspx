<%@ page title="" language="VB" masterpagefile="~/USR.master" autoeventwireup="false" inherits="GPSSettings, App_Web_iqn0gzeb" enableeventvalidation="true" viewStateEncryptionMode="Always" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="asp" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">

    <style type="text/css">
        .red {
            background:#f59797;
            border:1px solid #e63c3c;
            border-radius:5px;
            padding:15px;
        }
        .green {
            background:#a5f1a9;
            border:1px solid #43ea4c;
            border-radius:5px;
            padding:15px;
            color:#000;
        }
    </style>

    <table cellspacing="0px" cellpadding="0px" width="100%" border="0px">
        <tr>
            <td style="width: 100%;" valign="top" align="left">
                <div id="main">

                    <h1>Settings</h1>
                    <%--<asp:UpdatePanel ID="UpdatePanel1" runat="server">--%>
                    <%-- <Triggers>
        <asp:PostBackTrigger ControlID = "btnlogin" />
  </Triggers> --%>
                    <%--
   <ContentTemplate>--%>
                    <div class="form">
                        <asp:TabContainer ID="TabContainer1" runat="server"
                            Height="648px"
                            Width="600px"
                            ActiveTabIndex="0"
                            OnDemand="true"
                            AutoPostBack="false"
                            TabStripPlacement="Top"
                            ScrollBars="None"
                            VerticalStripWidth="120px">

                            <asp:TabPanel ID="TabPanel1" runat="server"
                                HeaderText="GPS SETTINGS"
                                Enabled="true"
                                ScrollBars="Auto"
                                OnDemandMode="Once">

                                <ContentTemplate>
                                    <table cellspacing="5px" cellpadding="0px" width="100%" border="0px">
                                        <tr>
                                            <td style="width: 100%" colspan="2">
                                                <br />
                                                <h2>User -
    <label>
        Vehicle</label>Mapping</h2>
                                                <br />
                                            </td>
                                        </tr>


                                        <tr>
                                            <td style="text-align: right"></td>
                                            <td style="">
                                                <asp:Label ID="uservehiclemap" ForeColor="Red" runat="server"></asp:Label>
                                            </td>
                                        </tr>

                                        <tr>
                                            <td style="text-align: right">
                                                <label>*Select Mapping Document Type [?] : </label>
                                            </td>
                                            <td style="">
                                                <asp:DropDownList ID="ddlDocumentType" runat="server" AutoPostBack="true" Width="250px" OnSelectedIndexChanged="FilldropDown" CssClass="txtBox">
                                                </asp:DropDownList>
                                            </td>
                                        </tr>

                                        <tr>
                                            <td style="text-align: right">
                                                <label>*Select User Field [?] : </label>
                                            </td>
                                            <td style="">
                                                <asp:DropDownList ID="ddUserField" runat="server" Width="250px" CssClass="txtBox">
                                                </asp:DropDownList>


                                            </td>
                                        </tr>

                                        <tr>
                                            <td style="text-align: right">
                                                <label>*Start Date Field [?] : </label>
                                            </td>
                                            <td style="">
                                                <asp:DropDownList ID="StartDate" runat="server" Width="250px" CssClass="txtBox">
                                                </asp:DropDownList>

                                            </td>
                                        </tr>

                                        <tr>
                                            <td style="text-align: right">
                                                <label>*End Date Field [?] : </label>
                                            </td>
                                            <td style="">
                                                <asp:DropDownList ID="EndDate" runat="server" Width="250px" CssClass="txtBox">
                                                </asp:DropDownList>




                                            </td>
                                        </tr>
                                        <tr>
                                            <td style="text-align: right">
                                                <label>*Select Vehicle Field [?] : </label>
                                            </td>
                                            <td style="">
                                                <asp:DropDownList ID="ddVechicleField" runat="server" Width="250px" CssClass="txtBox">
                                                </asp:DropDownList>
                                            </td>
                                        </tr>

                                        <tr>
                                            <td style="text-align: right"></td>
                                            <td style="">
                                                <asp:Button ID="btnUname" runat="server" CssClass="btnNew"
                                                    Text="Save" />
                                            </td>
                                        </tr>
                                        <%--<tr>
   <td style="text-align:right"> &nbsp;</td>
   <td style=""> <asp:Label style="color:red" ID="Label3" runat="server"></asp:Label></td>
</tr>--%>



                                        <tr>
                                            <td style="width: 100%" colspan="2">
                                                <br />
                                                <h2>Vehicle - IMEI Mapping</h2>
                                                <br />
                                            </td>
                                        </tr>

                                        <tr>
                                            <td style="text-align: right"></td>
                                            <td style="">
                                                <asp:Label ID="savemappingdocumenttype" ForeColor="Red" runat="server"></asp:Label>
                                            </td>
                                        </tr>


                                        <tr>
                                            <td style="text-align: right">
                                                <label>*Select Mapping Document Type[?] : </label>
                                            </td>
                                            <td style="">

                                                <asp:DropDownList ID="ddlVDtype" runat="server" Width="250px" AutoPostBack="true" OnSelectedIndexChanged="FilldropDownVehicle" CssClass="txtBox">
                                                </asp:DropDownList>
                                            </td>
                                        </tr>

                                        <tr>
                                            <td style="text-align: right">
                                                <label>*select Vehicle Field [?] : </label>
                                            </td>
                                            <td style="">
                                                <asp:DropDownList ID="ddVehMapping" runat="server" Width="250px" CssClass="txtBox">
                                                </asp:DropDownList>

                                            </td>

                                        </tr>

                                        <tr>
                                            <td style="text-align: right">
                                                <label>*IMEI Field [?] : </label>
                                            </td>
                                            <td style="">
                                                <asp:DropDownList ID="ddVIMEIfield" runat="server" Width="250px" CssClass="txtBox">
                                                </asp:DropDownList>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td style="text-align: right"></td>
                                            <td style="">
                                                <asp:Button ID="btnImei" runat="server" CssClass="btnNew"
                                                    Text="Save" />
                                            </td>
                                        </tr>


                                    </table>
                                </ContentTemplate>

                            </asp:TabPanel>

                            <asp:TabPanel ID="TabPanel2" runat="server"
                                HeaderText="CAB SETTINGS"
                                Enabled="true"
                                ScrollBars="Auto"
                                OnDemandMode="Once">




                                <ContentTemplate>
                                    <table cellspacing="5px" cellpadding="0px" width="100%" border="0px">

                                        <tr>
                                            <td style="width: 100%" colspan="2">
                                                <br />
                                                <h2>User -
    <label>
        Vehicle</label> Mapping</h2>
                                                <br />
                                            </td>
                                        </tr>

                                        <%--<tr>
   <td style="text-align:right"> <label> *Select Mapping Document Type&nbsp; : </label></td>
   <td style=""> 
   <asp:DropDownList ID="APPTYPE" runat="server"  AutoPostBack="true"  width="250px"  OnSelectedIndexChanged="FilldropDown"  CssClass="txtBox">

</asp:DropDownList>
     </td>
</tr>--%>
                                        <tr>
                                            <td style="text-align: right"></td>
                                            <td style="">
                                                <asp:Label ID="savedvehiclemapping" ForeColor="Red" runat="server"></asp:Label>
                                            </td>
                                        </tr>

                                        <tr>
                                            <td style="text-align: right">
                                                <label>*Select VehicleDocument Type&nbsp; : </label>
                                            </td>
                                            <td style="">
                                                <asp:DropDownList ID="ddlDocumentTypec" runat="server" AutoPostBack="true" Width="250px" OnSelectedIndexChanged="FilldropDownc" CssClass="txtBox">
                                                </asp:DropDownList>
                                            </td>
                                        </tr>

                                        <tr>
                                            <td style="text-align: right">
                                                <label>*Select Vehicle Status&nbsp; : </label>
                                            </td>
                                            <td style="">
                                                <asp:DropDownList ID="ddstatus" runat="server" Width="250px" CssClass="txtBox">
                                                </asp:DropDownList>


                                            </td>
                                        </tr>
                                        <tr>
                                            <td style="text-align: right">
                                                <label>*Select Vehicle Owner&nbsp; : </label>
                                            </td>
                                            <td style="">
                                                <asp:DropDownList ID="ddowner" runat="server" Width="250px" CssClass="txtBox">
                                                </asp:DropDownList>


                                            </td>
                                        </tr>
                                        <tr>
                                            <td style="text-align: right">
                                                <label>*Select Vehicle Type &nbsp; : </label>
                                            </td>
                                            <td style="">
                                                <asp:DropDownList ID="ddVehicleType" runat="server" Width="250px" CssClass="txtBox">
                                                </asp:DropDownList>


                                            </td>
                                        </tr>
                                        <tr>
                                            <td style="text-align: right">
                                                <label>*select Rate Card Doc&nbsp; : </label>
                                            </td>
                                            <td style="">
                                                <asp:DropDownList ID="ddRatecarddoc" runat="server" Width="250px" CssClass="txtBox">
                                                </asp:DropDownList>

                                            </td>
                                        </tr>

                                        <tr>
                                            <td style="text-align: right">
                                                <label>*Select Customer Doc&nbsp; : </label>
                                            </td>
                                            <td style="">
                                                <asp:DropDownList ID="ddlCustomerDoc" runat="server" Width="250px" CssClass="txtBox">
                                                </asp:DropDownList>


                                            </td>
                                        </tr>
                                        <tr>
                                            <td style="text-align: right">&nbsp;</td>
                                            <td style="">&nbsp;</td>
                                        </tr>

                                        <tr>
                                            <td style="text-align: right"></td>
                                            <td style="">
                                                <asp:Button ID="btnUnamec" runat="server" CssClass="btnNew"
                                                    Text="Save" />
                                            </td>
                                        </tr>
                                        <tr>
                                            <td style="text-align: right">&nbsp;</td>
                                            <td style="">
                                                <asp:Label Style="color: red" ID="lblUname" runat="server"></asp:Label></td>
                                        </tr>

                                    </table>
                                </ContentTemplate>


                            </asp:TabPanel>
                            <asp:TabPanel ID="TabPanel3" runat="server"
                                HeaderText="MAP SETTINGS"
                                Enabled="true"
                                ScrollBars="Auto"
                                OnDemandMode="Once">

                                <ContentTemplate>
                                    <table cellspacing="5px" cellpadding="0px" width="100%" border="0px">
                                        
                                        <tr>
                                            <td style="text-align: right"></td>
                                            <td style="">
                                                <asp:Label ID="mapkey" ForeColor="Red" Visible="False" runat="server"></asp:Label>
                                            </td>
                                        </tr>

                                        <tr>
                                            <td style="text-align: right">
                                                <label>*Select map type[?] : </label>
                                            </td>
                                            <td style="">

                                                <asp:DropDownList ID="ddlHomeMap" runat="server" Width="250px" AutoPostBack="True" CssClass="txtBox">
                                                    <asp:ListItem Value="GmapHome.aspx">Google</asp:ListItem>
                                                    <asp:ListItem Value="NMapHome.aspx">Nokia</asp:ListItem>
                                                </asp:DropDownList>
                                            </td>
                                        </tr>

                                        
                                        <tr>
                                            <td style="text-align: right"></td>
                                            <td style="">
                                                <asp:Button ID="APISave" runat="server" CssClass="btnNew"
                                                    Text="Save" />
                                            </td>
                                        </tr>
                                        <tr>
                                            <td style="text-align: right">&nbsp;</td>
                                            <td style="">
                                                <asp:Label Style="color: red" ID="Label1" runat="server"></asp:Label></td>
                                        </tr>


                                    </table>
                                </ContentTemplate>

                            </asp:TabPanel>

                            <asp:TabPanel ID="TabPanel4" runat="server"
                                HeaderText="CALENDER SETTINGS"
                                ScrollBars="Auto"
                                OnDemandMode="Once">

                                <ContentTemplate>
                                    <table cellspacing="5px" cellpadding="0px" width="100%" border="0px">
                                        <tr>
                                            <td style="width: 100%" colspan="2">
                                                <br />
                                                <h2>Document Type -
    <label>
        Calender</label> Setting</h2>
                                                <br />
                                            </td>
                                        </tr>


                                        <tr>
                                            <td style="text-align: right"></td>
                                            <td style="">
                                                <asp:Label ID="calndr" ForeColor="Red" runat="server"></asp:Label>
                                            </td>
                                        </tr>

                                        <tr>
                                            <td style="text-align: right">
                                                <label>*Select Mapping Document Type [?] : </label>
                                            </td>
                                            <td style="">
                                                <asp:DropDownList ID="doctypecal" runat="server" AutoPostBack="True" Width="250px" OnSelectedIndexChanged="FilldropDown" CssClass="txtBox">
                                                </asp:DropDownList>
                                            </td>
                                        </tr>

                                        <tr>
                                            <td style="text-align: right">
                                                <label>*Select User Field [?] : </label>
                                            </td>
                                            <td style="">
                                                <asp:DropDownList ID="usrfieldcal" runat="server" Width="250px" CssClass="txtBox">
                                                </asp:DropDownList>


                                            </td>
                                        </tr>

                                        <tr>
                                            <td style="text-align: right">
                                                <label>*Start Date Field [?] : </label>
                                            </td>
                                            <td style="">
                                                <asp:DropDownList ID="strtdatecal" runat="server" Width="250px" CssClass="txtBox"> 
                                                </asp:DropDownList>

                                            </td>
                                        </tr>

                                        <tr>
                                            <td style="text-align: right">
                                                <label>*End Date Field [?] : </label>
                                            </td>
                                            <td style="">
                                                <asp:DropDownList ID="enddatecal" runat="server" Width="250px" CssClass="txtBox">
                                                </asp:DropDownList>
                                            </td>
                                        </tr>
                                        <caption>
                                            &nbsp;&nbsp;
                    <tr>
                        <td style="text-align: right">&nbsp;</td>
                        <td style="text-align: right">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
                                    <asp:Button ID="btncal" runat="server" CssClass="btnNew" Text="Save" OnClick="Editrecord" />
                            &nbsp;</td>
                    </tr>
                                        </caption>
                                    </table>
                                    <table cellspacing="5px" cellpadding="0px" width="100%" border="0px">
                                        <tr style="color: #000000">
                                            <td style="text-align: left;" valign="top">
                                                <asp:GridView ID="gvData" runat="server" AutoGenerateColumns="False" CellPadding="2" DataKeyNames="Tid" Width="100%"
                                                    ForeColor="#333333" AllowSorting="True" AllowPaging="True">
                                                    <AlternatingRowStyle BackColor="White" />
                                                    <Columns>
                                                        <asp:TemplateField HeaderText="S.No">
                                                            <ItemTemplate>
                                                                <%#Container.DataItemIndex+1 %>
                                                            </ItemTemplate>
                                                        </asp:TemplateField>
                                                        <asp:BoundField DataField="doctype" HeaderText="Document Type">
                                                            <HeaderStyle HorizontalAlign="Left" />
                                                            <ItemStyle Wrap="True" />
                                                        </asp:BoundField>

                                                        <asp:BoundField DataField="Nameusrfield" HeaderText="User Field">
                                                            <HeaderStyle HorizontalAlign="Left" />
                                                        </asp:BoundField>

                                                        <asp:BoundField DataField="Namestartdate" HeaderText="Start Date">
                                                            <HeaderStyle HorizontalAlign="Left" />
                                                        </asp:BoundField>

                                                        <asp:BoundField DataField="Nameenddate" HeaderText="End Date">
                                                            <HeaderStyle HorizontalAlign="Left" />
                                                        </asp:BoundField>

                                                        <asp:TemplateField HeaderText="Action">
                                                            <ItemTemplate>
                                                                <asp:ImageButton ID="btnDtl" ImageUrl="~/images/Delete.png" runat="server" Height="16px" Width="16px" OnClick="DeleteHit" ToolTip="Delete User" AlternateText="Delete" />
                                                                <asp:ConfirmButtonExtender ID="confrmdelete" runat="server" ConfirmText="Are You Sure Want Delete" TargetControlID="btnDtl" />
                                                                &nbsp;
                              <%--  <asp:ImageButton ID="btnEdit" runat="server" ImageUrl="~/images/edit.png" Height="16px" Width="16px" OnClick="EditHit" ToolTip="Edit User" AlternateText="Edit" />--%>
                                                            </ItemTemplate>
                                                            <ItemStyle Width="80px" HorizontalAlign="Center" />
                                                        </asp:TemplateField>
                                                    </Columns>
                                                    <EditRowStyle BackColor="#2461BF" />
                                                    <FooterStyle BackColor="#507CD1" Font-Bold="True" ForeColor="White" />
                                                    <HeaderStyle BackColor="CornflowerBlue" Font-Bold="True" ForeColor="White" />
                                                    <PagerStyle BackColor="CornflowerBlue" ForeColor="White" HorizontalAlign="Center" />
                                                    <RowStyle BackColor="#EFF3FB" />
                                                    <SelectedRowStyle BackColor="#D1DDF1" Font-Bold="True" ForeColor="#333333" />
                                                </asp:GridView>
                                    </table>
                                </ContentTemplate>

                            </asp:TabPanel>

                            <asp:TabPanel ID="TabPanel5" runat="server" HeaderText="Geofence Alert Settings" OnDemandMode="Once">
                                <ContentTemplate>
                                    <div style="width:90%; margin:auto">
                                        <div style="margin-top:20px;text-align:center;">
                                            <h2>
                                                Vehicle out side geofence boundary settings
                                            </h2>
                                        </div>
                                       <table cellspacing="5">
                                           <tr >
                                               <td>Geofence Document<span style="color:#f00">*</span>:</td>
                                               <td>
                                                   <asp:DropDownList ID="ddlGeoDocument" runat="server" Width="250px" AutoPostBack="True" CssClass="txtBox"></asp:DropDownList> 
                                               </td>
                                               <td>&nbsp;</td>
                                           </tr>
                                           <tr>
                                               <td>Geofence Document Name<span style="color:#f00">*</span>:</td>
                                               <td>
                                                   <asp:DropDownList ID="ddlGeoDocName" runat="server" Width="250px" CssClass="txtBox"></asp:DropDownList> 
                                               </td>
                                               <td>&nbsp;</td>
                                           </tr>
                                           <tr>
                                               <td>Geofence Document Type<span style="color:#f00">*</span>:</td>
                                               <td>
                                                   
                                                   <asp:DropDownList ID="ddlGeoDocType" runat="server" AutoPostBack="True"  Width="250px" CssClass="txtBox">
                                                       <asp:ListItem Value="City" Selected="True">City</asp:ListItem>
                                                       <asp:ListItem Value="State">State</asp:ListItem>
                                                       <asp:ListItem Value="Others">Others</asp:ListItem>
                                                   </asp:DropDownList>
                                                   
                                               </td>
                                               <td>&nbsp;</td>
                                           </tr>
                                           <tr id="trGeoField" runat="server" visible="False">
                                               <td runat="server">Geofence Field<span style="color:#f00">*</span>:</td>
                                               <td runat="server">
                                                   
                                                   <asp:DropDownList ID="ddlGeofence" runat="server"  Width="250px" CssClass="txtBox">
                                                   </asp:DropDownList>
                                                   
                                               </td>
                                               <td runat="server"></td>
                                           </tr>

                                            <tr>
                                               <td>User Map Geofence Document<span style="color:#f00">*</span>:</td>
                                               <td>
                                                   
                                                   <asp:DropDownList ID="ddlUserMapGeo" runat="server"  Width="250px" CssClass="txtBox">
                                                       
                                                   </asp:DropDownList>
                                                   
                                               </td>
                                               <td></td>
                                           </tr>

                                           <tr>
                                               <td>Vehicle Document<span style="color:#f00">*</span>:</td>
                                               <td>
                                                   
                                                   <asp:DropDownList ID="ddlVehicleDoc" runat="server" AutoPostBack="True" Width="250px" CssClass="txtBox">
                                                   </asp:DropDownList>
                                                   
                                               </td>
                                               <td></td>
                                           </tr>
                                           <tr>
                                               <td>Vehicle Number<span style="color:#f00">*</span>:</td>
                                               <td>
                                                   
                                                   <asp:DropDownList ID="ddlVehicleNo" runat="server"  Width="250px" CssClass="txtBox">
                                                   </asp:DropDownList>
                                                   
                                               </td>
                                               <td></td>
                                           </tr>
                                           <tr>
                                               <td>Vehicle Name<span style="color:#f00">*</span>:</td>
                                               <td>
                                                   <asp:DropDownList ID="ddlVehicleName" runat="server"  Width="250px" CssClass="txtBox">
                                                   </asp:DropDownList>
                                               </td>
                                               <td></td>
                                           </tr>
                                           <tr>
                                               <td>Vehicle IMEI No<span style="color:#f00">*</span>:</td>
                                               <td>
                                                   <asp:DropDownList ID="ddlVehicleIMEI" runat="server"  Width="250px" CssClass="txtBox">
                                                   </asp:DropDownList>
                                               </td>
                                               <td></td>
                                           </tr>
                                           <tr>
                                               <td>Vehicle Map Geofence<span style="color:#f00">*</span>:</td>
                                               <td>
                                                   <asp:DropDownList ID="ddlVehicleMapGeofence" runat="server"  Width="250px" CssClass="txtBox">
                                                   </asp:DropDownList>
                                               </td>
                                               <td></td>
                                           </tr>
                                           <tr>
                                               <td></td>
                                               <td></td>
                                           </tr>
                                           <tr>
                                               <td></td>
                                               <td>
                                                   <asp:Button ID="btnSaveGeofenceAlertSettings" runat="server" Text="Save settings" CssClass="btnNew" />
                                               </td>
                                           </tr>
                                       </table> 
                                        <div id="divMsg" runat="server" visible="False">

                                        </div>
                                    </div>
                                </ContentTemplate>
                            </asp:TabPanel>

                        </asp:TabContainer>


                    </div>
                   
                </div>
            </td>
        </tr>
    </table>



</asp:Content>

