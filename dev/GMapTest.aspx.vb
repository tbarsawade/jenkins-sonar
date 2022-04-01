Imports System.Data
Imports System.Data.SqlClient
Imports Microsoft.VisualBasic.FileIO
Partial Class GMapTest
    Inherits System.Web.UI.Page
    Protected Sub Page_Load(sender As Object, e As System.EventArgs) Handles Me.Load
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
            Try
                oda.SelectCommand.CommandText = "select * from mmm_mst_entity where eid=" & Session("EID") & " "
                Dim ds As New DataSet()
                'oda.Fill(ds, "data")
                'If ds.Tables("data").Rows.Count > 0 Then
                '    Udtype = ds.Tables("data").Rows(0).Item("uvdtype").ToString
                '    Ufld = ds.Tables("data").Rows(0).Item("uvuserfield").ToString
                '    UVfld = ds.Tables("data").Rows(0).Item("uvvehiclefield").ToString
                '    Vdtype = ds.Tables("data").Rows(0).Item("VIDType").ToString
                '    Vfld = ds.Tables("data").Rows(0).Item("vivehiclefield").ToString
                '    vemei = ds.Tables("data").Rows(0).Item("viimeifield").ToString
                'End If
                Dim dtt As New DataTable()
                Circle.Items.Clear()
                oda.SelectCommand.CommandText = "select distinct top 20 tid,fld1 from mmm_mst_master where DOCUMENTTYPE='Site Type' and eid=" & Session("EID") & " and fld1<>'' order by fld1"
                oda.Fill(dtt)
                For i As Integer = 0 To dtt.Rows.Count - 1
                    If dtt.Rows(i).Item("fld1").ToString.ToUpper = "HUB" Then
                        Circle.Items.Add(Convert.ToString(dtt.Rows(i).Item("fld1").ToString))
                        Circle.Items(i).Value = dtt.Rows(i).Item("tid").ToString
                        Circle.Items(i).Attributes.Add("style", "color:Orange;")
                    ElseIf dtt.Rows(i).Item("fld1").ToString.ToUpper = "BSC" Then
                        Circle.Items.Add(Convert.ToString(dtt.Rows(i).Item("fld1").ToString))
                        Circle.Items(i).Value = dtt.Rows(i).Item("tid").ToString
                        Circle.Items(i).Attributes.Add("style", "color:lightblue;")
                    ElseIf dtt.Rows(i).Item("fld1").ToString.ToUpper = "STRATEGIC" Then
                        Circle.Items.Add(Convert.ToString(dtt.Rows(i).Item("fld1").ToString))
                        Circle.Items(i).Value = dtt.Rows(i).Item("tid").ToString
                        Circle.Items(i).Attributes.Add("style", "color:DarkBlue;")
                    ElseIf dtt.Rows(i).Item("fld1").ToString.ToUpper = "NON STRATEGIC" Then
                        Circle.Items.Add(Convert.ToString(dtt.Rows(i).Item("fld1").ToString))
                        Circle.Items(i).Value = dtt.Rows(i).Item("tid").ToString
                        Circle.Items(i).Attributes.Add("style", "color:Blueviolet;")
                    End If
                    ''Circle.Items(i).Attributes.Add("style", "color:DarkGreen;")
                    Circle.Items(i).Selected = True
                    Circle.Items(i).Attributes.Add("onclick", "ShowHideMap(this)")
                Next
                'Circle.Items(0).Attributes.Add("onclick", "ShowHideMap(this)")
                'Circle.Items(0).Selected = True
                'Circle.Items(1).Attributes.Add("onclick", "ShowHideMap(this)")
                '' Circle.Items(1).Selected = True
                'Circle.Items(2).Attributes.Add("onclick", "ShowHideMap(this)")
                'Circle.Items(3).Attributes.Add("onclick", "ShowHideMap(this)")

                oda.SelectCommand.CommandText = "select * from mmm_mst_master where eid=" & Session("EID") & " and documenttype='vehicle type' "
                dtt.Clear()
                oda.Fill(dtt)
                For i As Integer = 0 To dtt.Rows.Count - 1
                    chkvtype.Items.Add(Convert.ToString(dtt.Rows(i).Item("fld1").ToString))
                    chkvtype.Items(i).Value = dtt.Rows(i).Item("tid").ToString
                    chkvtype.Items(i).Attributes.Add("onclick", "ShowHideMap(this)")
                    chkvtype.Items(i).Selected = True
                Next
                BindAll()
            Catch ex As Exception
            Finally
                If Not con Is Nothing Then
                    con.Close()
                    oda.Dispose()
                    con.Dispose()
                End If
                If Not oda Is Nothing Then
                    oda.Dispose()
                End If
            End Try
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
    Private Sub BindAll()
        Dim id As String = ""
        For i As Integer = 0 To Circle.Items.Count - 1
            'If Circle.Items(i).Selected = True Then
            id = id & Circle.Items(i).Value & ","
            'End If
        Next
        For i As Integer = 0 To chkvtype.Items.Count - 1
            'If chkvtype.Items(i).Selected = True Then
            id = id & chkvtype.Items(i).Value & ","
            'End If
        Next
        If id.Length > 0 Then
            id = Left(id, id.Length - 1)
            Maprendorforsite(id)
        End If
    End Sub
    Protected Sub FilterSite(sender As Object, e As System.EventArgs)
        Dim id As String = ""
        For i As Integer = 0 To Circle.Items.Count - 1
            If Circle.Items(i).Selected = True Then
                id = id & Circle.Items(i).Value & ","
            End If
        Next
        For i As Integer = 0 To chkvtype.Items.Count - 1
            If chkvtype.Items(i).Selected = True Then
                id = id & chkvtype.Items(i).Value & ","
            End If
        Next
        If id.Length > 0 Then
            id = Left(id, id.Length - 1)
            Maprendorforsite(id)
        End If
    End Sub
    Protected Function Maprendorforsite(ByVal sid As String) As String
        Try
            Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
            Dim con As New SqlConnection(conStr)
            Dim apikey As String = String.Empty
            Dim oda As SqlDataAdapter = New SqlDataAdapter("", con)
            'Clear List Box
            LstVehicle.Items.Clear()
            'Change Check Box Text Color
            Circle.Items(0).Attributes.Add("style", "color:lightblue;")
            Circle.Items(1).Attributes.Add("style", "color:Orange;")
            Circle.Items(2).Attributes.Add("style", "color:Blueviolet;")
            Circle.Items(3).Attributes.Add("style", "color:DarkBlue;")
            Dim ds As New DataSet()
            ds.Tables.Add("table1")

            'Dim r As String = "select m.tid[TID],m.fld10[SiteID],replace(replace(replace(replace(m.fld11,'(',' '),')',' '),'{',' '),'}',' ')[SiteName],m.fld13[Address],m1.fld1[Site],''[Info],ltrim(rtrim(left(replace(m.fld21,',','        '),9)))[Lat] ,rtrim(ltrim(right(replace(m.fld21,',','        '),9)))[Long], m.fld12[Group] from mmm_mst_master m with (nolock) left outer join mmm_mst_master m1 with (nolock) on convert(nvarchar,m1.tid)=m.fld12  where m.documenttype='Site' and m1.documenttype='Site Type' and m1.eid=54 and m.eid=54 and m.fld12 in (" & sid & ") and m.fld21<>'' and m.fld21 is not null and m.fld21<>'0' and m.fld21 not like '%NA%' "
            'oda.SelectCommand.CommandText = r
            'oda.Fill(ds, "table1")

            Dim lines = IO.File.ReadAllLines(Server.MapPath("DOCS/sitefiletest.csv"))

            Dim tbl = New DataTable
            Dim colCount = lines.First.Split(","c).Length
            For i As Int32 = 0 To colCount - 1
                ds.Tables("table1").Columns.Add(New DataColumn(lines.First.Split(","c).ToArray(i).ToString, GetType(String)))
                'Exit For
            Next
            Dim k As Integer = 0
            For Each line In lines
                If lines(0).ToString = lines(k).ToString Then
                Else
                    Dim objFields = From field In line.Split(","c)
                             Select CType((field), Object)
                    'Dim newRow = tbl.Rows.Add()
                    Dim newRow = ds.Tables("table1").Rows.Add()
                    newRow.ItemArray = objFields.ToArray()
                End If
                k = k + 1
            Next
            Dim str As String = "select max(g.tid)[TID],IMIENO[SiteID],max(ctime)[SiteName],max(m2.fld1)[Address],(select msensor from mmm_mst_gpsdata where tid=max(g.tid))[Site] ,datediff(minute,max(ctime),getdate())[info],(select Lattitude from mmm_mst_gpsdata where tid=max(g.tid))[Lat],(select longitude from mmm_mst_gpsdata where tid=max(g.tid))[Long], max(m2.fld14)[Group],max(m2.fld10)[Site Name],(select speed from mmm_mst_gpsdata where tid=max(g.tid))[Site Address],''[OandM Head],''[Maintenance Head],''[Opex Manager],''[Security Manager],''[Zonal Head],''[Cluster Manager],''[Supervisor],''[Technician]"
            str = str & " from mmm_mst_gpsdata g with (nolock) left outer join mmm_mst_master m1 with (nolock) on m1.fld1=g.imieno left outer join mmm_mst_master m2 with (nolock) on m2.fld12=convert(nvarchar,m1.fld1)  where m2.documenttype='vehicle' and m2.eid=54 and m1.eid=54 and m1.documenttype='GPS Device' and m2.fld14 in (" & sid & ") group by imieno"
            oda.SelectCommand.CommandText = str
            oda.SelectCommand.CommandTimeout = 180
            oda.Fill(ds, "table1")

            Dim centerUser As String = String.Empty
            Dim JLineCoOrdinate As String = ""

            ' Dim inii As String
            Dim j As Integer = 0
            'inii = ds.Tables("table1").Rows.Count.ToString

            Dim icon As String = ""
            For i As Integer = 0 To ds.Tables("table1").Rows.Count - 1

                If ds.Tables("table1").Rows(i).Item("Site").ToString().ToUpper = "HUB" Then
                    icon = " images/darkyellow.png"
                    JLineCoOrdinate = JLineCoOrdinate & "['SiteID : " & ds.Tables("table1").Rows(i).Item("SiteID").ToString() & "  <br>Site : " + ds.Tables("table1").Rows(i).Item("Site").ToString() + " <br>Site Name : " + ds.Tables("table1").Rows(i).Item("Site Name").ToString() + " <br>OandM Head: " + ds.Tables("table1").Rows(i).Item("OandM Head").ToString() + " <br> Maintenance Head : " + ds.Tables("table1").Rows(i).Item("Maintenance Head").ToString() + " <br>Opex Manager : " + ds.Tables("table1").Rows(i).Item("Opex Manager").ToString() + " <br>Security Manager : " + ds.Tables("table1").Rows(i).Item("Security Manager").ToString() + " <br>Zonal Head : " + ds.Tables("table1").Rows(i).Item("Zonal Head").ToString() + " <br>Cluster Manager :" + ds.Tables("table1").Rows(i).Item("Cluster Manager").ToString() + " <br>Supervisor : " + ds.Tables("table1").Rows(i).Item("Supervisor").ToString() + " <br>Technician : " + ds.Tables("table1").Rows(i).Item("Technician").ToString() + "'," & ds.Tables("table1").Rows(i).Item("Lat").ToString() & "," & ds.Tables("table1").Rows(i).Item("Long").ToString() & ",'" + icon + "'," & "'" & ds.Tables("table1").Rows(i).Item("Group").ToString() & "'],"
                ElseIf ds.Tables("table1").Rows(i).Item("Site").ToString().ToUpper = "BSC" Then
                    icon = " images/lightblue.png"
                    JLineCoOrdinate = JLineCoOrdinate & "['SiteID : " & ds.Tables("table1").Rows(i).Item("SiteID").ToString() & "  <br>Site : " + ds.Tables("table1").Rows(i).Item("Site").ToString() + " <br>Site Name : " + ds.Tables("table1").Rows(i).Item("Site Name").ToString() + " <br>OandM Head: " + ds.Tables("table1").Rows(i).Item("OandM Head").ToString() + " <br> Maintenance Head : " + ds.Tables("table1").Rows(i).Item("Maintenance Head").ToString() + " <br>Opex Manager : " + ds.Tables("table1").Rows(i).Item("Opex Manager").ToString() + " <br>Security Manager : " + ds.Tables("table1").Rows(i).Item("Security Manager").ToString() + " <br>Zonal Head : " + ds.Tables("table1").Rows(i).Item("Zonal Head").ToString() + " <br>Cluster Manager :" + ds.Tables("table1").Rows(i).Item("Cluster Manager").ToString() + " <br>Supervisor : " + ds.Tables("table1").Rows(i).Item("Supervisor").ToString() + " <br>Technician : " + ds.Tables("table1").Rows(i).Item("Technician").ToString() + "'," & ds.Tables("table1").Rows(i).Item("Lat").ToString() & "," & ds.Tables("table1").Rows(i).Item("Long").ToString() & ",'" + icon + "'," & "'" & ds.Tables("table1").Rows(i).Item("Group").ToString() & "'],"
                ElseIf ds.Tables("table1").Rows(i).Item("Site").ToString().ToUpper = "STRATEGIC" Then
                    icon = " images/blue.png"
                    JLineCoOrdinate = JLineCoOrdinate & "['SiteID : " & ds.Tables("table1").Rows(i).Item("SiteID").ToString() & "  <br>Site : " + ds.Tables("table1").Rows(i).Item("Site").ToString() + " <br>Site Name : " + ds.Tables("table1").Rows(i).Item("Site Name").ToString() + " <br>OandM Head: " + ds.Tables("table1").Rows(i).Item("OandM Head").ToString() + " <br> Maintenance Head : " + ds.Tables("table1").Rows(i).Item("Maintenance Head").ToString() + " <br>Opex Manager : " + ds.Tables("table1").Rows(i).Item("Opex Manager").ToString() + " <br>Security Manager : " + ds.Tables("table1").Rows(i).Item("Security Manager").ToString() + " <br>Zonal Head : " + ds.Tables("table1").Rows(i).Item("Zonal Head").ToString() + " <br>Cluster Manager :" + ds.Tables("table1").Rows(i).Item("Cluster Manager").ToString() + " <br>Supervisor : " + ds.Tables("table1").Rows(i).Item("Supervisor").ToString() + " <br>Technician : " + ds.Tables("table1").Rows(i).Item("Technician").ToString() + "'," & ds.Tables("table1").Rows(i).Item("Lat").ToString() & "," & ds.Tables("table1").Rows(i).Item("Long").ToString() & ",'" + icon + "'," & "'" & ds.Tables("table1").Rows(i).Item("Group").ToString() & "'],"
                ElseIf ds.Tables("table1").Rows(i).Item("Site").ToString().ToUpper = "NON STRATEGIC" Then
                    icon = " images/darkk.png"
                    JLineCoOrdinate = JLineCoOrdinate & "['SiteID : " & ds.Tables("table1").Rows(i).Item("SiteID").ToString() & "  <br>Site : " + ds.Tables("table1").Rows(i).Item("Site").ToString() + " <br>Site Name : " + ds.Tables("table1").Rows(i).Item("Site Name").ToString() + " <br>OandM Head: " + ds.Tables("table1").Rows(i).Item("OandM Head").ToString() + " <br> Maintenance Head : " + ds.Tables("table1").Rows(i).Item("Maintenance Head").ToString() + " <br>Opex Manager : " + ds.Tables("table1").Rows(i).Item("Opex Manager").ToString() + " <br>Security Manager : " + ds.Tables("table1").Rows(i).Item("Security Manager").ToString() + " <br>Zonal Head : " + ds.Tables("table1").Rows(i).Item("Zonal Head").ToString() + " <br>Cluster Manager :" + ds.Tables("table1").Rows(i).Item("Cluster Manager").ToString() + " <br>Supervisor : " + ds.Tables("table1").Rows(i).Item("Supervisor").ToString() + " <br>Technician : " + ds.Tables("table1").Rows(i).Item("Technician").ToString() + "'," & ds.Tables("table1").Rows(i).Item("Lat").ToString() & "," & ds.Tables("table1").Rows(i).Item("Long").ToString() & ",'" + icon + "'," & "'" & ds.Tables("table1").Rows(i).Item("Group").ToString() & "'],"
                Else
                    If ds.Tables("table1").Rows(i).Item("Site").ToString().Trim() <> "" Or ds.Tables("table1").Rows(i).Item("Info").ToString().Trim() <> "" Then
                        If ds.Tables("table1").Rows(i).Item("Site").ToString().Trim() = "0" Or ds.Tables("table1").Rows(i).Item("Site").ToString().Trim() = "1" Then
                            If ds.Tables("table1").Rows(i).Item("Site").ToString() = "0" And Val(ds.Tables("table1").Rows(i).Item("Info")) < 10 Then
                                icon = " images/car4.png"
                                JLineCoOrdinate = JLineCoOrdinate & "['IMEINO : " & ds.Tables("table1").Rows(i).Item("SiteID").ToString() & " <br>Vehicle Name : " + ds.Tables("table1").Rows(i).Item("Site Name").ToString() + " <br>Vehicle No : " + ds.Tables("table1").Rows(i).Item("Address").ToString() + "<br>Speed : " + ds.Tables("table1").Rows(i).Item("Site Address").ToString() + " Km/h <br>Last Record Time : " + ds.Tables("table1").Rows(i).Item("SiteName").ToString() + " '," & ds.Tables("table1").Rows(i).Item("Lat").ToString() & "," & ds.Tables("table1").Rows(i).Item("Long").ToString() & ",'" + icon + "'," & "'" & ds.Tables("table1").Rows(i).Item("Group").ToString() & "'," & ds.Tables("table1").Rows(i).Item("SiteID").ToString() & "],"
                                LstVehicle.Items.Add(ds.Tables("table1").Rows(i).Item("Address").ToString())
                                LstVehicle.Items(j).Text = ds.Tables("table1").Rows(i).Item("Site Name").ToString()
                                LstVehicle.Items(j).Value = ds.Tables("table1").Rows(i).Item("SiteID").ToString()
                                LstVehicle.Items(j).Attributes.Add("style", "color:Black;")
                                LstVehicle.Items(j).Attributes.Add("onclick", "focusMarker(this);")
                                j = j + 1
                            ElseIf ds.Tables("table1").Rows(i).Item("Site").ToString() = "1" And Val(ds.Tables("table1").Rows(i).Item("Info")) < 10 Then
                                icon = " images/car2.png"
                                JLineCoOrdinate = JLineCoOrdinate & "['IMEINO : " & ds.Tables("table1").Rows(i).Item("SiteID").ToString() & " <br>Vehicle Name : " + ds.Tables("table1").Rows(i).Item("Site Name").ToString() + " <br>Vehicle No : " + ds.Tables("table1").Rows(i).Item("Address").ToString() + "<br>Speed : " + ds.Tables("table1").Rows(i).Item("Site Address").ToString() + " Km/h <br>Last Record Time : " + ds.Tables("table1").Rows(i).Item("SiteName").ToString() + " '," & ds.Tables("table1").Rows(i).Item("Lat").ToString() & "," & ds.Tables("table1").Rows(i).Item("Long").ToString() & ",'" + icon + "'," & "'" & ds.Tables("table1").Rows(i).Item("Group").ToString() & "'," & ds.Tables("table1").Rows(i).Item("SiteID").ToString() & "],"
                                LstVehicle.Items.Add(ds.Tables("table1").Rows(i).Item("Address").ToString())
                                LstVehicle.Items(j).Text = ds.Tables("table1").Rows(i).Item("Site Name").ToString()
                                LstVehicle.Items(j).Value = ds.Tables("table1").Rows(i).Item("SiteID").ToString()
                                LstVehicle.Items(j).Attributes.Add("style", "color:Green;")
                                LstVehicle.Items(j).Attributes.Add("onclick", "focusMarker(this);")
                                j = j + 1
                            ElseIf Val(ds.Tables("table1").Rows(i).Item("Site")) > 10 And Val(ds.Tables("table1").Rows(i).Item("Info")) < 240 Then
                                icon = " images/car5.png"
                                JLineCoOrdinate = JLineCoOrdinate & "['IMEINO : " & ds.Tables("table1").Rows(i).Item("SiteID").ToString() & " <br>Vehicle Name : " + ds.Tables("table1").Rows(i).Item("Site Name").ToString() + " <br>Vehicle No : " + ds.Tables("table1").Rows(i).Item("Address").ToString() + "<br>Speed : " + ds.Tables("table1").Rows(i).Item("Site Address").ToString() + " Km/h <br>Last Record Time : " + ds.Tables("table1").Rows(i).Item("SiteName").ToString() + " '," & ds.Tables("table1").Rows(i).Item("Lat").ToString() & "," & ds.Tables("table1").Rows(i).Item("Long").ToString() & ",'" + icon + "'," & "'" & ds.Tables("table1").Rows(i).Item("Group").ToString() & "'," & ds.Tables("table1").Rows(i).Item("SiteID").ToString() & "],"
                                LstVehicle.Items.Add(ds.Tables("table1").Rows(i).Item("Address").ToString())
                                LstVehicle.Items(j).Text = ds.Tables("table1").Rows(i).Item("Site Name").ToString()
                                LstVehicle.Items(j).Value = ds.Tables("table1").Rows(i).Item("SiteID").ToString()
                                LstVehicle.Items(j).Attributes.Add("style", "color:Yellow;")
                                LstVehicle.Items(j).Attributes.Add("onclick", "focusMarker(this);")
                                j = j + 1
                            Else
                                icon = " images/car1.png"
                                JLineCoOrdinate = JLineCoOrdinate & "['IMEINO : " & ds.Tables("table1").Rows(i).Item("SiteID").ToString() & " <br>Vehicle Name : " + ds.Tables("table1").Rows(i).Item("Site Name").ToString() + " <br>Vehicle No : " + ds.Tables("table1").Rows(i).Item("Address").ToString() + "<br>Speed : " + ds.Tables("table1").Rows(i).Item("Site Address").ToString() + " Km/h <br>Last Record Time : " + ds.Tables("table1").Rows(i).Item("SiteName").ToString() + " '," & ds.Tables("table1").Rows(i).Item("Lat").ToString() & "," & ds.Tables("table1").Rows(i).Item("Long").ToString() & ",'" + icon + "'," & "'" & ds.Tables("table1").Rows(i).Item("Group").ToString() & "'," & ds.Tables("table1").Rows(i).Item("SiteID").ToString() & "],"
                                LstVehicle.Items.Add(ds.Tables("table1").Rows(i).Item("Address").ToString())
                                LstVehicle.Items(j).Text = ds.Tables("table1").Rows(i).Item("Site Name").ToString()
                                LstVehicle.Items(j).Value = ds.Tables("table1").Rows(i).Item("SiteID").ToString()
                                LstVehicle.Items(j).Attributes.Add("style", "color:Red;")
                                LstVehicle.Items(j).Attributes.Add("onclick", "focusMarker(this);")
                                j = j + 1
                            End If
                        End If
                    End If

                End If

            Next
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
                url = "<script type='text/javascript'>google.load('maps', '4.7', { 'other_params': 'sensor=true' }); </script>"
            End If

            'to get central location divide it by two
            If ds.Tables("table1").Rows.Count > 0 Then
                ''Dim cLocation As Integer = ds.Tables("table1").Rows.Count / 2
                '' Dim JQMAP As String = " " + url + " <script>     function initialize() { var locations = [ " + JLineCoOrdinate + " ]; var map = new google.maps.Map(document.getElementById('map'), { zoom: 7,center: new google.maps.LatLng(19.8761653,75.3433139),mapTypeId: google.maps.MapTypeId.ROADMAP  }); var infowindow = new google.maps.InfoWindow();var lineCoordinates = []; for (i = 0; i < locations.length; i++) { lineCoordinates.push(new google.maps.LatLng(0, 0)); }    var FrPath = new google.maps.Polyline({  path: lineCoordinates,strokeColor: 'None'  });FrPath.setMap(map);for (i = 0; i < locations.length; i++) {marker = new google.maps.Marker({ position: new google.maps.LatLng(locations[i][1], locations[i][2]),map: map,icon:  locations[i][3],group: locations[i][4], size: new google.maps.Size(1 , 1)});  myMarkersArray.push(marker);  google.maps.event.addListener(marker, 'click', (function (marker, i) { return function () { infowindow.setContent(locations[i][0]);infowindow.open(map, marker);}})(marker, i));}} google.maps.event.addDomListener(window, 'load', initialize);</script>"
                Dim JQMAP As String = " " + url + "<script> var locations = [ " + JLineCoOrdinate + " ]; </script>"
                'hdndata.Value = JLineCoOrdinate

                Page.ClientScript.RegisterStartupScript(Me.GetType(), "", JQMAP)

            End If
            oda.Dispose()
            ds.Dispose()
            con.Dispose()
            Return "0"
        Catch ex As Exception
            Throw
        End Try
    End Function

End Class
