<%@ page title="" language="VB" masterpagefile="~/USR.master" autoeventwireup="false" inherits="MapReport, App_Web_0xvfyc51" viewStateEncryptionMode="Always" %>
<%@ Register assembly="AjaxControlToolkit" namespace="AjaxControlToolkit" tagprefix="asp" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">

    <style type="text/css">
.gradientBoxesWithOuterShadows { 
height: 100%;
width: 99%; 
padding: 5px;
background-color: white; 

/* outer shadows  (note the rgba is red, green, blue, alpha) */
-webkit-box-shadow: 0px 0px 12px rgba(0, 0, 0, 0.4); 
-moz-box-shadow: 0px 1px 6px rgba(23, 69, 88, .5);

/* rounded corners */
-webkit-border-radius: 12px;
-moz-border-radius: 7px; 
border-radius: 7px;

/* gradients */
background: -webkit-gradient(linear, left top, left bottom, 
color-stop(0%, white), color-stop(15%, white), color-stop(100%, #D7E9F5)); 
background: -moz-linear-gradient(top, white 0%, white 55%, #D5E4F3 130%); 
}

    .style2
    {
        width: 30%;
    }

</style>

<br />
<div class="gradientBoxesWithOuterShadows" style="margin-top:10px">

  <table width="100%"  align="center" border="1" cellpadding="3" cellspacing="0" border="#E0E0E0" class="m9"> 
         
           <tr>
          <td>
          <asp:Label ID="errormsg" runat="server" ForeColor="Red"  Text=""></asp:Label>
          </td>
          </tr>
          <tr>
                            
                            <td  align="left" class="m8b" width="35%">
                          
                           <fieldset>
                            <legend style=" color:Black;">User-Vehicle</legend>
                <asp:Panel ID="Panel1" runat="server" Height="150px" ScrollBars="Auto" 
                      style=" display:block ">
                      
                      <asp:CheckBoxList ID="UsrVeh"  runat="server">
                      </asp:CheckBoxList>
                  </asp:Panel>
                  </fieldset>
              </td>
            
              <td align="left" class="style2">
                  <table   >
                      <tr>
                    
                          <td>
                              
                          <fieldset style="width:326px;">
                          <legend style=" color:Black;">Date Range</legend>
                          <table>
                          <tr>
                          <td>
                             <asp:TextBox ID="txtsdate" runat="server"></asp:TextBox>
                                 <asp:CalendarExtender  ID="calendersdate"   TargetControlID="txtsdate"  Format="yyyy-MM-dd"   runat="server" /> <br/>
                             <asp:TextBox ID="txtedate"  runat="server" AutoCompleteType="Search" ></asp:TextBox>
                                 <asp:CalendarExtender  ID="CalendarExtender1" TargetControlID="txtedate" Format="yyyy-MM-dd" runat="server" /> 
                                 </td>
                                 <td>

                             &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<asp:TextBox ID="TxtStime" Width="50px"  runat="server"></asp:TextBox>
                             
                             <ajaxToolkit:TextBoxWatermarkExtender ID="TBWE2" runat="server"
    TargetControlID="TxtStime"
    WatermarkText="HH:MM"
    WatermarkCssClass="watermarked" /> <br />
                                                    
                              &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<asp:TextBox ID="txtetime" Width="50px" runat="server"></asp:TextBox>
                              <ajaxToolkit:TextBoxWatermarkExtender ID="TextBoxWatermarkExtender1" runat="server"
    TargetControlID="txtetime"
    WatermarkText="HH:MM"
    WatermarkCssClass="watermarked" />
                              </td>
                              </tr>
                              </table>
                 </fieldset>
                          </td>
                      </tr>
                  </table>
              </td>
                            <td align="left" width="25%">
                           <asp:ImageButton ID="btnshow" runat="server" Height="25px" Width="50px"  ImageUrl="~/images/showbutton.png"   ToolTip="Search  " />
              </td>
             
          </tr>
     
         
         
      


 </table>
  </div>
                           <div id="map" style="width: 1000px; height: 1000px;"></div>


 </asp:Content>
