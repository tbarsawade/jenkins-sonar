<%@ page title="" language="VB" masterpagefile="~/USR.master" autoeventwireup="false" inherits="RoleWiseRights, App_Web_tuuwcp04" viewStateEncryptionMode="Always" %>


<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
    <link rel="stylesheet" href="kendu/homekendo.common.min.css" />
    <link rel="stylesheet" href="kendu/homekendo.rtl.min.css" />
    <link href="kendu/styles/kendo.default.min.css" rel="stylesheet" />
    <link rel="stylesheet" href="kendu/homekendo.mobile.all.min.css" />
    <script src="jquery/jquery-3.3.1.js" type="text/javascript"></script>
    <script src="jquery/jquery-3.3.1.min.js" type="text/javascript"></script>
    <script src="Jquery/jquery-ui-v1.12.1.js" type="text/javascript"></script>
    <script src="kendu/homekendo.all.min.js"></script>
    <script src="kendu/content/shared/js/console.js"></script>
    <script src="kendu/js/jszip.min.js"></script>
    <link href="css/style.css" rel="stylesheet" />
    <%-- <asp:Panel ID="pnlmenu" runat="server" Width="100%" Height="800px" ScrollBars="Auto">
        <asp:GridView ID="grdMenuData" runat="server" CellPadding="4" ForeColor="#333333" GridLines="None">
            <AlternatingRowStyle BackColor="White" />
            <EditRowStyle BackColor="#2461BF" />
            <FooterStyle BackColor="#507CD1" Font-Bold="True" ForeColor="White" />
            <HeaderStyle BackColor="#507CD1" Font-Bold="True" ForeColor="White" />
            <PagerStyle BackColor="#2461BF" ForeColor="White" HorizontalAlign="Center" />
            <RowStyle BackColor="#EFF3FB" />
            <SelectedRowStyle BackColor="#D1DDF1" Font-Bold="True" ForeColor="#333333" />
            <SortedAscendingCellStyle BackColor="#F5F7FB" />
            <SortedAscendingHeaderStyle BackColor="#6D95E1" />
            <SortedDescendingCellStyle BackColor="#E9EBEF" />
            <SortedDescendingHeaderStyle BackColor="#4870BE" />
        </asp:GridView>
    </asp:Panel>--%>
    <div style="text-align: center;width:100%" >
        <div style="width:22%; float:left;">
            <select id="ddlOption" class="k-select" style="width: 90%">
            </select>
        </div>
        <div id="User" style="width:22%;float:left;">
            <select id="ddlUser" class="k-select" style="width: 90%">
            </select>
        </div>
        <div id="Search" style="width:22%;float:left;">
            <input type="text" id="txtSearch" class="k-textbox" style="width:90%" />
        </div>
        <div style="width:22%;float:right;">
            <input type="button" id="btnSearch" value="Search" class="k-button" style="width:50%"  />
        </div>

    </div>

  
    <div id="kgrd" style="margin-top:5% !important;">
    </div>
    <div id="dvloader" style="position: absolute; display: none; z-index: 1; left: 50%; top: 50%; z-index: 10001">
        <input type="image" id="Imageprog" src="images/prg.gif" style="height: 25px;" />
    </div>
    <script type="text/javascript">
        function HideShow(selectedIndex) {
            if (selectedIndex == 0) {
                $("#User").hide();
                $("#Search").hide();
            }
            else if (selectedIndex == 1) {
                $("#User").show();
                $("#Search").hide();
            }
            else {
                $("#Search").show();
                $("#User").hide();
            }
        }
        $("#ddlOption").change(function () {
            var index = $('option:selected', this).index();
            HideShow(index);

        });
        function BindUser() {
            $.ajax({
                type: "POST",
                url: "RoleWiseRights.aspx/GetUser",
                contentType: "application/json; charset=utf-8",
                data: null,
                dataType: "json",
                success: function (response) {
                    var res = response.d;
                    //dvsqEditLoader
                    var data = $.parseJSON(res.ds);

                    $("#ddlUser").kendoDropDownList({
                        dataTextField: "username",
                        dataValueField: "userid",
                        dataSource: data,
                    });
                },
                error: function (data) {
                }
            });
        }

    </script>
    <script type="text/javascript">
        $(document).ready(function () {
            debugger;
            $("#ddlOption").append($("<option>Select Searh Option</option><option>Master Data</option><option>Comma Seperated UID</option>"));
            $("#ddlOption").kendoDropDownList();
            var index = $("#ddlOption option:selected").index();
            HideShow(index);
            BindUser();
            $("#btnSearch").click(function () {
                var index = $("#ddlOption option:selected").index();
                if (index == 0)
                {
                    alert('Please Select Any One Search Option');
                }
                else if (index == 1) {
                    GetData($("#ddlUser").val());
                }
                else {
                    if ($("#txtSearch").val() == "") {
                        alert("Please Fill Comma Seperated UID in Text Box");
                    }
                    else {
                        var data = "'" + $("#txtSearch").val() + "'";
                        GetData(data);
                    }
                }
                
            });
        });


        var kGrid
        function GetData(uid) {
            $("#dvloader").show();
            //var dataItem = this.dataItem(e.item.index());
            if ($(kGrid).length > 0) {
                $('#kgrd').empty();
                kGrid.data().kendoGrid.destroy();
            }

            $.ajax({
                type: "POST",
                contentType: "application/json; charset=utf-8",
                url: "RoleWiseRights.aspx/GetRoleMenuData",
                global: false,
                data: '{ UID: ' + uid + ' }',
                dataType: "json",
                async: true,
                success: function (data) {
                    //  var ds = JSON.parse(data.d);

                    var ds = $.parseJSON(data.d.data);
                    var columns = data.d.columns;
                    //var aggregate = data.d.aggregate;
                    kGrid = $("#kgrd").kendoGrid({
                        toolbar: ['excel'],
                        excel: {
                            fileName: "Report.xlsx",
                            filterable: true,
                            pageable: true,
                            allPages: true
                        },
                        dataSource: {
                            pageSize: 20,
                            data: ds
                            //aggregate: aggregate
                        },

                        columns: columns,
                        pageable: true,
                        filterable: true,
                        sortable: true,
                        resizable: true
                    });
                    $("#dvloader").hide();
                    $("#kgrd").show();
                    $("#dvNoRecord").hide();
                    if (ds.length == 0) {
                        $("#kgrd").hide();
                        $("#dvNoRecord").show();
                        $("#dvloader").hide();
                    }
                },
                error: function (response) {
                    var responseJson = jQuery.parseJSON(response.responseText);
                    alert(responseJson.Message);
                    alert('Error');
                }
            });
        }
    </script>


</asp:Content>

