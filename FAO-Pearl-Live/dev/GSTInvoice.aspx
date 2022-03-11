<%@ Page Title="" Language="VB" MasterPageFile="~/usrFullScreenBPM.master" AutoEventWireup="false" CodeFile="GSTInvoice.aspx.vb" Inherits="GSTInvoice" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
    <div class="form" style="min-height:480px !important;">
        <div class="doc_header">
            GST Compliance
        </div>
        <%--  <div class="row">--%>
        <div class="container-fluid">
              <table class="table table-bordered">
            <thead>
                <tr>
                    <th><input id="allcb" type="checkbox" />All</th>
                    <th>ERP DocID</th>
                    <th>ERP Date</th>
                    <th>Vendor</th>
                    <th>Invoice No.</th>
                    <th>Invoice Date</th>
                    <th >Invoice Amount</th>
                    <th style="width:150px">Vendor Amount</th>
                    <th>Vendor Remarks</th>
                </tr>
            </thead>
            <tbody>
                <tr id="tr1">
                    <td>
                        <input type="checkbox" id="cb1" />
                    </td>
                    <td>
                        <label>1000019928</label>
                    </td>
                    <td>
                        <label>02-08-2017</label>
                    </td>
                    <td>
                        <label>
                            RENT WORK INDIA LTD.
                        </label>
                    </td>

                    <td>
                        <label>
                            213
                        </label>
                    </td>
                    <td>
                        <label>
                            09/01/17
                        </label>
                    </td>
                    <td>
                        <label>
                            4435
                        </label>
                    </td>
                    <td>
                        <input type="text" class="txtBox" id="v1" style="width:200px" />
                    </td>
                    <td>
                        <textarea rows="2" cols="4" class="txtBox" id="r1"></textarea>
                    </td>

                </tr>
                <tr id="tr2">
                    <td>
                        <input type="checkbox" id="cb2" />
                    </td>
                    <td>
                        <label>5105692540
</label>
                    </td>
                    <td>
                        <label>02-09-2017
</label>
                    </td>
                    <td>
                        <label>
                            RENT WORK INDIA LTD.

                        </label>
                    </td>

                    <td>
                        <label>
                          90744483

                        </label>
                    </td>
                    <td>
                        <label>
                           23/01/17

                        </label>
                    </td>
                    <td>
                        <label>
                           39787

                        </label>
                    </td>
                    <td>
                        <input type="text" class="txtBox" id="v2" style="width:200px" />
                    </td>
                    <td>
                        <textarea rows="2" cols="4" class="txtBox" id="r2"></textarea>
                    </td>

                </tr>
                <tr id="tr3">
                    <td>
                        <input type="checkbox" id="cb3" />
                    </td>
                    <td>
                        <label>1000019927
</label>
                    </td>
                    <td>
                        <label>02-16-2017
</label>
                    </td>
                    <td>
                        <label>
                           RENT WORK INDIA LTD.

                        </label>
                    </td>

                    <td>
                        <label>
                          214

                        </label>
                    </td>
                    <td>
                        <label>
                           09/01/17

                        </label>
                    </td>
                    <td>
                        <label>
                          4750

                        </label>
                    </td>
                    <td>
                        <input type="text" class="txtBox" id="v3" style="width:200px" />
                    </td>
                    <td>
                        <textarea rows="2" cols="4" class="txtBox" id="r3"></textarea>
                    </td>

                </tr>
                <tr id="tr4">
                    <td>
                        <input type="checkbox" id="cb4" />
                    </td>
                    <td>
                        <label>5105692539
</label>
                    </td>
                    <td>
                        <label>02-07-2017
</label>
                    </td>
                    <td>
                        <label>
                          RENT WORK INDIA LTD.

                        </label>
                    </td>

                    <td>
                        <label>
                          90744482

                        </label>
                    </td>
                    <td>
                        <label>
                          23/01/17

                        </label>
                    </td>
                    <td>
                        <label>
                          64997

                        </label>
                    </td>
                    <td>
                        <input type="text" class="txtBox" id="v4" style="width:200px" />
                    </td>
                    <td>
                        <textarea rows="2" cols="4" class="txtBox" id="r4"></textarea>
                    </td>

                </tr>
                <tr id="tr5">
                    <td>
                        <input type="checkbox" id="cb5" />
                    </td>
                    <td>
                        <label>5105692538
</label>
                    </td>
                    <td>
                        <label>02-10-2017
</label>
                    </td>
                    <td>
                        <label>
                          RENT WORK INDIA LTD.

                        </label>
                    </td>

                    <td>
                        <label>
                         90744481

                        </label>
                    </td>
                    <td>
                        <label>
                          23/01/17

                        </label>
                    </td>
                    <td>
                        <label>
                          53050

                        </label>
                    </td>
                    <td>
                        <input type="text" class="txtBox" id="v5" style="width:200px" />
                    </td>
                    <td>
                        <textarea rows="2" cols="4" class="txtBox" id="r5"></textarea>
                    </td>

                </tr>
                <tr id="tr6">
                    <td>
                        <input type="checkbox" id="cb6" />
                    </td>
                    <td>
                        <label>1000019926
</label>
                    </td>
                    <td>
                        <label>02-03-2017
</label>
                    </td>
                    <td>
                        <label>
                            RENT WORK INDIA LTD.

                        </label>
                    </td>

                    <td>
                        <label>
                         RPPL/2016-17/4024

                        </label>
                    </td>
                    <td>
                        <label>
                           23/01/17

                        </label>
                    </td>
                    <td>
                        <label>
                         5272

                        </label>
                    </td>
                    <td>
                        <input type="text" class="txtBox" id="v6" style="width:200px" />
                    </td>
                    <td>
                        <textarea rows="2" cols="4" class="txtBox" id="r6"></textarea>
                    </td>

                </tr>
                <tr id="tr7">
                    <td>
                        <input type="checkbox" id="cb7" />
                    </td>
                    <td>
                        <label>5105692537
</label>
                    </td>
                    <td>
                        <label>02-02-2017
</label>
                    </td>
                    <td>
                        <label>
                         RENT WORK INDIA LTD.

                        </label>
                    </td>

                    <td>
                        <label>
                          90744480

                        </label>
                    </td>
                    <td>
                        <label>
                          23/01/17

                        </label>
                    </td>
                    <td>
                        <label>
                          66313

                        </label>
                    </td>
                    <td>
                        <input type="text" class="txtBox" id="v7" style="width:200px" />
                    </td>
                    <td>
                        <textarea rows="2" cols="4" class="txtBox" id="r7"></textarea>
                    </td>

                </tr>
            </tbody>
        </table>
        <div class="pull-right" style="padding:5px;" >
            <button type="submit" class="btn btnNew" id="btnAccept">Accept</button>
            <button type="submit" class="btn btnNew" id="btnReject">Reject</button>
        </div>
        </div>
      

    </div>
    <%--  </div>--%>
    <script type="text/javascript">
        $(document).ready(function () {
            $("#btnAccept").click(function () {
                if ($("#cb1").prop("checked")==true)$("#tr1").hide();            
                if ($("#cb2").prop("checked") == true) $("#tr2").hide();
                if ($("#cb3").prop("checked") == true) $("#tr3").hide();
                if ($("#cb4").prop("checked") == true) $("#tr4").hide();
                if ($("#cb5").prop("checked") == true) $("#tr5").hide();
                if ($("#cb6").prop("checked") == true) $("#tr6").hide();
                if ($("#cb7").prop("checked") == true) $("#tr7").hide();
                alert('Your GST request has been accepted');
                return false;
            });
            $("#btnReject").click(function () {
                if ($("#cb1").prop("checked") == true) {
                    if ($("#v1").val() == "") {
                        alert('Please fill vendor amount for rejection')
                        return false;
                    }
                    if ($("#r1").val() == "")
                    {
                        alert('Please fill vendor remarks for rejection')
                        return false;
                    }
                    $("#tr1").hide()
                };
                if ($("#cb2").prop("checked") == true) {
                    if ($("#v2").val() == "") {
                        alert('Please fill vendor amount for rejection')
                        return false;
                    }
                    if ($("#r2").val() == "") {
                        alert('Please fill vendor remarks for rejection')
                        return false;
                    }
                    $("#tr2").hide()
                };
                if ($("#cb3").prop("checked") == true) {
                    if ($("#v3").val() == "") {
                        alert('Please fill vendor amount for rejection')
                        return false;
                    }
                    if ($("#r3").val() == "") {
                        alert('Please fill vendor remarks for rejection')
                        return false;
                    }
                    $("#tr3").hide()
                };
                if ($("#cb4").prop("checked") == true) {
                    if ($("#v4").val() == "") {
                        alert('Please fill vendor amount for rejection')
                        return false;
                    }
                    if ($("#r4").val() == "") {
                        alert('Please fill vendor remarks for rejection')
                        return false;
                    }
                    $("#tr4").hide()
                };
                if ($("#cb5").prop("checked") == true) {
                    if ($("#v5").val() == "") {
                        alert('Please fill vendor amount for rejection')
                        return false;
                    }
                    if ($("#r5").val() == "") {
                        alert('Please fill vendor remarks for rejection')
                        return false;
                    }
                    $("#tr5").hide()
                };
                if ($("#cb6").prop("checked") == true) {
                    if ($("#v6").val() == "") {
                        alert('Please fill vendor amount for rejection')
                        return false;
                    }
                    if ($("#r6").val() == "") {
                        alert('Please fill vendor remarks for rejection')
                        return false;
                    }
                    $("#tr6").hide()
                };
                if ($("#cb7").prop("checked") == true) {
                    if ($("#v7").val() == "") {
                        alert('Please fill vendor amount for rejection')
                        return false;
                    }
                    if ($("#r7").val() == "") {
                        alert('Please fill vendor remarks for rejection')
                        return false;
                    }
                    $("#tr7").hide()
                };
                alert('Your GST request has been rejected');
                return false;
            });
        });
        
    $('#allcb').change(function () {
        $('tbody tr td input[type="checkbox"]').prop('checked', $(this).prop('checked'));
    });
</script>
</asp:Content>


