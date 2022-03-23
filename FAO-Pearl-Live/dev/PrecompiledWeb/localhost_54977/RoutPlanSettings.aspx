<%@ page title="" language="VB" masterpagefile="~/USR.master" autoeventwireup="false" inherits="RoutPlanSettings, App_Web_erizob0y" viewStateEncryptionMode="Always" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
    <style type="text/css">
        .report {
            border-radius:3px;
            padding:15px;
             color:#000;
             background:rgba(226, 226, 226, 0.40);
             margin-bottom:10px;
             height:80%;
             margin-top:20px;
        }

        .report1 {
            border-radius:3px;
            padding:10px;
             color:#000;
             background:rgba(226, 226, 226, 0.40);
             margin-bottom:10px;
             
             margin-top:5px;
        }

            .report table tr td {
                height:30px;
                padding:2px;
            }
        .error {
            color:#000;
            background:rgba(247, 128, 106, 0.76);
            border:1px solid #f53636;
        }
        .success {
            color:#000;
            background:rgba(132, 178, 104, 0.76);
            border:1px solid #6dbe52;
        }
            .report legend {
                width:100px;
                padding:3px;
                border:1px solid #9b9898;
                border-radius:3px;
                background:#E2E2E2;
            }
        .rptddl {
            width:250px;
            height:25px;
           
        }
        .rptddl1 {
            width:118px;
            height:25px;
            
        }
        .rptddl2 {
            height:25px;
        }
        .hide {
            display:none;
        }
        .btn {
            border:1px solid #000;
            padding:3px;
            cursor:pointer;
            color:#fff;
            background:#000;
        }
            .btn:hover{
                background-color:#458d14;
            }



      .loading-div
        {
             width: 300px;
             height: 200px;
             background-color: #fff !important;
             text-align:center;
             position:absolute;
             left: 50%;
             top: 50%;
             margin-left:-150px;
             margin-top: -100px;
             z-index:2000;
             opacity:1.0;
             color:black;
              border:3px solid #9b9898;
         }

        .loading-div-background 
        {
            display:none;
            position:fixed;
            top:0;
            left:0;
            background:rgba(0, 0, 0, 0.54);
            width:100%;
            height:100%;
            z-index:1000;
         }

        
    </style>

    <asp:UpdatePanel ID="UpdatePanel1" runat="server">
        <ContentTemplate>

    <asp:Panel ID="Panel1" runat="server">
       
    <fieldset class="report">
       
        <table style="width:100%">
            <colgroup>
                <col style="width:15%; height:30px;"/>
                <col style="width:25%; height:30px;" />
                <col style="width:10%; height:30px;"/>
                <col style="width:15%; height:30px;"/>
                <col style="width:25%; height:30px;" />
                <col style="width:10%; height:30px;"/>
                <tr>
                    <td>Site Document<span style="color:red">*</span>:</td>
                    <td>
                        <asp:DropDownList ID="ddlSiteDoc" runat="server" AutoPostBack="True" CssClass="rptddl">
                            <asp:ListItem Value="0">Select</asp:ListItem>
                        </asp:DropDownList>
                    </td>
                    <td>&nbsp;</td>
                    <td>Site ID Field<span style="color:red">*</span>:</td>
                    <td>
                        <asp:DropDownList ID="ddlSiteID" runat="server" CssClass="rptddl">
                            <asp:ListItem Value="0">Select</asp:ListItem>
                        </asp:DropDownList>
                    </td>
                    <td>&nbsp;</td>
                </tr>
                <tr>
                    <td>Site Name Field<span style="color:red">*</span>:</td>
                    <td>
                        <asp:DropDownList ID="ddlSiteNamefld" runat="server" CssClass="rptddl">
                            <asp:ListItem Value="0">Select</asp:ListItem>
                        </asp:DropDownList>
                    </td>
                    <td>&nbsp;</td>
                    <td>Site Fence Field<span style="color:red">*</span>:</td>
                    <td>
                        <asp:DropDownList ID="ddlSiteFencefld" runat="server" CssClass="rptddl">
                            <asp:ListItem Value="0">Select</asp:ListItem>
                        </asp:DropDownList>
                    </td>
                    <td>&nbsp;</td>
                </tr>
                <tr>
                    <td>Vehicle Document<span style="color:red">*</span>:</td>
                    <td>
                        <asp:DropDownList ID="ddlVehicleDoc" runat="server" AutoPostBack="True" CssClass="rptddl">
                            <asp:ListItem Value="0">Select</asp:ListItem>
                        </asp:DropDownList>
                    </td>
                    <td>&nbsp;</td>
                    <td>Vehicle No. Field<span style="color:red">*</span>:</td>
                    <td>
                        <asp:DropDownList ID="ddlVehicleNo" runat="server" CssClass="rptddl">
                            <asp:ListItem Value="0">Select</asp:ListItem>
                        </asp:DropDownList>
                    </td>
                    <td>&nbsp;</td>
                </tr>
                <tr>
                    <td>Vehicle IMEI Field<span style="color:red">*</span>:</td>
                    <td>
                        <asp:DropDownList ID="ddlIMEIfld" runat="server" CssClass="rptddl">
                            <asp:ListItem Value="0">Select</asp:ListItem>
                        </asp:DropDownList>
                    </td>
                    <td>&nbsp;</td>
                    <td>Vehicle Type Field<span style="color:red">*</span>:</td>
                    <td>
                        <asp:DropDownList ID="ddlVehicleType" runat="server" CssClass="rptddl">
                            <asp:ListItem Value="0">Select</asp:ListItem>
                        </asp:DropDownList>
                    </td>
                    <td>&nbsp;</td>
                </tr>
                <tr>
                    <td>Plan Document<span style="color:red">*</span>:</td>
                    <td>
                        <asp:DropDownList ID="ddlRoutPlan" runat="server" AutoPostBack="True" CssClass="rptddl">
                            <asp:ListItem Value="0">Select</asp:ListItem>
                        </asp:DropDownList>
                    </td>
                    <td>&nbsp;</td>
                    <td>Plan Vehicle No<span style="color:red">*</span>:</td>
                    <td>
                        <asp:DropDownList ID="ddlPlanVehicleNo" runat="server" CssClass="rptddl">
                            <asp:ListItem Value="0">Select</asp:ListItem>
                        </asp:DropDownList>
                    </td>
                    <td>&nbsp;</td>
                </tr>
                <tr>
                    <td>Plan Vehice Type<span style="color:red">*</span>:</td>
                    <td>
                        <asp:DropDownList ID="ddlPlanVehicleType" runat="server" CssClass="rptddl">
                            <asp:ListItem Value="0">Select</asp:ListItem>
                        </asp:DropDownList>
                    </td>
                    <td>&nbsp;</td>
                    <td></td>
                    <td></td>
                    <td></td>
                </tr>
                <tr>
                    <td>Route Plan Matrix<span style="color:red">*</span>:</td>
                    <td>
                        <asp:DropDownList ID="ddlPlanMatrixDoc" runat="server" AutoPostBack="True" CssClass="rptddl">
                            <asp:ListItem Value="0">Select</asp:ListItem>
                        </asp:DropDownList>
                    </td>
                    <td>&nbsp;</td>
                    <td>Destination Field<span style="color:red">*</span>:</td>
                    <td>
                        <asp:DropDownList ID="ddlDestinationfld" runat="server" CssClass="rptddl">
                            <asp:ListItem Value="0">Select</asp:ListItem>
                        </asp:DropDownList>
                    </td>
                    <td>&nbsp;</td>
                </tr>
                <tr>
                    <td>Date Field<span style="color:red">*</span>:</td>
                    <td>
                        <asp:DropDownList ID="ddlDatefld" runat="server" CssClass="rptddl">
                            <asp:ListItem Value="0">Select</asp:ListItem>
                        </asp:DropDownList>
                    </td>
                    <td>&nbsp;</td>
                    <td>Time Field<span style="color:red">*</span>:</td>
                    <td>
                        <asp:DropDownList ID="ddlTimefld" runat="server" CssClass="rptddl">
                            <asp:ListItem Value="0">Select</asp:ListItem>
                        </asp:DropDownList>
                    </td>
                    <td>&nbsp;</td>
                </tr>

                <tr>
                    <td>Hault duration:<span style="color:red">*</span>:</td>
                    <td>
                        <asp:DropDownList ID="ddlHault" runat="server" CssClass="rptddl">
                            <asp:ListItem Value="0">Select</asp:ListItem>
                        </asp:DropDownList>
                    </td>
                    <td>&nbsp;</td>
                    <td></td>
                    <td>
                        
                    </td>
                    <td>&nbsp;</td>
                </tr>

                <tr>
                    <td>Role Definition Document<span style="color:red">*</span>:</td>
                    <td>
                        <asp:DropDownList ID="ddlRoleDef" runat="server" CssClass="rptddl">
                            <asp:ListItem Value="0">Select</asp:ListItem>
                        </asp:DropDownList>
                    </td>
                    <td>&nbsp;</td>
                    <td>Vehicle and Role Definition Document Mapping<span style="color:red">*</span>:</td>
                    <td>
                        <asp:DropDownList ID="ddlVehicleMapping" runat="server" CssClass="rptddl">
                            <asp:ListItem Value="0">Select</asp:ListItem>
                        </asp:DropDownList>
                    </td>
                    <td>&nbsp;</td>
                </tr>
                
                <tr>
                    <td>IsActive:</td>
                    <td>
                        <asp:CheckBox ID="chkActive" runat="server" />
                    </td>
                    <td>&nbsp;</td>
                    <td></td>
                    <td>
                        
                    </td>
                    <td>&nbsp;</td>
                </tr>
            </colgroup>
        </table>

    </fieldset>
   
    <fieldset class="report1">
        <asp:Button ID="btnSvae" runat="server" CssClass="btn" Text="Save" />
        <asp:Button ID="btnUpdate" runat="server" CssClass="btn" Text="Update" />
        <br />
        <br />
        <asp:Label ID="lblMsg" runat="server" Font-Bold="True"></asp:Label>
    </fieldset>
    </asp:Panel>
            </ContentTemplate>
    </asp:UpdatePanel>

    <asp:UpdateProgress ID="UpdateProgress1" runat="server">
        <ProgressTemplate>
            <div id="Layer1" style="position: absolute; z-index: 9999999; left: 50%; top: 50%">
                <asp:Image ID="Image1" runat="server" Height="25px" ImageUrl="~/Images/prg.gif" />
                please wait...
            </div>
        </ProgressTemplate>
    </asp:UpdateProgress>

</asp:Content>