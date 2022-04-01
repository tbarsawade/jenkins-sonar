<%@ Page Title="" Language="VB" MasterPageFile="~/usrFullScreenBPM.master" AutoEventWireup="false" EnableEventValidation="false" CodeFile="IHCLONdemand.aspx.vb" Inherits="IHCLONdemand" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="asp" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
    <script src='http://code.jquery.com/jquery-latest.min.js' type='text/javascript'> </script>
    <script src="scripts/jquery-1.9.1.min.js" type="text/javascript"></script>
   
    <style>
         .k-grid-toolbar a {
            float: right;
        }
        .error {
            border: 1px solid red !important;
        }
        .loader {
            left: 50%;
            top: 50%;
            position: absolute;
            z-index: 101;
            opacity : 0.5;
            background-repeat : no-repeat;
            background-position : center;
            width: 32px;
            height: 32px;
            margin-left: -16px;
            margin-top: -16px;
           
        }
    </style>
    <asp:UpdatePanel ID="updPnlGrid" runat="server">
          <ContentTemplate>
      <div class="form" style="text-align: left">
        <div class="doc_header">
            Reports Integration
        </div>
               <br />  

           <div class="row mg">
                    <div class="col-md-1 col-sm-1">
                        </div>
                    <div class="col-md-1 col-sm-1">
                        <label>Report Name:</label>
                    </div>
                    <div class="col-md-3 col-sm-3">
                     
                        <asp:DropDownList ID="ddlReportName" runat="server" AutoPostBack="true"  CssClass="txtBox">
                                            </asp:DropDownList>
                       
                    </div>
                    <div class="col-md-2 col-sm-2">
                        <label>Integration Type:</label>
                    </div>
                    <div class="col-md-3 col-sm-3">

                      
                   <asp:DropDownList ID="ddlSchType" runat="server" AutoPostBack="true" CssClass="txtBox">
                        
                                       </asp:DropDownList>
                              
                      
                    </div>
                   
                     
                </div>

             <br />  

            <div class="row mg" id="dvcal" style="display:none" runat="server">
                 <%--  <div class="col-md-1 col-sm-1">
                        </div>--%>
           <div class="col-md-8 col-sm-8">
                   <%-- <label>From Date:</label>--%>
                      <asp:Label ID="Label4" runat="server"  style="margin-left:121px; font-size: 13px;"  Font-Bold="True" Text="Please Enter Comma Separated PEARL_IDs(Ex# PER0001,PER002,PER003) :"></asp:Label>
             </div>
             
                <br />
                     <asp:TextBox ID="TextBox2" Font-Size="Large" runat="server"  style="margin-left:135px;" margin-left="135px" Height="35px" Width="1052px"></asp:TextBox>
           <%--     </div>--%>
               <%-- <div class="col-md-2 col-sm-2">
                    <label>To Date:</label>
                </div>
                <div class="col-md-3 col-sm-3">
                
                    
                </div>--%>
                
                 
            </div>

          <br />  

              <div class="row">
                <div class="col-md-1 col-sm-1">
                    </div>
                <div class="col-md-11 col-sm-11">
                     <label id="lblMsg"></label>
                    </div>
               </div>
                   
        <br />

            <div class="row mg" id="dvbtn">
            <div class="col-md-7 col-sm-7">

            </div>
            <div class="col-md-1 col-sm-1">
                   <%--  <input type="button" id="btnsearch" value="ondemand" class="btnNew" onclick="" />--%>
                <asp:Button ID="btnsearch" runat="server" Text="ondemand"  OnClick="ondemandexecute" visible="false"
                                     CssClass="btnNew" />

                 <asp:Button ID="Button1" runat="server" Text="ReportScheduler"  OnClick="ondemandexecute1" visible="false"
                                     CssClass="btnNew" />
                </div>
            <div class="col-md-2 col-sm-2">
                  <%--   <input type="button" id="btnsave" value="Run Integration" class="btnNew" onclick="" />--%>
                <asp:Button ID="btnsave" runat="server"  visible="false" Text ="Run Integration"  OnClick="onReRun"
                                     CssClass="btnNew" />
                </div>
           <div class="col-md-2 col-sm-2">

            </div>
        </div>
        <br />
                <br />
        <div class="row">
            <div class="col-md-1 col-sm-1">
                 <br /><br />
                       
                
            </div>
            <div class="col-md-10 col-sm-10" id="dvkgd" style="display:none" runat="server">
                 <asp:Label ID="lblError" runat="server" Font-Bold="True" ForeColor="Red" Text=""></asp:Label>
                <br />
        <br />   
                 <asp:GridView ID="GridView1" Visible="true" runat="server" Height="145px" Width="477px">
        </asp:GridView>
                <asp:Label ID="Label1" runat="server" Text="Respone: " Font-Bold="True"></asp:Label>
        :<br />
        <br />
        <br />
        <asp:TextBox ID="TextBox1" Font-Size="Medium" runat="server" Height="59px" Width="1251px"></asp:TextBox>
        <br />
                <div id="kgrid"></div>
            </div>
            <div class="col-md-1 col-sm-1">
           
                
                
            </div>
        </div>
          <br />


          </div>


              

 <%--   <div id="dvloader" style="display: none;" class="loader">
            <input type="image" src="../images/preloader22.gif" />
        </div>
        <div id="dvloaderSave" style="display: none;" class="loader">
            <input type="image" src="../images/preloader22.gif" />
        </div>--%>
              
              </ContentTemplate>
         </asp:UpdatePanel>
       
   
</asp:Content>

