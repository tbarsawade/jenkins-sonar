<%@ page language="VB" autoeventwireup="false" inherits="DocDetailMaster, App_Web_pnyzbdje" viewStateEncryptionMode="Always" %>
<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title>Master Detail</title>
    <link href="css/style.css" rel="Stylesheet" type="text/css" />
    <link href="css/ui-lightness/jquery-ui-1.10.2.custom.css" type="text/css" rel="stylesheet" />
    <script src="Jquery/jquery-3.3.1.min.js" type="text/javascript"></script>
   <script src="Jquery/jquery-ui-v1.12.1.js" type="text/javascript"></script>
    <script type="text/javascript" src="scripts/jquery.slidePanel.min.js"></script>

     <link href="js_child/jquery-ui[1].css" rel="stylesheet" type="text/css" />


       <script type="text/javascript">
           var selTab;
           $(function () {
               var tabs = $("#tabs").tabs({ show: function () {

                   //get the selected tab index  
                   selTab = $('#tabs').tabs('option', 'selected');

               }
               });

           }); ;
           $(function () {
               $(".btnDyn").button()
           });

           function pageLoad(sender, args) {

               if (args.get_isPartialLoad()) {
                   $("#tabs").tabs({ show: function () {

                       //get the selected tab index on partial postback  
                       selTab = $('#tabs').tabs('option', 'selected');
                   }, selected: selTab
                   });

                   $(function () {
                       $(".btnDyn").button()
                   });
               }

           };  
  
     </script>
    <script type='text/javascript'>
        function pageLoad() { // this gets fired when the UpdatePanel.Update() completes
            //     ReBindMyStuff();
            //     window.alert('partial postback');
            $(function () {
                $(".btnDyn").button()
            });
            $(document).ready(function () {
                $("#tabs").tabs();
            });
            $(function () {
                $("#tab").tabs();
            });
            $(function () {
                $("#tabss").tabs();
            });

        }

        window.onunload = function () {
            window.opener.location.reload();
        };
     
</script>
    <script>
        $(function () {
            $("#tabs").tabs();
        });
	</script>
     <script>
         $(function () {
             $("#tabs").tabs();
         });
         $(function () {
             $("#tabss").tabs();
         });

         $(function () {
             $("#tab").tabs();
         });
	</script>
   


<%--    <script type='text/javascript'>
        window.onunload = function () {
            window.opener.location.reload();
        };
</script>--%>

</head>
<body>
    <form id="form1" runat="server">
     <%-- <ajaxToolkit:ToolkitScriptManager ID="tscript"    runat ="server"  ></ajaxToolkit:ToolkitScriptManager>--%>
   


 



<%--
  <asp:UpdatePanel ID="updMain" runat="server">
              <ContentTemplate>--%>

<table width="100%" cellspacing="0px" cellpadding ="0px" >
     <tr>
        <td style="width:80%">
           <div id="tabs" >
        	<ul>
        	<li><asp:Label ID="lblpending" runat="server"><a href="#tabPending">CREATION DATE</a></asp:Label></li>
        	<li><asp:Label ID="lblaction" runat="server" >	<a href="#tabMy">HISTORY</a></asp:Label></li>
          
            </ul>
        	
		   <div id="tabPending" style="min-height:300px;">
            
                <div class="box" style="text-align:center">
        
       <table cellspacing="2" cellpadding="2" width="100%" border="1px">
     <tr>
        <td colspan ="2">
        <asp:Label ID="lblDetail" runat="server" Text="myndsaas"></asp:Label>
        </td>
     </tr>
</table> 

 </div>
           </div>
           
           <div id="tabMy" style="min-height:300px;" >
        <%--   <asp:UpdatePanel ID="updPNLMyUpload" runat="server" style="overflow:auto">
            <ContentTemplate> --%>
      <br />
                          <asp:Panel ID="pnlhis" runat="server" Width="1200px" ScrollBars="both" >
          <span style="color:Black">HISTORY</span>
       <asp:GridView ID="gvHistory" runat="server" AutoGenerateColumns="true" DataKeyNames="tid" Width="100%"  PageSize="20">
                    <HeaderStyle  CssClass="GridHeader" />
                    <Columns>
                    <asp:TemplateField HeaderText="S.No" >
                    <ItemTemplate>    
                    <%# CType(Container, GridViewRow).RowIndex + 1%>
                    </ItemTemplate>
                   <ItemStyle Width="20px" />
                   </asp:TemplateField>

                    </Columns>
                    <SortedAscendingCellStyle BackColor="#EDF6F6" />
                    <SortedAscendingHeaderStyle BackColor="#0D4AC4" />
                    <SortedDescendingCellStyle BackColor="#D6DFDF" />
                    <SortedDescendingHeaderStyle BackColor="#002876" />
                </asp:GridView>

                      </asp:Panel>
<%--            </ContentTemplate></asp:UpdatePanel>--%>
           </div>
         

           </div>
        
     </tr>

     </table>

     <%--
              </ContentTemplate></asp:UpdatePanel>
--%>

</form>
</body>
</html>
