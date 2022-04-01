<%@ page title="" language="VB" masterpagefile="~/USR.master" autoeventwireup="false" inherits="MonthWiseActivity, App_Web_cjg31vo3" viewStateEncryptionMode="Always" %>

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
    <style>
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
        .k-grid-footer
        {
            display: none;
        }
        .red {
  background-color: red;
}

  </style>
    <script type="text/javascript">
      
        function GetReport() {

            var str = '{ "company": "' + $("#ddlCompany").val() + '", "Y1": "' + $("#ddlYear").val() + '"}';
           
            $.ajax({
                type: "POST",
                url: "MonthWiseActivity.aspx/GetReport",
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
                  
                        bindGridDetail($.parseJSON(data.d.Data));
                        $(".k-grid-header tr th").css("text-align", "center");
                    //    $("#kgrid tr").css("background-color", "yellow");
                    }
                },
                error: function (err) { alert('err'); },
                failure: function (response) {

                }
            });

        }

       var onDataBound = function() {
     $('tr td').each(function(){if($(this).text()=='Total'){$(this).addClass('red')}});
      $('td').each(function(){if($(this).text()=='Total'){$(this).addClass('red')}});
       $('th').each(function(){if($(this).text()=='Total'){$(this).addClass('red')}});
   // $('td').each(function(){if($(this).text()=='G'){$(this).addClass('green')}});
    //$('td').each(function(){if($(this).text()=='R+'){$(this).addClass('darkred')}});
      };
      
        function bindGridDetail(Data1) {
        
            //CatName	PID	CatType	CatID	Pname	PDesc	Plike	PVisit	PSell	IsAct	CreatedOn	Price
            $("#kgrid").kendoGrid({
                dataSource: {
                    data: Data1,
                    schema: {
                        model: {
                            fields: {
                                //vtype,Circle,Cluster,VType,VNumIMEI,
                                Month: { type: "string" },
                                Manual: { type: "string" },
                                MPE: { type: "string" },
                                MCON : { type: "string" },
                                Scheduled: { type: "string" },
                                SPE: { type: "string" },
                                SCON : { type: "string" },
                                Total: { type: "number" }
                               
                            }
                        }
                    }
                },
                 dataBound: onDataBound,
                
                height: 550,
                scrollable: true,
                resizable: true,
                reorderable: true,
                sortable: true,
                filterable: true,
               toolbar: ["excel"],

                // buttons:"Expand All",

                excel: {
                    fileName: "MonthlyActivity.xlsx",
                    filterable: true,
                    pageable: true,
                    allPages: true
                },
                columns: [
                        {
                            field: "Month",
                            title: "Month",
                            width: 100
                        },
                         
                          
                           {
                            title: "Manual",
                            columns: 
                            [
                            {
                                field: "MPE",
                                title: "PE",
                                width: 120
                            },
                            {
                                field: "MCON",
                                title: "Con",
                                width: 130
                            }
                         
                         ]
                      },

                        {
                            title: "Scheduled",
                            columns:
                            [
                            {
                                field: "SPE",
                                title: "PE",
                                width: 120
                            },
                            {
                                field: "SCON",
                                 title: "Con",
                                width: 130
                            }

                         ]
                      },
                        {
                            field: "Total",
                            title: "Total",
                            width: 100
                           
                        }],
           
           
     
                //columns: columns,
               
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
           
             $("#pager").kendoPager({
            info: false,
             autoBind: false
              });

              }
             

         var AllVType = "";
      
        function GetCompany() {
            $.ajax({
                type: "POST",
                url: "MonthWiseActivity.aspx/GetCompany",
                contentType: "application/json; charset=utf-8",
                data: {},
                dataType: "json",
                success: function (response) {
                    var res = response.d;
                    //dvsqEditLoader
                    var data = $.parseJSON(res);
                    AllVType=  $("#ddlCompany").kendoMultiSelect({
                        dataTextField: "Company",
                        dataValueField: "Company",
                        dataSource: data,
                    }).data("kendoMultiSelect");
                    //$("#sallVtype").bind("click", { ID: "ddlVType",key:"vtype" }, SelectALL);
                },
                error: function (data) {
                }
            });
        }
       function SelectALLvtype() {
            var multiSelect = $("#ddlCompany").data("kendoMultiSelect");
            var selectedValues = "";
            var strComma = "";
            for (var i = 0; i < multiSelect.dataSource.data().length; i++) {
                var item = multiSelect.dataSource.data()[i];
                selectedValues += strComma + item.Company;
                strComma = ",";
            }
            multiSelect.value(selectedValues.split(","));
        }
      
         $(document).ready(function () {
            GetCompany();
            $("#sallVtype").bind("click", {}, SelectALLvtype);
            });


    </script>
    <br />
    <table style="width: 900px;">
        <tr>
            <td style="width: 10px">
                Company:
            </td>
            <td style="width: 100px">
                <select id="ddlCompany" data-placeholder="Select  Company" style="width: 200px;">
                </select>
            </td>
            <td style="width: 30px">
                <input type="button" class="k-button" id="sallVtype" value="ALL" style="visibility: hidden" />
            </td>
            <td style="width: 10px">
                Year:
            </td>
            <td style="width: 150px">
                <asp:DropDownList ID="ddlYear" ClientIDMode="Static" Style="width: 200px; height: 22px"
                    runat="server">
                    <asp:ListItem Selected="True">2016</asp:ListItem>
                    <asp:ListItem>2017</asp:ListItem>
                    <asp:ListItem>2018</asp:ListItem>
                    <asp:ListItem>2019</asp:ListItem>
                    <asp:ListItem>2020</asp:ListItem>
                </asp:DropDownList>
            </td>
            <td style="width: 100px">
                <input type="button" id="btnReport" class="k-button" role="button" aria-disabled="false"
                    value="Search" tabindex="0" onclick="GetReport()" />
                <asp:Label ID="lblReport" runat="server" Text=""></asp:Label>
            </td>
        </tr>
    </table>
    <br />
    <div style="height:300px">
        <div id="kgrid" style="width: 999px">
        </div>
        <div id="pager">
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
