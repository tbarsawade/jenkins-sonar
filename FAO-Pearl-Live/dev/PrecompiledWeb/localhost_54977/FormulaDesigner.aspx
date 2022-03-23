<%@ page language="VB" autoeventwireup="false" masterpagefile="~/USR.master" inherits="FormulaDesigner, App_Web_s1ukpvof" viewStateEncryptionMode="Always" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="asp" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">

    <asp:UpdatePanel ID="updPnlGrid" runat="server" UpdateMode="Conditional">
        <Triggers>
        </Triggers>

        <ContentTemplate>
            <%--<script src="//code.jquery.com/jquery-1.10.2.js" type="text/javascript"></script>
            <script src="https://ajax.googleapis.com/ajax/libs/jquery/1.6.4/jquery.min.js" type="text/javascript"></script>--%>
            <script src="jquery/jquery-3.3.1.js" type="text/javascript"></script>
            <script src="jquery/jquery-3.3.1.min.js" type="text/javascript"></script>
            <link href="css/style.css" rel="Stylesheet" type="text/css" />
            <script type="text/javascript">
                function storeCaret(textEl) { textEl.caretPos = document.selection.createRange().duplicate(); }
                function insertAtCaret(textEl, text) { if (textEl.createTextRange && textEl.caretPos) { textEl.caretPos.text = text; } else { alert('no carat'); } }

                function insertCode(t) {
                    var subject = document.getElementById('<%= txtcondition.ClientID%>');
                    if (document.selection) {
                        insertAtCaret(document.getElementById('<%= txtcondition.ClientID%>'), '[' + t + ']');
                    }
                    else if (subject.selectionStart || subject.selectionStart == '0') {
                        var str = subject.value;
                        var a = subject.selectionStart, b = subject.selectionEnd;
                        subject.value = str.substring(0, a) + arguments[0] + (arguments[1] ? str.substring(a, b) + arguments[1] : "") + str.substring(b, str.length);
                        return;
                    }
                };

                function setCaret(t) {
                   // var inp = document.getElementById('selectList');
                    insertCode('[' + t + ']');
                }
                          














                function setCaretcccccccccc(t) {
                    // var textarea = document.getElementById('<%= txtcondition.ClientID%>');
                    var e = document.getElementById('<%= tv.ClientID%>');
                    var text = e.options[e.selectedIndex].value;
                    var textarea = document.getElementById('txtcondition'),
                        tempStr1 = textarea.value.substring(0, globalCursorPos),
                        tempStr2 = textarea.value.substring(globalCursorPos);
                    textarea.value = tempStr1 + t + tempStr2;
                    document.getElementById("Line").innerHTML = '<strong>Start</strong> ' + tempStr1;
                    document.getElementById("Column").innerHTML = '<strong>End</strong> ' + tempStr2;
                }
                function updatePosition(t) {
                    globalCursorPos = getCursorPos(t);
                }
                var globalCursorPos;
                function getCursorPos(textElement) {
                    //save off the current value to restore it later,
                    var sOldText = textElement.value;


                    //create a range object and save off it's text
                    var objRange = document.selection.createRange();
                    var sOldRange = objRange.text;


                    //set this string to a small string that will not normally be encountered
                    var sWeirdString = '#%~';


                    //insert the weirdstring where the cursor is at
                    objRange.text = sOldRange + sWeirdString; objRange.moveStart('character', (0 - sOldRange.length - sWeirdString.length));


                    //save off the new string with the weirdstring in it
                    var sNewText = textElement.value;


                    //set the actual text value back to how it was
                    objRange.text = sOldRange;


                    //look through the new string we saved off and find the location of
                    //the weirdstring that was inserted and return that value
                    for (i = 0; i <= sNewText.length; i++) {
                        var sTemp = sNewText.substring(i, i + sWeirdString.length);
                        if (sTemp == sWeirdString) {
                            var cursorPos = (i - sOldRange.length);
                            return cursorPos;
                        }
                    }
                }

            </script>

            <style type="text/css">
                body
                {
                    /*padding: 50px 100px;*/
                    font: 13px/150% Verdana, Tahoma, sans-serif;
                }

                /* tutorial */

                input, textareat
                {
                    padding: 6px;
                    border: solid 1px #E5E5E5;
                    outline: 0;
                    font: normal 13px/100% Verdana, Tahoma, sans-serif;
                    width: 200px;
                    background: #FFFFFF url('bg_form.png') left top repeat-x;
                    background: -webkit-gradient(linear, left top, left 25, from(#FFFFFF), color-stop(4%, #EEEEEE), to(#FFFFFF));
                    background: -moz-linear-gradient(top, #FFFFFF, #EEEEEE 1px, #FFFFFF 25px);
                    box-shadow: #bbe2d1 0px 0px 8px;
                    -moz-box-shadow: #bbe2d1 0px 0px 8px;
                    -webkit-box-shadow: #bbe2d1 0px 0px 8px;
                }

                textarea
                {
                    width: 400px;
                    max-width: 400px;
                    height: 150px;
                    line-height: 150%;
                }

                    input:hover, textarea:hover,
                    input:focus, textarea:focus
                    {
                        border-color: #C9C9C9;
                        -webkit-box-shadow: rgba(0, 0, 0, 0.15) 0px 0px 8px;
                    }

                .label1
                {
                    margin-left: 10px;
                    color: #999999;
                }

                .gap tr td
                {
                    padding-bottom: 10px;
                }
            </style>

            <table cellpadding="0px" cellspacing="0px" style="text-align: left" width="100%" border="0">
                <tr style="color: #000000">
                    <td style="text-align: left;">

                        <asp:Label ID="lblMsg" runat="server" Font-Bold="True" ForeColor="Red"
                            Width="100%" Font-Size="Small"></asp:Label>

                    </td>
                </tr>

                <tr>
                    <td>
                        <asp:Label ID="lblHeader" runat="server" Font-Bold="True" ForeColor="Red" Text="FORMULA DESIGNER"
                            Width="100%" Font-Size="Small"></asp:Label></td>
                </tr>


                <tr style="color: #000000">

                    <td style="text-align: left; width: 100%; border: 3px double green">

                        <table style="text-align: left; margin-top: 10px;" cellpadding="5px" cellspacing="5px" width="100%" class="gap"
                            border="0">
                            <tr>
                                <td style="width: 100px;">
                                    <asp:Label ID="lblformulaName" CssClass="label1" runat="server" Width="100px" Text="Formula Name"></asp:Label>

                                </td>
                                <td style="width: 150px;">
                                    <asp:TextBox ID="txtFormulaName" placeholder="Formula Name" CssClass="textareat" runat="server" Width="150px"></asp:TextBox>
                                </td>
                                <td style="width: 150px;">
                                    <asp:Label ID="Lblfcategory" CssClass="label1" runat="server" Width="150px" Text="Formula Category" ToolTip="Formula Category"></asp:Label>
                                </td>
                                <td style="width: 150px">
                                    <asp:TextBox ID="txtformulacategory" placeholder="Formula Category" CssClass="textareat" runat="server" Width="150px"></asp:TextBox>

                                </td>
                                <td style="width: 150px;">
                                    <asp:Label ID="lbldescription" CssClass="label1" runat="server" Width="150px" Text="Formula Desc" ToolTip="Formula Description"></asp:Label>
                                </td>
                                <td style="width: 150px">
                                    <asp:TextBox ID="txtdescription" CssClass="textareat" placeholder="Formula Description" runat="server" Width="150px"></asp:TextBox>

                                </td>

                            </tr>
                            <tr>

                                <td style="width: 100px;">
                                    <asp:Label ID="lblformsource" CssClass="label1" runat="server" Width="100px" Text="Form Source"></asp:Label>
                                </td>
                                <td style="width: 150px;">
                                    <asp:DropDownList ID="ddlformsource" CssClass="textareat" runat="server" Width="150px" AutoPostBack="true">
                                        <asp:ListItem Value="0">SELECT</asp:ListItem>
                                        <asp:ListItem Value="1">DOCUMENT</asp:ListItem>
                                        <asp:ListItem Value="2">MASTER</asp:ListItem>
                                        <asp:ListItem Value="3">DETAIL FORM</asp:ListItem>
                                        <asp:ListItem Value="4">ACTION DRIVEN</asp:ListItem>
                                    </asp:DropDownList>
                                </td>
                                <td style="width: 150px;">
                                    <asp:Label ID="lblDoctype" CssClass="label1" runat="server" Width="150px" Text="Document Type"></asp:Label>
                                </td>
                                <td style="width: 150px;">
                                    <asp:DropDownList ID="ddldoctype" AutoPostBack="true" CssClass="textareat" runat="server" Width="150px">
                                    </asp:DropDownList>
                                </td>
                                <td style="width: 150px;">

                                    <asp:Label ID="lblactive" CssClass="label1" runat="server" Text="IsActive" Width="150px"></asp:Label>

                                </td>
                                <td style="width: 150px; text-align: left;">


                                    <asp:CheckBox ID="chkisactive" runat="server" />


                                </td>
                            </tr>

                            <tr runat="server" id="divrow" visible="false" cellpadding="2px" cellspacing="2px">
                                <td style="width: 100%;" colspan="2"  >
                                    <div style="height: 300px;   width: 300px; padding:0px 0px 0px 0px; overflow: scroll;">
                                        <div>
                                            <asp:TreeView ID="tv"  runat="server"  Height="100%" ImageSet="Arrows"
                                                Width="100%" ShowLines="True">
                                                <ParentNodeStyle Font-Bold="False" />
                                                <HoverNodeStyle Font-Underline="True" ForeColor="#5555DD" />
                                                <SelectedNodeStyle Font-Underline="True" HorizontalPadding="0px"
                                                    VerticalPadding="0px" ForeColor="#5555DD" />
                                                <NodeStyle Font-Names="Verdana" Font-Size="8pt" ForeColor="Black"
                                                    HorizontalPadding="5px" NodeSpacing="0px" VerticalPadding="0px" />
                                            </asp:TreeView>
                                        </div>

                                    </div>
                                </td>

                                <td style="height: 300px; width: 100%;" colspan="4">
                                    <table>
                                        <tr>
                                            <td style="width:300px;">
                                                <asp:DropDownList ID="ddlformulaes" Visible="false"  runat="server" AutoPostBack="true" CssClass="txtBox" ></asp:DropDownList>
                                            </td>
                                        </tr>
                                        <tr><td style="width:300px;">
                                            <asp:TextBox Height="300px" ID="txtcondition" onclick="storeCaret(this);"
onselect="storeCaret(this);" onkeyup="storeCaret(this);" runat="server" Style="border: 1px dashed  #54c618;" TextMode="MultiLine" Width="100%"></asp:TextBox>

                                            </td></tr>

                                    </table>
                                   <%-- <asp:TextBox Height="300px" ID="txtcondition" onmouseup="updatePosition(this);" onmousedown="updatePosition(this);"
            onkeyup="updatePosition(this);" onkeydown="updatePosition(this);" runat="server" Style="border: 1px dashed  #54c618;" TextMode="MultiLine" Width="100%"></asp:TextBox>--%>
                                     
                                </td>
                                

                            </tr>

                            </table>
                                    </fieldset>

                                </td>

                            </tr>

                            <tr>
                                <td colspan="10" align="center">
                                    <asp:Button ID="btnsave" runat="server" Text="Save" CssClass="btnNew" />
                                </td>
                            </tr>

                        </table>
                        <asp:UpdateProgress ID="UpdateProgress1" runat="server">
                            <ProgressTemplate>
                                <div id="Layer1" style="position: absolute; z-index: 1; left: 50%; top: 50%">
                                    <asp:Image ID="Image1" runat="server" Height="25px" ImageUrl="~/Images/prg.gif" />
                                    please wait...
                                </div>
                            </ProgressTemplate>
                        </asp:UpdateProgress>

                    </td>
                </tr>


            </table>

        </ContentTemplate>
    </asp:UpdatePanel>

</asp:Content>

