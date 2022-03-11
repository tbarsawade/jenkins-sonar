<%@ Page Title="" Language="VB" MasterPageFile="~/USR.master" AutoEventWireup="false" CodeFile="TqMapHome.aspx.vb" Inherits="TqMapHome" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="asp" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
     <%--<script src="jquery/jquery-1.3.2.min.js"></script>--%>
    <script src="js/jquery-1.9.1.min.js"></script>
    <script src="js/TqGmap.js"></script>
    
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
            Tqmap.GetMarkers('<%= LstVehicle.ClientID%>');
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

                        var op = '<option  onclick="focusMarker(this);" value=' + myVals[i].value + '  style="color:' + colr + '" >' + myVals[i].text + '</option>';
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

        <asp:UpdatePanel ID="updPnlGrid" runat="server">
            
            <ContentTemplate>
                
                 <table style="width: 100%; height: auto;">
                    <tr>
                        <td style="width: 13%; height: 410px; vertical-align: top;">
                            

                            <div style="width: 100%; height: 560px;" >
                               
<%--                                   
                                    <br />
                                    <legend style="color: Black; text-align: Left; font-weight: bold;">Select Vehicle Type</legend>
                                    <asp:Panel ID="Panel1" runat="server" Height="100px" Style="display: block" ScrollBars="Auto">
                                        <asp:CheckBoxList ID="chkvtype" runat="server" AutoPostBack="false" >
                                        </asp:CheckBoxList>
                                    </asp:Panel>
                                
                                
                                <br />--%>
                                <table>
                                        <colgroup style="width:80%;"></colgroup>
                                        <colgroup style="width:20%"></colgroup>
                                        <tr>
                                            <td>
                                                <%--<span style="color: Black; text-align: Left; font-weight: bold;">Search Vehicle</span>--%>
                                                <span style="color: Black; text-align: Left; font-weight: bold;">
                                                    <asp:Label ID="lblveh" runat="server" Text=""></asp:Label> </span>
                                            </td>
                                            <td>
                                                <img src="images/ref.png" id="img2" onclick="Tqmap.RefreshVechical1(this,'img1');" title="Refresh Vechicals" style="width:25px; height:25px; cursor:pointer;" />
                                                <img src="images/loadmap.gif" id="img1" class="hide" title="Refresh Vechicals" />
                                            </td>
                                        </tr>
                                    </table>
                                    <asp:Panel ID="Panel3" runat="server" Height="210px"  Style="display: block" ScrollBars="Auto">
                                      

                                        <div>
                                            <input type="text" id="VehSearch" placeholder="Type Vechicle name" runat="server" style="width:95%;" onkeyup="return SearchList();" />
                                        </div>
                                        <div >
                                        <asp:ListBox ID="LstVehicle" onchange="focusMarker(this);" runat="server" class="lstContent" Height="150px" Width="98%" DataTextField="Vehicle">
                                        </asp:ListBox>
                                            
                                        </div>
                                    </asp:Panel>

                                
                                
                            </div>
                        </td>
                        <%--</fieldset>--%>
                        <td style="width: 87%; height: 500px;" valign="top" >
                            <div id="map" style="width: 100%; text-align:center;vertical-align:middle ;height: 100%; border:1px solid rgba(0, 0, 0, 0.4); ">
                                <div style="width:100%; height:500px; padding-top:150px; background-color: rgba(104, 149, 166, 0.3);   ">
                                     <img src="images/MapLoading.gif" height="60%" width="40%" />
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
                            <div id="infoChart" runat="server">
                                <table cellspacing="0" celpadding="0" border="1" style="width:100.3%; border:1px solid rgba(0, 0, 0, 0.4); vertical-align:middle;">
                                   
                                    </colgroup>
                                        <colgroup>
                                            <col style="width:80px; text-align:center; " />
                                            <col style="" />
                                            <tr>
                                                <td>
                                                    <asp:Image ID="Image11" runat="server" ImageUrl="~/images/car1.png" />
                                                </td>
                                                <td>&nbsp;Vehicle data not received for more than 24 Hrs.</td>
                                            </tr>
                                            <tr>
                                                <td><asp:Image ID="Image2" runat="server" ImageUrl="~/images/car2.png" /></td>
                                                <td>&nbsp;Vehicle is Moving</td>
                                            </tr>
                                            <tr>
                                                <td><asp:Image ID="Image3" runat="server" ImageUrl="~/images/car4.png" /></td>
                                                <td> &nbsp;Vehicle is Halted from 10 minutes to 4 Hours</td>
                                            </tr>
                                            <tr>
                                                <td><asp:Image ID="Image4" runat="server" ImageUrl="~/images/car5.png" /></td>
                                                <td>&nbsp;Vehicle is Halted more than 4 Hours</td>
                                            </tr>
                                    </colgroup>
                                </table>
                            </div>
                        </td>
                    </tr>
                </table>
                
                
                </ContentTemplate>
        </asp:UpdatePanel>
               </div>
</asp:Content>

