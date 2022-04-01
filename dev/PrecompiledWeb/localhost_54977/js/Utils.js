function Utils() { }

var exts1 = ['.mp4','.msg', '.doc', '.docx', '.rtf', '.odt', '.jpg', '.jpeg', '.bmp', '.png', '.html', '.xls', '.xlsx', '.pdf', '.zip', '.rar', '.gif', '.txt', '.ppt', '.pptx', '.xml', '.xaml', '.xlr', '.csv', '.tif', '.tiff', '.xps'];

Utils.prototype.UploadFile = function (sender, hdn, lbl) {

    var filevalidate = $(sender).val();
    var extension = filevalidate.substring(filevalidate.lastIndexOf('.'));
    if ($.inArray(extension.toLowerCase(), exts1) == -1) {
        alert("Invalid file type.");
        $(sender).val('');
        $(sender).next().attr("value", '');
        $(sender).next().next().html('');
        return false;
    }
 else {
        var MymeType = $(sender).get(0).files[0]['type'];
      //  alert('file extension is ' + extension.toLowerCase() + ' and mime type is ' + MymeType.toLowerCase());
        if (extension.toLowerCase() == ".jpg" && MymeType.toLowerCase() == "image/jpeg") {
        }
        else if (extension.toLowerCase() == ".jpeg" && MymeType.toLowerCase() == "image/jpeg") {
        }
        else if (extension.toLowerCase() == ".msg" && (MymeType.toLowerCase() == "" ||MymeType.toLowerCase()== "application/vnd.ms-outlook")) {
        }
        else if (extension.toLowerCase() == ".doc" && MymeType.toLowerCase() == "application/msword") {
        }
        else if (extension.toLowerCase() == ".rtf" && MymeType.toLowerCase() == "application/rtf") {
        }
        else if (extension.toLowerCase() == ".bmp" && MymeType.toLowerCase() == "image/bmp") {
        }
        else if (extension.toLowerCase() == ".png" && MymeType.toLowerCase() == "image/png") {
        }
        else if (extension.toLowerCase() == ".html" && MymeType.toLowerCase() == "text/html") {
        }
        else if (extension.toLowerCase() == ".xls" && (MymeType.toLowerCase() == "application/excel" || MymeType.toLowerCase() == "application/vnd.ms-excel" || MymeType.toLowerCase() == "application/x-excel" || MymeType.toLowerCase() == "application/x-msexcel")) {
        }
        else if (extension.toLowerCase() == ".pdf" && MymeType.toLowerCase() == "application/pdf") {
        }
        else if (extension.toLowerCase() == ".zip" && (MymeType.toLowerCase() == "application/x-compressed" || MymeType.toLowerCase() == "application/x-zip-compressed" || MymeType.toLowerCase() == "application/zip" || MymeType.toLowerCase() == "multipart/x-zip")) {
        }
        else if (extension.toLowerCase() == ".gif" && MymeType.toLowerCase() == "image/gif") {
        }
        else if (extension.toLowerCase() == ".txt" && MymeType.toLowerCase() == "text/plain") {
        }
        else if (extension.toLowerCase() == ".ppt" && MymeType.toLowerCase() == "application/vnd.ms-powerpoint") {
        }
        else if (extension.toLowerCase() == ".ppt" && MymeType.toLowerCase() == "application/vnd.ms-powerpoint") {
        }
        else if (extension.toLowerCase() == ".xml" && (MymeType.toLowerCase() == "application/xml" || MymeType.toLowerCase() == "text/xml")) {
        }
        else if (extension.toLowerCase() == ".tif" && (MymeType.toLowerCase() == "image/tiff" || MymeType.toLowerCase() == "image/x-tiff")) {
        }
        else if (extension.toLowerCase() == ".tiff" && (MymeType.toLowerCase() == "image/tiff" || MymeType.toLowerCase() == "image/x-tiff")) {
        }
        else if (extension.toLowerCase() == ".odt" && MymeType.toLowerCase() == "application/vnd.oasis.opendocument.text") {
        }
        else if (extension.toLowerCase() == ".xlsx" && MymeType.toLowerCase() == "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet") {
        }
        else if (extension.toLowerCase() == ".xlsx" && MymeType.toLowerCase() == "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet") {
        }
        else if (extension.toLowerCase() == ".docx" && MymeType.toLowerCase() == "application/vnd.openxmlformats-officedocument.wordprocessingml.document") {
        }
        else if (extension.toLowerCase() == ".xlsx" && MymeType.toLowerCase() == "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet") {
        }
        else if (extension.toLowerCase() == ".rar" && (MymeType.toLowerCase() == "application/x-rar" || MymeType.toLowerCase() == "")) {
        }
        else if (extension.toLowerCase() == ".zip" && (MymeType.toLowerCase() == "application/zip" || MymeType.toLowerCase() == "")) {
        }
        else if (extension.toLowerCase() == ".csv" && MymeType.toLowerCase() == "application/vnd.ms-excel") {
        }
        else if (extension.toLowerCase() == ".pptx" && MymeType.toLowerCase() == "application/vnd.openxmlformats-officedocument.presentationml.presentation") {
        }
        else if (extension.toLowerCase() == ".pptx" && MymeType.toLowerCase() == "application/vnd.openxmlformats-officedocument.presentationml.presentation") {
        }
        else if (extension.toLowerCase() == ".xaml" && MymeType.toLowerCase() == "application/x-ms-xbap") {
        }
	else if (extension.toLowerCase() == ".mp4" && MymeType.toLowerCase() == "video/mp4") {
        }

       else {
            alert("Invalid Mime file type");
            $(sender).val('');
            $(sender).next().attr("value", '');
            $(sender).next().next().html('');
            return false;
        }
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
    options.url = "FileUploadHandler.ashx";
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

Utils.prototype.UploadFile1 = function (sender, hdn, lbl) {
    var filevalidate = $(sender).val();
    var extension = filevalidate.substring(filevalidate.lastIndexOf('.'));
    if ($.inArray(extension.toLowerCase(), exts1) == -1) {
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
    options.url = "FileUploadHandler.ashx";
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

Utils.prototype.PreservFile = function ()
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


 
Utils.prototype.getInternetExplorerVersion = function ()
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

var UtilJs = new Utils();
