Imports System.Data
Imports System.Data.SqlClient
Imports System.Web.Services
Imports System.Drawing

Partial Class GpsNMapReport
    Inherits System.Web.UI.Page
    Protected Sub Page_Load(sender As Object, e As System.EventArgs) Handles Me.Load

        Session("uid") = 5897 '5906 '
        Session("username") = "vinay kumar"
        Session("userrole") = "su" '"CircleUser" '
        Session("code") = "Industowers"
        Session("userimage") = "2.jpg"
        Session("clogo") = "hfcl.png"
        Session("eid") = 54
        Session("islocal") = "true"
        Session("ipaddress") = "vinay"
        Session("macaddress") = "vinay"
        Session("intime") = Now
        Session("email") = "vinay.kumar@myndsol.com"
        Session("lid") = "25"
        Session("headerstrip") = "hfclstrip.jpg"
        Session("roles") = "su" '"CircleUser" 


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
        Dim sDate As String = ""
        Dim eDate As String = ""
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

        Dim tid As String = Request.QueryString("tid").ToString()
        'tid=244445146&type=Automatic
        Dim IMIENO As String = String.Empty
        Dim vehicle As String = String.Empty

        'Dim sDate As String = String.Empty
        'Dim eDate As String = String.Empty
        'Dim Trip_Start_Location As String = String.Empty
        'Dim Trip_End_Location As String = String.Empty
        'Dim Total_Distance As String = String.Empty
        Dim uid As String = String.Empty

        If Request.QueryString("type").ToString().ToUpper <> "MANUAL" Then
            Tripmap.Visible = True

            oda.SelectCommand.CommandText = "select * from mmm_mst_gpsdata where tid=" & tid & " "
            oda.Fill(ds, "gpsdata")
            If ds.Tables("gpsdata").Rows.Count > 0 Then
                IMIENO = ds.Tables("gpsdata").Rows(0).Item("IMIENO").ToString
                'vehicle = ds.Tables("elogbook").Rows(0).Item("vehicle_no").ToString
                'Trip_Start_Location = ds.Tables("elogbook").Rows(0).Item("Trip_Start_Location").ToString
                'Trip_End_Location = ds.Tables("elogbook").Rows(0).Item("Trip_End_Location").ToString
                'Total_Distance = ds.Tables("elogbook").Rows(0).Item("Total_Distance").ToString
                'uid = ds.Tables("elogbook").Rows(0).Item("uid").ToString
                sDate = DateTime.Parse(ds.Tables("gpsdata").Rows(0).Item("ctime").ToString).ToString("yyyy-MM-dd ")

                eDate = DateTime.Parse(ds.Tables("gpsdata").Rows(0).Item("ctime").ToString()).ToString("yyyy-MM-dd ")
            End If
            'VehicleNo.Text = vehicle
            'TripStartLocation.Text = Trip_Start_Location
            'TripEndLocation.Text = Trip_End_Location
            'TripStartDateTime.Text = sDate
            'TripEndDateTime.Text = eDate
            'TotalDistance.Text = Total_Distance
            'oda.SelectCommand.CommandText = "select * from mmm_mst_user where uid=" & uid & " "
            'oda.Fill(ds, "Username")
            'User.Text = ds.Tables("Username").Rows(0).Item("UserName")

            Dim r As String = "select lattitude,longitude,ctime,speed,recordTime from MMM_MST_GPSDATA where tid in (select max(tid) from MMM_MST_GPSDATA where IMIENO='" & IMIENO & "'  and   cTime >= '" + sDate & "00:00' AND ctime <= '" & eDate & "23:59'  group by lattitude,longitude) order by ctime "
            oda.SelectCommand.CommandText = r
            oda.Fill(ds, "table1")

            oda.SelectCommand.CommandText = "select max(speed) as speed from MMM_MST_GPSDATA where tid in (select max(tid) from MMM_MST_GPSDATA where IMIENO='" & IMIENO & "'  and   cTime >= '" + sDate & " 00:00 '  AND ctime <= '" & eDate & "23:59'  group by lattitude,longitude)  "
            oda.Fill(ds, "speedtable")
            MaximumSpeed.Text = ds.Tables("speedtable").Rows(0).Item("speed").ToString
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
                    icon = "start"
                ElseIf c = ds.Tables("table1").Rows.Count - 1 Then
                    icon = "End"
                Else
                    'Dim lat1 As Double = Convert.ToDouble(drPrev("lattitude"))
                    'Dim long1 As Double = Convert.ToDouble(drPrev("longitude"))
                    'Dim lat2 As Double = Convert.ToDouble(dr("lattitude"))
                    'Dim long2 As Double = Convert.ToDouble(dr("longitude"))

                    'Dim dir = GetAngle(lat1, long1, lat2, long2)

                    'If dir = 0 Then
                    '    icon = "images/dir0.png"
                    'ElseIf dir > 0 And dir <= 45 Then
                    '    icon = "images/dir45.png"
                    'ElseIf dir > 45 And dir <= 90 Then
                    '    icon = "images/dir90.png"
                    'ElseIf dir > 90 And dir <= 135 Then
                    '    icon = "images/dir135.png"
                    'ElseIf dir > 135 And dir <= 180 Then
                    '    icon = "images/dir180.png"
                    'ElseIf dir > 180 And dir <= 225 Then
                    '    icon = "images/dir225.png"
                    'ElseIf dir > 225 And dir <= 270 Then
                    '    icon = "images/dir270.png"
                    'ElseIf dir > 270 And dir < 360 Then
                    '    icon = "images/dir315.png"
                    'Else
                    '    icon = "images/Greendot.png"
                    'End If

                    If Convert.ToDouble(dr("speed")) <= 0 Then
                        icon = "red"
                    Else
                        icon = "middle"
                    End If


                End If

                JLineCoOrdinate = JLineCoOrdinate & "['Speed " & dr("speed").ToString() & " Km / Hr <br>" + dr("cTime").ToString() + " '," & dr("lattitude").ToString() & "," & dr("longitude").ToString() & ", '" & icon & "'],"
                drPrev = dr
                c += 1
            Next

            JLineCoOrdinate = JLineCoOrdinate.Remove(JLineCoOrdinate.Length - 1)

            Dim url As String
            Dim jqueryInclude As HtmlGenericControl = New HtmlGenericControl("script")

            'If apikey = "" Then
            '    jqueryInclude.Attributes.Add("type", "text/javascript")
            '    jqueryInclude.Attributes.Add("src", "http://maps.google.com/maps/api/js?sensor=false")
            '    url = ""
            '    Page.Header.Controls.Add(jqueryInclude)
            'Else
            '    jqueryInclude.Attributes.Add("type", "text/javascript")
            '    jqueryInclude.Attributes.Add("src", "http://www.google.com/jsapi?key=" + apikey + "")
            '    Page.Header.Controls.Add(jqueryInclude)
            '    url = "<script type='text/javascript'>google.load('maps', '3.7', { 'other_params': 'sensor=true' }); </script>"
            'End If

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
    Public Shared strPathJson As String = HttpContext.Current.Server.MapPath("DOCS/CsvJson.txt")

    <WebMethod()> _
    <Script.Services.ScriptMethod()> _
    Public Shared Function GetMarkerListJSON() As String
        Dim d As String = IO.File.ReadAllText(strPathJson)
        Return d
    End Function


    <WebMethod()> _
 <Script.Services.ScriptMethod()> _
    Public Shared Function GetMarkerInfo(Id As String, IsVehical As String, Ids As String) As String
        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim con As New SqlConnection(conStr)
        Dim Info As String

        If Not IsVehical = "0" Then
            Dim query As String = "select m.tid[TID],m.fld10[SiteID],m.fld11[SiteName],"
            query &= "m.fld13[Address],m1.fld1[Site],ltrim(rtrim(left(replace(m.fld21,',','        '),9)))[Lat] ,rtrim(ltrim(right(replace(m.fld21,',','        '),9)))[Long], m.fld12[Group],m.fld11[Site Name],u.UserName[OandM Head],u1.UserName[Maintenance Head], u2.UserName[Opex Manager],u3.UserName[Security Manager],u4.UserName[Zonal Head],u5.UserName[Cluster Manager],m2.fld1[Supervisor], m3.fld1[Technician], m.fld2[No of OPCOs],m.fld19[Anchor OPCO],m.fld15[Cluster] from  mmm_mst_master m with (nolock) inner join mmm_mst_user u with (nolock) on convert(nvarchar,u.uid)=m.fld23 join mmm_mst_user u1 with (nolock) on convert(nvarchar,u1.uid)=m.fld24 join mmm_mst_user u2 with (nolock) on convert(nvarchar,u2.uid)=m.fld25  join mmm_mst_user u3 with (nolock) on convert(nvarchar,u3.uid)=m.fld26  join mmm_mst_user u4 with (nolock) on convert(nvarchar,u4.uid)=m.fld27 join mmm_mst_user u5 with (nolock) on convert(nvarchar,u5.uid)=m.fld28 join mmm_mst_master m2 with (nolock) on convert(nvarchar,m2.tid)=m.fld29 join mmm_mst_master m3 with (nolock) on convert(nvarchar,m3.tid)=m.fld3 join mmm_mst_master m1 with (nolock) on convert(nvarchar,m1.tid)=m.fld12  where m.documenttype='Site' and m1.documenttype='Site Type' and m1.eid=54 and m.eid=54 and  u.eid=54 and "
            query &= " u1.eid=54 and u2.eid=54 and m2.eid=54 and m3.eid=54 and m.Tid=" & Id
            Dim oda As SqlDataAdapter = New SqlDataAdapter(query, con)
            Dim dt As New DataTable
            oda.Fill(dt)
            Info = "<span style='font-weight:bold;'> SiteID : " + dt.Rows(0).Item("SiteID") + "</span><br>Site : " + dt.Rows(0).Item("Site") + "<br>Site Name : " + dt.Rows(0).Item("SiteName") + "<br>OandM Head: " + dt.Rows(0).Item("OandM Head") + " <br> Maintenance Head : " + dt.Rows(0).Item("Maintenance Head") + " <br>Opex Manager : " + dt.Rows(0).Item("Opex Manager") + "<br>Security Manager : " + dt.Rows(0).Item("Security Manager") + "<br>Zonal Head : " + dt.Rows(0).Item("Zonal Head") + "<br>Cluster Manager :" + dt.Rows(0).Item("Cluster Manager") + "<br>Supervisor : " + dt.Rows(0).Item("Supervisor") + "<br>Technician : " + dt.Rows(0).Item("Technician") + "<br>No of OPCOs : " + dt.Rows(0).Item("No of OPCOs") + " <br>Anchor OPCO : " + dt.Rows(0).Item("Anchor OPCO") + ""
            Return Info
        Else
            Dim query As String = "select convert(nvarchar,g.tid)[TID],max(m2.fld10)[Site_Name],DMS.IdealTime(g.imieno) IdealTime,convert(nvarchar,Lattitude)[Lat],convert(nvarchar,longitude)[Long],convert(nvarchar,ctime,101)ctime, m2.fld1[vehicleNo], g.imieno, g.Speed"
            query &= " from mmm_mst_gpsdata g with (nolock) join mmm_mst_master m2 with (nolock) on m2.fld12=g.imieno "
            query &= "where m2.documenttype='vehicle' and m2.fld14 in (" & Ids.TrimEnd(CChar(",")) & ") and m2.eid=54  and m2.fld12='" & Id & "' "
            query &= " group by g.tid,imieno,ctime,m2.fld1,msensor,lattitude,longitude,speed having ctime=(select max(ctime)"
            query &= " from mmm_mst_gpsdata with (nolock) where imieno=g.imieno) "
            Dim oda As SqlDataAdapter = New SqlDataAdapter(query, con)
            Dim dt As New DataTable
            oda.Fill(dt)

            Dim TotalHrs As Integer = Convert.ToInt32(dt.Rows(0).Item("IdealTime")) / 60
            Dim TotalMints As Integer = Convert.ToInt32(dt.Rows(0).Item("IdealTime")) Mod 60

            Dim hr As String
            Dim mm As String

            hr = If(TotalHrs < 10, "0" & TotalHrs.ToString(), TotalHrs.ToString())
            mm = If(TotalMints < 10, "0" & TotalMints.ToString(), TotalMints.ToString())

            Dim dipTime As String = hr & ":" & mm

            Info = "<span style='font-weight:bold;'>IMEINO : " + dt.Rows(0).Item("imieno") + "</span> <br>Vehicle Name : " + dt.Rows(0).Item("Site_Name") + " <br>Vehicle No : " + dt.Rows(0).Item("vehicleNo").ToString() + " <br>Speed : " + dt.Rows(0).Item("Speed").ToString() + " Km/h <br>Ideal Time : " + dipTime.ToString() + "(HH:MM) <br>Last Record Time : " + dt.Rows(0).Item("ctime").ToString() + " <br> Lattitude : " + dt.Rows(0).Item("Lat").ToString() + "<br>Longitude : " + dt.Rows(0).Item("Long").ToString() + ""

            Return Info
        End If

    End Function

End Class
