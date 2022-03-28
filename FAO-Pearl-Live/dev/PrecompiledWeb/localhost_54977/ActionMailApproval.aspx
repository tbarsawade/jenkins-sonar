<%@ page language="VB" autoeventwireup="false" inherits="ActionMailApproval, App_Web_pnyzbdje" viewStateEncryptionMode="Always" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">

<head runat="server">

    <link href="App_Themes/M1/bootstrap.min.css" rel="stylesheet" />

    <title></title>

    <style type="text/css">
        .section-box {
            background-color: #ffffff;
            border-radius: 8px;
            box-shadow: 0 0 5px #a0a0a0;
            box-sizing: border-box;
            margin: 8px auto;
            min-height: auto;
            width: auto;
        }

            .section-box img {
                margin: auto;
            }

        .note-msg {
            font-family: Open Sans;
            font-size: 20px;
            color: #7a7556;
            padding: 10px;
            font-weight: 800;
            text-shadow: 1px 1px 8px #bbb;
            text-align: center;
        }

        .mg20 {
            margin: 20px;
        }
    </style>
</head>
<body>
    <form id="form1" runat="server">
        <div class="container">
            <section class="section-box">
                <div id="happySign" runat="server">
                    <%--  <img src="images/thanks.png" class="img-responsive" />--%>
                    <div class="note-msg" id="thanksNote" runat="server">We can't find the page you looking for...</div>
                </div>
                <div id="sadsign" runat="server">
                    <%--<img src="images/sorry.png" class="img-responsive" />--%>
                    <div class="note-msg" id="sorrynote" runat="server">We can't find the page you looking for... </div>
                </div>
            </section>
            <div id="loginPassword" runat="server" visible="false">


                <div class="col-md-12 col-sm-12" style="text-align: center; color: red;">
                    <asp:Label ID="lblMsg" runat="server"></asp:Label>
                </div>
                <div class="col-md-12 col-sm-12">
                    <div class="col-md-3 col-sm-3">
                        &nbsp;
                    </div>
                    <div class="col-md-2">
                        Password *
                    </div>
                    <div class="col-md-4 col-sm-4">
                        <asp:TextBox ID="txtPassword" runat="server" CssClass="form-control" TextMode="Password" ValidationGroup="a"></asp:TextBox>
                        <asp:RequiredFieldValidator ID="req_txtPassword" runat="server" ValidationGroup="a" SetFocusOnError="true" ControlToValidate="txtPassword" ForeColor="Red" ErrorMessage="Please enter password"></asp:RequiredFieldValidator>
                    </div>
                </div>
                <div class="clearfix">
                    <asp:HiddenField ID="hdnUserID" runat="server" />
                    <asp:HiddenField ID="hdnDocID" runat="server" />
                    <asp:HiddenField ID="hdnCurrStatus" runat="server" />
                    <asp:HiddenField ID="hdnEID" runat="server" />
                    <asp:HiddenField ID="hdnAction" runat="server" />
                </div>
                <div class="mg20">
                </div>
                <div class="col-md-4 col-md-offset-5">
                    <asp:Button ID="btnLogin" runat="server" Text="Submit" CssClass="btn btn-primary" OnClick="btnLogin_Click" ValidationGroup="a" />
                    <asp:Button ID="btnClear" runat="server" Text="Cancel" CssClass="btn btn-default" />
                </div>
                <div class="col-md-12 col-sm-12">
                    &nbsp;
                </div>
                <div class="col-md-5 col-sm-5">
                    &nbsp;
                </div>
                <div class="col-md-7 col-sm-7">
                    <p><a id="passRedirect" href="Default.aspx">click here</a><span id="spnPass" runat="server"></span></p>
                </div>
            </div>
            <div id="loginMpin" runat="server" visible="false">


                <div class="col-md-12 col-sm-12" style="text-align: center; color: red;">
                    <asp:Label ID="lblMpinMsg" runat="server"></asp:Label>
                </div>
                <div class="col-md-12 col-sm-12">
                    <div class="col-md-3 col-sm-3">
                        &nbsp;
                    </div>
                    <div class="col-md-2">
                        PIN *
                    </div>
                    <div class="col-md-4 col-sm-4">
                        <asp:TextBox ID="txtMpin" runat="server" CssClass="form-control" TextMode="Password" ValidationGroup="b"></asp:TextBox>
                        <asp:RequiredFieldValidator ID="Req_txtMpin" runat="server" ValidationGroup="b" SetFocusOnError="true" ControlToValidate="txtMpin" ForeColor="Red" ErrorMessage="Please enter pin"></asp:RequiredFieldValidator>
                    </div>
                </div>
                <div class="mg20">
                </div>
                <div class="col-md-4 col-md-offset-5">
                    <asp:Button ID="btnMpin" runat="server" Text="Submit" CssClass="btn btn-primary" OnClick="btnMpin_Click" ValidationGroup="b" />
                    <asp:Button ID="btnMpinCancel" runat="server" Text="Cancel" CssClass="btn btn-default" />
                </div>
                <div class="col-md-12 col-sm-12">
                    &nbsp;
                </div>
                <div class="col-md-5 col-sm-5">
                    &nbsp;
                </div>
                <div class="col-md-7 col-sm-7">
                    <p><a id="pinRedirect" href="Default.aspx">click here</a><span id="spnPin" runat="server"></span></p>
                </div>
            </div>

            <div id="usriptmappr" runat="server" visible="false">
                <div class="col-md-12 col-sm-12" style="text-align: center; color: red;">
                    <asp:Label ID="lblusripmapprmsg" runat="server"></asp:Label>
                </div>
                <div class="col-md-12 col-sm-12">
                    <div class="col-md-3 col-sm-3">
                        &nbsp;
                    </div>
                    <div class="col-md-2" id="dvCaption" runat="server">
                         <span style="color:red">*</span>
                    </div>
                    <div class="col-md-4 col-sm-4">
                       <%-- <asp:DropDownList ID="ddlusriptmappr" runat="server" CssClass="form-control" ValidationGroup="c"></asp:DropDownList>--%>
                        <asp:TextBox ID="txtusriptmappr" runat="server" CssClass="form-control"  ValidationGroup="c"></asp:TextBox>
                        <asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" ValidationGroup="c" SetFocusOnError="true" ControlToValidate="txtusriptmappr" ForeColor="Red" ErrorMessage="Please Enter Name"></asp:RequiredFieldValidator>
                    </div>
                </div>
                <div class="clearfix">
                     <asp:HiddenField ID="hdndoctype" runat="server" />
                </div>
                <div class="mg20">
                </div>
                <div class="col-md-4 col-md-offset-5">
                    <asp:Button ID="btnddlusriptmappr" runat="server" Text="Submit" CssClass="btn btn-primary" ValidationGroup="c" OnClick="btnddlusriptmappr_Click" />
                    <asp:Button ID="btncancelddlusriptmappr" runat="server" Text="Cancel" CssClass="btn btn-default" />
                </div>
                <div class="col-md-12 col-sm-12">
                    &nbsp;
                </div>
                <div class="col-md-5 col-sm-5">
                    &nbsp;
                </div>
                <%--  <div class="col-md-7 col-sm-7">
                    <p><a id="pinRedirect" href="Default.aspx">click here</a><span id="Span1" runat="server"></span></p>
                </div>--%>
            </div>
        </div>
    </form>
</body>
</html>
