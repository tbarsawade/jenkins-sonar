<%@ Page Language="VB" AutoEventWireup="false" MasterPageFile="~/USR.master" CodeFile="GMapHome.aspx.vb"
    Inherits="GMapHome" EnableEventValidation="false" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="asp" %>


<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
    <script src="jquery/jquery-1.3.2.min.js"></script>
    <%--<script src="https://maps.google.com/maps/api/js?sensor=false" type="text/javascript" charset="utf-8"></script>--%>
    <script src="js/Gmap.js"></script>
    
    <style type="text/css">
        .hide {
            display:none;
        }
        .gm-style-iw {
           /*font-weight:bold !important;*/
            text-align:left !important;
        }

.lstContent option:hover
{
    background-color:#DEEFFF;
    cursor:pointer;
    color:blue;
}

        #idSite tr:hover {
             background-color:#DEEFFF;
    cursor:pointer;
    color:blue;
    
        }
        .hide {
            display:none;
        }
        .show {
            display:block;
        }
        .show1 {
            display:block;
            width:40px;
            height:25px;
        }

input[type=text], textarea {
  @include transition(all 0.30s ease-in-out);
  outline: none;
  padding: 3px 0px 3px 3px;
  margin: 5px 1px 3px 0px;
  border: 1px solid #DDDDDD;
}
 
input[type=text]:focus, textarea:focus {
  @include box-shadow(0 0 5px rgba(81, 203, 238, 1));
  padding: 3px 0px 3px 3px;
  margin: 5px 1px 3px 0px;
  border: 1px solid rgba(81, 203, 238, 1);
}

    </style>
    <script type="text/javascript">
        var myMarkersArray = [];
        var RemovedMarkers = myMarkersArray.slice(0);

        function ShowHideMap(sender) {
            var group = $(sender).attr("value");

            if ($(sender).is(':checked')) {

                for (i = 0; i < RemovedMarkers.length; i++) {
                    if (RemovedMarkers[i].group == group) {
                        myMarkersArray.push(RemovedMarkers[i])

                        RemovedMarkers[i].setVisible(true);
                        RemovedMarkers.splice(i, 1);
                        i = -1;
                    }
                }

            } else {


                for (i = 0; i < myMarkersArray.length; i++) {
                    if ((myMarkersArray[i].group == group)) {
                        RemovedMarkers.push(myMarkersArray[i])
                        myMarkersArray[i].setVisible(false);
                        myMarkersArray.splice(i, 1);

                        i = -1;
                    }
                }

            }


        }

        function focusMarker(sender) {
            var v = $(sender).attr("value");
            for (i = 0; i < myMarkersArray.length; i++) {
                if ((myMarkersArray[i].primaryKey == v)) {

                    google.maps.event.trigger(myMarkersArray[i], 'click')
                    map.setCenter(myMarkersArray[i].getPosition());
                    myMarkersArray[i].setVisible(true);
                    return false;
                }
            }

            //for (i = 0; i < RemovedMarkers.length; i++) {
            //    if ((RemovedMarkers[i].primaryKey == v)) {

            //        google.maps.event.trigger(RemovedMarkers[i], 'click')
            //        map.setCenter(RemovedMarkers[i].getPosition());
            //        RemovedMarkers[i].setVisible(true);
            //        return false;

            //    }
            //}
        }

    </script>

    <script type="text/javascript">

        $(document).ready(function () {
            Gmap.GetMarkers('<%= Circle.ClientID%>', '<%= chkvtype.ClientID%>');
            CacheValues();
            var map;
        });

    </script>

    <script type="text/javascript">

        var myVals = new Array();

        function CacheValues() {
            var l = document.getElementById('<%= LstVehicle.ClientID%>');

            for (var i = 0; i < l.options.length; i++) {

                myVals[i] = l.options[i];
            }
            return;
        }

        function SearchList() {
            // alert('hello');
            var l = document.getElementById('<%= LstVehicle.ClientID%>');
            var tb = document.getElementById('VehSearch');
            var strlb = String;
            if (!String.prototype.startsWith) {
                String.prototype.startsWith = function (str) {
                    return !this.indexOf(str);
                }
            }

            l.options.length = 0;

            if (tb.value == "") {
                for (var i = 0; i < myVals.length; i++) {
                    var colr = myVals[i].style.color;

                    var op = '<option onclick="focusMarker(this);" value=' + myVals[i].value + ' style="color:' + colr + '" >' + myVals[i].text + '</option>';
                    $('#<%= LstVehicle.ClientID%>').append(op);

                }
            }
            else {


                for (var i = 0; i < myVals.length; i++) {

                    if (myVals[i].text.toLowerCase().match(tb.value.toLowerCase())) {

                        var colr = myVals[i].style.color;

                        var op = '<option  onclick="focusMarker(this);" value=' + myVals[i].value + '  style="color:'+colr+'" >' + myVals[i].text + '</option>';
                        $('#<%= LstVehicle.ClientID%>').append(op);

                    }
                    else {
                        // do nothing

                    }
                }
            }
        }



    </script>

    <div>
        
        <input type="hidden" id="hdndata" value="" RefreshTime="" />
        <asp:HiddenField ID="hdnRefTime" Value="" runat="server" ClientIDMode="Static" />
        <asp:HiddenField ID="hdnCluster" Value="" runat="server" ClientIDMode="Static" />
         <asp:HiddenField ID="hdnMapCenter" Value="" runat="server" ClientIDMode="Static" />

        <asp:UpdatePanel ID="updPnlGrid" runat="server">
            <Triggers>
                <asp:PostBackTrigger ControlID="Circle" />
                <asp:PostBackTrigger ControlID="chkvtype" />
            </Triggers>
            <ContentTemplate>
                <table>
                                        <colgroup style="width:80%;"></colgroup>
                                        <colgroup style="width:20%"></colgroup>
                                        <tr>
                                            <td>
                                                <span style="color: Black; text-align: Left; font-weight: bold;">Select Site</span>
                                            </td>
                                            <td>
                                             </td>
                                        </tr>
                                    </table>
                 <table style="width: 100%; height: 670px;">
                    <tr>
                        <td style="width: 13%; height: 670px; vertical-align: top;">
                            

                            <div style="width: 100%; height: 770px;" >
                               
                                   
                                    <asp:Panel ID="Panel2" runat="server" Height="80px" Style="display: block" ScrollBars="Auto">
                                        <asp:CheckBoxList ID="Circle" runat="server" AutoPostBack="false" OnTextChanged="FilterSite">
                                        </asp:CheckBoxList>
                                    </asp:Panel>
                                    <br />
                                    <legend style="color: Black; text-align: Left; font-weight: bold;">Select Vehicle Type</legend>
                                    <asp:Panel ID="Panel1" runat="server" Height="100px" Style="display: block" ScrollBars="Auto">
                                        <asp:CheckBoxList ID="chkvtype" runat="server" AutoPostBack="false" OnTextChanged="FilterSite">
                                        </asp:CheckBoxList>
                                    </asp:Panel>
                                
                                
                                
                                <br />
                                <table>
                                        <colgroup style="width:80%;"></colgroup>
                                        <colgroup style="width:20%"></colgroup>
                                        <tr>
                                            <td>
                                                <span style="color: Black; text-align: Left; font-weight: bold;">Search Vehicle</span>
                                            </td>
                                            <td>
                                                <img src="images/ref.png" id="img2" onclick="Gmap.RefreshVechical1(this,'img1');" title="Refresh Vechicals" style="width:25px; height:25px; cursor:pointer;" />
                                                <img src="images/loadmap.gif" id="img1" class="hide" title="Refresh Vechicals" />
                                            </td>
                                        </tr>
                                    </table>
                                    <asp:Panel ID="Panel3" runat="server" Height="210px"  Style="display: block" ScrollBars="Auto">
                                      

                                        <div>
                                            <input type="text" id="VehSearch" placeholder="Type Vechicle name" style="width:95%;" onkeyup="return SearchList();" />
                                        </div>
                                        <div >
                                        <asp:ListBox ID="LstVehicle" onchange="focusMarker(this);" runat="server" class="lstContent" Height="150px" Width="98%" DataTextField="Vehicle">
                                        </asp:ListBox>
                                            
                                        </div>
                                    </asp:Panel>

                                <legend style="color: Black; text-align: Left; font-weight: bold;">Search Sites</legend>
                                        <input type="text" id="txtSearchV" style="width:96%; " placeholder="Type Site Name" onkeyup="return Gmap.SearchSite(this,'#idSite',false);"  />
                                <input type="text" id="txtSearchId" style="width:96%; " placeholder="Type SiteID" onkeyup="return Gmap.SearchSite(this,'#idSite',true);"  />
                                    <asp:Panel ID="Panel4" runat="server" Height="70px" Width="130px" Style="display: block; border:1px solid #CCCCCC;" ScrollBars="Vertical">
                                        

                                        <table style="width:98%;">
                                            <tbody id="idSite">

                                            </tbody>
                                        </table>

                                        </asp:Panel>
                                
                            </div>
                        </td>
                        <%--</fieldset>--%>
                        <td style="width: 87%; height: 770px;" valign="top" >
                            <div id="map" style="width: 100%; text-align:center;vertical-align:middle ;height: 100%; border:2px solid rgba(0, 0, 0, 0.4); ">
                                <div style="width:100%; height:760px; background-color: rgba(104, 149, 166, 0.3);   ">
                                     <img src="images/MapLoading.gif" height="50%" width="50%" />
                                </div>
                               
                                <asp:UpdateProgress ID="UpdateProgress1" runat="server">
                                    <ProgressTemplate>
                                        <div id="Layer1" style="position: absolute; z-index: 1; left: 50%; top: 50%">
                                            <asp:Image ID="Image1" runat="server" Height="25px" ImageUrl="~/Images/prg.gif" />
                                            please wait...
                                        </div>
                                    </ProgressTemplate>
                                </asp:UpdateProgress>
                            </div>
                        </td>
                    </tr>
                    <tr valign="top" >
                        <td valign="top" >
                        </td>
                        <td valign="top"  >
                         <table cellspacing="0" celpadding="0" width="100%" border="1" style=" vertical-align:middle;">
                             <colgroup style="width:10%;"></colgroup>
                             <colgroup style="width:40%; "></colgroup>
                             <colgroup style="width:10%;  "></colgroup>
                             <colgroup style="width:40%; "></colgroup>
                                <tr>
                                    <td style="text-align:center;">
                                        <asp:Image ID="Image5" runat="server" ImageUrl="~/images/blue.png" />
                                    </td>
                                    <td>
                                        &nbsp; Strategic</td>
                                    <td style="text-align:center; vertical-align:middle;">
                                        <span style="width:30px; height:15px; background:red; display:inline-block; margin-top:5px;">
                                        </span>
                                    </td>
                                    <td>
                                       
                                       &nbsp;Vehicle data not received for more than 24 Hrs.
                                    </td>
                                </tr>
                                <tr>
                                    <td style="text-align:center;">
                                        <asp:Image ID="Image6" runat="server" ImageUrl="~/images/darkyellow.png" />
                                    </td>
                                    <td>
                                        &nbsp; Normal Hub</td>
                                     <td style="text-align:center; vertical-align:middle;">
                                        <span style="width:30px; height:15px; background:green; display:inline-block; margin-top:5px;">
                                        </span>
                                    </td>
                                    <td>
                                        &nbsp;Vehicle is Moving</td>
                                </tr>
                                <tr>
                                    <td style="text-align:center;">
                                        <asp:Image ID="Image7" runat="server" ImageUrl="~/images/darkk.png" />
                                    </td>
                                    <td>
                                        &nbsp; Non Strategic</td>
                                    <td style="text-align:center; vertical-align:middle;">

                                        <span style="width:30px; height:15px; background:gray; display:inline-block; margin-top:5px;">
                                        </span>
                                    </td>
                                    <td>
                                        &nbsp;Vehicle is Halted from 10 minutes to 4 Hours</td>
                                </tr>
                                <tr>
                                    <td style="text-align:center;">
                                        <asp:Image ID="Image8" runat="server" ImageUrl="~/images/lightblue.png" />
                                    </td>
                                    <td>
                                        &nbsp; BSC</td>
                                    <td style="text-align:center; vertical-align:middle;">
                                        <span style="width:30px; height:15px; background:#DB94FF; display:inline-block; margin-top:5px;">
                                        </span>
                                    </td>
                                    <td>
                                        &nbsp;Vehicle is Halted more than 4 Hours</td>
                                </tr>
                                <tr>
                                    <td style="text-align:center;">
                                        <asp:Image ID="Image11" runat="server" ImageUrl="~/images/car4.png" />
                                    </td>
                                    <td >
                                        &nbsp; CM, ZH and Admin Vehical </td>
                                    <td >
                                        &nbsp;</td>
                                    <td >
                                        &nbsp; </td>
                                </tr>
                                <tr>
                                    <td style="text-align:center; vertical-align:middle;">
                                        <span style="padding:5px;">
                                            <asp:Image ID="Image2" runat="server"  ImageUrl="~/images/truck_gray.png" />
                                        </span>
                                    </td>
                                    <td>
                                        &nbsp; Warehouse</td>
                                    <td>
                                        &nbsp;</td>
                                    <td>
                                        &nbsp;</td>
                                </tr>
                                <tr>
                                    <td style="text-align:center;">
                                        <asp:Image ID="Image15" runat="server" ImageUrl="~/images/dfgray.png" />
                                    </td>
                                    <td>
                                        &nbsp; Diesel Filing </td>
                                    <td>
                                        
                                    </td>
                                    <td>
                                        &nbsp;</td>
                                </tr>
                                <tr>
                                    <td style="text-align:center;">
                                        <asp:Image ID="Image3" runat="server" ImageUrl="~/images/mob_gray.png" />
                                    </td>
                                    <td>
                                        &nbsp; Mobile DG</td>
                                    <td>
                                        &nbsp;</td>
                                    <td>
                                        &nbsp;</td>
                                </tr>
                             <tr>
                                    <td style="text-align:center;">
                                        <asp:Image ID="Image4" runat="server" ImageUrl="~/images/sc_gray.png" />
                                    </td>
                                    <td>
                                        &nbsp; SMS</td>
                                    <td>
                                        &nbsp;</td>
                                    <td>
                                        &nbsp;</td>
                                </tr>
                            </table>

                    
                          </td>
                    </tr>
                </table>
                
                
                </ContentTemplate>
        </asp:UpdatePanel>
               </div>

    
  

</asp:Content>
