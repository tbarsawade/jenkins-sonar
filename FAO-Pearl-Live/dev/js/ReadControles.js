function ReadControles() { }


ReadControles.prototype.getData = function () {
    var controls = $('[IsSearch]');
    var result = {};
    var Key = "";
    $.each(controls, function (i, e) {
        var type = $(e).attr('data-ty');
        Key = $(e).attr('fld');
        switch (type) {
            case "DATETIME":
                result[Key] = $(e).val();
                break;
            case 'NUMERIC':
                result[Key] = $(e).val();
                break;
            case 'MULTISELECT':
                var vals = [];
                $('#' + $(e).attr('id') + ' input[type=checkbox]:checked').each(function () {
                    vals.push($(this).attr('value'));
                });
                result[Key] = vals;
                break;
            case 'SINGLESELECT':
                $('#' + $(e).attr('id') + ' input[type=checkbox]:checked').each(function () {
                    result[Key] = $(this).attr('value');
                });
                break;
            default:
                break;
        }
    });

    result.DocFromDate = $('#ContentPlaceHolder1_Frflddate').val();
    result.DocToDate = $('#ContentPlaceHolder1_Toflddate').val();
    result.SC = Rc.GetQueryString("SC");
    result.IsView = $('#hdnView').attr('Value');

    return result;
}


function getReport() {
    var obj = Rc.getData();
    var str1 = JSON.stringify(obj);

    //$.ajax({
    //    type: "POST",
    //    url: "NewReportMaster.aspx/GetData",
    //    data: '{ "json": ' + str1 + ' }',
    //    contentType: "application/json; charset=utf-8",
    //    dataType: "json",
    //    success: Rc.BindGrid,
    //    failure: function (response) {
    //        $("#lblTab").text('Unknown error. Please contact your system administrator.');
    //    }
    //});

    $.ajax({
        type: "POST",
        url: "NewReportMaster.aspx/GetData",
        data: '{ "json": ' + str1 + ' }',
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: Rc.BindGrid,
        error: function (err) { alert('err');},
        failure: function (response) {
            $("#lblTab").text('Unknown error. Please contact your system administrator.');
        }
    });


}

ReadControles.prototype.BindGrid = function (result) {
    $("#mask").css("display", "none"); $("#loader").css("display", "none");
    if (!result.d.Success) {
        $("#lblTab").text(result.d.Message);
        return false;
    }
    else {
        $("#lblTab").text('');
    }
    $("#lblCaption").text(result.d.Count + ' records found.');
    var Columns = result.d.Column;
    var data = JSON.parse(result.d.Data);
    var chartData = result.d.Chart;
    //Rc.bindGrid("kgrid", data, Columns, true, true, true, 550);

    BindChart(chartData);

    bindGrid1("kgrid", data, Columns, true, true, true, 550);
}


ReadControles.prototype.GetQueryString = function (param) {
    var url = window.location.href.slice(window.location.href.indexOf('?') + 1).split('&');
    for (var i = 0; i < url.length; i++) {
        var urlparam = url[i].split('=');
        if (urlparam[0] == param) {
            return urlparam[1];
        }
    }
}


function bindGrid1(gridDiv, Data1, columns, pageable, filterable, sortable, height) {

    $("#" + gridDiv).html('');

    var g = $("#" + gridDiv).data("kendoGrid");

    if (g != null || g != undefined) {
        g.destroy();
        $("#" + gridDiv).html('');
    }

    gridDiv = $("#" + gridDiv).kendoGrid({


        dataSource: {
            pageSize: 20,
            data: Data1
        },
        scrollable: {
            virtual: true
        },

        columns: columns,
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
        height: height,
        toolbar: ["excel"],
        excel: {
            fileName: "Report.xlsx",
            filterable: true,
            pageable: true,
            allPages: true
        }

    });

}

function onDataBound(e) {
    var grid = $("#kgrid").data("kendoGrid");
    var gridData = grid.dataSource.view();
    for (var i = 0; i < gridData.length; i++) {
        var DOCID = gridData[i].DocID;
        grid.table.find("tr[data-uid='" + gridData[i].uid + "']").bind("click", { DOCID: DOCID }, DetailHandler);
    }
}


function BindChart(data) {
    $("#Chart0").html('');
    $("#Chart1").html('');
    $("#Chart2").html('');
    $("#Chart3").html('');
    $("#Chart4").html('');

    var arrCharts = data.split("==");

    for (var i = 0; i < arrCharts.length; i++) {
        if (arrCharts[i] == '') {
            continue;
        }
        var arr = arrCharts[i].split("|");
        var arrCate = new Array();
        var arrPer = new Array();
        var arrNotper = new Array();
        for (var j = 0; j < arr.length; j++) {
            var rowArr = arr[j].split(",");
            arrCate.push(rowArr[0]);
            arrPer.push(rowArr[1]);
            arrNotper.push(rowArr[2]);
            var obj = {};
            obj.id = "#Chart" + i;
            obj.ChartText = "Summary Chart (Performed/Not Performed)";
            obj.SeriesData = [{
                name: "Performed",
                data: arrPer,
                color: 'green'
            }, {
                name: "Not Performed",
                data: arrNotper,
                color: 'red'
            }];
            obj.CategoryData = arrCate;
            createChart(obj);
        }

    }

    //var arr = data.split("|");
    //var arrCate = new Array();
    //var arrPer = new Array();
    //var arrNotper = new Array();
    //for (var i = 0; i < arr.length; i++)
    //{
    //    var rowArr = arr[i].split(",");
    //    arrCate.push(rowArr[0]);
    //    arrPer.push(rowArr[1]);
    //    arrNotper.push(rowArr[2]);
    //}

    //var obj = {};
    //obj.id = "#divChart";
    //obj.ChartText = "Summary Chart (Performed/Not Performed)";
    //obj.SeriesData = [{
    //    name: "Performed",
    //    data: arrPer
    //}, {
    //    name: "Not Performed",
    //    data: arrNotper
    //}];
    //obj.CategoryData = arrCate;
    //createChart(obj);
}


function createChart(obj) {
    $(obj.id).html('');
    $(obj.id).kendoChart({
        title: {
            text: obj.ChartText
        },
        legend: {
            position: "bottom"
        },
        seriesDefaults: {
            type: "column"
        },
        series: obj.SeriesData,
        valueAxis: {
            line: {
                visible: false
            }
        },
        categoryAxis: {
            categories: obj.CategoryData,
            majorGridLines: {
                visible: false,
                format: "{0}"
            }
        },
        tooltip: {
            visible: true
            //format: "{0}"
        }
    });
}



ReadControles.prototype.bindGrid1 = function (gridDiv, Data1, columns, pageable, filterable, sortable, height) {

    var g = $("#" + gridDiv).data("kendoGrid");

    if (g != null || g != undefined) {
        g.destroy();
        $("#" + gridDiv).html('');
    }

    gridDiv = $("#" + gridDiv).kendoGrid({
        dataSource: {
            pageSize: 20,
            data: Data1
        },
        scrollable: {
            virtual: true
        },
        columns: columns,
        pageable: true,
        pageSize: 20,
        scrollable: true,
        reorderable: true,
        groupable: true,
        sortable: true,
        filterable: true,
        resizable: true,
        height: height,
        toolbar: ["excel"],
        excel: {
            fileName: "Report.xlsx",
            filterable: true,
            pageable: true,
            allPages: true
        }
    });
}


var Rc = new ReadControles();