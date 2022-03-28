<%@ page title="" language="VB" masterpagefile="~/PublicMaster.master" autoeventwireup="false" inherits="DevSMS, App_Web_iqn0gzeb" viewStateEncryptionMode="Always" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="asp" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
<%--<script type="text/javascript" src="https://ajax.googleapis.com/ajax/libs/jquery/1.8.3/jquery.min.js"></script>--%>
    <script src="jquery/jquery-3.3.1.js" type="text/javascript"></script>
    <script src="jquery/jquery-3.3.1.min.js" type="text/javascript"></script>
<script type="text/javascript">
    function ShowProgress() {
        setTimeout(function () {
            var modal = $('<div />');
            modal.addClass("modal");
            $('body').append(modal);
            var loading = $(".loading");
            loading.show();
            var top = Math.max($(window).height() / 2 - loading[0].offsetHeight / 2, 0);
            var left = Math.max($(window).width() / 2 - loading[0].offsetWidth / 2, 0);
            loading.css({ top: top, left: left });
        }, 200);
    }
    $('form').live("submit", function () {
        ShowProgress();
    });
</script>
    <style type="text/css">
        .gradientBoxesWithOuterShadows
        {
            height: 100%;
            width: 99%;
            padding: 5px;
            background-color: white; /* outer shadows  (note the rgba is red, green, blue, alpha) */
            -webkit-box-shadow: 0px 0px 12px rgba(0, 0, 0, 0.4);
            -moz-box-shadow: 0px 1px 6px rgba(23, 69, 88, .5); /* rounded corners */
            -webkit-border-radius: 12px;
            -moz-border-radius: 7px;
            border-radius: 7px; /* gradients */
            background: -webkit-gradient(linear, left top, left bottom, 
color-stop(0%, white), color-stop(15%, white), color-stop(100%, #D7E9F5));
            background: -moz-linear-gradient(top, white 0%, white 55%, #D5E4F3 130%);
        }
        .modal
        {
            position: fixed;
            top: 0;
            left: 0;
            background-color: black;
            z-index: 99;
            opacity: 0.8;
            filter: alpha(opacity=80);
            -moz-opacity: 0.8;
            min-height: 100%;
            width: 100%;
        }
        .loading
        {
            font-family: Arial;
            font-size: 10pt;
            border: 5px solid #67CFF5;
            width: 200px;
            height: 100px;
            display: none;
            position: fixed;
            background-color: White;
            z-index: 999;
        }
    </style>
    <div class="gradientBoxesWithOuterShadows" style="margin-top: 10px; width: 1000px;
        height: 600px">
        <table style="margin-top: 180px; margin-left: 120px">
            <tr>
                <td>
                    <asp:Label ID="lbltxt" runat="server" Text="Enter Mobile Number :" Style="font-weight: 700"
                        Font-Names="Arial"></asp:Label>
                </td>
                <td>
                    <asp:TextBox ID="txtMSG" runat="server" Width="450px" BorderStyle="Solid" BorderColor="Black"
                        TextMode="MultiLine"></asp:TextBox>
                </td>
                <td>
                    <asp:Button ID="btnSubmit" runat="server" Text="Send" BorderColor="#003300" BorderStyle="Groove"
                        Font-Bold="True" ForeColor="Black" Width="70px" Height="30px" />
                </td>
            </tr>
        </table>
        <div class="loading" align="center">
            Sending... Please wait.<br />
            <br />
            <img alt="Sending..." src="images/spinner4-black.gif" />
        </div>
        <table style="margin-top: 280px; margin-left: 400px">
            <tr>
                <td>
                    <asp:Label ID="Label1" runat="server" Text="Note: Only for Teltonika devices" Font-Bold="True"
                        Font-Overline="False" Font-Size="X-Small" Font-Underline="True" ForeColor="#FF6600"></asp:Label>
                </td>
            </tr>
        </table>
    </div>
</asp:Content>
