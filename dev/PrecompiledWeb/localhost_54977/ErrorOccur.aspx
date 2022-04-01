<%@ page title="" language="VB" masterpagefile="~/usrFullScreenBPM.master" autoeventwireup="false" inherits="ErrorOccur, App_Web_gn32bfei" viewStateEncryptionMode="Always" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
    <style type="text/css">
        .error-bg {
            background: url(images/error-bg_banner.png) no-repeat scroll 0 0px / cover;
            height: 510px;
        }
        .err-bgcontent {
            color: #333;
            font-family: sans-serif;
            font-size: 16px;
            margin: 290px 65px 0px;
            text-align: left;
            line-height: 30px;
            font-weight: bold;
        }

        #footer {
            
        }

       
    </style>

   
    <div class="form error-bg">
        <div class="container">
            <div class="col-md-2 col-sm-2"></div>
            <div class="col-md-8 col-sm-8">
                <div class="err-bgcontent">
                    We are sorry ! An unexpected error occurred in your session.
			        <br />
                    <br />
                    We request you to re-login and try again.
                </div>
            </div>
            <div class="col-md-2 col-sm-2"> </div>
        </div>
    </div>
</asp:Content>

