<%@ Page Title="" Language="VB" MasterPageFile="~/usrFullScreen.master"  AutoEventWireup="false" CodeFile="FileExplorerEcompliance.aspx.vb" Inherits="FileExplorerEcompliance" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
    <link href="kendu/styles/kendo.common.min.css" rel="stylesheet" />
    <link href="kendu/styles/kendo.rtl.min.css" rel="stylesheet" />
    <link href="kendu/styles/kendo.default.min.css" rel="stylesheet" />
    <link href="kendu/styles/kendo.dataviz.min.css" rel="stylesheet" />
    <link href="kendu/styles/kendo.dataviz.default.min.css" rel="stylesheet" />
    <%--<script src="kendu/js/jquery.min.js"></script>--%>
    <script src="kendu/js/kendo.all.min.js"></script>
    <script src="kendu/content/shared/js/console.js"></script>
    <%--<script src="scripts/jquery-1.9.1.min.js" type="text/javascript"></script>
    <script src="scripts/jquery-ui-1.10.2.custom.min.js" type="text/javascript"></script>--%>
    <link href="css/style.css" rel="stylesheet" />
    <style type="text/css">
        .mycontent {
    background-color: #ffffff;
    border-radius: 8px;
    box-shadow: 0 0 5px #a0a0a0;
    margin: 120px auto 20px !important;
    min-height: 430px !important;
    padding: 15px;
    width: 100% !important;
}
        .modalBackground {
            background-color: Black;
            filter: alpha(opacity=60);
            opacity: 0.6;
        }

        .modalPopup {
            background-color: #FFFFFF;
            width: 350px;
            border: 3px solid #0DA9D0;
            border-radius: 12px;
            padding: 0;
        }

            .modalPopup .header {
                background-color: red;
                height: 30px;
                color: White;
                line-height: 30px;
                text-align: center;
                font-weight: bold;
                border-top-left-radius: 6px;
                border-top-right-radius: 6px;
            }

            .modalPopup .body {
                min-height: 50px;
                line-height: 30px;
                text-align: center;
                font-weight: bold;
            }

            .modalPopup .footer {
                padding: 6px;
            }

            .modalPopup .yes, .modalPopup .no {
                height: 23px;
                color: White;
                line-height: 23px;
                text-align: center;
                font-weight: bold;
                cursor: pointer;
                border-radius: 4px;
            }

            .modalPopup .yes {
                background-color: #598526;
                border: 1px solid #5C5C5C;
            }

            .modalPopup .no {
                background-color: #598526;
                border: 1px solid #5C5C5C;
            }

        .menu-bar {
            width: 95%;
            margin: 4px 0px 0px 0px;
            padding: 1px 1px 1px 1px;
            height: auto;
            line-height: inherit;
            border-radius: 10px;
            -webkit-border-radius: 10px;
            -moz-border-radius: 10px;
            box-shadow: 2px 2px 3px #666666;
            -webkit-box-shadow: 2px 2px 3px #666666;
            -moz-box-shadow: 2px 2px 3px #666666;
            background: #8B8B8B;
            background: linear-gradient(top, #ccc7c5, #7A7A7A);
            background: -ms-linear-gradient(top, #ccc7c5, #7A7A7A);
            background: -webkit-gradient(linear, left top, left bottom, from(#ccc7c5), to(#7A7A7A));
            background: -moz-linear-gradient(top, #ccc7c5, #7A7A7A);
            border: solid 1px #6D6D6D;
            position: relative;
            /*z-index:999;
  -webkit-z-index:999;*/
        }

            .menu-bar li {
                margin: 0px 0px 1px 0px;
                padding: 0px 1px 0px 1px;
                float: left;
                position: relative;
                list-style: none;
            }

            .menu-bar a {
                text-decoration: none;
                padding: 1px 2px 1px 2px;
                margin-bottom: 1px;
                border-radius: 1px;
                -webkit-border-radius: 1px;
                -moz-border-radius: 1px;
                font-weight: bold;
                font-family: arial;
                font-style: normal;
                font-size: 12px;
                color: #d0e41f;
                text-shadow: 1px 1px 1px #000000;
                display: block;
                margin: 0;
                margin-bottom: 1px;
                border-radius: 4px;
                -webkit-border-radius: 4px;
                -moz-border-radius: 4px;
                text-shadow: 2px 2px 3px #000000;
            }

            .menu-bar li ul li a {
                margin: 0;
            }

            .menu-bar .active a, .menu-bar li:hover > a {
                background: #0399D4;
                background: linear-gradient(top, #EB2F2F, #960000);
                background: -ms-linear-gradient(top, #EB2F2F, #960000);
                background: -webkit-gradient(linear, left top, left bottom, from(#EB2F2F), to(#960000));
                background: -moz-linear-gradient(top, #EB2F2F, #960000);
                color: #F2F2F2;
                -webkit-box-shadow: 0 1px 1px rgba(0, 0, 0, .2);
                -moz-box-shadow: 0 1px 1px rgba(0, 0, 0, .2);
                box-shadow: 0 1px 1px rgba(0, 0, 0, .2);
                text-shadow: 2px 2px 3px #FFA799;
            }

            .menu-bar ul li:hover a, .menu-bar li:hover li a {
                background: none;
                border: none;
                color: #666;
                -box-shadow: none;
                -webkit-box-shadow: none;
                -moz-box-shadow: none;
            }

            .menu-bar ul a:hover {
                background: #0399D4 !important;
                background: linear-gradient(top, #EB2F2F, #960000) !important;
                background: -ms-linear-gradient(top, #EB2F2F, #960000) !important;
                background: -webkit-gradient(linear, left top, left bottom, from(#EB2F2F), to(#960000)) !important;
                background: -moz-linear-gradient(top, #EB2F2F, #960000) !important;
                color: #FFFFFF !important;
                border-radius: 0;
                -webkit-border-radius: 0;
                -moz-border-radius: 0;
                text-shadow: 2px 2px 3px #FFA799;
            }

            .menu-bar ul {
                background: #DDDDDD;
                background: linear-gradient(top, #FFFFFF, #CFCFCF);
                background: -ms-linear-gradient(top, #FFFFFF, #CFCFCF);
                background: -webkit-gradient(linear, left top, left bottom, from(#FFFFFF), to(#CFCFCF));
                background: -moz-linear-gradient(top, #FFFFFF, #CFCFCF);
                display: none;
                margin: 0;
                padding: 0;
                width: 0px;
                position: absolute;
                /*top: 10px;*/
                left: 0;
                border: solid 1px #B4B4B4;
                border-radius: 10px;
                -webkit-border-radius: 10px;
                -moz-border-radius: 10px;
                -webkit-box-shadow: 2px 2px 3px #222222;
                -moz-box-shadow: 2px 2px 3px #222222;
                box-shadow: 2px 2px 3px #222222;
            }

            .menu-bar li:hover > ul {
                display: block;
            }

            .menu-bar ul li {
                float: none;
                margin: 0;
                padding: 0;
            }

        .menu-bar {
            display: inline-block;
        }

        html[xmlns] .menu-bar {
            display: block;
        }

        * html .menu-bar {
            height: 1%;
        }

        .doclink li {
            list-style: none;
            text-decoration: none;
            color: #000;
            padding: 6px 5px;
            border-bottom: 1px solid rgba(0, 0, 0, .2);
        }

            .doclink li a {
                text-decoration: none;
            }

                .doclink li a:hover {
                    text-decoration: none;
                }

            .doclink li:hover {
                background: #598526;
                color: #fff !important;
            }


        .mar_botm {
            margin-bottom: 10px;
            border: 1px solid green;
            text-align: left;
        }

        /*.mar_botm div {
            border: 1px solid green; overflow: auto; width: 100%; text-align: left; padding-left: 0px;display:none; 
        }*/
        .divCustom {
            border: 1px solid green;
            overflow: auto;
            width: 100%;
            text-align: left;
            padding-left: 0px;
            display: none;
        }

        .divCustom1 {
            border: 1px solid green;
            overflow: auto;
            width: 100%;
            text-align: left;
            padding-left: 0px;
        }

        .loader {
            background: url(images/loading12.gif) no-repeat center center;
            width: 100%;
            height: 50px;
            text-align: center;
        }

        .loader1 {
            background-image: url("images/loading.gif");
            background-repeat: no-repeat;
            background-position: center center;
            position: relative;
            top: calc(50% - 16px);
            display: block;
            height: 50px;
        }

        .tdhover {
            background: #c6def5;
        }

            .tdhover:hover {
                color: #3585f3;
                text-underline-position: below;
            }
    </style>

   <%--   <div id="mask" runat="server" visible ="false" >
        <div id="loader" >
            <img src="images/preloader22.gif" alt="" />
        </div>
    </div>--%>

    <div class="doc_header" style="background: #e96125; color: #fff; font: bold 17px verdana; height: 47px; padding-left: 10px; padding-top: 15px; width: 100%; border-radius: 5px;">File Explorer</div>

    <br />
    <div class='loader' id="ldrdv"></div>
    <div id="example" style="display:inline-block;width:100%;" >
        <div class="demo-section k-content" style="width: 25%; border: solid 1px darkgray; float: left; overflow-y: auto; overflow-x: hidden; height: 90%;">
          
            <div id="treeview-right" style="padding-top: 10px; padding-left: 2px;height:90%;overflow-x:scroll;overflow-y:scroll;"> 
                   <asp:TreeView ID="tvData" runat="server" AutoGenerateDataBindings="False" AutoPostBack="false" Width="300px" >
    </asp:TreeView>

            </div>
        </div>
        <div id="example1" style="margin: 0px 0 0 0; width: 75%; float: left; padding-left: 11px;">
            <div id="Layer1" style="position: absolute; display: none; z-index: 1; left: 50%; top: 50%; z-index: 10001">
                <input type="image" id="Imageprog" src="../../images/prg.gif" style="height: 25px;" />
            </div>
            <div id="kgrd" style="display: block; width: 100%; font-size: 14px !important;"></div>
              <div id="NoRecord1" style="display: none; text-align: center; min-height: 80px;border:inset;">
                                <span style="color: red; position: relative; top: 30px;">No record found</span>
                            </div>
        </div>
    </div>
     <div id="details">
    </div>
    <script type="text/javascript">
        $(document).ready(function () {
            //ldrdv example
            //$("#wrap").css("width", "100%");
            //var div = $("div .content");
            //div.addClass("mycontent");
            //div.removeClass("content");
            //.removeClass("");
            //mycontent
            $("#ldrdv").show();
          // $("#example").show();
           $('#Layer1').show();
            //$.ajax({
            //    type: "POST",
            //    contentType: "application/json; charset=utf-8",
            //    url: "/FileExplorerEcompliance.aspx/GetData",
            //    global: false,
            //    data: [],
            //    dataType: "json",
            //    async: true,
            //    success: function (data) {
            //        var arr = [];
            //        var test = data.d;
            //        var ds = data.d.items;
            //        //alert(ds);
            //        $("#treeview-right").kendoTreeView({
            //            dataSource: [{ tid: 1, text: "Act Document", expanded: true, items: ds }],
            //            select: onSelect
            //        });
            //        $("#ldrdv").hide();
            //        $("#example").show();
            //    }

            //});
            $('#Layer1').hide();
            $("#ldrdv").hide();
            //var kGrid
        
           
        });
        //var UPload1Handler1 = function UPload1Handler1(e) {
        //    e.preventDefault();
        //    var dataItem = {};
        //    dataItem = this.dataItem($(e.currentTarget).closest("tr"));
        //    var FileUpload = dataItem.FileUpload1
        //    if (FileUpload != '') {
        //        window.open('../../docs/' + FileUpload + '');
        //    }
        //    else { alert('File does not exist.'); }
        //}
        //var UPload1Handler2 = function UPload1Handler1(e) {
        //    e.preventDefault();
        //    var dataItem = {};
        //    dataItem = this.dataItem($(e.currentTarget).closest("tr"));
        //    var FileUpload = dataItem.FileUpload2
        //    if (FileUpload != '') {
        //        window.open('../../docs/' + FileUpload + '');
        //    }
        //    else { alert('File does not exist.'); }
        //}
        //var UPload1Handler3 = function UPload1Handler1(e) {
        //    e.preventDefault();
        //    var dataItem = {};
        //    dataItem = this.dataItem($(e.currentTarget).closest("tr"));
        //    var FileUpload = dataItem.FileUpload3
        //    if (FileUpload != '') {
        //        window.open('../../docs/' + FileUpload + '');
        //    }
        //    else { alert('File does not exist.'); }
        //}
        //var UPload1Handler4 = function UPload1Handler1(e) {
        //    e.preventDefault();
        //    var dataItem = {};
        //    dataItem = this.dataItem($(e.currentTarget).closest("tr"));
        //    var FileUpload = dataItem.FileUpload4
        //    if (FileUpload != '') {
        //        window.open('../../docs/' + FileUpload + '');
        //    }
        //    else { alert('File does not exist.'); }
        //}
        //var UPload1Handler5 = function UPload1Handler1(e) {
        //    e.preventDefault();
        //    var dataItem = {};
        //    dataItem = this.dataItem($(e.currentTarget).closest("tr"));
        //    var FileUpload = dataItem.FileUpload5
        //    if (FileUpload != '') {
        //        window.open('../../docs/' + FileUpload + '');
        //    }
        //    else { alert('File does not exist.'); }
        //}
        function test(tid)
        { alert("hi"); }
        var kGrid
        //CompanyID As String, siteID As String, actid As String, year As String, month As String
        function onSelect(companyid,siteid,actid,year,month) {
            //kendoConsole.log("Selecting: " + this.text(e.node));
            //var data = $('#treeview-right').data('kendoTreeView').dataItem(e.node);
            //var tid = data.WhereCondition;
            
            if (companyid != undefined ) {
                $('#Layer1').show();
               // if ($('#kgrd').data() != undefined && $(kGrid) != undefined ) {

                if ($(kGrid).length > 0) {
                    $('#kgrd').empty();
                    if ($('#kgrd').data().kendoGrid = undefined) {

                        $('#kgrd').data().kendoGrid.destroy();
                       
                    }
                    }
               // }

                $.ajax({
                    type: "POST",
                    contentType: "application/json; charset=utf-8",
                    url: "FileExplorerEcompliance.aspx/GetDocument",
                    global: false,
                    data: '{CompanyID: "' + companyid + '",siteID:"' + siteid + '" ,actid:"' + actid + '",year:"' + year + '",month:"' + month + '"}',
                    dataType: "json",
                    async: true,
                    success: function (data) {
                        var ds = JSON.parse(data.d).data;
                        if (ds.length > 0) {
                            $('#NoRecord1').hide();
                            kGrid = $("#kgrd").kendoGrid({
                                dataSource: {
                                    pageSize: 10,
                                    data: ds
                                },
                                dataBound: function (e) {
                                    var grid = e.sender;
                                    if (grid.dataSource.total() == 0) {
                                        var colCount = grid.columns.length;
                                        $(e.sender.wrapper)
                                            .find('tbody')
                                            .append('<tr class="kendo-data-row"><td colspan="' + colCount + '" class="no-data"><span style="margin-left:46%;">No data found.</span></td></tr>');
                                    }
                                },
                                columns: [

                                           { field: "COMPANY", title: "Company", width: 120 },
                                           { field: "Site", title: "Site", width: 150 },
                                           { field: "Act", title: "Act", width: 120 },
                                           //{ field: "Activity", title: "Activity", width: 120 },
                                           { field: "CreationDate", title: "Creation Date", width: 150 },
                                            { command: { text: 'View Details', click: showDetails } }
                                        
                                ],
                                pageable: true,
                                filterable: true,
                                sortable: true,
                                resizable: true
                            });
                            $('#Layer1').hide();
                        }
                        else {
                            $('#NoRecord1').show();
                            $('#Layer1').hide();
                        }
                    },
                    error: function (data) {
                        alert(data.error);
                        $('#Layer1').hide();
                       
                        
                    }
                      
                });
               
                wnd = $("#details")
              .kendoWindow({
                  title: "Document Details",
                  modal: true,
                  visible: false,
                  resizable: false,
                  width: 900
              }).data("kendoWindow");

                detailsTemplate = kendo.template($("#popup_editor").html());
            }

        }
        function showDetails(e) {
            e.preventDefault();

            var dataItem = this.dataItem($(e.currentTarget).closest("tr"));
            // alert(dataItem.Tid);
            wnd.content(detailsTemplate(dataItem));
            wnd.center().open();
        }
        function UPload1Handler1(FileUpload) {

            if (FileUpload != '') {
                window.open('../../docs/' + FileUpload + '');
            }
            else { alert('File does not exist.'); }
        }
       
    </script>
    <script id="popup_editor" type="text/x-kendo-template"> 
              
         <div id="details-container">
        <table style="width:100%;">
              <tr>
                  <td align="left" style="padding-left: 20px" width="30%"><b>Company:</b></td>
                  <td align="left" width="70%"><h2>#= COMPANY #</h2></td>
              </tr>
              <tr style="height:10px;">
                  <td style="padding-left: 20px"></td>
                  <td></td>
              </tr>
              <tr>
                  <td align="left" style="padding-left: 20px" width="30%"><b>Site:</b></td>
                  <td align="left" width="70%"><h2>#= Site #</h2></td>
              </tr>
        <tr style="height:10px;">
                  <td style="padding-left: 20px"></td>
                  <td></td>
              </tr>
              <tr>
                  <td align="left" style="padding-left: 20px" width="30%"><b>Act:</b></td>
                  <td align="left" width="70%"><h2>#= Act #</h2></td>
              </tr>
        <tr style="height:10px;">
                  <td style="padding-left: 20px"></td>
                  <td></td>
              </tr>
             
             
     
              <tr >
                  <td align="left" style="padding-left: 20px" width="30%">
        </td>
                  <td align="left"  width="70%">
        #if(File1 === '')
        {# #}
        else{#  <a class="k-button" title="Click to download attachment" onclick="return UPload1Handler1('#=File1#')"  >#=File1D#</a> #}#
         #if(File2 === '')
        {##}
        else{#  <a class="k-button" title="Click to download attachment" onclick="return UPload1Handler1('#=File2#')">#=File2D#</a> #}#
         #if(File3 === '')
        {##}
        else{#   <a class="k-button"  title="Click to download attachment" onclick="return UPload1Handler1('#=File3#')">#=File3D#</a> #}#
         #if(File4 === '')
        {##}
        else{#     <a class="k-button" title="Click to download attachment"  onclick="return UPload1Handler1('#=File4#')">#=File4D#</a> #}#
         #if(File5 === '')
        {##}
        else{#  <a class="k-button" title="Click to download attachment"  onclick="return UPload1Handler1('#=File5#')">#=File5D#</a> #}
        #
      
       
        
     
        

        </td>
              </tr> 
        
          </table>
                 
        
       
        
      
                    
                </div> 
     
    </script>
    <style type="text/css">
        #details-container {
            padding: 10px;
        }

            #details-container h2 {
                margin: 0;
                color: black;
            }
    </style>
</asp:Content>

