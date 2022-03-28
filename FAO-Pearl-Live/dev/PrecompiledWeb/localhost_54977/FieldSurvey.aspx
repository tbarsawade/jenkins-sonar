<%@ page title="" language="VB" masterpagefile="~/USR.master" autoeventwireup="false" inherits="FieldSurvey, App_Web_l4hlb3yz" viewStateEncryptionMode="Always" %>

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
        
        #mask
        {
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
        
        #loader
        {
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
        
        .k-grid-header .k-header
        {
            overflow: visible;
            white-space: normal;
        }
        .textbox
        {
            border: 1px solid #848484;
            -webkit-border-radius: 30px;
            -moz-border-radius: 30px;
            border-radius: 30px;
            outline: 0;
            height: 25px;
            width: 275px;
            padding-left: 10px;
            padding-right: 10px;
        }
        .btn
        {
            color: #fff;
            background-color: #2a2d33;
            border: 1px solid #505156;
            background-clip: padding-box;
            position: relative;
            display: inline-block;
            padding: 0.667em 2.000em;
            line-height: 0.933em;
            transition-property: background-color, color;
            transition-duration: 0.2s;
            transition-timing-function: ease;
            border-radius: 2px;
            -webkit-appearance: none;
            font-size: 15px;
            font-weight: bold;
            text-align: center;
            margin: 1.467em 0.267em;
        }
        
        .btn:hover
        {
            background-color: orange;
        }
        
        .btn-primary
        {
            background-color: #ee5315;
            border-color: #ee5315;
            margin-right: 1.200em;
        }
        
        .btn-primary:hover
        {
            background-color: orange;
            border-color: #c2410e;
            cursor: pointer;
        }
    </style>
    <script type="text/javascript">
      
        function GetReport() {

            var str = '{ "d1": "' + $("#ContentPlaceHolder1_txtSdate").val() + '", "d2": "' + $("#ContentPlaceHolder1_txtEdate").val() + '"}';
           
            $.ajax({
                type: "POST",
                url: "FieldSurvey.aspx/GetDSlip",
                data: str,
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function (data) {
                    //$("#popupdivgrd").data("kendoWindow").center().open();
                    $("#kgrid").html("");
                    if (data.d.length == 0) {
                        alert("No Record Found!");
                        return;
                    }
                    else {
                    
                        bindGridDetail($.parseJSON(data.d));
                        $(".k-grid-header tr th").css("text-align", "center");
                    }
                },
                error: function (err) { alert('err'); },
                failure: function (response) {

                }
            });

        }


      
        function bindGridDetail(Data1) {
            //CatName	PID	CatType	CatID	Pname	PDesc	Plike	PVisit	PSell	IsAct	CreatedOn	Price
            $("#kgrid").kendoGrid({
                dataSource: {
                    data: Data1,
                    schema: {
                        model: {
                            fields: {
                                //vtype,Circle,Cluster,VType,VNumIMEI,
                                StoreName: { type: "string" },
                                Zone: { type: "string" },
                                Distributor: { type: "string" },

                                Docomo: { type: "string" },
                                DocomoFPP: { type: "string" },
                                DocomoPP: { type: "string" },
                                DocomoVI: { type: "string" },

                                Vodafone: { type: "string" },
                                VodafoneFPP: { type: "string" },
                                VodafonePP: { type: "string" },
                                VodafoneVI: { type: "string" },

                                 Airtel: { type: "string" },
                                AirtelFPP: { type: "string" },
                                AirtelPP: { type: "string" },
                                AirtelVI: { type: "string" },

                                 IDEA: { type: "string" },
                                IDEAFPP: { type: "string" },
                                IDEAPP: { type: "string" },
                                IDEAVI: { type: "string" },

                                 Other: { type: "string" },
                                OtherFPP: { type: "string" },
                                OtherPP: { type: "string" },
                                OtherVI: { type: "string" }

                            }
                        }
                    }
                },
                height: 550,
                scrollable: true,
                resizable: true,
                reorderable: true,
                sortable: true,
                filterable: true,
                pageable: false,
                toolbar: ["excel"],

                // buttons:"Expand All",

                excel: {
                    fileName: "FieldSurvey.xlsx",
                    filterable: true,
                    pageable: true,
                    allPages: true
                },
                columns: [
                        {
                            field: "StoreName",
                            title: "Store Name",
                            width: 100
                        },
                         {
                            field: "Zone",
                            title: "Zone",
                            width: 100
                        },
                         {
                             field: "Distributor",
                             title: "Distributor",
                             width: 100
                         },
                          
                           {
                            title: "Docomo",
                            columns: 
                            [
                            {
                                field: "DocomoFPP",
                                title: "FootPrint Point",
                                width: 120
                            },
                            {
                                field: "DocomoPP",
                                title: "Placement Point",
                                width: 130
                            },
                          {
                              field: "DocomoVI",
                              title: "Visibility Index",
                              width: 130
                          }
                         
                         ]
                      },

                        {
                            title: "Vodafone",
                            columns:
                            [
                            {
                                field: "VodafoneFPP",
                                title: "FootPrint Point",
                                width: 120
                            },
                            {
                                field: "VodafonePP",
                                 title: "Placement Point",
                                width: 130
                            },
                          {
                              field: "VodafoneVI",
                              title: "Visibility Index",
                              width: 130
                          }

                         ]
                      },

                        {
                            title: "Airtel",
                            
                            columns:
                            [
                            {
                                field: "AirtelFPP",
                               title: "FootPrint Point",
                              
                                width: 120
                            },
                            {
                                field: "AirtelPP",
                                title: "Placement Point",
                                width: 130
                            },
                          {
                              field: "AirtelVI",
                            title: "Visibility Index",
                              width: 130
                          }

                         ]
                      },
                        {
                            title: "IDEA",
                         
                            columns:
                            [
                            {
                                field: "IDEAFPP",
                               title: "FootPrint Point",
                                width: 120,
                               
                            },
                            {
                                field: "IDEAPP",
                                 title: "Placement Point",
                                width: 130
                            },
                          {
                              field: "IDEAVI",
                              title: "Visibility Index",
                              width: 130
                          }

                         ]
                      },
                         {
                             title: "Other",
                             columns:
                            [
                            {
                                field: "OtherFPP",
                              title: "FootPrint Point",

                                width: 120
                            },
                            {
                                field: "OtherPP",
                                title: "Placement Point",
                                width: 130
                            },
                          {
                              field: "OtherVI",
                               title: "Visibility Index",
                              width: 130
                          }

                         ]
                         }
                        
                        ],
           
     
                //columns: columns,
                pageable: true,
                pageSize: 20,
                scrollable: true,
                
                reorderable: true,
                columnMenu: true,
                //dataBound: onDataBound,
                groupable: true,
                sortable: true,
                filterable: true,
                resizable: true,
                toolbar: ['excel'],
                excel: {
                    fileName: "Report.xlsx",
                    filterable: true,
                    pageable: true,
                    allPages: true
                }

            });

        }

    </script>
    <div id="divFilters">
        <table style="width: 100%;">
            <tr>
                <td>
                    From : &nbsp;
                    <input id="currency" type="text" value="30" min="0" max="100" style="display: none;"
                        data-role="numerictextbox" role="spinbutton" class="k-input" aria-valuemin="0"
                        aria-valuemax="100" aria-valuenow="30" aria-disabled="false" aria-readonly="false">
                    <asp:TextBox ID="txtSdate" Style="width: 200px;" runat="server" class="k-input"></asp:TextBox>&nbsp;&nbsp;
                    <asp:CalendarExtender ID="calendersdate" TargetControlID="txtsdate" Format="yyyy-MM-dd"
                        runat="server" />
                    &nbsp; To : &nbsp;
                    <asp:TextBox ID="txtEdate" Style="width: 200px;" runat="server" class="k-input"></asp:TextBox>
                    <asp:CalendarExtender ID="CalendarExtender1" TargetControlID="txtEdate" Format="yyyy-MM-dd"
                        runat="server" />
                    &nbsp;&nbsp;
                    <input type="button" id="btnReport" class="k-button" role="button" aria-disabled="false"
                        value="Search" tabindex="0" onclick="GetReport()" />
                    <asp:Label ID="lblReport" runat="server" Text=""></asp:Label>
                </td>
            </tr>
        </table>
    </div>
    <div>
        <div id="kgrid" style="width: 999px">
        </div>
    </div>
    <div id="mask">
        <div id="loader">
            <img src="images/loading.gif" />
        </div>
    </div>
    <asp:UpdateProgress ID="UpdateProgress1" runat="server">
        <ProgressTemplate>
            <div id="Layer1" style="position: absolute; z-index: 9999999; left: 50%; top: 50%">
                <%--      <asp:Image ID="Image1" runat="server" Height="25px" ImageUrl="~/images/attch.gif" />--%>
                please wait...
            </div>
        </ProgressTemplate>
    </asp:UpdateProgress>
</asp:Content>
