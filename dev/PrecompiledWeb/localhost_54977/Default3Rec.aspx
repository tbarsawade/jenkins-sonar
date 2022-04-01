<%@ page language="VB" autoeventwireup="false" inherits="Default3, App_Web_2pfbw2jy" viewStateEncryptionMode="Always" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="asp" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>

             <link href="http://code.google.com/apis/maps/documentation/javascript/examples/default.css" rel="stylesheet" type="text/css" /> 
    <script src="https://maps.googleapis.com/maps/api/js?v=3.exp&sensor=false"></script>
</head>
<body>
    


    <button  onclick="ReSet()" style="width:90px; height:26px;" >RESET</button>

        <form id="form1" runat="server">
    <div>


        <asp:ScriptManager ID="ScriptManager1" runat="server">
        </asp:ScriptManager>
       <asp:DropDownList ID="DropDownList1" runat="server">
        </asp:DropDownList> &nbsp;&nbsp;&nbsp;&nbsp;
        <asp:Button ID="Button1" runat="server" Text="SEE ON MAP" onclick="Button1_Click" />

  <asp:TextBoxWatermarkExtender ID="TextBoxWatermarkExtender1" runat="server"    TargetControlID="TextBox1"
    WatermarkText="HH:MM">
    </asp:TextBoxWatermarkExtender>
               <asp:TextBox ID="Date1" runat="server"></asp:TextBox>
&nbsp;<asp:TextBox ID="TextBox1" runat="server"></asp:TextBox>
<asp:CalendarExtender ID="CalendarExtender1"   TargetControlID="Date1"  Format="yyyy/M/d" runat="server">  </asp:CalendarExtender>


    To <asp:TextBox ID="Date2" runat="server"></asp:TextBox>
      <asp:TextBoxWatermarkExtender ID="TextBoxWatermarkExtender2" runat="server"    TargetControlID="TextBox2"
    WatermarkText="HH:MM">
    </asp:TextBoxWatermarkExtender>
    <asp:TextBox ID="TextBox2" runat="server"></asp:TextBox>
    <asp:CalendarExtender ID="CalendarExtender2"   TargetControlID="Date2"  Format="yyyy/M/d" runat="server"></asp:CalendarExtender>
Speed of pointer
        <select id="Select1">
            <option>100</option>
               <option>200</option>
                  <option>300</option>
                  <option>1000</option>
        </select>

               <asp:Label ID="lblSomeData" runat="server" Text=""></asp:Label>
 

  
  

    </div>
    </form>


               <div id="map-canvas"></div>


</body>
</html>
