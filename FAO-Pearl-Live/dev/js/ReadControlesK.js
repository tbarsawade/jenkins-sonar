function ReadControles() { }

ReadControles.prototype.getData = function () {
    var controls = $('[IsSearch]');
    var result = { };
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
                $('#'+$(e).attr('id') + ' input[type=checkbox]:checked').each(function () {
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
    $.ajax({
        type: "POST",
        url: "ReportMasterK.aspx/GetData",
        data: '{ "json": '+str1+' }',
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: Rc.BindGrid,
        failure: function (response) {
            $("#lblTab").text('Unknown error. Please contact your system administrator.');
        }
    });
}

ReadControles.prototype.BindGrid = function (result) {

    if (!result.d.Success) {
        $("#lblTab").text(result.d.Message);
        return false;
    }
    else {
        $("#lblTab").text('');
    }
    $("#lblCaption").text(result.d.Count+' records found.');
    var Columns = result.d.Column;
    var data = JSON.parse(result.d.Data);

    Rc.bindGrid("kgrid", data, Columns, true, true, true, 550);

}


ReadControles.prototype.GetQueryString= function (param) {
    var url = window.location.href.slice(window.location.href.indexOf('?') + 1).split('&');
    for (var i = 0; i < url.length; i++) {
        var urlparam = url[i].split('=');
        if (urlparam[0] == param) {
            return urlparam[1];
        }
    }
}


ReadControles.prototype.bindGrid = function (gridDiv, Data1, columns, pageable, filterable, sortable, height) {

    var g = $("#" + gridDiv).data("kendoGrid");

    if (g != null || g != undefined)
    {
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
        height: height
        
    });
}

var Rc = new ReadControles();