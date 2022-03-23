<%@ Page Title="" Language="VB" MasterPageFile="~/USR.master" AutoEventWireup="false" CodeFile="RuleEngineDesigner.aspx.vb" Inherits="RuleEngineDesigner" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="asp" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
    <asp:UpdatePanel ID="updPnlGrid" runat="server" UpdateMode="Conditional">
        <Triggers>
        </Triggers>

        <ContentTemplate>
            <script src="//code.jquery.com/jquery-1.10.2.js" type="text/javascript"></script>
            <link href="css/style.css" rel="Stylesheet" type="text/css" />
            <script type="text/javascript">
                function insertAtCursor(text) {
                                        var field = document.getElementById('<%= txtcondition.ClientID%>');
                                        var e = document.getElementById('<%= ddlfunctionFields.ClientID%>');
                    //  var val = $("#" + '<%= ddlfunctionFields.ClientID%>').val();
                                        var text = e.options[e.selectedIndex].value;
                    if (document.selection) {
                        var range = document.selection.createRange();
                        if (!range || range.parentElement() != field) {
                            field.focus();
                            range = field.createTextRange();
                            range.collapse(false);
                        }
                        range.text = "{" + text + "}";
                        range.collapse(false);
                        range.select();
                    } else {
                        field.focus();
                        var val = field.value;
                        var selStart = field.selectionStart;
                        var caretPos = selStart + text.length;
                        field.value = val.slice(0, selStart) + text + val.slice(field.selectionEnd);
                        field.setSelectionRange(caretPos, caretPos);
                    }
                }

                function storeCaret(textEl) { textEl.caretPos = document.selection.createRange().duplicate(); }
                function insertAtCaret(textEl, text) { if (textEl.createTextRange && textEl.caretPos) { textEl.caretPos.text = text; } else { alert('no carat'); } }

                function insertCode(t) {
                    var subject = document.getElementById('<%= txtcondition.ClientID%>');
                    if (document.selection) {
                        insertAtCaret(document.getElementById('<%= txtcondition.ClientID%>'), '{' + t + '}');
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
                    insertCode('{' + t + '}');
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

                /////
                //function insertAtCursorTree(text) {


                /// var field = document.getElementById('<%= txtcondition.ClientID%>');
                //  var e = window["<%=tv.ClientID%>" + "_Data"];
                // var src = window.event != window.undefined ? window.event.srcElement : evt.target;
                //  if (e.selectedNodeID.value != "") {

                //   var selectedNode = document.getElementById(e.selectedNodeID.value);
                //   var value = selectedNode.href.substring(selectedNode.href.indexOf(",") + 3, selectedNode.href.length - 2);
                //var ParentNode = selectedNode.parentNode.parentNode.parentNode.parentNode.parentNode.previousSibling.innerText;
                //  



                // var text = "FORM." + ParentNode + "." + selectedNode.innerHTML;
                //      if (document.selection) {
                //          var range = document.selection.createRange();
                //          if (!range || range.parentElement() != field) {
                //              field.focus();
                ////              range = field.createTextRange();
                //            range.collapse(false);
                //          }
                //          range.text = "{" + text + "}";
                //          range.collapse(false);
                //////////          range.select();
                // } else {
                //           field.focus();
                //          var val = field.value;
                //          var selStart = field.selectionStart;
                //          var caretPos = selStart + text.length;
                //          field.value = val.slice(0, selStart) + text + val.slice(field.selectionEnd);
                //          field.setSelectionRange(caretPos, caretPos);
                //      }
                //  }
                //}



                // function GetNodeValue(node) {
                // function GetNodeValue(node) {
                //  var nodeValue = "";
                //  var nodePath = node.href.substring(node.href.indexOf(",") + 2, node.href.length - 2);
                //  var nodeValues = nodePath.split("\\");
                //  if (nodeValues.length > 1)
                //      nodeValue = nodeValues[nodeValues.length - 1];
                //  else
                //      nodeValue = nodeValues[0].substr(1);
                //
                //  return nodepath;
                //}
                /////
                //function insertAtCursorTreeSource(text) {


                //  var field = document.getElementById('<%= txtcondition.ClientID%>');
                //  var e = window["<%=tvsource.ClientID()%>" + "_Data"];
                //  var src = window.event != window.undefined ? window.event.srcElement : evt.target;
                //  if (e.selectedNodeID.value != "") {

                // var selectedNode = document.getElementById(e.selectedNodeID.value);
                //  var value = selectedNode.href.substring(selectedNode.href.indexOf(",") + 3, selectedNode.href.length - 2);
                //   var ParentNode = selectedNode.parentNode.parentNode.parentNode.parentNode.parentNode.previousSibling.innerText;




                //    var text = "DS." + ParentNode + "." + selectedNode.innerHTML;
                //   if (document.selection) {
                //      var range = document.selection.createRange();
                //      if (!range || range.parentElement() != field) {
                //        field.focus();
                //         range = field.createTextRange();
                //       range.collapse(false);
                //     }
                //   range.text = "{" + text + "}";
                //    range.collapse(false);
                //     range.select();
                //  } else {
                //      field.focus();
                //     var val = field.value;
                //var selStart = field.selectionStart;
                //          var caretPos = selStart + text.length;
                //          field.value = val.slice(0, selStart) + text + val.slice(field.selectionEnd);
                //          field.setSelectionRange(caretPos, caretPos);
                //      }
                //  }
                //}

                // Tree View code ends here

                function myFunction() {
                    var greeting;
                    var V = "";
                    var T = "";
                    $(function () {
                        $('#ddlfunctionFields').change(function () {
                            V = $('#dropdown_name').val();
                            T = $('#dropdown_name :selected').text();
                        });
                    });
                    var option = document.getElementById('<%=ddlfunctionFields.ClientID%>');
                    if (t = "DAY") {
                        greeting = "Format: DAY[DateTime], Returns day e.g. Day[dd/mm/yy] result Thursday";

                    } else if (t = "DATE") {
                        greeting = "Format: DATE[datetime], Returns date e.g. Date[dd/mm/yy] result dd ";

                    }
                    else if (t = "MONTH") {
                        greeting = "Format: MONTH[datetime], Returns month e.g. Month(dd/mm/yy) result month ";

                    }
                    else if (t = "PREVIOUS MONTH") {
                        greeting = "Format: PREVIOUS MONTH(datetime), Returns Previous Month e.g. Previous Month(4-Sep-14) result August or 8";

                    }
                    else if (t = "YEAR") {
                        greeting = "Format: YEAR(datetime), Returns year e.g. Year(4-Sep-14) result 2014";

                    }
                    else if (t = "DATE RANGE") {
                        greeting = "Format: DATERANGE((datetime),(datetime)), Returns date range e.g. DateRange((4-Sep-14),(5-Sep-14))";

                    }
                    else if (t = "CONCATENATE") {
                        greeting = "Format: CONCATENATE(fld1,fld2,text,spl charcter,fld 15,…….,n)";

                    }
                    else if (t = "AND") {
                        greeting = "Format: AND or && ";


                    }
                    else if (t = "NOT") {
                        greeting = "Format: Not or !=";


                    } else if (t = "OR") {
                        greeting = "Format: OR or ||";


                    } else if (t = "MAX") {
                        greeting = "Format: Max(fld1,fld2,...,n), Returns the max value e.g Max(30,12) gives 30";


                    }
                    else if (t = "MIN") {
                        greeting = "Format: Min(fld1,fld2,....,n)";


                    }
                    document.getElementById('<%=lblexampl.ClientID%>').innerHTML = greeting;
                }
            </script>
            <table cellpadding="0px" cellspacing="0px" style="text-align: left" width="100%" border="0">
                <tr style="color: #000000">
                    <td style="text-align: left;">

                        <asp:Label ID="lblMsg" runat="server" Font-Bold="True" ForeColor="Red"
                            Width="100%" Font-Size="Small"></asp:Label>

                    </td>
                </tr>

                <tr>
                    <td>
                        <asp:Label ID="lblHeader" runat="server" Font-Bold="True" ForeColor="Red" Text="RULE ENGINE"
                            Width="100%" Font-Size="Small"></asp:Label></td>
                </tr>


                <tr style="color: #000000">

                    <td style="text-align: left; width: 100%; border: 3px double green">

                        <table style="text-align: left" cellpadding="5px" cellspacing="5px" width="100%"
                            border="0" class="rule">
                            <tr>
                                <td style="width: 100px;">
                                    <asp:Label ID="lblRuleName" runat="server"  Width="99%" Text="Rule Name"></asp:Label>

                                </td>
                                <td style="width: 150px;">
                                    <asp:TextBox ID="txtRuleName" runat="server" Width="150px"></asp:TextBox>
                                </td>
                                <td style="width: 80px;">
                                    <asp:Label ID="lbldocnature" runat="server" Width="150px" Text="Doc Nature"></asp:Label>
                                  <%--  <asp:Label ID="lblactive" runat="server" Text="IsActive" Width="150px"></asp:Label>--%>

                                </td>
                                <td style="width: 150px; text-align: left;">
                                  <%--  <asp:CheckBox ID="chkisactive" runat="server" />--%>
                                    <asp:DropDownList ID="ddldocnature" runat="server" Width="150px">
                                        <asp:ListItem Value="0">SELECT</asp:ListItem>
                                        <asp:ListItem Value="1">CREATED</asp:ListItem>
                                        <asp:ListItem Value="2">AMENDMENT</asp:ListItem>
                                        <asp:ListItem Value="3">CANCEL</asp:ListItem>
                                    </asp:DropDownList>
                                </td>
                                <td style="width: 150px;">
                                    <asp:Label ID="lbldescription" runat="server" Width="150px" Text="Rule Description"></asp:Label>
                                </td>
                                <td style="width: 450px">
                                    <asp:TextBox ID="txtdescription" runat="server" Width="95%"></asp:TextBox>

                                </td>

                            </tr>
                            <tr>

                                <td style="width: 100px;">
                                    <asp:Label ID="lblformsource" runat="server" Text="Form Source"></asp:Label>
                                </td>
                                <td style="width: 150px;">
                                    <asp:DropDownList ID="ddlformsource" runat="server" Width="99%" AutoPostBack="true">
                                        <asp:ListItem Value="0">SELECT</asp:ListItem>
                                        <asp:ListItem Value="1">DOCUMENT</asp:ListItem>
                                        <asp:ListItem Value="2">MASTER</asp:ListItem>
                                        <asp:ListItem Value="3">DETAIL FORM</asp:ListItem>
                                        <asp:ListItem Value="4">ACTION DRIVEN</asp:ListItem>
                                    </asp:DropDownList>
                                </td>
                                <td style="width: 100px;">
                                    <asp:Label ID="lblDoctype" runat="server" Width="150px" Text="Document Type"></asp:Label>
                                </td>
                                <td style="width: 150px;">
                                    <asp:DropDownList ID="ddldoctype" AutoPostBack="true" runat="server" Width="150px">
                                    </asp:DropDownList>
                                </td>
                                <td style="width: 100px;">
                                    <asp:Label ID="lbltypeofrun" runat="server" Width="150px" Text="When to Run"></asp:Label>
                                </td>
                                <td style="width: 150px;">
                                    <asp:DropDownList ID="ddltypeofrun" AutoPostBack="true" runat="server" Width="99%">

                                        <asp:ListItem Value="0">SELECT</asp:ListItem>
                                        <asp:ListItem Value="1">FORM LOAD</asp:ListItem>
                                        <asp:ListItem Value="2">SUBMIT</asp:ListItem>
                                        <asp:ListItem Value="3">CONTROL</asp:ListItem>
                                        <asp:ListItem Value="4">APPROVE</asp:ListItem>
                                        <asp:ListItem Value="5">REJECT</asp:ListItem>
                                        <asp:ListItem Value="6">CRM(HOLD)</asp:ListItem>
                                        <asp:ListItem Value="7">DRAFT</asp:ListItem>
                                        <asp:ListItem Value="8">ACTIVE</asp:ListItem>
                                        <asp:ListItem Value="9">INACTIVE</asp:ListItem>
                                    </asp:DropDownList>
                                </td>
                            </tr>
                            <tr>




                                <%--<td style="width: 150px;">
                                    <asp:Label ID="lblactiontype" Visible="false" runat="server" Text="Action Type" Width="150px"></asp:Label>
                                </td>

                                <td style="width: 150px;">
                                    <asp:DropDownList ID="ddlactiontype" Visible="false" runat="server" Width="150px" CssClass="textbox">
                                    </asp:DropDownList>
                                </td>--%>
                                <td style="width: 150px;">
                                    <asp:Button ID="btnpop" runat="server" Text="Add More Document" CssClass="btnNew" OnClick="Add" />
                                </td>

                                <td style="width: 150px;">
                                    <asp:Label ID="lblcontrolfield" runat="server" Width="150px" Visible="false" Text="Control Field"></asp:Label>

                                </td>
                                <td>
                                    <asp:DropDownList ID="ddlcontrolfield" Width="150px" runat="server" Visible="false"></asp:DropDownList>
                                    

                                </td>
                                <td>
                                    
                                    </td>
                                <td>
                                  

                                </td>
                            </tr>
                            <tr runat="server" id="divrow" visible="false">
                                <td style="width: 1000px;" colspan="7">
                                    <fieldset>
                                        
                                        <table width="100%" cellpadding="2px" cellspacing="2px">
                                            <tr>
                                                <td style="height: 300px; width: 300px;">
                                                    <div style="height: 300px; width: 300px; overflow: scroll;">
                                                        <div>
                                                            <asp:TreeView ID="tv" EnableClientScript="true" runat="server" Height="100%" ImageSet="Inbox"
                                                                Width="100%">
                                                                <ParentNodeStyle Font-Bold="False" />
                                                                <HoverNodeStyle Font-Underline="True" />
                                                                <SelectedNodeStyle Font-Underline="True" HorizontalPadding="0px"
                                                                    VerticalPadding="0px" />
                                                                <NodeStyle Font-Names="Verdana" Font-Size="8pt" ForeColor="Black"
                                                                    HorizontalPadding="5px" NodeSpacing="0px" VerticalPadding="0px" />
                                                            </asp:TreeView>
                                                        </div>
                                                        <div>
                                                            <asp:TreeView ID="tvsource" EnableClientScript="true" runat="server" Height="100%" ImageSet="Inbox"
                                                                Width="100%">
                                                                <ParentNodeStyle Font-Bold="False" />
                                                                <HoverNodeStyle Font-Underline="True" />
                                                                <SelectedNodeStyle Font-Underline="True" HorizontalPadding="0px"
                                                                    VerticalPadding="0px" />
                                                                <NodeStyle Font-Names="Verdana" Font-Size="8pt" ForeColor="Black"
                                                                    HorizontalPadding="5px" NodeSpacing="0px" VerticalPadding="0px" />
                                                            </asp:TreeView>
                                                            <asp:Panel ID="pnllsd" runat="server">
                                                            </asp:Panel>
                                                        </div>
                                                        
                                                    </div>


                                                    <td style="width: 100%" colspan="7">
                                                        <div style="height: 300px; width: 100%;">
                                                            <table width="100%">
                                                                <tr>
                                                                    <td colspan="6">
                                                                        <asp:Panel runat="server" ID="pblm" Height="120px" ScrollBars="Vertical"  >
                                                                            <asp:GridView ID="gvmap" OnRowDataBound="OnRowDataBound" OnRowDeleting="OnRowDeleting" Width="98%" runat="server" AutoGenerateColumns="true">
                                                                                <Columns>
                                                                                    <asp:TemplateField HeaderText="Action">
                                                                                        <ItemTemplate>
                                                                                            <asp:ImageButton ID="btnDelete" ImageUrl="~/images/closered.png" CommandName="Delete" runat="server" Height="16px" Width="16px" ToolTip="Delete Role" AlternateText="Delete" />
                                                                                        </ItemTemplate>
                                                                                        <ItemStyle Width="50px" HorizontalAlign="Center" />
                                                                                       
                                                                                    </asp:TemplateField>
                                                                                    
                                                                                    <asp:TemplateField HeaderStyle-HorizontalAlign="Left" HeaderText="S.No">
                                                                                        <ItemTemplate>
                                                                                            <%# CType(Container, GridViewRow).RowIndex + 1%>
                                                                                        </ItemTemplate>
                                                                                        <HeaderStyle HorizontalAlign="Left" />
                                                                                    </asp:TemplateField>

                                                                                </Columns>
                                                                            </asp:GridView>

                                                                        </asp:Panel>
                                                                    </td>

                                                                </tr>
                                                                <tr>
                                                                    <td style="width: 150px;">
                                                                        <asp:Label ID="lblfunctioncategory" runat="server" Width="150px" Text="Function Categories"></asp:Label>
                                                                    </td>
                                                                    <td style="width: 150px;">
                                                                        <asp:DropDownList ID="ddlfunctionCat" AutoPostBack="true" runat="server" Width="150px">
                                                                            <asp:ListItem Value="0">Select</asp:ListItem>
                                                                            <asp:ListItem Value="1">DATE & TIME</asp:ListItem>
                                                                            <asp:ListItem Value="2">TEXT</asp:ListItem>
                                                                            <asp:ListItem Value="3">LOGICAL</asp:ListItem>
                                                                            <asp:ListItem Value="4">MATH</asp:ListItem>

                                                                        </asp:DropDownList>

                                                                    </td>
                                                                    <td style="width: 150px;">
                                                                        <asp:Label ID="lblfunctionfields" runat="server" Text="Function Fields" Width="150px"></asp:Label>
                                                                    </td>
                                                                    <td style="width: 150px;">
                                                                        <asp:DropDownList ID="ddlfunctionFields" Onchange="myFunction();" AutoPostBack="True" runat="server" Width="150px">
                                                                            <asp:ListItem Value="DAY[datetime]">DAY</asp:ListItem>
                                                                            
                                                                            <asp:ListItem Value="DATE[datetime]">DATE</asp:ListItem>
                                                                                                   
                                                                            <asp:ListItem Value="MONTH[datetime]">MONTH</asp:ListItem>
                                                                            
                                                                            <asp:ListItem Value="PREVIOUS MONTH[datetime]">PREVIOUS MONTH</asp:ListItem>
                                                                            
                                                                            <asp:ListItem Value="YEAR[datetime]">YEAR</asp:ListItem>
                                                                            
                                                                            <asp:ListItem Value="DATERANGE[datetime,datetime]">DATE RANGE</asp:ListItem>
                                                                            
                                                                            <asp:ListItem Value="CONCATENATE[fld1,fld2,text,spl charcter,fld 15,…….,n]">CONCATENATE</asp:ListItem>
                                                                            
                                                                            <asp:ListItem Value="&&">AND</asp:ListItem>
                                                                                                                
                                                                            <asp:ListItem Value="!=">NOT</asp:ListItem>
        
                                                                            <asp:ListItem Value="||">OR</asp:ListItem>
        
                                                                            <asp:ListItem Value="MAX[comma separated values]">MAX</asp:ListItem>
        
                                                                            <asp:ListItem Value="MIN[comma separated values]">MIN</asp:ListItem>
                                                                            
 		       
                                                                        </asp:DropDownList>
                                                                      <%--  <asp:DropDownList ID="ddlfunctionFields" runat="server" Width="150px"></asp:DropDownList>--%>

                                                                    </td>
                                                                    <td>

                                                                        <asp:Button ID="btnfflds" Visible="True" runat="server" onmousedown="insertAtCursor('<%= txtcondtion.clientid %>'); return false" CssClass="btnNew" Text="Insert" />
                                                                    </td>
                                                                </tr>
                                                                <tr align="right">
                                                                    <td colspan="4">
                                                                        <asp:Label ID="lblexampl" ForeColor="Blue" Font-Size="X-Small" runat="server"></asp:Label></td>
                                                                </tr>
                                                                <caption>
                                                                    
                                                                    <tr>
                                                                        <td colspan="5" style="width: 99%; height: 130px;">
                                                                            <span>Condtion :</span>
                                                                            <asp:TextBox ID="txtcondition" onclick="storeCaret(this);"
                                                                                onselect="storeCaret(this);" onkeyup="storeCaret(this);" runat="server" Height="80%" Style="border: 1px dashed  #54c618;" TextMode="MultiLine" Width="98%"></asp:TextBox>
                                                                        </td>
                                                                    </tr>
                                                                </caption>


                                                            </table>

                                                        </div>
                                                    </td>
                                                </td>

                                            </tr>
                                            <tr>
                                                <%--<td style="width: 150px">
                                                    <asp:Label ID="lblfields" Width="150px" runat="server" Text="Fields"></asp:Label>
                                                </td>
                                                <td style="width: 150px">
                                                    <asp:DropDownList ID="ddlfields" runat="server" Width="150px"></asp:DropDownList>
                                                </td>--%>
                                                <td style="width: 150px"></td>

                                            </tr>
                                        </table>
                                    </fieldset>

                                </td>

                            </tr>
                            <tr runat="server" id="divrows" visible="false">
                                <td style="width: 100%;" colspan="6">
                                    <fieldset>
                                        <legend>&nbsp;&nbsp;&nbsp;Success Action:&nbsp;&nbsp;&nbsp;  </legend>
                                        <table width="100%" cellpadding="2" cellspacing="5">
                                            <tr>
                                                <td style="width: 160px">
                                                    <asp:Label ID="lblsaction" runat="server" Width="100px" Text="Action Type"></asp:Label></td>
                                                <td style="width: 150px;">
                                                    <asp:DropDownList ID="ddlsuccessaction" AutoPostBack="true" runat="server" Width="150px" CssClass="textbox">
                                                    </asp:DropDownList>

                                                </td>
                                                <td style="width: 150px;">
                                                    <asp:DropDownList ID="ddlsf" Visible="false" runat="server" Width="150px"></asp:DropDownList>

                                                    <asp:Button ID="btnopentargetcontrol" Visible="false"  runat="server" Text="Choose Target Fields" CssClass="btnNew" />
                                                    </td>

                                                <td >
                                                   
                                                    <asp:TextBox ID="txtboxtargetcontrol" Visible="false"  runat="server" ReadOnly="true"></asp:TextBox>
                                                </td>
                                                
                                                <td style="width: 150px">
                                                    <asp:Label ID="lbler" Width="150px" runat="server" Text="Message"></asp:Label>
                                                </td>
                                                <td style="width: 150px;" colspan="2">
                                                    <asp:TextBox ID="txtsuccessmsg" Style="border: 1px dashed  #54c618;" runat="server" Width="150px" TextMode="SingleLine"></asp:TextBox>
                                                </td>

                                                
                                                
                                            </tr>
                                            <tr>

                                                <td colspan="7"><span style="font-size: smaller;">Note: This message will appear when the success condition is TRUE</span> </td>
                                            </tr>



                                        </table>


                                    </fieldset>



                                </td>
                            </tr>
                            <tr runat="server" id="divrow2" visible="false">
                                <td style="width: 100%;" colspan="6">
                                    <fieldset>
                                        <legend>&nbsp;&nbsp;&nbsp; Failure Action:&nbsp;&nbsp;&nbsp;  </legend>
                                        <table width="100%" cellpadding="2" cellspacing="5">
                                            <tr>
                                                <td style="width: 160px">
                                                    <asp:Label ID="lblerroraction" runat="server" Width="100px" Text="Action Type"></asp:Label></td>
                                                <td style="width: 150px;">
                                                    <asp:DropDownList ID="ddlfaction" runat="server" Width="150px" CssClass="textbox" Height="16px">
                                                    </asp:DropDownList>
                                                </td>
                                                <td style="width: 150px;">
                                                    <asp:DropDownList ID="ddlfflds" Visible="false" runat="server" Width="150px"></asp:DropDownList>
                                                </td>
                                                
                                                <td style="width: 150px">
                                                    <asp:Label ID="lblerrormessage" Width="150px" runat="server" Text="Message"></asp:Label>
                                                </td>
                                                <td style="width: 150px;" colspan="3">
                                                    <asp:TextBox ID="txterrormessage" Style="border: 1px dashed  #fc3737;" runat="server" Width="150PX" TextMode="SingleLine"></asp:TextBox>
                                                </td>

                                            </tr>
                                            <tr>
                                                <td>
                                                    <span></span>
                                                </td>
                                                <td colspan="4"><span style="font-size: smaller;">Note: This message will appear when the error condition is TRUE</span> </td>
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


           <%-- <asp:Button ID="btnShowPopupDelFolder" runat="server" Style="display:none;" />
            <asp:ModalPopupExtender ID="btnDelFolder_ModalPopupExtender" runat="server"
                TargetControlID="btnShowPopupDelFolder" PopupControlID="pnlPopupDelFolder"
                CancelControlID="btnCloseDelFolder" BackgroundCssClass="modalBackground"
                DropShadow="true">
            </asp:ModalPopupExtender>

            <asp:Panel ID="pnlPopupDelFolder" runat="server" style="display:grid ;" Width="500px" BackColor="aqua">
                <div class="box">
                    <table cellspacing="2px" cellpadding="2px" width="100%">
                        <tr>
                            <td style="width: 480px">
                                <h3>Delete Rule Relation</h3>
                            </td>
                            <td style="width: 20px">
                                <asp:ImageButton ID="btnCloseDelFolder"
                                    ImageUrl="images/close.png" runat="server" /></td>
                        </tr>
                        <tr>
                            <td colspan="2">
                                <asp:UpdatePanel ID="updatePanelDelFolder" runat="server" UpdateMode="Conditional">
                                    <ContentTemplate>
                                        <table cellspacing="4px" cellpadding="0px" width="100%" border="0">
                                            <tr>
                                                <td colspan="2" align="left">
                                                    <asp:Label ID="lblMsgDelFolder" runat="server" Text="Are you sure want to delete" Font-Bold="True" ForeColor="Red"
                                                        Width="100%" Font-Size="X-Small"></asp:Label>

                                                </td>
                                            </tr>
                                        </table>
                                        <div style="width: 100%; text-align: right">
                                            <asp:Button ID="btnActDelFolder" runat="server" Text="Yes Delete"
                                                CssClass="btnNew" Font-Bold="True"
                                                Font-Size="X-Small" Width="100px" />
                                        </div>
                                    </ContentTemplate>
                                </asp:UpdatePanel>
                            </td>
                        </tr>
                    </table>
                </div>
            </asp:Panel>--%>
            <asp:Button ID="btnShowPopupEdit" runat="server" Style="display: none" />
            <asp:ModalPopupExtender ID="btnEdit_ModalPopupExtender" runat="server"
                TargetControlID="btnShowPopupEdit" PopupControlID="pnlPopupEdit"
                CancelControlID="btnCloseEdit" BackgroundCssClass="modalBackground"
                DropShadow="true">
            </asp:ModalPopupExtender>

            <asp:Panel ID="pnlPopupEdit" runat="server" Width="600px" Style="display: none" BackColor="aqua">
                <div class="box">
                    <table cellspacing="2px" cellpadding="2px" width="100%">
                        <tr>
                            <td style="width: 580px">
                                <h3>Add Rule Relation</h3>
                            </td>
                            <td style="width: 20px">
                                <asp:ImageButton ID="btnCloseEdit"
                                    ImageUrl="images/close.png" runat="server" /></td>
                        </tr>
                        <tr>
                            <td colspan="2">

                                <asp:UpdatePanel ID="updatePanelEdit" runat="server" UpdateMode="Conditional">
                                    <ContentTemplate>

                                        <table cellspacing="4px" cellpadding="0px" width="100%" border="0px">
                                            <tr>
                                                <td colspan="2" align="left">
                                                    <asp:Label ID="lblMsgEdit" runat="server" Font-Bold="True" ForeColor="Red"
                                                        Width="100%" Font-Size="X-Small"></asp:Label>

                                                </td>
                                            </tr>

                                            <tr>
                                                <td style="width: 125px" align="left">Source Document<b>  </b></td>
                                                <td align="left">
                                                    <asp:Label ID="lblsd" runat="server"></asp:Label></td>

                                                <td style="width: 150px" align="left">Source Doc Fields</td>
                                                <td>
                                                    <asp:DropDownList ID="ddlsdf" runat="server" Width="99%"></asp:DropDownList>
                                                </td>

                                            </tr>
                                            <tr>
                                                <td style="width: 150px" align="left">
                                                    <asp:Label ID="lblsformsource" Text="Target Doc Type" runat="server"></asp:Label>
                                                </td>
                                                <td style="width: 150px;">
                                                    <asp:DropDownList ID="ddlsourcetype" AutoPostBack="true" Width="99%" runat="server">
                                                        <asp:ListItem Value="0">SELECT</asp:ListItem>
                                                        <asp:ListItem Value="1">DOCUMENT</asp:ListItem>
                                                        <asp:ListItem Value="2">MASTER</asp:ListItem>
                                                        <asp:ListItem Value="3">DETAIL FORM</asp:ListItem>
                                                        <asp:ListItem Value="4">ACTION DRIVEN</asp:ListItem>
                                                    </asp:DropDownList></td>
                                                <td>Target Document Name</td>
                                                <td style="width: 150px;">
                                                    <asp:DropDownList Width="99%" AutoPostBack="true" ID="ddlsourcedoc" runat="server">
                                                        <asp:ListItem></asp:ListItem>
                                                    </asp:DropDownList>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td style="width: 150px" align="left">
                                                    <asp:Label ID="lbldss" runat="server" Text="Target Fields"></asp:Label>
                                                </td>
                                                <td style="width: 150px" align="left">
                                                    <asp:DropDownList ID="ddltf" Width="99%" runat="server"></asp:DropDownList>
                                                </td>
                                                <td style="width: 150px" align="left"> <asp:Label ID="lblsortsource" runat="server" Text="Sorting Fields"></asp:Label></td>
                                                <td style="width: 150px" align="left">
                                                     <asp:DropDownList Width="99%"  ID="ddlsortingfields" runat="server">
                                                        
                                                    </asp:DropDownList>
                                                </td>
                                            </tr>
                                           


                                        </table>
                                        <div style="width: 100%; text-align: right">
                                            <asp:Button ID="btnActEdit" runat="server" Text="Update"
                                                OnClick="EditRecord" CssClass="btnNew" Font-Bold="True"
                                                Font-Size="X-Small" Width="100px" />
                                        </div>
                                    </ContentTemplate>
                                </asp:UpdatePanel>

                            </td>
                        </tr>
                    </table>
                </div>
            </asp:Panel>

            <%-- Code is for taget control fields --%>
            <asp:ModalPopupExtender ID="modalpopuptagetfields" runat="server"
                TargetControlID="btnopentargetcontrol" PopupControlID="pnltagetfields"
                CancelControlID="btnclosetargtet" BackgroundCssClass="modalBackground"
                DropShadow="true">
            </asp:ModalPopupExtender>

            <asp:Panel ID="pnltagetfields" runat="server" Width="500px" style="display:none;" BackColor="aqua">
                <div class="box">
                    <table cellspacing="2px" cellpadding="2px" width="100%">
                        <tr>
                            <td style="width: 480px">
                                <h3>Choose Target Controls</h3>
                            </td>
                            <td style="width: 20px">
                                <asp:ImageButton ID="btnclosetargtet"
                                    ImageUrl="images/close.png" runat="server" /></td>
                        </tr>
                        <tr>
                            <td colspan="2">
                                
                                <asp:UpdatePanel ID="updatePanel1" runat="server" UpdateMode="Conditional">
                                    <ContentTemplate>
                                        <table cellspacing="4px" cellpadding="0px" width="100%" border="0">
                                            <tr>
                                                <td colspan="2" align="left">
                                                    <div style="height: 200px; overflow: scroll;">
                                              <asp:CheckBoxList ID="chktargetfields" runat="server" ></asp:CheckBoxList>
                                                    </div>
                                                </td>
                                            </tr>
                                        </table>
                                        <div style="width: 100%; text-align: right">
                                            <asp:Button ID="btnsavetargetfields" runat="server" Text="Ok"
                                                CssClass="btnNew" Font-Bold="True"
                                                Font-Size="X-Small" Width="100px" />
                                        </div>
                                    </ContentTemplate>
                                </asp:UpdatePanel>
                               
                            </td>
                        </tr>
                    </table>
                </div>
            </asp:Panel>


        </ContentTemplate>
    </asp:UpdatePanel>

</asp:Content>

