Imports System.Data.SqlClient
Imports System.Data
Imports System.Configuration

Partial Class ShowMap
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(sender As Object, e As System.EventArgs) Handles Me.Load
        ' If Not IsPostBack Then
        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim con As New SqlConnection(conStr)
        Dim Udtype As String = String.Empty
        Dim Ufld As String = String.Empty
        Dim UVfld As String = String.Empty
        Dim Vdtype As String = String.Empty
        Dim Vfld As String = String.Empty
        Dim vemei As String = String.Empty
        Dim apikey As String = String.Empty

        'Dim IMIENO As String = Request.QueryString("IMIE").ToString()
        'Dim sDate As String = Request.QueryString("start").ToString()
        'Dim eDate As String = Request.QueryString("end").ToString()
        'Dim tid As String = Request.QueryString("tid").ToString()

        Dim oda As SqlDataAdapter = New SqlDataAdapter("", con)
        oda.SelectCommand.CommandText = "select * from mmm_mst_entity where eid=" & Session("EID").ToString() & " "
        Dim ds As New DataSet()
        oda.Fill(ds, "data")
        If ds.Tables("data").Rows.Count > 0 Then
            Udtype = ds.Tables("data").Rows(0).Item("uvdtype").ToString
            Ufld = ds.Tables("data").Rows(0).Item("uvuserfield").ToString
            UVfld = ds.Tables("data").Rows(0).Item("uvvehiclefield").ToString
            Vdtype = ds.Tables("data").Rows(0).Item("VIDType").ToString
            Vfld = ds.Tables("data").Rows(0).Item("vivehiclefield").ToString
            vemei = ds.Tables("data").Rows(0).Item("viimeifield").ToString
            apikey = ds.Tables("data").Rows(0).Item("APIkey").ToString
        End If
        Try
            Dim tid As String = Request.QueryString("tid").ToString()
            Dim IMIENO As String = String.Empty
            Dim vehicle As String = String.Empty

            Dim sDate As String = String.Empty
            Dim eDate As String = String.Empty
            Dim Trip_Start_Location As String = String.Empty
            Dim Trip_End_Location As String = String.Empty
            Dim Total_Distance As String = String.Empty
            Dim uid As String = String.Empty

            If Request.QueryString("type").ToString().ToUpper <> "MANUAL" Then
                Tripmap.Visible = True

                oda.SelectCommand.CommandText = "select * from mmm_mst_newelogbook where tid=" & tid & " "
                oda.Fill(ds, "elogbook")
                If ds.Tables("elogbook").Rows.Count > 0 Then
                    IMIENO = ds.Tables("elogbook").Rows(0).Item("IMEI_NO").ToString
                    vehicle = ds.Tables("elogbook").Rows(0).Item("vehicle_no").ToString
                    sDate = DateTime.Parse(ds.Tables("elogbook").Rows(0).Item("Trip_Start_DateTime").ToString).ToString("yyyy-MM-dd HH:mm")
                    eDate = DateTime.Parse(ds.Tables("elogbook").Rows(0).Item("Trip_end_DateTime").ToString()).ToString("yyyy-MM-dd HH:mm")
                    Trip_Start_Location = ds.Tables("elogbook").Rows(0).Item("Trip_Start_Location").ToString
                    Trip_End_Location = ds.Tables("elogbook").Rows(0).Item("Trip_End_Location").ToString
                    'Old Code
                    'Total_Distance = ds.Tables("elogbook").Rows(0).Item("Total_Distance").ToString
                    If CDate(sDate.ToString) > CDate("2014-03-31 23:59".ToString) Then
                        If IMIENO = "356307044795317" Then
                            oda.SelectCommand.CommandText = "select isnull((select sum(devdist) from mmm_mst_gpsdata where imieno=gs.imieno and devdist>0.05 and speed=0 and cTime >= '" + sDate + "' AND ctime <= '" + eDate + "'),0)+ isnull((select sum(devdist) from mmm_mst_gpsdata where imieno=gs.imieno and devdist>0.07 and speed>0 and cTime >= '" + sDate + "' AND ctime <= '" + eDate + "'),0)[devdist],max(speed) speed from MMM_MST_GPSDATA gs where  devdist>0 and  IMIENO='" & IMIENO & "'  and   cTime >= '" + sDate + "' AND ctime <= '" + eDate + "' group by imieno"
                        Else
                            oda.SelectCommand.CommandText = "select isnull((select sum(devdist) from mmm_mst_gpsdata where imieno=gs.imieno and devdist>0.03 and speed=0 and cTime >= '" + sDate + "' AND ctime <= '" + eDate + "'),0)+ isnull((select sum(devdist) from mmm_mst_gpsdata where imieno=gs.imieno and devdist>0 and speed>0 and cTime >= '" + sDate + "' AND ctime <= '" + eDate + "'),0)[devdist],max(speed) speed from MMM_MST_GPSDATA gs where  devdist>0 and  IMIENO='" & IMIENO & "'  and   cTime >= '" + sDate + "' AND ctime <= '" + eDate + "' group by imieno"
                        End If
                    Else
                        oda.SelectCommand.CommandText = "select sum(DevDist) as DevDist,max(speed) speed from MMM_MST_GPSDATA where devdist <> 0 and   IMIENO='" & IMIENO & "'  and   cTime >= '" + sDate + "' AND ctime <= '" + eDate + "' "
                    End If
                    If con.State <> ConnectionState.Open Then
                        con.Open()
                    End If
                    Total_Distance = oda.SelectCommand.ExecuteScalar()
                    uid = ds.Tables("elogbook").Rows(0).Item("uid").ToString
                End If
                VehicleNo.Text = vehicle
                TripStartLocation.Text = Trip_Start_Location
                TripEndLocation.Text = Trip_End_Location
                TripStartDateTime.Text = sDate
                TripEndDateTime.Text = eDate
                TotalDistance.Text = Total_Distance
                oda.SelectCommand.CommandText = "select * from mmm_mst_user where uid=" & uid & " "
                oda.Fill(ds, "Username")
                User.Text = ds.Tables("Username").Rows(0).Item("UserName")

                Dim r As String = "select lattitude,longitude,ctime,speed,recordTime from MMM_MST_GPSDATA where tid in (select max(tid) from MMM_MST_GPSDATA where IMIENO='" & IMIENO & "'  and   cTime >= '" + sDate & "' AND ctime <= '" & eDate & "' group by lattitude,longitude) order by ctime "
                oda.SelectCommand.CommandText = r
                oda.Fill(ds, "table1")

                oda.SelectCommand.CommandText = "select max(speed) as speed from MMM_MST_GPSDATA where tid in (select max(tid) from MMM_MST_GPSDATA where IMIENO='" & IMIENO & "'  and   cTime >= '" + sDate & "' AND ctime <= '" & eDate & "' group by lattitude,longitude)  "
                oda.Fill(ds, "speedtable")
                MaximumSpeed.Text = ds.Tables("speedtable").Rows(0).Item("speed").ToString
                Dim centerUser As String = String.Empty
                Dim JLineCoOrdinate As String = ""

                If ds.Tables("table1").Rows.Count = 0 Then
                    msg.Text = "I have no data for this trip"
                    Exit Sub
                End If

                For Each dr As DataRow In ds.Tables("table1").Rows
                    JLineCoOrdinate = JLineCoOrdinate & "['Speed " & dr("speed").ToString() & " Km / Hr <br>" + dr("cTime").ToString() + " '," & dr("lattitude").ToString() & "," & dr("longitude").ToString() & "],"
                Next

                JLineCoOrdinate = JLineCoOrdinate.Remove(JLineCoOrdinate.Length - 1)

                Dim url As String
                Dim jqueryInclude As HtmlGenericControl = New HtmlGenericControl("script")

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
                Dim cLocation As Integer = ds.Tables("table1").Rows.Count / 2


                Dim JQMAP As String = " " + url + " <script>     function initialize() { var locations = [ " + JLineCoOrdinate + " ]; var map = new google.maps.Map(document.getElementById('map'), { zoom: 12,center: new google.maps.LatLng(" & ds.Tables("table1").Rows(cLocation).Item("lattitude").ToString() & "," & ds.Tables("table1").Rows(cLocation).Item("longitude").ToString() & "),mapTypeId: google.maps.MapTypeId.ROADMAP  }); var infowindow = new google.maps.InfoWindow();var lineCoordinates = []; for (i = 0; i < locations.length; i++) { lineCoordinates.push(new google.maps.LatLng(locations[i][1], locations[i][2])); }    var FrPath = new google.maps.Polyline({  path: lineCoordinates,strokeColor: 'black'  });FrPath.setMap(map);for (i = 0; i < locations.length; i++) { marker = new google.maps.Marker({ position: new google.maps.LatLng(locations[i][1], locations[i][2 ]),map: map,icon:  'images/Greendot.png', size: new google.maps.Size(1 , 1)});google.maps.event.addListener(marker, 'click', (function (marker, i) { return function () { infowindow.setContent(locations[i][0]);infowindow.open(map, marker);}})(marker, i));}} google.maps.event.addDomListener(window, 'load', initialize);</script>"
                Page.ClientScript.RegisterStartupScript(Me.GetType(), "", JQMAP)
                ' End If
            Else
                nomap.Visible = True
            End If
        Catch ex As Exception
        Finally
            oda.Dispose()
            con.Dispose()
        End Try
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
End Class
