<%@ Page Title="" Language="VB" MasterPageFile="~/USR.master" AutoEventWireup="false" CodeFile="NeedToActConfig.aspx.vb" Inherits="NeedToActConfig" EnableEventValidation="false" %>


<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">

    <script type="text/javascript" src="js/jquery1.min.js"></script>



    <script type="text/javascript">
        $(document).ready(function () {
            $.ajax({
                type: "POST",
                contentType: "application/json; charset=utf-8",
                url: "NeedToActConfig.aspx/BindDocumentDropdown",
                data: "{}",
                dataType: "json",
                success: function (data) {
                    $("#ContentPlaceHolder1_ddlDocumentType").empty();
                    $("<option value='-1'>SELECT</option>").appendTo("#ContentPlaceHolder1_ddlDocumentType");
                    $.each(data.d, function (key, value) {
                        $("#ContentPlaceHolder1_ddlDocumentType").append($("<option></option>").val(value.DocumentName).html(value.DocumentName));
                    });
                },
                error: function (result) {
                    alert("Error");
                }
            });
        });
    </script>
    <script type="text/javascript">
        function ddlDispName(dispname) {
            var selectedText = dispname.options[dispname.selectedIndex].innerHTML;
            $('#ContentPlaceHolder1_txtDispname').val(selectedText)
        }

    </script>
    <script type="text/javascript">
        function ddlChangeEvent(ddlDocumentType) {

            var selectedText = ddlDocumentType.options[ddlDocumentType.selectedIndex].innerHTML;
            var OperationType = $("#ContentPlaceHolder1_ddlOperationType").get(0).selectedIndex;

            if (OperationType == "0") {

            }
            else if (OperationType == "1") {
                $("#ContentPlaceHolder1_ddlFieldsname").empty();
                $("<option value='-1'>SELECT</option>").appendTo("#ContentPlaceHolder1_ddlFieldsname");
                $("<option value='d.fdate'>Received Date</option>").appendTo("#ContentPlaceHolder1_ddlFieldsname");
                $("<option value='M.curstatus'>Status</option>").appendTo("#ContentPlaceHolder1_ddlFieldsname");
                $("<option value='datediff(day,fdate,getdate())'>Pending Days</option>").appendTo("#ContentPlaceHolder1_ddlFieldsname");
                $("<option value='CONVERT(VARCHAR(10),M.adate,105)'>Creation Date</option>").appendTo("#ContentPlaceHolder1_ddlFieldsname");
                $("<option value='U.UserName'>Apply By</option>").appendTo("#ContentPlaceHolder1_ddlFieldsname");
            }
            else {

                $.ajax({
                    type: "POST",
                    contentType: "application/json; charset=utf-8",
                    url: "NeedToActConfig.aspx/BindDocumentFieldName",
                    data: "{ 'documentType': '" + selectedText + "', 'OptionType': '" + OperationType + "' }",
                    dataType: "json",
                    success: function (data) {
                        $("#ContentPlaceHolder1_ddlFieldsname").empty();
                        $("<option value='-1'>SELECT</option>").appendTo("#ContentPlaceHolder1_ddlFieldsname");
                        $.each(data.d, function (key, value) {
                            $("#ContentPlaceHolder1_ddlFieldsname").append($("<option></option>").val(value.DocumentFieldsValue).html(value.DocumentFieldsText));
                        });
                    },
                    error: function (result) {
                        alert("Error");
                    }
                });
            }


        }
    </script>
    <script type="text/javascript">
        function ddlTypeChangeEvent() {
            var selectedText = $('[id*=ContentPlaceHolder1_ddlDocumentType] option:selected').text()
            var DocIndex = $('#ContentPlaceHolder1_ddlDocumentType').get(0).selectedIndex;
            var OperationType = $("#ContentPlaceHolder1_ddlOperationType").get(0).selectedIndex;

            if (OperationType == "0") {

            }
            else if (OperationType == "1") {
                $("#ContentPlaceHolder1_ddlFieldsname").empty();
                $("<option value='-1'>SELECT</option>").appendTo("#ContentPlaceHolder1_ddlFieldsname");
                $("<option value='d.fdate'>Received Date</option>").appendTo("#ContentPlaceHolder1_ddlFieldsname");
                $("<option value='M.curstatus'>Status</option>").appendTo("#ContentPlaceHolder1_ddlFieldsname");
                $("<option value='datediff(day,fdate,getdate())'>Pending Days</option>").appendTo("#ContentPlaceHolder1_ddlFieldsname");
                $("<option value='CONVERT(VARCHAR(10),M.adate,105)'>Creation Date</option>").appendTo("#ContentPlaceHolder1_ddlFieldsname");
                $("<option value='U.UserName'>Apply By</option>").appendTo("#ContentPlaceHolder1_ddlFieldsname");
                $("<option value='M.TID'>DocID</option>").appendTo("#ContentPlaceHolder1_ddlFieldsname");
                $("<option value='M.documenttype'>Document Type</option>").appendTo("#ContentPlaceHolder1_ddlFieldsname");
                $("<option value='M.PRIORITY'>PRIORITY</option>").appendTo("#ContentPlaceHolder1_ddlFieldsname");

            }
            else {
                if (DocIndex != 0 || DocIndex != -1) {
                    $.ajax({
                        type: "POST",
                        contentType: "application/json; charset=utf-8",
                        url: "NeedToActConfig.aspx/BindDocumentFieldName",
                        data: "{ 'documentType': '" + selectedText + "', 'OptionType': '" + OperationType + "' }",
                        dataType: "json",
                        success: function (data) {
                            $("#ContentPlaceHolder1_ddlFieldsname").empty();
                            $("<option value='-1'>SELECT</option>").appendTo("#ContentPlaceHolder1_ddlFieldsname");
                            $.each(data.d, function (key, value) {
                                $("#ContentPlaceHolder1_ddlFieldsname").append($("<option></option>").val(value.DocumentFieldsValue).html(value.DocumentFieldsText));
                            });
                        },
                        error: function (result) {
                            alert("Error");
                        }
                    });
                }
            }
        }
    </script>
    <script type="text/javascript">
        $(document).ready(function () {
            $('#<%=btnSave.ClientID %>').on('click', function (event) {
                event.preventDefault();
                var ErrMsg = 'Please Select '

                if ($('[id*=ContentPlaceHolder1_ddlDocumentType] option:selected').index() == 0) {
                    ErrMsg = ErrMsg + "Document Type , "
                }
                if ($('[id*=ContentPlaceHolder1_ddlOperationType] option:selected').index() == 0 || $('[id*=ContentPlaceHolder1_ddlOperationType] option:selected').index() == -1) {
                    ErrMsg = ErrMsg + "Operation Type , "
                }
                if ($('[id*=ContentPlaceHolder1_ddlFieldsname] option:selected').index() == 0 || $('[id*=ContentPlaceHolder1_ddlFieldsname] option:selected').index() == -1) {
                    ErrMsg = ErrMsg + "Field Name , "
                }
                if ($('#ContentPlaceHolder1_txtDisporder').val() == "") {
                    ErrMsg = ErrMsg + "Display order, "
                }
                if (ErrMsg.length > 14) {
                    $('#<%= lblErrorMsg.ClientID %>').html(ErrMsg.substring(0, ErrMsg.length - 2));
                    return false;

                }
                else {
                    var DocumentType = $('[id*=ContentPlaceHolder1_ddlDocumentType] option:selected').text();
                    var OperationType = $('[id*=ContentPlaceHolder1_ddlOperationType] option:selected').text();
                    var FieldMapping = $('[id*=ContentPlaceHolder1_ddlFieldsname] option:selected').val();
                    var FieldText = $('#ContentPlaceHolder1_txtDispname').val();
                    var DisplayOrder = $('#ContentPlaceHolder1_txtDisporder').val();
                    $.ajax({
                        type: "POST",
                        contentType: "application/json;charset=utf-8",
                        url: "NeedToActConfig.aspx/SaveData",
                        data: "{ 'DocType': '" + DocumentType + "', 'OpType': '" + OperationType + "', 'Fieldval': '" + FieldMapping + "', 'DispText':'" + FieldText + "','DispOrder':" + DisplayOrder + "}",
                        dataType: "json",
                        success: function (data) {
                            GetConfigs(1);
                            alert(data.d);
                        },
                        error: function (result) {
                            alert("error");
                        }
                    });
                }

            })
        }
        )

    </script>
    <script src="Jquery/ASPSnippets_Pager.min.js" type="text/javascript"></script>
    <script type="text/javascript">
        $(function () {
            GetConfigs(1);
        });
        $(".Pager .page").live("click", function () {
            GetConfigs(parseInt($(this).attr('page')));
        });
        function GetConfigs(pageIndex) {

            $.ajax({
                type: "POST",
                url: "NeedToActConfig.aspx/GetConfigs",
                data: '{pageIndex: ' + pageIndex + '}',
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: OnSuccess,
                failure: function (response) {
                    alert(response.d);
                },
                error: function (response) {
                    alert(response.d);
                }
            });
        }
        function OnSuccess(response) {
            var xmlDoc = $.parseXML(response.d);
            var xml = $(xmlDoc);
            var customers = xml.find("Customers");
            var row = $("[id*=gvCustomers] tr:last-child").clone(true);
            $("[id*=gvCustomers] tr").not($("[id*=gvCustomers] tr:first-child")).remove();
            $.each(customers, function () {
                var customer = $(this);
                $("td", row).eq(0).html($(this).find("DocType").text());
                $("td", row).eq(1).html($(this).find("Type").text());
                $("td", row).eq(2).html($(this).find("FieldName").text());
                $("td", row).eq(3).html($(this).find("DisplayOrder").text());
                $("td", row).eq(5).find('#hdnTId').val($(this).find("TID").text());
                var TID = $(this).find("TID").text();

                $("td", row).eq(4).html('<a href="#" id=' + TID + ' class="Delete">Delete</a>').append('|').append('<a href="#" id=UP' + TID + ' class="UP">UP</a>').append('|').append('<a href="#" id=DOWN' + TID + ' class="DOWN">DOWN</a>');
                $("[id*=gvCustomers]").append(row);
                row = $("[id*=gvCustomers] tr:last-child").clone(true);
            });
            var pager = xml.find("Pager");
            $(".Pager").ASPSnippets_Pager({
                ActiveCssClass: "current",
                PagerCssClass: "pager",
                PageIndex: parseInt(pager.find("PageIndex").text()),
                PageSize: parseInt(pager.find("PageSize").text()),
                RecordCount: parseInt(pager.find("RecordCount").text())
            });
        };
        function AppendRow(row, customerId, name, country) {
            //Bind CustomerId.
            $(".CustomerId", row).find("span").html(customerId);

            //Bind Name.
            $(".Name", row).find("span").html(name);
            $(".Name", row).find("input").val(name);

            //Bind Country.
            $(".Country", row).find("span").html(country);
            $(".Country", row).find("input").val(country);
            $("[id*=gvCustomers]").append(row);
        }
    </script>

    <script type="text/javascript">
        //Delete event handler.
        $("body").on("click", "[id*=gvCustomers] .Delete", function () {
            if (confirm("Do you want to delete this row?")) {
                var row = $(this).closest("tr");
                var customerId = $(this).attr('id');

                $.ajax({
                    type: "POST",
                    url: "NeedToActConfig.aspx/DeleteCustomer",
                    data: '{customerId: ' + customerId + '}',
                    contentType: "application/json; charset=utf-8",
                    dataType: "json",
                    success: function (response) {
                        GetConfigs(1);
                        alert('Your record has been successfully delted.');
                    }
                });
            }

            return false;
        });
    </script>
    <script type="text/javascript">
        //Delete event handler.
        $("body").on("click", "[id*=gvCustomers] .UP", function () {
            var row = $(this).closest("tr");
            var PcustomerId = row.prev().find(".UP").attr("id");
            var customerId = $(this).attr('id');

            $.ajax({
                type: "POST",
                url: "NeedToActConfig.aspx/UPCustomer",
                data: "{'CurrentId': '" + customerId + "','ParrentId':'" + PcustomerId + "'}",
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function (response) {
                    GetConfigs(1);
                    alert('Your record has been successfully Moved UP.');
                }
            });


            return false;
        });
    </script>
    <script type="text/javascript">
        //Delete event handler.
        $("body").on("click", "[id*=gvCustomers] .DOWN", function () {

            var row = $(this).closest("tr");
            var PcustomerId = row.next().find(".DOWN").attr("id");
            var customerId = $(this).attr('id');

            $.ajax({
                type: "POST",
                url: "NeedToActConfig.aspx/UPCustomer",
                data: "{'CurrentId': '" + customerId + "','ParrentId':'" + PcustomerId + "'}",
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function (response) {
                    GetConfigs(1);
                    alert('Your record has been successfully Moved DOWN.');
                }
            });


            return false;
        });
    </script>
    <div id="main" style="min-height: 400px">
        <table cellpadding="0px" cellspacing="0px" style="text-align: left" width="100%" border="0px">
            <tr>
                <td colspan="6">

                    <span id="ContentPlaceHolder1_lblCaption">
                        <div class="doc_header">Need To Act Configuration</div>
                    </span>

                </td>
            </tr>
            <tr>
                <td colspan="6">
                    <asp:Label ID="lblErrorMsg" runat="server" ForeColor="Red"
                        Font-Bold="true"></asp:Label>
                </td>
            </tr>
            <tr style="vertical-align: top">
                <td>
                    <asp:Label ID="lblID" runat="server" Font-Bold="true">Document Type</asp:Label>
                </td>
                <td>
                    <asp:DropDownList ID="ddlDocumentType" Width="150px" runat="server" CssClass="txtBox" onchange="ddlChangeEvent(this);"></asp:DropDownList>
                </td>
                <td>
                    <asp:Label ID="lblOPType" runat="server" Font-Bold="true">Operation Type</asp:Label>
                </td>
                <td>
                    <asp:DropDownList ID="ddlOperationType" runat="server" Width="150px" CssClass="txtBox" onchange="ddlTypeChangeEvent();">
                        <asp:ListItem>SELECT</asp:ListItem>
                        <asp:ListItem>STATIC</asp:ListItem>
                        <asp:ListItem>DYNAMIC</asp:ListItem>
                    </asp:DropDownList>
                </td>
                <td>
                    <asp:Label ID="lblFieldsName" runat="server" Font-Bold="true">Fields Name</asp:Label>
                </td>
                <td>
                    <asp:DropDownList ID="ddlFieldsname" Width="150px" runat="server" CssClass="txtBox" onchange="ddlDispName(this);"></asp:DropDownList>
                </td>
            </tr>
            <tr>
                <td>
                    <asp:Label ID="lblDispName" runat="server" Font-Bold="true">Display Name</asp:Label>
                </td>
                <td>
                    <asp:TextBox ID="txtDispname" runat="server" CssClass="txtBox"></asp:TextBox>
                </td>
                <td>
                    <asp:Label ID="lblDispOrder" runat="server" Font-Bold="true">Display Order</asp:Label>
                </td>
                <td>
                    <asp:TextBox ID="txtDisporder" runat="server" CssClass="txtBox"></asp:TextBox>
                </td>
            </tr>
            <tr>
                <td colspan="6" align="center">
                    <asp:Button ID="btnSave" runat="server" CssClass="btnNew" Text="Save" />
                </td>
            </tr>
            <tr>
                <td colspan="6">

                    <asp:GridView ID="gvCustomers" runat="server" AutoGenerateColumns="false" RowStyle-BackColor="#A1DCF2"
                        HeaderStyle-BackColor="#3AC0F2" HeaderStyle-ForeColor="White" Width="100%">
                        <Columns>
                            <asp:BoundField ItemStyle-Width="150px" DataField="DocType" HeaderText="Document Type" />
                            <asp:BoundField ItemStyle-Width="150px" DataField="Type" HeaderText="Operation Type" />
                            <asp:BoundField ItemStyle-Width="150px" DataField="FieldName" HeaderText="Field Name" />
                            <asp:BoundField ItemStyle-Width="150px" DataField="DisplayOrder" HeaderText="Display Order" />
                            <asp:TemplateField HeaderStyle-Width="50" HeaderText="Action">
                                <ItemTemplate>
                                    <asp:Label ID="hdnTId" runat="server" Text="" />
                                    <asp:LinkButton ID="lnkDelete" Text="Delete" runat="server" CssClass="Delete" />
                                    <asp:LinkButton ID="LinkUP" Text="UP" runat="server" CssClass="UP" />
                                    <asp:LinkButton ID="LinkDOWN" Text="UP" runat="server" CssClass="DOWN" />
                                </ItemTemplate>
                            </asp:TemplateField>
                        </Columns>
                    </asp:GridView>
                    <br />
                    <div class="Pager"></div>

                </td>
            </tr>
        </table>
    </div>
</asp:Content>


