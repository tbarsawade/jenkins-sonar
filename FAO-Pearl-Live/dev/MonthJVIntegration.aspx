<%@Page Title="" Language="VB" MasterPageFile="~/usrFullScreenBPM.master" AutoEventWireup="false" CodeFile="MonthJVIntegration.aspx.vb" Inherits="MonthJVIntegration" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="asp" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">

    <link href="kendu/styles/kendo.common.min.css" rel="stylesheet" />
    <link href="kendu/styles/kendo.default.min.css" rel="stylesheet" />
    <link href="kendu/styles/kendo.dataviz.min.css" rel="stylesheet" />
    <link href="kendu/styles/kendo.dataviz.default.min.css" rel="stylesheet" />
    <script src="kendu/js/jquery.min.js"></script>
    <script src="kendu/js/kendo.all.min.js"></script>
    <link href="kendu/styles/kendo.common.min.css" rel="stylesheet" />
    <link href="kendu/styles/kendo.rtl.min.css" rel="stylesheet" />
    <script src="kendu/content/shared/js/console.js"></script>
    <script src="kendu/js/jszip.min.js"></script>
   

 
<style type="text/css">
        #divFilters 
        {
            padding: 15px;
            background: #F7F7F7;
            min-height: 60px; /*width:1000px;*/
            box-sizing: border-box;
        }

        #mask {
            position: fixed;
            top: 0;
            left: 0;
            right: 0;
            bottom: 0;
            background-color: rgba(0, 0, 0, 0.59);
            z-index: 10000;
            height: 100%;
            display: none;
            opacity: 0.9;
        }

        #loader {
            position: absolute;
            left: 50%;
            top: 50%;
            width: 200px;
            height: 70px;
            background: none;
            margin: -100px 0 0 -100px;
            display: none;
            padding-top: 25px;
        }

        .k-grid-header .k-header {
            overflow: visible;
            white-space: normal;
        }


        .sch {
            border: 1px solid #333;
            padding: 3px 3px;
            border-radius: 5px;
            margin: 6px 0px 0px;
        }

        .mg {
            margin: 10px 0px;
        }

        .loader {
            border: 16px solid #f3f3f3;
            border-radius: 50%;
            border-top: 16px solid #3498db;
            width: 120px;
            height: 120px;
            -webkit-animation: spin 2s linear infinite; /* Safari */
            animation: spin 2s linear infinite;
        }

        /* Safari */
        @-webkit-keyframes spin {
            0% {
                -webkit-transform: rotate(0deg);
            }

            100% {
                -webkit-transform: rotate(360deg);
            }
        }

        @keyframes spin {
            0% {
                transform: rotate(0deg);
            }

            100% {
                transform: rotate(360deg);
            }
        }
    </style>


 <script type="text/javascript">


        function GetReport() {
            $("#dvloader").show();
            var t1 = $("#txtd1").val();
            var t2 = $("#txtd2").val();
            $("#kgrdtl").html('');

            var str = '{"sdate": "' + t1 + '", "tdate": "' + t2 + '"}';


            $.ajax({
                type: "POST",
                url: "MonthJVIntegration.aspx/GetData",
                data: str,
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: BindReport,
                error: function (err) { alert('Error in processing report, please contact system admin'); },
                failure: function (response) {

                }
            });

        }
     
        function BindReport(result) {

            if (result.d.Success == false)
            {
                $("#kgrid").html('<span style="color:red; font-weight:bold;">' + result.d.Message + '</span>');
                $("#dvloader").hide();
                return;
            }   
        }

    

 </script>

        <div id="divProgress" style="text-align: center; display: none;">
            <div class="loader"></div>
        </div>

    <div class="container-fluid">      
        <div class="form" style="text-align: left">
            <div class="doc_header">
                Month JV Report
            </div>
            <div class="row mg">
                        <div class="col-md-2 col-sm-2">
                            <label>Select GL Date:</label>
                        </div>
                        <div class="col-md-2 col-sm-2">
                            <asp:TextBox runat="server" ID="txtd1" placeholder="Start Date" ReadOnly="true" CssClass="txtBox"
                                ClientIDMode="Static"></asp:TextBox>
                            <asp:CalendarExtender ID="CalendarExtender3" runat="server" Format="yyyy-MM-dd" TargetControlID="txtd1">
                            </asp:CalendarExtender>
                        </div>
               
                        <div class="col-md-2 col-sm-2">
                            <asp:Button ID="btnSearch" Height="30px" runat="server" Text="Run Integration" OnClientClick="GetReport(); return false;" />
                        </div>
            </div>

            <div class="row mg">
                        <div class="col-md-12 col-sm-12">
                             <div id="dvloader" style="display: none; text-align: center; height: 35px; padding-top: 10px;">
                                   <input type="image" src="../images/preloader22.gif" />
                               </div>
                             <div id="kgrid" style="width: 100%">
                             </div>
                        </div>
            </div>

            <asp:UpdateProgress ID="UpdateProgress1" runat="server">
                    <ProgressTemplate>
                        <div id="Layer1" style="position: absolute; z-index: 9999999; left: 50%; top: 50%">
                               please wait...
                        </div>
                    </ProgressTemplate>
            </asp:UpdateProgress>
        </div>
    </div>

</asp:Content>