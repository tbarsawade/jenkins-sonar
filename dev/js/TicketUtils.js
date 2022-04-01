function TicketUtils() { }

var exts = ['.msg','.doc', '.docx', '.rtf', '.odt', '.jpg', '.jpeg', '.bmp', '.png', '.html', '.xls', '.xlsx', '.pdf', '.zip', '.rar', '.gif', '.txt', '.ppt', '.pptx', '.xml', '.xaml', '.xlr', '.csv', '.tif', '.tiff'];

TicketUtils.prototype.UploadFile = function (sender, hdn, lbl) {

    var filevalidate = $(sender).val();
    var extension = filevalidate.substring(filevalidate.lastIndexOf('.'));
    if ($.inArray(extension.toLowerCase(), exts) == -1) {
        alert("Invalid file type");
        $(sender).val('');
        $(sender).next().attr("value", '');
        $(sender).next().next().html('');
        return false;
    }

    $("#loading-div-background").show();

    var fileUpload = $(sender).get(0);
    // var file = document.querySelector("input[type='file']");
    var files = fileUpload.files;
    var data = new FormData();
    
    for (var i = 0; i < files.length; i++) {
        data.append(files[i].name, files[i]);
    }
    var options = {};
    options.url = "TicketFileUploaderHandler.ashx";
    options.type = "POST";
    options.data = data;
    options.contentType = false;
    options.processData = false;
    options.success = function (result) {
        $("#loading-div-background").hide();
        $(fileUpload).next().attr("value", result);
        $(fileUpload).next().next().html(result);

        //alert(hdn + '  ' + lbl);

        // $(fileUpload).parent('td').append('<img src="images/Cancel.gif" id="imgCancel" runat="server" clientidmode="Static" onclick="cancel(this,' + hdn + ',' + lbl + ')" />');

    };
    options.error = function (err) { $("#loading-div-background").hide(); alert(err.statusText); };

    $.ajax(options);
    evt.preventDefault();


}

TicketUtils.prototype.UploadFile1 = function (sender, hdn, lbl) {

    var filevalidate = $(sender).val();
    var extension = filevalidate.substring(filevalidate.lastIndexOf('.'));
    if ($.inArray(extension.toLowerCase(), exts) == -1) {
        alert("Invalid file type");
        $(sender).val('');
        $(sender).next().attr("value", '');
        $(sender).next().next().html('');
        return false;
    }
    $("#loading-div-background").show();
    var fileUpload = $(sender).get(0);
    // var file = document.querySelector("input[type='file']");
    var files = fileUpload.files;
    var data = new FormData();

    for (var i = 0; i < files.length; i++) {
        data.append(files[i].name, files[i]);
    }
    var options = {};
    options.url = "TicketFileUploaderHandler.ashx";
    options.type = "POST";
    options.data = data;
    options.contentType = false;
    options.processData = false;
    options.success = function (result) {
        $("#loading-div-background").hide();
        $(fileUpload).next().attr("value", result);
        $(fileUpload).next().next().html(result);
        $(fileUpload).next().next().next().attr("value", "1");
        //alert(hdn + '  ' + lbl);

        // $(fileUpload).parent('td').append('<img src="images/Cancel.gif" id="imgCancel" runat="server" clientidmode="Static" onclick="cancel(this,' + hdn + ',' + lbl + ')" />');

    };
    options.error = function (err) { $("#loading-div-background").hide(); alert(err.statusText); };

    $.ajax(options);
    evt.preventDefault();


}

TicketUtils.prototype.PreservFile = function ()
{
    $(':hidden').each(function () {
        try {
           
            if ($(this).prev().type().is(':file')) {
               
                var v = $(this).attr('value');
                $(this).next().html(v);
              
            }

        } catch (e) {
            return true;
        }
    });
    return false;
}


 
TicketUtils.prototype.getInternetExplorerVersion = function ()
{
    //returns -1 if not IE
    var rv = -1;
    if (navigator.appName == 'Microsoft Internet Explorer') {
        var ua = navigator.userAgent;
        var re = new RegExp("MSIE ([0-9]{1,}[\.0-9]{0,})");
        if (re.exec(ua) != null)
            rv = parseFloat(RegExp.$1);
    }
    return rv;
}

var TicketUtils = new TicketUtils();
