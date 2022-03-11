Imports System.Data
Imports System.Data.SqlClient
Imports System.Web.Services
Imports System.Drawing


Partial Class GmapWindow
    Inherits System.Web.UI.Page
    Dim WindowFlag As Integer

    Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
    Dim con As New SqlConnection(conStr)
    Dim Udtype As String = String.Empty
    Dim Ufld As String = String.Empty
    Dim UVfld As String = String.Empty
    Dim Vdtype As String = String.Empty
    Dim Vfld As String = String.Empty
    Dim vemei As String = String.Empty
    Dim apikey As String = String.Empty
    Dim sDate As String = ""
    Dim eDate As String = ""
   
    Protected Sub Page_Load(sender As Object, e As System.EventArgs) Handles Me.Load
        WindowFlag = Convert.ToInt32(Request.QueryString("flag"))
        ShowMap()
    End Sub

    Private Sub ShowMap()
        If WindowFlag = 0 Then
            ShowMap0()
        ElseIf WindowFlag = 1 Then
            ShowMap1()
        ElseIf WindowFlag = 2 Then
            ShowMap2()
        ElseIf WindowFlag = 3 Then
            ShowMap3()
        ElseIf WindowFlag = 4 Then
            ShowMapDynamic()
        End If
    End Sub
    ''' <summary>
    ''' ShowMapIndus
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub ShowMap0()
        Try
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
            Dim tid As String = Request.QueryString("tid").ToString()
            Dim IMIENO As String = String.Empty
            Dim vehicle As String = String.Empty
            Dim uid As String = String.Empty

            If Request.QueryString("type").ToString().ToUpper <> "MANUAL" Then
                Tripmap.Visible = True

                oda.SelectCommand.CommandText = "select * from mmm_mst_gpsdata where tid=" & tid & " "
                oda.Fill(ds, "gpsdata")
                If ds.Tables("gpsdata").Rows.Count > 0 Then
                    IMIENO = ds.Tables("gpsdata").Rows(0).Item("IMIENO").ToString
                    sDate = DateTime.Parse(ds.Tables("gpsdata").Rows(0).Item("ctime").ToString).ToString("yyyy-MM-dd ")
                    eDate = DateTime.Parse(ds.Tables("gpsdata").Rows(0).Item("ctime").ToString()).ToString("yyyy-MM-dd ")
                End If
                
                Dim r As String = "select lattitude,longitude,ctime,speed,recordTime from MMM_MST_GPSDATA where tid in (select max(tid) from MMM_MST_GPSDATA where IMIENO='" & IMIENO & "'  and   cTime >= '" + sDate & "00:00' AND ctime <= '" & eDate & "23:59'  group by lattitude,longitude) order by ctime "
                oda.SelectCommand.CommandText = r
                oda.Fill(ds, "table1")

                oda.SelectCommand.CommandText = "select max(speed) as speed from MMM_MST_GPSDATA where tid in (select max(tid) from MMM_MST_GPSDATA where IMIENO='" & IMIENO & "'  and   cTime >= '" + sDate & " 00:00 '  AND ctime <= '" & eDate & "23:59'  group by lattitude,longitude)  "
                oda.Fill(ds, "speedtable")
                lblMaxSpeed.Text = ds.Tables("speedtable").Rows(0).Item("speed").ToString

                Dim centerUser As String = String.Empty
                Dim JLineCoOrdinate As String = ""

                If ds.Tables("table1").Rows.Count = 0 Then
                    msg.Text = "I have no data for this trip"
                    Exit Sub
                End If

                Dim drPrev As DataRow
                Dim c = 0
                For Each dr As DataRow In ds.Tables("table1").Rows
                    Dim icon As String
                    If c = 0 Then
                        icon = "images/start1.png"
                    ElseIf c = ds.Tables("table1").Rows.Count - 1 Then
                        icon = "images/end1.png"
                    Else
                        If Convert.ToDouble(dr("speed")) <= 0 Then
                            icon = "images/reddot.png"
                        Else
                            icon = "images/Greendot.png"
                        End If

                    End If

                    JLineCoOrdinate = JLineCoOrdinate & "['Speed " & dr("speed").ToString() & " Km / Hr <br>" + dr("cTime").ToString() + " '," & dr("lattitude").ToString() & "," & dr("longitude").ToString() & ", '" & icon & "'],"
                    drPrev = dr
                    c += 1
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
                Dim clat = ds.Tables("table1").Rows(cLocation).Item("lattitude").ToString()
                Dim clong = ds.Tables("table1").Rows(cLocation).Item("longitude").ToString()

                Dim JQMAP As String = " " + url + " <script> var eid=" & Session("EID") & ";  var locations=[ " + JLineCoOrdinate + " ]; ; var map;  var lineCoordinates = []; var clat=" & clat & ";  var clong=" & clong & "; Markers.initialize(); </script>"
                ' Dim JQMAP As String = " " + url + " <script>     function initialize() { var locations = [ " + JLineCoOrdinate + " ]; var map = new google.maps.Map(document.getElementById('map'), { zoom: 12,center: new google.maps.LatLng(" & ds.Tables("table1").Rows(cLocation).Item("lattitude").ToString() & "," & ds.Tables("table1").Rows(cLocation).Item("longitude").ToString() & "),mapTypeId: google.maps.MapTypeId.ROADMAP  }); var infowindow = new google.maps.InfoWindow();var lineCoordinates = []; for (i = 0; i < locations.length; i++) { lineCoordinates.push(new google.maps.LatLng(locations[i][1], locations[i][2])); }    var FrPath = new google.maps.Polyline({  path: lineCoordinates,strokeColor: 'black'  });FrPath.setMap(map);for (i = 0; i < locations.length; i++) { marker = new google.maps.Marker({ position: new google.maps.LatLng(locations[i][1], locations[i][2 ]),map: map,icon:  'images/Greendot.png', size: new google.maps.Size(1 , 1)});google.maps.event.addListener(marker, 'click', (function (marker, i) { return function () { infowindow.setContent(locations[i][0]);infowindow.open(map, marker);}})(marker, i));}} google.maps.event.addDomListener(window, 'load', initialize);</script>"
                Page.ClientScript.RegisterStartupScript(Me.GetType(), "", JQMAP)
                ' End If
            Else
                nomap.Visible = True
            End If

        Catch ex As Exception

        End Try
    End Sub
    ''' <summary>
    ''' ShowMap
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub ShowMap1()
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

                oda.SelectCommand.CommandText = "select * from mmm_mst_elogbook where tid=" & tid & " "
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
                        oda.SelectCommand.CommandText = "select convert(int,round(isnull(sum(devdist),0),0)) as DevDist,max(speed) speed from MMM_MST_GPSDATA where  IMIENO='" & IMIENO & "'  and   cTime >= '" + sDate + "' AND ctime <= '" + eDate + "' and ((devdist>0.03 and speed=0) or (devdist>0 and speed>0)) "
                    End If
                    If con.State <> ConnectionState.Open Then
                        con.Open()
                    End If
                    Total_Distance = oda.SelectCommand.ExecuteScalar()
                    uid = ds.Tables("elogbook").Rows(0).Item("uid").ToString
                End If
                lblVehicleNumber.Text = vehicle
                lblStartLocation.Text = Trip_Start_Location
                lblEndLocation.Text = Trip_End_Location
                lblstartDt.Text = sDate
                lblendDt.Text = eDate
                lblTotalDist.Text = Total_Distance

                oda.SelectCommand.CommandText = "select * from mmm_mst_user where uid=" & uid & " "
                oda.Fill(ds, "Username")
                If Session("EID") = 32 Then
                    'User.Text = ds.Tables("Username").Rows(0).Item("UserName")
                End If

                Dim r As String = "select lattitude,longitude,ctime,speed,recordTime from MMM_MST_GPSDATA where tid in (select max(tid) from MMM_MST_GPSDATA where IMIENO='" & IMIENO & "'  and   cTime >= '" + sDate & "' AND ctime <= '" & eDate & "' group by lattitude,longitude) order by ctime "
                oda.SelectCommand.CommandText = r
                oda.Fill(ds, "table1")

                oda.SelectCommand.CommandText = "select max(speed) as speed from MMM_MST_GPSDATA where tid in (select max(tid) from MMM_MST_GPSDATA where IMIENO='" & IMIENO & "'  and   cTime >= '" + sDate & "' AND ctime <= '" & eDate & "' group by lattitude,longitude)  "
                oda.Fill(ds, "speedtable")
                lblMaxSpeed.Text = ds.Tables("speedtable").Rows(0).Item("speed").ToString


                Dim centerUser As String = String.Empty
                Dim JLineCoOrdinate As String = ""

                If ds.Tables("table1").Rows.Count = 0 Then
                    msg.Text = "I have no data for this trip"
                    Exit Sub
                End If
                Dim c As Integer = 0
                For Each dr As DataRow In ds.Tables("table1").Rows
                    Dim icon As String
                    If c = 0 Then
                        icon = "images/start1.png"
                    ElseIf c = ds.Tables("table1").Rows.Count - 1 Then
                        icon = "images/end1.png"
                    Else
                        If Convert.ToDouble(dr("speed")) <= 0 Then
                            icon = "images/reddot.png"
                        Else
                            icon = "images/Greendot.png"
                        End If

                    End If
                    JLineCoOrdinate = JLineCoOrdinate & "['Speed " & dr("speed").ToString() & " Km / Hr <br>" + dr("cTime").ToString() + " '," & dr("lattitude").ToString() & "," & dr("longitude").ToString() & ",'" & icon & "' ],"
                    c = c + 1

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
                Dim clat = ds.Tables("table1").Rows(cLocation).Item("lattitude").ToString()
                Dim clong = ds.Tables("table1").Rows(cLocation).Item("longitude").ToString()
                Dim JQMAP As String = " " + url + " <script> var eid=" & Session("EID") & ";  var locations=[ " + JLineCoOrdinate + " ]; ; var map;  var lineCoordinates = []; var clat=" & clat & ";  var clong=" & clong & "; Markers.initialize(); </script>"

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
    ''' <summary>
    ''' ShowMapIndusNew
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub ShowMap2()
        Dim vehicleno As String = Request.QueryString("Vehicle No").ToString()
        Dim starttime As String = Request.QueryString("Start Date").ToString()
        Dim endtime As String = Request.QueryString("End Date").ToString()
        lblVehicleNumber.Text = vehicleno
        lblstartDt.Text = starttime
        lblendDt.Text = endtime
        trStartEndLoc.Visible = False
        Dim oda As SqlDataAdapter = New SqlDataAdapter("", con)
        Dim ds As New DataSet()
        Dim IMIENO As String = String.Empty
        Dim IMIENOlog As String = String.Empty
        oda.SelectCommand.CommandText = "select fld12[imei] from mmm_mst_master where fld1='" & vehicleno & "' and eid=" & Session("EID").ToString() & " and documenttype='vehicle' "
        oda.Fill(ds, "vehimei")
        IMIENO = ds.Tables("vehimei").Rows(0).Item("imei").ToString
        oda.SelectCommand.CommandText = "select * from mmm_mst_entity where eid=" & Session("EID").ToString() & " "
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
        Dim vehicle As String = String.Empty
        sDate = starttime
        eDate = endtime

        Dim uid As String = String.Empty
        oda.SelectCommand.CommandText = "select triptype from mmm_mst_elogbook where vehicle_no='" & vehicleno & "' and Trip_Start_DateTime>='" & starttime & "' and Trip_End_DateTime<='" & endtime & "' and eid=" & Session("EID").ToString() & " "
        oda.Fill(ds, "ttype")
        Dim type As String = ds.Tables("ttype").Rows(0).Item("triptype").ToString
        If type.ToString().ToUpper <> "MANUAL" Then
            Tripmap.Visible = True
            oda.SelectCommand.CommandText = "select * from mmm_mst_elogbook where vehicle_no='" & vehicleno & "' and Trip_Start_DateTime>='" & starttime & "' and Trip_End_DateTime<='" & endtime & "' and eid=" & Session("EID").ToString() & " "
            oda.Fill(ds, "gpsdata")
            If ds.Tables("gpsdata").Rows.Count > 0 Then
                IMIENO = ds.Tables("gpsdata").Rows(0).Item("IMEI_NO").ToString

                sDate = DateTime.Parse(ds.Tables("gpsdata").Rows(0).Item("trip_start_datetime").ToString).ToString("yyyy-MM-dd ")

                eDate = DateTime.Parse(ds.Tables("gpsdata").Rows(0).Item("trip_end_datetime").ToString()).ToString("yyyy-MM-dd ")
            End If

            Dim r As String = "select lattitude,longitude,ctime,speed,recordTime from MMM_MST_GPSDATA where tid in (select max(tid) from MMM_MST_GPSDATA where IMIENO='" & IMIENO & "'  and   cTime >= '" & starttime & "' AND ctime <= '" & endtime & "'  group by lattitude,longitude) order by ctime "
            oda.SelectCommand.CommandText = r
            oda.Fill(ds, "table1")

            ' oda.SelectCommand.CommandText = "select max(speed) as speed,round(convert(numeric(10,0),sum(devdist)),0) as Dist from MMM_MST_GPSDATA where tid in (select max(tid) from MMM_MST_GPSDATA where IMIENO='" & IMIENO & "'  and   cTime >= '" & starttime & "' AND ctime <= '" & endtime & "'  group by lattitude,longitude)  "
            oda.SelectCommand.CommandText = "select convert(int,round(isnull(sum(devdist),0),0)) as Dist,max(speed) speed from MMM_MST_GPSDATA where  IMIENO='" & IMIENO & "'  and   cTime >= '" + sDate + "' AND ctime <= '" + eDate + "' and ((devdist>0.03 and speed=0) or (devdist>0 and speed>0))"
            oda.Fill(ds, "speedtable")
            lblMaxSpeed.Text = ds.Tables("speedtable").Rows(0).Item("speed").ToString
            lblTotalDist.Text = ds.Tables("speedtable").Rows(0).Item("Dist").ToString
            Dim centerUser As String = String.Empty
            Dim JLineCoOrdinate As String = ""

            If ds.Tables("table1").Rows.Count = 0 Then
                msg.Text = "There is no data for this trip"
                Exit Sub
            End If

            Dim drPrev As DataRow
            Dim c = 0
            For Each dr As DataRow In ds.Tables("table1").Rows
                Dim icon As String
                If c = 0 Then
                    icon = "images/start1.png"
                ElseIf c = ds.Tables("table1").Rows.Count - 1 Then
                    icon = "images/end1.png"
                Else

                    If Convert.ToInt32(dr("speed")) = 0 Then
                        icon = "images/reddot.png"
                    Else
                        icon = "images/Greendot.png"
                    End If
                End If
                JLineCoOrdinate = JLineCoOrdinate & "['Speed " & dr("speed").ToString() & " Km / Hr <br>" + dr("cTime").ToString() + " '," & dr("lattitude").ToString() & "," & dr("longitude").ToString() & ", '" & icon & "'],"
                drPrev = dr
                c += 1
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
            Dim clat = ds.Tables("table1").Rows(cLocation).Item("lattitude").ToString()
            Dim clong = ds.Tables("table1").Rows(cLocation).Item("longitude").ToString()

            Dim JQMAP As String = " " + url + " <script> var eid=" & Session("EID") & ";  var locations=[ " + JLineCoOrdinate + " ]; ; var map;  var lineCoordinates = []; var clat=" & clat & ";  var clong=" & clong & "; Markers.initialize(); </script>"
            ' Dim JQMAP As String = " " + url + " <script>     function initialize() { var locations = [ " + JLineCoOrdinate + " ]; var map = new google.maps.Map(document.getElementById('map'), { zoom: 12,center: new google.maps.LatLng(" & ds.Tables("table1").Rows(cLocation).Item("lattitude").ToString() & "," & ds.Tables("table1").Rows(cLocation).Item("longitude").ToString() & "),mapTypeId: google.maps.MapTypeId.ROADMAP  }); var infowindow = new google.maps.InfoWindow();var lineCoordinates = []; for (i = 0; i < locations.length; i++) { lineCoordinates.push(new google.maps.LatLng(locations[i][1], locations[i][2])); }    var FrPath = new google.maps.Polyline({  path: lineCoordinates,strokeColor: 'black'  });FrPath.setMap(map);for (i = 0; i < locations.length; i++) { marker = new google.maps.Marker({ position: new google.maps.LatLng(locations[i][1], locations[i][2 ]),map: map,icon:  'images/Greendot.png', size: new google.maps.Size(1 , 1)});google.maps.event.addListener(marker, 'click', (function (marker, i) { return function () { infowindow.setContent(locations[i][0]);infowindow.open(map, marker);}})(marker, i));}} google.maps.event.addDomListener(window, 'load', initialize);</script>"
            Page.ClientScript.RegisterStartupScript(Me.GetType(), "", JQMAP)
            ' End If
        Else
            nomap.Visible = True
        End If
    End Sub
    ''' <summary>
    ''' ShowMapGPS
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub ShowMap3()
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

        Dim tid As String = Request.QueryString("tid").ToString()
        Dim IMIENO As String = String.Empty
        Dim vehicle As String = String.Empty

        Dim uid As String = String.Empty

        If Request.QueryString("type").ToString().ToUpper <> "MANUAL" Then
            Tripmap.Visible = True

            oda.SelectCommand.CommandText = "select * from mmm_mst_gpsdata where tid=" & tid & " "
            oda.Fill(ds, "gpsdata")
            If ds.Tables("gpsdata").Rows.Count > 0 Then
                IMIENO = ds.Tables("gpsdata").Rows(0).Item("IMIENO").ToString

                sDate = DateTime.Parse(ds.Tables("gpsdata").Rows(0).Item("ctime").ToString).ToString("yyyy-MM-dd ")

                eDate = DateTime.Parse(ds.Tables("gpsdata").Rows(0).Item("ctime").ToString()).ToString("yyyy-MM-dd ")
                'TripStartDateTime.Text = ds.Tables("gpsdata").Rows(0).Item("ctime").ToString
                'TripEndLocation.Text = ds.Tables("gpsdata").Rows(0).Item("ctime").ToString
            End If
            Dim stime = Request.QueryString("stime")
            Dim etime = Request.QueryString("etime")
            etime = IIf(etime.Trim = "", "23:59:59", etime & ":59")
            stime = IIf(stime.Trim = "", "00:00", stime)
            Dim r As String = "select lattitude,longitude,ctime,speed,recordTime from MMM_MST_GPSDATA where tid in (select max(tid) from MMM_MST_GPSDATA where IMIENO='" & IMIENO & "'  and   cTime >= '" + sDate & " " & stime & "' AND ctime <= '" & eDate & " " & etime & "'  group by lattitude,longitude) order by ctime "
            oda.SelectCommand.CommandText = r
            oda.Fill(ds, "table1")

            'oda.SelectCommand.CommandText = "select max(speed) as speed,round(convert(numeric(10,0),sum(devdist)),0) as Dist from MMM_MST_GPSDATA where tid in (select max(tid) from MMM_MST_GPSDATA where IMIENO='" & IMIENO & "'  and   cTime >= '" + sDate & " 00:00 '  AND ctime <= '" & eDate & "23:59'  group by lattitude,longitude)  "
            oda.SelectCommand.CommandText = "select convert(int,round(isnull(sum(devdist),0),0)) as Dist,max(speed) speed from MMM_MST_GPSDATA where  IMIENO='" & IMIENO & "'  and   cTime >= '" + sDate + " " & stime & "' AND ctime <= '" + eDate + " " & etime & "' and ((devdist>0.03 and speed=0) or (devdist>0 and speed>0))"
            oda.Fill(ds, "speedtable")
            lblMaxSpeed.Text = ds.Tables("speedtable").Rows(0).Item("speed").ToString
            lblTotalDist.Text = ds.Tables("speedtable").Rows(0).Item("Dist").ToString

            trVehicle.Visible = False
            trStartEndLoc.Visible = False
            trspeed.Visible = True
            trStartEndDt.Visible = False

            Dim centerUser As String = String.Empty
            Dim JLineCoOrdinate As String = ""

            If ds.Tables("table1").Rows.Count = 0 Then
                msg.Text = "I have no data for this trip"
                Exit Sub
            End If
            Dim c As Integer = 0
            For Each dr As DataRow In ds.Tables("table1").Rows
                Dim icon As String
                If c = 0 Then
                    icon = "images/start1.png"
                ElseIf c = ds.Tables("table1").Rows.Count - 1 Then
                    icon = "images/end1.png"
                Else
                    If Convert.ToDouble(dr("speed")) <= 0 Then
                        icon = "images/reddot.png"
                    Else
                        icon = "images/Greendot.png"
                    End If

                End If
                JLineCoOrdinate = JLineCoOrdinate & "['Speed " & dr("speed").ToString() & " Km / Hr <br>" + dr("cTime").ToString() + " '," & dr("lattitude").ToString() & "," & dr("longitude").ToString() & ",'" & icon & "'],"
                c = c + 1

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

            Dim clat = ds.Tables("table1").Rows(cLocation).Item("lattitude").ToString()
            Dim clong = ds.Tables("table1").Rows(cLocation).Item("longitude").ToString()
            Dim JQMAP As String = " " + url + " <script> var eid=" & Session("EID") & ";  var locations=[ " + JLineCoOrdinate + " ]; ; var map;  var lineCoordinates = []; var clat=" & clat & ";  var clong=" & clong & "; Markers.initialize(); </script>"
            'Dim JQMAP As String = " " + url + " <script>     function initialize() { var locations = [ " + JLineCoOrdinate + " ]; var map = new google.maps.Map(document.getElementById('map'), { zoom: 12,center: new google.maps.LatLng(" & ds.Tables("table1").Rows(cLocation).Item("lattitude").ToString() & "," & ds.Tables("table1").Rows(cLocation).Item("longitude").ToString() & "),mapTypeId: google.maps.MapTypeId.ROADMAP  }); var infowindow = new google.maps.InfoWindow();var lineCoordinates = []; for (i = 0; i < locations.length; i++) { lineCoordinates.push(new google.maps.LatLng(locations[i][1], locations[i][2])); }    var FrPath = new google.maps.Polyline({  path: lineCoordinates,strokeColor: 'black'  });FrPath.setMap(map);for (i = 0; i < locations.length; i++) { marker = new google.maps.Marker({ position: new google.maps.LatLng(locations[i][1], locations[i][2 ]),map: map,icon:  'images/Greendot.png', size: new google.maps.Size(1 , 1)});google.maps.event.addListener(marker, 'click', (function (marker, i) { return function () { infowindow.setContent(locations[i][0]);infowindow.open(map, marker);}})(marker, i));}} google.maps.event.addDomListener(window, 'load', initialize);</script>"
            Page.ClientScript.RegisterStartupScript(Me.GetType(), "", JQMAP)
            ' End If
        Else
            nomap.Visible = True
        End If
    End Sub

    Public Shared strPathJson As String = HttpContext.Current.Server.MapPath("DOCS/csvJson.txt")

    <WebMethod()> _
       <Script.Services.ScriptMethod()> _
    Public Shared Function GetMarkerListJSON() As String
        Dim strPathJson As String = HttpContext.Current.Server.MapPath("Scripts/csvJson_" & HttpContext.Current.Session("EID").ToString() & ".txt")
        Dim strList As String = ""
        Dim d As String = IO.File.ReadAllText(strPathJson)
        Return d
    End Function

    Public Shared Function RadToDeg(radians As Double) As Double
        Return radians * (180 / Math.PI)
    End Function

    Public Shared Function DegToRad(degrees As Double) As Double
        Return degrees * (Math.PI / 180)
    End Function

    Public Shared Function ConvertToBearing(deg As Double) As Double
        Return (deg + 360) Mod 360
    End Function

    Private Function GetAngle(lat1 As Double, long1 As Double, lat2 As Double, long2 As Double) As Double
        Dim degreesToRadians = Math.PI / 180
        Dim degreesPerRadian = 180.0 / Math.PI
        lat1 = GmapWindow.DegToRad(lat1)
        long1 = GmapWindow.DegToRad(long1)
        lat2 = GmapWindow.DegToRad(lat2)
        long2 = GmapWindow.DegToRad(long2)
        Dim ra = Math.PI / 180
        Dim deg = 180 / Math.PI


        Dim x = lat2 - lat1
        Dim y = long2 - long1
        Dim f = 0
        If x >= 0 And y >= 0 Then
            y = y * ra
            x = x * ra
            f = 90 - Math.Atan(y / x) * deg
        ElseIf x >= 0 And y <= 0 Then
            y = y * ra
            x = x * ra
            f = 90 + Math.Abs(Math.Atan(y / x) * deg)
        ElseIf x <= 0 And y <= 0 Then
            y = y * ra
            x = x * ra
            f = 270 - Math.Atan(y / x) * deg
        ElseIf x <= 0 And y >= 0 Then
            y = y * ra
            x = x * ra
            f = 270 + Math.Abs(Math.Atan(y / x) * deg)
        End If
        Return f
    End Function

    Private Sub ShowMapDynamic()
        Dim vehicleno As String = Request.QueryString("Vehicle No").ToString()
        Dim starttime As String = Request.QueryString("Start Date").ToString()
        Dim endtime As String = Request.QueryString("End Date").ToString()
        Dim IMEI As String = Request.QueryString("IMEI").ToString()
        lblVehicleNumber.Text = vehicleno
        lblstartDt.Text = starttime
        lblendDt.Text = endtime
        trStartEndLoc.Visible = False
        Dim oda As SqlDataAdapter = New SqlDataAdapter("", con)
        Dim ds As New DataSet()
        'Dim IMIENO As String = String.Empty
        'Dim IMIENOlog As String = String.Empty
        'oda.SelectCommand.CommandText = "select fld12[imei] from mmm_mst_master where fld1='" & vehicleno & "' and eid=" & Session("EID").ToString() & " and documenttype='vehicle' "
        'oda.Fill(ds, "vehimei")
        'IMIENO = ds.Tables("vehimei").Rows(0).Item("imei").ToString

        oda.SelectCommand.CommandText = "select * from mmm_mst_entity where eid=" & Session("EID").ToString() & " "
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
        Dim vehicle As String = String.Empty
        sDate = starttime
        eDate = endtime
        Dim uid As String = String.Empty
        oda.SelectCommand.CommandText = "select triptype from mmm_mst_elogbook where IMEI_NO='" & IMEI & "' and Trip_Start_DateTime>='" & starttime & "' and Trip_End_DateTime<='" & endtime & "' and eid=" & Session("EID").ToString() & " "
        oda.Fill(ds, "ttype")
        Dim type As String = ds.Tables("ttype").Rows(0).Item("triptype").ToString
        If type.ToString().ToUpper <> "MANUAL" Then
            Tripmap.Visible = True
            oda.SelectCommand.CommandText = "select * from mmm_mst_elogbook where IMEI_NO='" & IMEI & "' and Trip_Start_DateTime>='" & starttime & "' and Trip_End_DateTime<='" & endtime & "' and eid=" & Session("EID").ToString() & " "
            oda.Fill(ds, "gpsdata")
            If ds.Tables("gpsdata").Rows.Count > 0 Then
                ' IMIENO = ds.Tables("gpsdata").Rows(0).Item("IMEI_NO").ToString
                sDate = DateTime.Parse(ds.Tables("gpsdata").Rows(0).Item("trip_start_datetime").ToString).ToString("yyyy-MM-dd ")

                eDate = DateTime.Parse(ds.Tables("gpsdata").Rows(0).Item("trip_end_datetime").ToString()).ToString("yyyy-MM-dd ")

            End If

            Dim r As String = "select lattitude,longitude,ctime,speed,recordTime from MMM_MST_GPSDATA where tid in (select max(tid) from MMM_MST_GPSDATA where IMIENO='" & IMEI & "'  and   cTime >= '" & starttime & "' AND ctime <= '" & endtime & "'  group by lattitude,longitude) order by ctime "
            oda.SelectCommand.CommandText = r
            oda.Fill(ds, "table1")
            oda.SelectCommand.CommandText = "select max(speed) as speed,round(convert(numeric(10,0),sum(devdist)),0) as Dist from MMM_MST_GPSDATA where tid in (select max(tid) from MMM_MST_GPSDATA where IMIENO='" & IMEI & "'  and   cTime >= '" & starttime & "' AND ctime <= '" & endtime & "'  group by lattitude,longitude)  "
            oda.Fill(ds, "speedtable")
            lblMaxSpeed.Text = ds.Tables("speedtable").Rows(0).Item("speed").ToString
            lblTotalDist.Text = ds.Tables("speedtable").Rows(0).Item("Dist").ToString
            Dim centerUser As String = String.Empty
            Dim JLineCoOrdinate As String = ""

            If ds.Tables("table1").Rows.Count = 0 Then
                msg.Text = "There is no data for this trip"
                Exit Sub
            End If

            Dim drPrev As DataRow
            Dim c = 0
            For Each dr As DataRow In ds.Tables("table1").Rows
                Dim icon As String
                If c = 0 Then
                    icon = "images/start1.png"
                ElseIf c = ds.Tables("table1").Rows.Count - 1 Then
                    icon = "images/end1.png"
                Else

                    If Convert.ToInt32(dr("speed")) = 0 Then
                        icon = "images/reddot.png"
                    Else
                        icon = "images/Greendot.png"
                    End If
                End If
                JLineCoOrdinate = JLineCoOrdinate & "['Speed " & dr("speed").ToString() & " Km / Hr <br>" + dr("cTime").ToString() + " '," & dr("lattitude").ToString() & "," & dr("longitude").ToString() & ", '" & icon & "'],"
                drPrev = dr
                c += 1
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
            Dim clat = ds.Tables("table1").Rows(cLocation).Item("lattitude").ToString()
            Dim clong = ds.Tables("table1").Rows(cLocation).Item("longitude").ToString()
            Dim JQMAP As String = " " + url + " <script> var eid=" & Session("EID") & ";  var locations=[ " + JLineCoOrdinate + " ]; ; var map;  var lineCoordinates = []; var clat=" & clat & ";  var clong=" & clong & "; Markers.initialize(); </script>"
            ' Dim JQMAP As String = " " + url + " <script>     function initialize() { var locations = [ " + JLineCoOrdinate + " ]; var map = new google.maps.Map(document.getElementById('map'), { zoom: 12,center: new google.maps.LatLng(" & ds.Tables("table1").Rows(cLocation).Item("lattitude").ToString() & "," & ds.Tables("table1").Rows(cLocation).Item("longitude").ToString() & "),mapTypeId: google.maps.MapTypeId.ROADMAP  }); var infowindow = new google.maps.InfoWindow();var lineCoordinates = []; for (i = 0; i < locations.length; i++) { lineCoordinates.push(new google.maps.LatLng(locations[i][1], locations[i][2])); }    var FrPath = new google.maps.Polyline({  path: lineCoordinates,strokeColor: 'black'  });FrPath.setMap(map);for (i = 0; i < locations.length; i++) { marker = new google.maps.Marker({ position: new google.maps.LatLng(locations[i][1], locations[i][2 ]),map: map,icon:  'images/Greendot.png', size: new google.maps.Size(1 , 1)});google.maps.event.addListener(marker, 'click', (function (marker, i) { return function () { infowindow.setContent(locations[i][0]);infowindow.open(map, marker);}})(marker, i));}} google.maps.event.addDomListener(window, 'load', initialize);</script>"
            Page.ClientScript.RegisterStartupScript(Me.GetType(), "", JQMAP)
            ' End If
        Else
            nomap.Visible = True
        End If

    End Sub


End Class
