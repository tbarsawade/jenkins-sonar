Imports System.Data
Imports System.Data.SqlClient
Imports System.Globalization

Partial Class MapReport
    Inherits System.Web.UI.Page
    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Not IsPostBack Then

            Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
            Dim con As SqlConnection = New SqlConnection(conStr)
            Dim Udtype As String
            Dim Ufld As String
            Dim UVfld As String
            Dim Vdtype As String
            Dim Vfld As String
            Dim vemei As String
            Dim oda As SqlDataAdapter = New SqlDataAdapter("", con)
            oda.SelectCommand.CommandText = "select* from mmm_mst_entity where eid=" & Session("EID") & " "
            Dim ds As New DataSet()
            oda.Fill(ds, "data")
            If ds.Tables("data").Rows.Count > 0 Then
                Udtype = ds.Tables("data").Rows(0).Item("uvdtype").ToString
                Ufld = ds.Tables("data").Rows(0).Item("uvuserfield").ToString
                UVfld = ds.Tables("data").Rows(0).Item("uvvehiclefield").ToString
                Vdtype = ds.Tables("data").Rows(0).Item("VIDType").ToString
                Vfld = ds.Tables("data").Rows(0).Item("vivehiclefield").ToString
                vemei = ds.Tables("data").Rows(0).Item("viimeifield").ToString
            End If
            oda.SelectCommand.CommandText = "select imieno,username,VehicleNo from ( select  distinct m." & vemei & "[IMIENO], (select username from mmm_mst_user where eid=" & Session("EID") & " and uid=d." & Ufld & ")[UserName],m." & Vfld & "[VehicleNo] from mmm_mst_Doc d inner join mmm_mst_master m on m.tid=d." & UVfld & " inner join mmm_mst_gpsdata g on g.imieno=m." & vemei & " where d.eid=" & Session("EID") & " and  d.documenttype='" & Udtype & "'  and imieno is not null )as table1 order by username"
            ds.Clear()
            oda.Fill(ds, "vemei")
            UsrVeh.Items.Clear()
            For i As Integer = 0 To ds.Tables("vemei").Rows.Count - 1
                UsrVeh.Items.Add(ds.Tables("vemei").Rows(i).Item(1).ToString() & "-" & ds.Tables("vemei").Rows(i).Item(2).ToString())
                UsrVeh.Items(i).Value = ds.Tables("vemei").Rows(i).Item(0).ToString()
            Next
            'UsrVeh.Items.Add(ds.Tables("vemeino").Rows()
        End If


    End Sub

    'Add Theme Code
    Protected Sub Page_PreInit1(ByVal sender As Object, ByVal e As EventArgs) Handles Me.PreInit
        Try
            If Not Session("CTheme") Is Nothing And Not Session("CTheme") = String.Empty Then
                Page.Theme = Convert.ToString(Session("CTheme"))
            Else
                Page.Theme = "Default"
            End If
        Catch ex As Exception
        End Try

    End Sub
    Protected Sub btnshow_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnshow.Click

    End Sub


    Protected Sub btnshow_Click(sender As Object, e As System.Web.UI.ImageClickEventArgs) Handles btnshow.Click

        Dim parsed As DateTime
        Dim valid As Boolean = DateTime.TryParseExact(txtsdate.Text + " " + TxtStime.Text, "yyyy-MM-dd HH:mm", CultureInfo.InvariantCulture, DateTimeStyles.None, parsed)
        If valid = False Then
            errormsg.Text = "Enter the start date & time range as required Format"

            Exit Sub
        End If
        Dim validdate2 As Boolean = DateTime.TryParseExact(txtedate.Text + " " + txtetime.Text, "yyyy-MM-dd HH:mm", CultureInfo.InvariantCulture, DateTimeStyles.None, parsed)
        If validdate2 = False Then
            errormsg.Text = "Enter the last date & time range as required Format"

            Exit Sub
        End If
        Dim crrdate As String = Date.Now.ToString("yyyy-MM-dd HH:mm")
        If DateTime.Parse(txtsdate.Text) > DateTime.Parse(crrdate) Then
            errormsg.Text = "Future start range date/time is not allowed"
            Exit Sub
        ElseIf DateTime.Parse(txtedate.Text) > DateTime.Parse(crrdate) Then
            errormsg.Text = "Future end range date/time is not allowed"
            Exit Sub

        Else
            If CDate(txtsdate.Text) > CDate(txtedate.Text) Then
                errormsg.Text = "Start Date should be less than from End Date "
                Exit Sub
            End If
            If CDate(txtsdate.Text) = CDate(txtedate.Text) Then
                If TxtStime.Text > txtetime.Text Then
                    errormsg.Text = "Start Time should be less than from End Time "
                    Exit Sub
                End If

            End If


            Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
            Dim con As SqlConnection = New SqlConnection(conStr)

            Dim oda As SqlDataAdapter = New SqlDataAdapter("", con)
            Dim ds As New DataSet()
            Dim CMD As String = String.Empty

            Dim j As Integer = 0



            For i As Integer = 0 To UsrVeh.Items.Count - 1

                If UsrVeh.Items(i).Selected Then

                    CMD += "'" + UsrVeh.Items(i).Value + "'" + ","


                End If
            Next


            If CMD.Length = 0 Then
                errormsg.Text = "check one user "
                Exit Sub
            End If
            CMD = CMD.Remove(CMD.Length - 1)
            oda.SelectCommand.CommandText = "select* from mmm_mst_gpsdata where tid in (select max(tid) from MMM_MST_GPSDATA  where IMIENO in (" & CMD & ") and cTime >= '" + txtsdate.Text + " " + TxtStime.Text + "' AND ctime <= '" + txtedate.Text + " " + txtetime.Text + "' group by lattitude,longitude) order by IMIENO,ctime "
            oda.Fill(ds, "MMM_MST_GPSDATA")
            Dim JLineCoOrdinate As String = String.Empty

            Dim apikey As String = String.Empty
            Dim imei As String() = CMD.Split(",")
    

            Dim javaS As String = String.Empty
            Dim imeiincrement As Integer = 0
            ' Dim colors As  String()=((),(),(),())
            If ds.Tables("MMM_MST_GPSDATA").Rows.Count = 0 Then
                errormsg.Text = "we have no data for map "
                Exit Sub
            Else
                errormsg.Text = ""
            End If

            For i As Integer = 0 To ds.Tables("MMM_MST_GPSDATA").Rows.Count - 1
                JLineCoOrdinate = JLineCoOrdinate & "['" + "IMEI NO " + ds.Tables("MMM_MST_GPSDATA").Rows(i).Item("imieno").ToString() + "<br>Speed " & ds.Tables("MMM_MST_GPSDATA").Rows(i).Item("speed").ToString() & " Km / Hr <br>" + ds.Tables("MMM_MST_GPSDATA").Rows(i).Item("ctime").ToString() + " '," & ds.Tables("MMM_MST_GPSDATA").Rows(i).Item("lattitude").ToString() & "," & ds.Tables("MMM_MST_GPSDATA").Rows(i).Item("longitude").ToString() & ",],"
             

                If i < ds.Tables("MMM_MST_GPSDATA").Rows.Count - 1 Then

                    If ds.Tables("MMM_MST_GPSDATA").Rows(i).Item("imieno") <> ds.Tables("MMM_MST_GPSDATA").Rows(i + 1).Item("imieno").ToString Then
                        javaS = javaS + " var locations" + imeiincrement.ToString + " = [" + JLineCoOrdinate + "];"
                        javaS += " var lineCoordinates" & imeiincrement.ToString & " = [];  for (i = 0; i < locations" & imeiincrement.ToString & ".length; i++)  { lineCoordinates" + imeiincrement.ToString + ".push(new google.maps.LatLng(locations" & imeiincrement.ToString & "[i][1], locations" & imeiincrement.ToString & "[i][2])); } var FrPath" & imeiincrement.ToString & " = new google.maps.Polyline({    path: lineCoordinates" & imeiincrement.ToString & ", strokeColor: 'black'     }); FrPath" & imeiincrement.ToString & ".setMap(map);   for (i = 0; i < locations" & imeiincrement.ToString & ".length; i++) { marker = new google.maps.Marker({ position: new google.maps.LatLng(locations" & imeiincrement.ToString & "[i][1], locations" & imeiincrement.ToString & "[i][2]), map: map, icon: 'images/car.png', size: new google.maps.Size(1, 1) });    google.maps.event.addListener(marker, 'click', (function (marker, i) {  return function () {  infowindow.setContent(locations" & imeiincrement.ToString & "[i][0]);  infowindow.open(map, marker);     }  })  (marker, i));  }"

                    
                        imeiincrement = imeiincrement + 1
                        JLineCoOrdinate = String.Empty
                    End If

                Else
                    javaS += javaS + " var locations" + imeiincrement.ToString + " = [" + JLineCoOrdinate + "];"

                    javaS += " var lineCoordinates" & imeiincrement.ToString & " = [];  for (i = 0; i < locations" & imeiincrement.ToString & ".length; i++)  { lineCoordinates" + imeiincrement.ToString + ".push(new google.maps.LatLng(locations" & imeiincrement.ToString & "[i][1], locations" & imeiincrement.ToString & "[i][2])); } var FrPath" & imeiincrement.ToString & " = new google.maps.Polyline({    path: lineCoordinates" & imeiincrement.ToString & ", strokeColor: 'black'     }); FrPath" & imeiincrement.ToString & ".setMap(map);   for (i = 0; i < locations" & imeiincrement.ToString & ".length; i++) { marker = new google.maps.Marker({ position: new google.maps.LatLng(locations" & imeiincrement.ToString & "[i][1], locations" & imeiincrement.ToString & "[i][2]), map: map, icon: 'images/car.png', size: new google.maps.Size(1, 1) });    google.maps.event.addListener(marker, 'click', (function (marker, i) {  return function () {  infowindow.setContent(locations" & imeiincrement.ToString & "[i][0]);  infowindow.open(map, marker);     }  })  (marker, i));  }"

                

                    imeiincrement = imeiincrement + 1
                    JLineCoOrdinate = String.Empty
                End If
            Next


            Dim url As String
            Dim jqueryInclude As HtmlGenericControl = New HtmlGenericControl("script")
            oda.SelectCommand.CommandText = "select * from mmm_mst_entity where eid=" & Session("EID").ToString() & " "
            oda.Fill(ds, "mmm_mst_entity")
            If ds.Tables("mmm_mst_entity").Rows.Count > 0 Then

                apikey = ds.Tables("mmm_mst_entity").Rows(0).Item("APIkey").ToString

            End If
            If apikey = "" Then
                jqueryInclude.Attributes.Add("type", "text/javascript")
                jqueryInclude.Attributes.Add("src", "http://maps.google.com/maps/api/js?sensor=false")
                url = ""
                Page.Header.Controls.Add(jqueryInclude)
            Else

                jqueryInclude.Attributes.Add("type", "text/javascript")
                jqueryInclude.Attributes.Add("src", "http://www.google.com/jsapi?key=" + apikey + "")
                Page.Header.Controls.Add(jqueryInclude)
                url = "<script type='text/javascript'>google.load('maps', '3.7', { 'other_params': 'sensor=true' }); </script>"
            End If

            'to get central location divide it by two
            Dim cLocation As Integer = ds.Tables("MMM_MST_GPSDATA").Rows.Count / 2


            Dim JQMAP As String = " " + url + " <script>     function initialize() {  var map = new google.maps.Map(document.getElementById('map'), { zoom: 12,center: new google.maps.LatLng(" & ds.Tables("MMM_MST_GPSDATA").Rows(cLocation).Item("lattitude").ToString() & "," & ds.Tables("MMM_MST_GPSDATA").Rows(cLocation).Item("longitude").ToString() & "),mapTypeId: google.maps.MapTypeId.ROADMAP  }); var infowindow = new google.maps.InfoWindow();  " + javaS + " } google.maps.event.addDomListener(window, 'load', initialize);</script>"

            Page.ClientScript.RegisterStartupScript(Me.GetType(), "", JQMAP)



            oda.Dispose()
            ds.Dispose()
            con.Dispose()
        End If
    End Sub
End Class
