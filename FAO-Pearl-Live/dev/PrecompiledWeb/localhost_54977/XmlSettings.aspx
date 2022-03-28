<%@ page title="" language="VB" masterpagefile="~/USR.master" autoeventwireup="false" inherits="XmlSettings, App_Web_0gl03q5k" viewStateEncryptionMode="Always" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
    <script src="js/jquery-1.9.1.min.js"></script>

    <style type="text/css">
        .main {
            border-radius:3px;
            padding:15px;
             color:#000;
             background:rgba(226, 226, 226, 0.40);
             min-height:500px;
        }
        .inner {
            min-height:40px;
            margin-bottom:15px;
        }
        .inner1 {
            min-height:40px;
            margin-bottom:5px;
            border: 1px solid #bab6b6;
            min-height:300px;
            max-height:300px;
            overflow-y:scroll;
        }
        .rptddl {
            width:250px;
            height:25px;
        }
        .txt {
            width:263px;
            height:25px;
        }
        .err {
            color:#f00;
        }
        .success {
            color:#0faa09;
        }
    </style>
    <script type="text/javascript">
        var specialKeys = new Array();
        specialKeys.push(8); //Backspace
        specialKeys.push(9); //Tab
        $(function () {
            $(".numeric").bind("keypress", function (e) {
                var keyCode = e.which ? e.which : e.keyCode
                var ret = ((keyCode >= 48 && keyCode <= 57) || specialKeys.indexOf(keyCode) != -1);
                $(".error").css("display", ret ? "none" : "inline");
                if (ret)
                {
                    if (parseInt($(this).val()) > parseInt($(this).attr('max')) && keyCode == 9)
                    {
                        //if (keyCode == 9) { $(this).val(''); }
                        $(this).val('');
                        ret = false;
                    }
                }
                return ret;
            });
            $(".numeric").bind("paste", function (e) {
                return false;
            });
            $(".numeric").bind("drop", function (e) {
                return false;
            });
        });

        function getFields(ele)
        {
            $('#tblflds').html('');
            
            if ($(ele).val() == '0')
            {
                return false;
            }

            var obj = {};
            obj.DocType = $('#' + $(ele).attr('id') + ' option:selected').text();
            $.ajax({
                type: "POST",
                url: "XmlSettings.aspx/GetFields",
                data: JSON.stringify(obj),
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: getFieldsSucceess,
                failure: function (response) {
                    $("#lblTab").text('Unknown error. Please contact your system administrator.');
                }
            });
        }
        


        function getFieldsSucceess(result) {
            var obj = result.d;
            if (!obj.Success || obj.Fields=='')
            { return false; }
            $('#txtHr').val(obj.FtpHr);
            $('#txtMin').val(obj.FtpMin);
            var arr = csv2array(obj.Fields, '^', '|');
            var str = '';
            for (var i = 0; i < arr.length; i++)
            {
                var chkd = 'checked="checked"';
                var txt = '<input type="text" class="txt" id="txt_' + arr[i][0] + '" fld="' + arr[i][0] + '" value="' + arr[i][2] + '" />';
                if (arr[i][2] == '')
                {
                    chkd = '';
                }               
                str += '<tr id="'+ arr[i][0] +'">';
                str += '<td><input style="display:none;" type="checkbox" '+ chkd +' value="' + arr[i][0] + '" /> ' + arr[i][1] + '</td>';
                str += '<td id="td_' + arr[i][0] + '"> '+ txt +' </td>';
                str += '</tr>';
            }

            $('#tblflds').html(str);
        }


        function csv2array(csvString, ColDelimeter, RowDelimeter) {
            var csvArray = [];
            var csvRows = csvString.split(RowDelimeter);

            for (var rowIndex = 0; rowIndex < csvRows.length; ++rowIndex) {
                var rowArray = csvRows[rowIndex].split(ColDelimeter);

                var rowObject = csvArray[rowIndex] = {};

                for (var propIndex = 0; propIndex < rowArray.length; ++propIndex) {
                    rowObject[propIndex] = rowArray[propIndex];
                }
                csvArray[rowIndex] = rowObject;
            }
            return csvArray;
        }
        
        function SaveSettings()
        {
            if ($('#ddlDocType').val() == '0')
            {
                return false;
            }
            var arr = $("[fld]");
            var ob = {};
            ob.FormId = $('#ddlDocType').val();
            ob.Hr = $('#txtHr').val();
            ob.Min = $('#txtMin').val();
            var ob1 = {};
            for (var i = 0; i < arr.length - 1; i++)
            {
                ob1[$(arr[i]).attr('fld')] = $(arr[i]).val();
            }
            var obj = {};
            obj.Form = ob;
            obj.Fields = ob1;

            $.ajax({
                type: "POST",
                url: "XmlSettings.aspx/SaveSettings",
                data: JSON.stringify(obj),
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: SaveSettingsSuccess,
                failure: function (response) {
                    $("#divmsg").html('Unknown error. Please contact your system administrator.');
                    $("#divmsg").removeClass('success');
                    $("#divmsg").addClass('err');
                }
            });
        }

        function SaveSettingsSuccess(result)
        {
            $("#divmsg").html(result.d.Message);
            if (!result.d.Success) {
                $("#divmsg").removeClass('success');
                $("#divmsg").addClass('err');
            }
            else {
                $("#divmsg").removeClass('err');
                $("#divmsg").addClass('success');
            }
        }

    </script>


    <div class="main">
        <div class="inner">
            <table>
                <col style="width:200px;" />
                <col style="width:100px;" />
                <col style="width:200px;"/>
                <col style="width:200px;"/>
                <tr>
                    <td>Select Document Type</td>
                    <td></td>
                    <td>Schedule time(Hr)[24 Hr format]</td>
                    <td>Schedule time(Min)</td>
                </tr>
                <tr>
                    <td>
                        <asp:DropDownList ID="ddlDocType" ClientIDMode="Static" class="rptddl" runat="server" onchange="getFields(this);">
                            <asp:ListItem Value="0">Select</asp:ListItem>
                        </asp:DropDownList>
                    </td>
                    <td></td>
                    <td>
                        <asp:TextBox ID="txtHr" max="24" ClientIDMode="Static" CssClass="numeric" Width="60px" Height="25px" runat="server"></asp:TextBox>

                    </td>
                    <td>
                        <asp:TextBox ID="txtMin" max="59" ClientIDMode="Static" CssClass="numeric" Width="60px" Height="25px"  runat="server"></asp:TextBox>
                    </td>
                </tr>
            </table>
        </div>
        <div class="inner1">
            <table border="0" style="width:80%;">
                <col style="width:338px" />
                <col style="width:400px" />
                <tbody id="tblflds">
                    
                </tbody>
            </table>
        </div>
        <div class="inner">
            <input id="btnSave" type="button" onclick="SaveSettings();" value="Save settings" />
        </div>

        <div class="inner" id="divmsg">
            
        </div>
    </div>

</asp:Content>