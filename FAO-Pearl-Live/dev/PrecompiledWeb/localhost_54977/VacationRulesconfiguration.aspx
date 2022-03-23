<%@ page title="" language="VB" masterpagefile="~/usrFullScreenBPM.master" autoeventwireup="false" enableeventvalidation="false" inherits="VacationRulesconfiguration, App_Web_sfds111l" viewStateEncryptionMode="Always" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="asp" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
      <script src="scripts/jquery-1.9.1.min.js" type="text/javascript"></script>
    <style type="text/css">
        .mg {
            margin: 10px 0px;
        }
    </style>
    <asp:UpdatePanel ID="updPnlGrid" runat="server">
<Triggers>
          
            <asp:PostBackTrigger ControlID="btnexport" />
            
       </Triggers>
  <ContentTemplate>
    <div class="form" style="text-align: left">

        <div class="doc_header">
           Vacation Rule
        </div>

         <div class="row mg">
                    <div class="col-md-2 col-sm-2">
                        <label>source user ID</label>
                    </div>

             <div class="col-md-3 col-sm-3">
  
          
 <asp:DropDownList ID="sourceuserID" runat="server" Autopostback="true"  CssClass="txtBox">
                                            </asp:DropDownList>
         </div>
       
           
                            <div class="col-md-2 col-sm-2">
                                <label>Delegate ID</label>
                            </div>
                            <div class="col-md-3 col-sm-3">
                                <asp:DropDownList ID="Delegate_to_Id" runat="server" Autopostback="true"  CssClass="txtBox">
                                            </asp:DropDownList>
<asp:RequiredFieldValidator ID="Req_ID" Display="Dynamic"  style="color:red" runat="server" ValidationGroup="Group1"  ControlToValidate="Delegate_to_Id" InitialValue="SELECT" ErrorMessage="please Select Delegate Id."></asp:RequiredFieldValidator>
                            </div>
                       
             </div>

      <div class="row mg">

            <div class="col-md-2 col-sm-2">
                         <label>Delegate To Name</label>
                    </div>

         <div class="col-md-3 col-sm-3">
       <asp:TextBox ID="Delegate_to_Name" runat="server" CssClass="txtBox"></asp:TextBox>

                            </div>

     <div class="col-md-2 col-sm-2">
                                <label>Start Date</label>
                            </div>
                             <div class="col-md-3 col-sm-3">
                                   <asp:TextBox ID="txtSdate" runat="server" class="txtBox"></asp:TextBox>
   <asp:CalendarExtender ID="calendersdate" TargetControlID="txtSdate" Format="yyyy-MM-dd"  OnClientDateSelectionChanged="checkDate" runat="server"   />
<asp:RequiredFieldValidator runat="server" id="reqtxtSdate" style="color:red"  controltovalidate="txtSdate" ValidationGroup="Group1"  errormessage="Please enter your start Date!" />
                            </div>


          </div>

         <div class="row mg">
              <div class="col-md-2 col-sm-2">
                                <label>End date</label>
                            </div>
                             <div class="col-md-3 col-sm-3">
                                 <asp:TextBox ID="txtEdate" runat="server"  class="txtBox" ></asp:TextBox>
                            <asp:CalendarExtender ID="calenderEdate" TargetControlID="txtEdate" OnClientDateSelectionChanged="checkDate1"  Format="yyyy-MM-dd" runat="server" />
<asp:RequiredFieldValidator runat="server" id="reqtxtEdate" controltovalidate="txtEdate" style="color:red" ValidationGroup="Group1"   errormessage="Please enter your End Date!" />
                            </div>
<%--------%>
 <div class="col-md-2 col-sm-2">
                                 <label>Delegation Reason</label>
                            </div>
                           <div class="col-md-3 col-sm-3">

                                    <textarea  id="Delegation_Reason" class="txtBox" runat="server"  rows="4" cols="50" />
<asp:RequiredFieldValidator runat="server" id="reqName" controltovalidate="Delegation_Reason" ValidationGroup="Group1"  style="color:red"  errormessage="Please enter Delegation Reason!" />

                            </div>
<%--------%>

         </div>

         <div class="row">
                            <div class="col-md-12 col-sm-12" style="text-align: center;">
                                <asp:Button ID="btnActAdd" runat="server" Text="SAVE"  OnClick="AddRecord" ValidationGroup="Group1"
                                     CssClass="btnNew" />
                            </div>
                        </div>

      <%--  --sate grid--%>

        <div class="row mg">
<div class="col-md-12 col-sm-12 mg"">
<asp:ImageButton ID="btnexport" runat="server"  ToolTip="Export" style="float: right;margin-bottom: 1%; width: 1%;" ImageUrl="~/Images/excel.gif"/>
<asp:GridView ID="gvData" runat="server" AutoGenerateColumns="False" CellPadding="2" DataKeyNames="tid"
                            ForeColor="#333333" Width="100%" AllowSorting="True" AllowPaging="false">
                            <FooterStyle BackColor="#507CD1" Font-Bold="True" ForeColor="White" />
                            <RowStyle BackColor="#EFF3FB" />
                            <EditRowStyle BackColor="#2461BF" />
                            <SelectedRowStyle BackColor="#D1DDF1" Font-Bold="True" ForeColor="#333333" />
                            <PagerStyle BackColor="CornflowerBlue" ForeColor="White" HorizontalAlign="Center" />
                            <HeaderStyle BackColor="CornflowerBlue" Font-Bold="True" ForeColor="White" />
                            <AlternatingRowStyle BackColor="White" />
                            <Columns>

                                <asp:TemplateField HeaderText="S.No">
                                    <ItemTemplate>
                                        <%# CType(Container, GridViewRow).RowIndex + 1%>
                                    </ItemTemplate>
                                    <ItemStyle Width="50px" />
                                </asp:TemplateField>
                                <asp:BoundField DataField="CreatorID" HeaderText="Creater Id">
                                    <HeaderStyle HorizontalAlign="Left" />
                                </asp:BoundField>
                                <asp:BoundField DataField="CreatorName" HeaderText="Creater Name">
                                    <HeaderStyle HorizontalAlign="Left" />
                                </asp:BoundField>
                                <asp:BoundField DataField="Delegate_to_Id" HeaderText="Delegate userid  Id">
                                    <HeaderStyle HorizontalAlign="Left" />
                                </asp:BoundField>
<asp:BoundField DataField="Delegate_to_Name" HeaderText="Delegate user  Name">
                                    <HeaderStyle HorizontalAlign="Left" />
                                </asp:BoundField>
<asp:BoundField DataField="Start_Date" HeaderText="Start Date">
                                    <HeaderStyle HorizontalAlign="Left" />
                                </asp:BoundField>
<asp:BoundField DataField="End_Date" HeaderText="End Date">
                                    <HeaderStyle HorizontalAlign="Left" />
                                </asp:BoundField>
<asp:BoundField DataField="Delegation_Reason" HeaderText="Delegation Reason">
                                    <HeaderStyle HorizontalAlign="Left" />
                                </asp:BoundField>
                             
                                <asp:TemplateField HeaderText="Action">
                                    <ItemTemplate>
                                        <asp:ImageButton ID="btnEdit" runat="server" ImageUrl="~/images/edit.jpg" Height="16px" Width="16px"  OnClick="EditHit" AlternateText="Edit" />
                                    </ItemTemplate>
                                    <ItemStyle Width="60px" HorizontalAlign="Center" />
                                </asp:TemplateField>
                            </Columns>
                        </asp:GridView>
    </div>
            </div>
       <%-- ----end grid--%>

        </div>
            </ContentTemplate>
    </asp:UpdatePanel>

<%------end date--%>


    <asp:Button ID="btnShowPopupEdit" runat="server" Style="display: none" />
    <asp:ModalPopupExtender ID="btnEdit_ModalPopupExtender" runat="server"
        TargetControlID="btnShowPopupEdit" PopupControlID="pnlPopupEdit"
        CancelControlID="btnCloseEdit" BackgroundCssClass="modalBackground"
        DropShadow="true">
    </asp:ModalPopupExtender>

<asp:Panel ID="pnlPopupEdit" runat="server" Width="600px" BackColor="aqua">
        <div class="form">
            <div class="doc_header">
                Update 
                <div class="pull-right">
                    <asp:ImageButton ID="btnCloseEdit"
                        ImageUrl="images/close.png" runat="server" />
                </div>
            </div>
            <asp:UpdatePanel ID="updatePanelEdit" runat="server" UpdateMode="Conditional">
                <ContentTemplate>

                    <div class="row mg">
                        <div class="col-md-12 col-sm-12 mg" style="text-align: center;">
                            <asp:Label ID="lblMsgEdit" runat="server" Font-Bold="true" ForeColor="Red" Width="100%"></asp:Label>
                        </div>
                        <div class="row">
                            <div class="col-md-3 col-sm-3">
                                <label>Delegate to Id*</label>
                            </div>
                            <div class="col-md-9 col-sm-9">
                                <asp:DropDownList ID="Delegate_to_Id1" runat="server" Autopostback="true"  CssClass="txtBox">
                                            </asp:DropDownList>
<asp:RequiredFieldValidator ID="Req_ID1" Display="Dynamic"  style="color:red" runat="server" ValidationGroup="Group2"  ControlToValidate="Delegate_to_Id1" InitialValue="SELECT" ErrorMessage="please Select Delegate Id."></asp:RequiredFieldValidator>
                            </div>
                        </div>
                        <div class="row">
                            <div class="col-md-3 col-sm-3">
                                <label>Delegate_to_Name</label>
                            </div>
                            <div class="col-md-9 col-sm-9">
                                <asp:TextBox ID="Delegate_to_Name1" runat="server" CssClass="txtBox"></asp:TextBox>
                            </div>
                        </div>

                          <div class="row">
                            <div class="col-md-3 col-sm-3">
                                <label>Start Date</label>
                            </div>
                            <div class="col-md-9 col-sm-9">
                                <asp:TextBox ID="txtS1date" runat="server"></asp:TextBox>

   <asp:CalendarExtender ID="calenders1date" TargetControlID="txtS1date"  OnClientDateSelectionChanged="checkDate2"  Format="yyyy-MM-dd" PopupButtonID="ImgCalendar" runat="server"    />
<asp:RequiredFieldValidator runat="server" id="reqtxtSdate2" style="color:red"  controltovalidate="txtS1date" ValidationGroup="Group2"  errormessage="Please enter your start Date!" />
                            </div>
                        </div>

  <div class="row">
                            <div class="col-md-3 col-sm-3">
                                <label>End Date</label>
                            </div>
                            <div class="col-md-9 col-sm-9">
                                <asp:TextBox ID="txtE1date" runat="server"></asp:TextBox>
   <asp:CalendarExtender ID="calenderE1date" TargetControlID="txtE1date"   OnClientDateSelectionChanged="checkDate3"  Format="yyyy-MM-dd"   runat="server"  />
<asp:RequiredFieldValidator runat="server" id="reqtxtE1date" style="color:red"  controltovalidate="txtE1date" ValidationGroup="Group2"  errormessage="Please enter your end  Date!" />
                            </div>
                        </div>
                        <div class="row">
                            <div class="col-md-3 col-sm-3">
                                <label>Delegation_Reason</label>
                            </div>
                            <div class="col-md-9 col-sm-9">
                                 <textarea  id="Delegation_Reason1" class="txtBox" runat="server"   rows="4" cols="50" />
<asp:RequiredFieldValidator runat="server" id="reqName1" controltovalidate="Delegation_Reason1" ValidationGroup="Group2"  style="color:red"  errormessage="Please enter Delegation Reason!" />
                                 

                            </div>
                        </div>
                      
                       
                        <div class="row">
                            <div class="col-md-12 col-sm-12" style="text-align: center;">
                                <asp:Button ID="btnActEdit" runat="server" Text="Update"
                                   OnClick="EditRecord"   ValidationGroup="Group2"  CssClass="btnNew" />
                            </div>
                        </div>
                    </div>

                </ContentTemplate>
            </asp:UpdatePanel>
            </div>
     </asp:Panel>
     <script type="text/javascript">

     $('#ContentPlaceHolder1_txtEdate').on('change', function () {
        var startDate = $('#ContentPlaceHolder1_txtSdate').val();
        var endDate = $('#ContentPlaceHolder1_txtEdate').val();
        if (endDate < startDate) {
            alert('End date should be greater than Start date.');
            $('#ContentPlaceHolder1_txtEdate').val('');
        }
         });
    function checkDate(sender, args) {
        
            var toDate = new Date();
            toDate.setMinutes(0);
            toDate.setSeconds(0);
            toDate.setHours(0);
            toDate.setMilliseconds(0);
            if (sender._selectedDate < toDate) {
                alert("You can't select day earlier than today!");
                $('#ContentPlaceHolder1_txtSdate').val('');

                //sender._selectedDate = toDate;
                ////set the date back to the current date
              /*  *//*sender._textbox.set_Value(sender._selectedDate.format(sender._format))*/
               
             }
         }

         function checkDate1(sender, args) {
             var toDate = new Date();
             toDate.setMinutes(0);
             toDate.setSeconds(0);
             toDate.setHours(0);
             toDate.setMilliseconds(0);
             if (sender._selectedDate < toDate) {
                 alert("You can't select day earlier than today!");
                 $('#ContentPlaceHolder1_txtEdate').val('');

             }
    }
    function checkDate2(sender, args) {
        var toDate = new Date();
        toDate.setMinutes(0);
        toDate.setSeconds(0);
        toDate.setHours(0);
        toDate.setMilliseconds(0);
        if (sender._selectedDate < toDate) {
            alert("You can't select day earlier than today!");
            $('#ContentPlaceHolder1_txtS1date').val('');

        }
    }

    function checkDate3(sender, args) {
        var toDate = new Date();
        toDate.setMinutes(0);
        toDate.setSeconds(0);
        toDate.setHours(0);
        toDate.setMilliseconds(0);
        if (sender._selectedDate < toDate) {
            alert("You can't select day earlier than today!");
            $('#ContentPlaceHolder1_txtE1date').val('');

        }
    }

   
     </script>


 <%--<script type="text/javascript">
 $('#ContentPlaceHolder1_txtE1date').on('change', function () {
        var startDate = $('#ContentPlaceHolder1_txtS1date').val();
        var endDate = $('#ContentPlaceHolder1_txtE1date').val();
        if (endDate < startDate) {
            alert('End date should be greater than Start date.');
            $('#ContentPlaceHolder1_txtE1date').val('');
        }
    });

</script>--%>



</asp:Content>

