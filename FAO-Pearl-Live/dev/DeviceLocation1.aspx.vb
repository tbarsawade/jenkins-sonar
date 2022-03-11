Imports System.Data
Imports System.Data.SqlClient
Imports System.Collections.Generic
Imports System.Web.Services
Partial Class DeviceLocation
    Inherits System.Web.UI.Page
    Public ime As String
    Public dts1 As String
    Public dts2 As String
    Dim imeino As String
    Dim vehcleName As String
    Protected Sub Page_Load(sender As Object, e As System.EventArgs) Handles Me.Load
        If Not IsPostBack Then

            hdnui.Value = Session("EID").ToString()


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
                oda.Fill(ds, "data")
                If ds.Tables("data").Rows.Count > 0 Then
                    Udtype = ds.Tables("data").Rows(0).Item("uvdtype").ToString
                    Ufld = ds.Tables("data").Rows(0).Item("uvuserfield").ToString
                    UVfld = ds.Tables("data").Rows(0).Item("uvvehiclefield").ToString
                    Vdtype = ds.Tables("data").Rows(0).Item("VIDType").ToString
                    Vfld = ds.Tables("data").Rows(0).Item("vivehiclefield").ToString
                    vemei = ds.Tables("data").Rows(0).Item("viimeifield").ToString
                End If
                Dim dtt As New DataTable()
                Circle.Items.Clear()
                If Session("EID") = 32 Then
                    lblCircle.Text = "State"
                    lblcity.Text = "City"
                    oda.SelectCommand.CommandText = "select distinct  tid,fld1 from mmm_mst_master where DOCUMENTTYPE='State' and eid=" & Session("EID") & " order by fld1"
                Else
                    lblCircle.Text = "Circle"
                    lblcity.Text = "Cluster"
                    oda.SelectCommand.CommandText = "select distinct  tid,fld1 from mmm_mst_master where DOCUMENTTYPE='Circle' and eid=" & Session("EID") & " order by fld1"
                End If
                oda.Fill(dtt)
                For i As Integer = 0 To dtt.Rows.Count - 1
                    Circle.Items.Add(Convert.ToString(dtt.Rows(i).Item("fld1").ToString))
                    Circle.Items(i).Value = dtt.Rows(i).Item("tid").ToString

                Next
                City.Items.Clear()

                If Session("EID") = 32 Then
                    oda.SelectCommand.CommandText = "select distinct tid, fld1 from mmm_mst_master where DOCUMENTTYPE='City' and eid=" & Session("EID") & " order by fld1 "
                Else
                    oda.SelectCommand.CommandText = "select distinct tid, fld1 from mmm_mst_master where DOCUMENTTYPE='Cluster' and eid=" & Session("EID") & " order by fld1 "
                End If
                Dim circledata As New DataTable()
                oda.Fill(circledata)
                For i As Integer = 0 To circledata.Rows.Count - 1
                    City.Items.Add(Convert.ToString(circledata.Rows(i).Item("fld1")))
                    City.Items(i).Value = circledata.Rows(i).Item("tid").ToString
                Next
                If Session("EID") = 32 Then
                    If Session("USERROLE") = "SU" Or Session("USERROLE") = "FCAGGN" Or Session("USERROLE") = "BNK" Or Session("USERROLE") = "CADMIN" Or Session("USERROLE") = "FCANHQ" Then
                        'oda.SelectCommand.CommandText = "select  distinct m." & vemei & "[IMIENO], (select username from mmm_mst_user where eid=" & Session("EID") & " and uid=d." & Ufld & ")[UserName],m." & Vfld & "[VehicleNo] from mmm_mst_Doc d inner join mmm_mst_master m on m.tid=d." & UVfld & " inner join mmm_mst_gpsdata g on g.imieno=m." & vemei & " where d.eid=" & Session("EID") & " and  d.documenttype='" & Udtype & "'  and imieno is not null order by username "
                        oda.SelectCommand.CommandText = "select  distinct m." & vemei & "[IMIENO], u.username,m." & Vfld & "[VehicleNo] from mmm_mst_Doc d inner join mmm_mst_master m on convert(nvarchar,m.tid)=d." & UVfld & " inner join mmm_mst_user u on u.uid=d." & Ufld & " where d.eid=" & Session("EID") & " and  d.documenttype='" & Udtype & "'  and m." & vemei & " is not null and m." & vemei & " <>'' order by username "
                        '  ddlrtype.Items.Remove("LAST SIGNAL REPORT")
                        ' ddlrtype.Items.RemoveAt(5)
                        'ddlrtype.Items.Add("LAST SIGNAL REPORT")
                        'ddlrtype.Items(5).Value = 7
                        'ElseIf Session("USERROLE").ToUpper() = "USER" Then
                        '    oda.SelectCommand.CommandText = "select  distinct m." & vemei & "[IMIENO], u.username,m." & Vfld & "[VehicleNo] from mmm_mst_Doc d inner join mmm_mst_master m on m.tid=d." & UVfld & " inner join mmm_mst_gpsdata g on g.imieno=m." & vemei & " inner join mmm_mst_user u on u.uid=d." & Ufld & " where d.eid=" & Session("EID") & " and  d.documenttype='" & Udtype & "' and d." & Ufld & "= " & Session("UID").ToString() & "  and imieno is not null order by username "
                    Else
                        'If IsNothing(Session("SUBUID")) Then
                        '    oda.SelectCommand.CommandText = "select  distinct m." & vemei & "[IMIENO], u.username,m." & Vfld & "[VehicleNo] from mmm_mst_Doc d inner join mmm_mst_master m on m.tid=d." & UVfld & " inner join mmm_mst_gpsdata g on g.imieno=m." & vemei & " inner join mmm_mst_user u on u.uid=d." & Ufld & " where d.eid=" & Session("EID") & " and  d.documenttype='" & Udtype & "' and d." & Ufld & " in (" & Session("UID").ToString() & ")  and imieno is not null order by username "
                        'Else
                        '    oda.SelectCommand.CommandText = "select  distinct m." & vemei & "[IMIENO], u.username,m." & Vfld & "[VehicleNo] from mmm_mst_Doc d inner join mmm_mst_master m on m.tid=d." & UVfld & " inner join mmm_mst_gpsdata g on g.imieno=m." & vemei & " inner join mmm_mst_user u on u.uid=d." & Ufld & " where d.eid=" & Session("EID") & " and  d.documenttype='" & Udtype & "' and d." & Ufld & " in (" & Session("SUBUID").ToString() & ")  and imieno is not null order by username "
                        'End If
                        oda.SelectCommand.CommandType = CommandType.StoredProcedure
                        ' oda.SelectCommand.CommandText = "uspGetRoleUID"
                        oda.SelectCommand.CommandText = "uspGetRoleUIDWithSUID"
                        oda.SelectCommand.Parameters.Clear()
                        oda.SelectCommand.Parameters.AddWithValue("uid", Session("UID"))
                        oda.SelectCommand.Parameters.AddWithValue("eid", Session("EID"))
                        If IsNothing(Session("SUBUID")) Then
                            oda.SelectCommand.Parameters.AddWithValue("SUID", Session("UID"))
                        Else
                            oda.SelectCommand.Parameters.AddWithValue("SUID", Session("SUBUID"))
                        End If
                        oda.SelectCommand.Parameters.AddWithValue("role", Session("USERROLE"))
                    End If
                ElseIf Session("EID") = 66 Or Session("EID") = 63 Or Session("EID") = 71 Then
                    If Session("USERROLE").ToString.ToUpper = "SU" Or Session("USERROLE").ToString.ToUpper = "CADMIN" Or Session("USERROLE").ToString.ToUpper = "CORPORATEUSER" Then
                        oda.SelectCommand.CommandText = "select " & vemei & "[IMEI]," & Vfld & "[PhoneUserName] from mmm_mst_master with(nolock) where documenttype='" & Vdtype & "' and eid=" & Session("EID") & ""
                    Else
                        oda.SelectCommand.CommandText = "select " & vemei & "[IMEI]," & Vfld & "[PhoneUserName] from mmm_mst_master with(nolock) where documenttype='" & Vdtype & "' and eid=" & Session("EID") & ""
                    End If
                ElseIf Session("EID") = 56 Then
                    oda.SelectCommand.CommandText = "select fld2[IMEI],fld1[VehicleName],fld10[VehicleNo] from mmm_mst_master with(nolock) where documenttype='vehicle' and eid=" & Session("EID") & " and fld2<>''"
                Else
                    If Session("USERROLE").ToString.ToUpper = "SU" Or Session("USERROLE").ToString.ToUpper = "CADMIN" Or Session("USERROLE").ToString.ToUpper = "CORPORATEUSER" Then
                        oda.SelectCommand.CommandText = "select fld12[IMEI],fld10[VehicleName],fld1[VehicleNo] from mmm_mst_master where documenttype='vehicle' and eid=" & Session("EID") & ""
                    Else
                        oda.SelectCommand.CommandType = CommandType.StoredProcedure
                        ' oda.SelectCommand.CommandText = "uspGetRoleUID"
                        oda.SelectCommand.CommandText = "vehiclerightforIndus"
                        oda.SelectCommand.Parameters.Clear()
                        oda.SelectCommand.Parameters.AddWithValue("uid", Session("UID"))
                        oda.SelectCommand.Parameters.AddWithValue("eid", Session("EID"))
                        oda.SelectCommand.Parameters.AddWithValue("rolename", Session("USERROLE"))
                        oda.SelectCommand.Parameters.AddWithValue("docType", "Vehicle")
                    End If
                End If
                ds.Clear()
                oda.SelectCommand.CommandTimeout = 3000
                oda.Fill(ds, "vemei")
                For i As Integer = 0 To ds.Tables("vemei").Rows.Count - 1
                    If Session("EID") = 66 Or Session("EID") = 63 Or Session("EID") = 71 Then
                        UsrVeh.Items.Add(ds.Tables("vemei").Rows(i).Item(1).ToString() & "-" & ds.Tables("vemei").Rows(i).Item(0).ToString())
                    Else
                        UsrVeh.Items.Add(ds.Tables("vemei").Rows(i).Item(1).ToString() & "-" & ds.Tables("vemei").Rows(i).Item(2).ToString())
                    End If
                    UsrVeh.Items(i).Value = ds.Tables("vemei").Rows(i).Item(0).ToString()
                Next
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
    Protected Sub Button1_Click(sender As Object, e As System.EventArgs) Handles Button1.Click
        If ViewState("imei") IsNot Nothing Then
            imeino = ViewState("imei").ToString()
            hdimieno.Value = imeino
        End If
        If ViewState("VehicleName") IsNot Nothing Then
            vehcleName = ViewState("VehicleName").ToString()
            hdvehcleName.Value = vehcleName
        End If
        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim con As New SqlConnection(conStr)
        Dim apikey As String = String.Empty
        Dim oda As SqlDataAdapter = New SqlDataAdapter("", con)
        'oda.SelectCommand.CommandText = "select distinct lattitude,longitude from mmm_mst_gpsdata where imieno='356307043232304"
        'Dim ds As New DataSet()
        'oda.Fill(ds, "data")
        ime = hdimieno.Value
        dts1 = hddate1.Value
        dts2 = hddate2.Value

        oda.SelectCommand.CommandText = "select * from mmm_mst_entity where eid=32"
        Label1.Text = ""
        Label2.Text = ""
        Label3.Text = ""
        Dim ds As New DataSet()
        oda.Fill(ds, "data")
        If ds.Tables("data").Rows.Count > 0 Then
            apikey = ds.Tables("data").Rows(0).Item("APIkey").ToString
        End If
        Dim r As String = "select lattitude,longitude,ctime,speed,recordTime from MMM_MST_GPSDATA where tid in (select max(tid) from MMM_MST_GPSDATA where IMIENO='" & imeino & "'  and   cTime >= '" + Date1.Text + " " + TextBox1.Text + "' AND ctime <= '" + Date2.Text + " " + TextBox2.Text + "' group by lattitude,longitude)  order by ctime "
        oda.SelectCommand.CommandText = r
        oda.Fill(ds, "table1")
        If ds.Tables("table1").Rows.Count = 0 Then
            Label3.Text = "No Data Found, Make Sure you entered correct IMIE No"
            Exit Sub
        End If

        'Old logic
        'oda.SelectCommand.CommandText = "select sum(DevDist) as DevDist,max(speed) speed from MMM_MST_GPSDATA where devdist <> 0 and speed>0 and   IMIENO='" & txtIMIENo.Text & "'  and   cTime >= '" + Date1.Text + " " + TextBox1.Text + "' AND ctime <= '" + Date2.Text + " " + TextBox2.Text + "' "

        'New changes 05 Apr
        If CDate(Date1.Text.ToString) > CDate("2014-03-31") Then
            If imeino = "356307044795317" Then
                If CDate(Date1.Text.ToString) > CDate("2014-04-17") Then
                    oda.SelectCommand.CommandText = "select isnull((select sum(devdist) from mmm_mst_gpsdata where imieno=gs.imieno and devdist>0.07 and speed=0 and cTime >= '" + Date1.Text + " " + TextBox1.Text + "' AND ctime <= '" + Date2.Text + " " + TextBox2.Text + "'),0)+ isnull((select sum(devdist) from mmm_mst_gpsdata where imieno=gs.imieno and devdist>0 and speed>0 and cTime >= '" + Date1.Text + " " + TextBox1.Text + "' AND ctime <= '" + Date2.Text + " " + TextBox2.Text + "'),0)[devdist],max(speed) speed from MMM_MST_GPSDATA gs where  devdist>0 and  IMIENO='" & imeino & "'  and   cTime >= '" + Date1.Text + " " + TextBox1.Text + "' AND ctime <= '" + Date2.Text + " " + TextBox2.Text + "' group by imieno"
                Else
                    oda.SelectCommand.CommandText = "select isnull((select sum(devdist) from mmm_mst_gpsdata where imieno=gs.imieno and devdist>0.05 and speed=0 and cTime >= '" + Date1.Text + " " + TextBox1.Text + "' AND ctime <= '" + Date2.Text + " " + TextBox2.Text + "'),0)+ isnull((select sum(devdist) from mmm_mst_gpsdata where imieno=gs.imieno and devdist>0.07 and speed>0 and cTime >= '" + Date1.Text + " " + TextBox1.Text + "' AND ctime <= '" + Date2.Text + " " + TextBox2.Text + "'),0)[devdist],max(speed) speed from MMM_MST_GPSDATA gs where  devdist>0 and  IMIENO='" & imeino & "'  and   cTime >= '" + Date1.Text + " " + TextBox1.Text + "' AND ctime <= '" + Date2.Text + " " + TextBox2.Text + "' group by imieno"
                End If
            Else
                oda.SelectCommand.CommandText = "select isnull((select sum(devdist) from mmm_mst_gpsdata where imieno=gs.imieno and devdist>0.03 and speed=0 and cTime >= '" + Date1.Text + " " + TextBox1.Text + "' AND ctime <= '" + Date2.Text + " " + TextBox2.Text + "'),0)+ isnull((select sum(devdist) from mmm_mst_gpsdata where imieno=gs.imieno and devdist>0 and speed>0 and cTime >= '" + Date1.Text + " " + TextBox1.Text + "' AND ctime <= '" + Date2.Text + " " + TextBox2.Text + "'),0)[devdist],max(speed) speed from MMM_MST_GPSDATA gs where  devdist>0 and  IMIENO='" & imeino & "'  and   cTime >= '" + Date1.Text + " " + TextBox1.Text + "' AND ctime <= '" + Date2.Text + " " + TextBox2.Text + "' group by imieno"
            End If
        Else
            oda.SelectCommand.CommandText = "select sum(DevDist) as DevDist,max(speed) speed from MMM_MST_GPSDATA where devdist <> 0 and   IMIENO='" & imeino & "'  and   cTime >= '" + "2014-06-03 16:09:05.000" + "' AND ctime <= '" + "2014-06-03 17:43:59.000" + "' "
        End If
        oda.Fill(ds, "speedtable")
        If ds.Tables("speedtable").Rows.Count > 0 Then
            Label1.Text = ds.Tables("speedtable").Rows(0).Item("speed").ToString()
            Label3.Text = ds.Tables("speedtable").Rows(0).Item("DevDist").ToString()
        Else
            Label2.Text = "No Data Found"
            'Exit Sub
        End If

        Dim centerUser As String = String.Empty
        Dim JLineCoOrdinate As String = ""

        Dim inii As String

        inii = ds.Tables("table1").Rows.Count.ToString
        'For Each dr As DataRow In ds.Tables("table1").Rows
        'JLineCoOrdinate = JLineCoOrdinate & "['Speed " & dr("speed").ToString() & " Km / Hr <br>RecordTime " + dr("recordTime").ToString() + "<br>Ctime " + dr("ctime").ToString() + " '," & dr("lattitude").ToString() & "," & dr("longitude").ToString() & ",],"
        'Next
        Dim icon As String = ""
        For i As Integer = 0 To ds.Tables("table1").Rows.Count - 1
            If i = 0 Or i = ds.Tables("table1").Rows.Count - 1 Then
                icon = ""
            Else
                If ds.Tables("table1").Rows(i).Item("speed").ToString() > 0 Then
                    icon = " images/Greendot.png"
                Else
                    icon = " images/RedDot.png"
                End If
            End If
            JLineCoOrdinate = JLineCoOrdinate & "['Speed " & ds.Tables("table1").Rows(i).Item("speed").ToString() & " Km / Hr <br>RecordTime " + ds.Tables("table1").Rows(i).Item("recordTime").ToString() + "<br>Ctime " + ds.Tables("table1").Rows(i).Item("ctime").ToString() + " '," & ds.Tables("table1").Rows(i).Item("lattitude").ToString() & "," & ds.Tables("table1").Rows(i).Item("longitude").ToString() & ",'" + icon + "'],"

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
            url = "<script type='text/javascript'>google.load('maps', '3.7', { 'other_params': 'sensor=true' }); </script>"
        End If

        'to get central location divide it by two
        Dim cLocation As Integer = ds.Tables("table1").Rows.Count / 2
        Dim JQMAP As String = " " + url + " <script>     function initialize() { var locations = [ " + JLineCoOrdinate + " ]; var map = new google.maps.Map(document.getElementById('map'), { zoom: 12,center: new google.maps.LatLng(" & ds.Tables("table1").Rows(cLocation).Item("lattitude").ToString() & "," & ds.Tables("table1").Rows(cLocation).Item("longitude").ToString() & "),mapTypeId: google.maps.MapTypeId.ROADMAP  }); var infowindow = new google.maps.InfoWindow();var lineCoordinates = []; for (i = 0; i < locations.length; i++) { lineCoordinates.push(new google.maps.LatLng(locations[i][1], locations[i][2])); }    var FrPath = new google.maps.Polyline({  path: lineCoordinates,strokeColor: 'black'  });FrPath.setMap(map);for (i = 0; i < locations.length; i++) { marker = new google.maps.Marker({ position: new google.maps.LatLng(locations[i][1], locations[i][2 ]),map: map,icon:, size: new google.maps.Size(1 , 1)});google.maps.event.addListener(marker, 'click', (function (marker, i) { return function () { infowindow.setContent(locations[i][0]);infowindow.open(map, marker);}})(marker, i));}} google.maps.event.addDomListener(window, 'load', initialize);</script>"
        ClientScript.RegisterStartupScript(Me.GetType(), "someScript", "MovingMap();", True)
        oda.Dispose()
        ds.Dispose()
        con.Dispose()
    End Sub
    <WebMethod>
    Public Shared Function GetLocations(ByVal imeino As String, ByVal date1 As String, ByVal date2 As String, ByVal vname As String) As List(Of GeoLocation)
        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim con As New SqlConnection(conStr)
        Dim apikey As String = String.Empty
        Dim oda As SqlDataAdapter = New SqlDataAdapter("", con)
        oda.SelectCommand.CommandText = "select lattitude,longitude,ctime,speed from MMM_MST_GPSDATA where tid in (select max(tid) from MMM_MST_GPSDATA where IMIENO='" & imeino & "'  and   cTime >= '" & date1 & "' AND ctime <= '" & date2 & "' group by lattitude,longitude)  order by ctime"
        Dim ds As New DataSet()
        oda.Fill(ds, "Address")
        Dim li As New List(Of GeoLocation)
        For Each dr As DataRow In ds.Tables("Address").Rows
            Dim objGeoLocation As New GeoLocation()
            objGeoLocation.Lat = dr("lattitude")
            objGeoLocation.Longit = dr("longitude")
            objGeoLocation.Speeds = dr("speed")
            objGeoLocation.Ctimeing = dr("ctime")
            objGeoLocation.VehiName = vname
            objGeoLocation.IMEINUMBER = imeino
            li.Add(objGeoLocation)
        Next
        Return li
    End Function
    Protected Sub checkuncheckcicle(sender As Object, e As System.EventArgs) Handles circlecheck.CheckedChanged
        Dim id As String = ""
        If circlecheck.Checked = True Then
            For Each chkitem As System.Web.UI.WebControls.ListItem In Circle.Items
                chkitem.Selected = True
                'id = id & Circle.SelectedValue & ","
            Next
            For i As Integer = 0 To Circle.Items.Count - 1
                Circle.Items(i).Selected = True
                id = id & Circle.Items(i).Value & ","
            Next
            If id.Length > 0 Then
                id = Left(id, id.Length - 1)
                hdcir.Value = id
                filteronCircle(id)
            End If
        Else
            For Each chkitem As System.Web.UI.WebControls.ListItem In Circle.Items
                City.Items.Clear()
                Citycheck.Checked = False
                chkitem.Selected = False
            Next
        End If

    End Sub
    Protected Sub checkuncheckcicle1(sender As Object, e As System.EventArgs) Handles Circle.SelectedIndexChanged
        Dim id As String = ""
        For i As Integer = 0 To Circle.Items.Count - 1
            If Circle.Items(i).Selected = True Then
                id = id & Circle.Items(i).Value & ","
            End If
        Next
        City.Items.Clear()
        If id.Length > 0 Then
            id = Left(id, id.Length - 1)
            hdcir.Value = id
            filteronCircle(id)
        End If

    End Sub
    Protected Sub Citycheckuncheck(sender As Object, e As System.EventArgs) Handles Citycheck.CheckedChanged
        Dim id As String = ""
        If Citycheck.Checked = True Then
            For Each chkitem As System.Web.UI.WebControls.ListItem In City.Items
                chkitem.Selected = True
            Next
            For i As Integer = 0 To City.Items.Count - 1
                City.Items(i).Selected = True
                If Val(City.Items(i).Value) > 0 Then
                    id = id & City.Items(i).Value & ","
                End If
            Next
            If id.Length > 0 Then
                id = Left(id, id.Length - 1)
                hdcity.Value = id
                FilterUserOnCity(id)
            End If
        Else
            For Each chkitem As System.Web.UI.WebControls.ListItem In City.Items
                UsrVeh.Items.Clear()

                chkitem.Selected = False
            Next
        End If
    End Sub
    Protected Sub FilterUserOnCity(ByRef ct As String)
        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim con As SqlConnection = New SqlConnection(conStr)
        Dim oda As SqlDataAdapter = New SqlDataAdapter("", con)
        oda.SelectCommand.CommandText = "select * from mmm_mst_entity where eid=" & Session("EID") & " "
        Dim ds As New DataSet()
        Try
            oda.Fill(ds, "data")
            Dim Udtype As String
            Dim Ufld As String
            Dim UVfld As String
            Dim Vdtype As String
            Dim Vfld As String
            Dim vemei As String
            If ds.Tables("data").Rows.Count > 0 Then
                Udtype = ds.Tables("data").Rows(0).Item("uvdtype").ToString
                Ufld = ds.Tables("data").Rows(0).Item("uvuserfield").ToString
                UVfld = ds.Tables("data").Rows(0).Item("uvvehiclefield").ToString
                Vdtype = ds.Tables("data").Rows(0).Item("VIDType").ToString
                Vfld = ds.Tables("data").Rows(0).Item("vivehiclefield").ToString
                vemei = ds.Tables("data").Rows(0).Item("viimeifield").ToString
            End If
            If Session("EID") = 32 Then
                If Session("USERROLE") = "SU" Or Session("USERROLE") = "FCAGGN" Or Session("USERROLE") = "BNK" Or Session("USERROLE") = "CADMIN" Or Session("USERROLE") = "FCANHQ" Then
                    'oda.SelectCommand.CommandText = "select  distinct m." & vemei & "[IMIENO], (select username from mmm_mst_user where eid=" & Session("EID") & " and uid=d." & Ufld & ")[UserName],m." & Vfld & "[VehicleNo] from mmm_mst_Doc d inner join mmm_mst_master m on m.tid=d." & UVfld & " inner join mmm_mst_gpsdata g on g.imieno=m." & vemei & " where d.eid=" & Session("EID") & " and  d.documenttype='" & Udtype & "'  and imieno is not null order by username "
                    oda.SelectCommand.CommandText = "select  distinct m." & vemei & "[IMIENO], u.username,m." & Vfld & "[VehicleNo] from mmm_mst_Doc d inner join mmm_mst_master m on convert(nvarchar,m.tid)=d." & UVfld & "  inner join mmm_mst_user u on u.uid=d." & Ufld & " where d.eid=" & Session("EID") & " and  d.documenttype='" & Udtype & "' and m.documenttype='vehicle' and u.eid=" & Session("EID") & " and m.eid=" & Session("EID") & " and u.fld14 in (" & ct & ")  and m." & vemei & " is not null and m." & vemei & "<>'' order by username "
                    '  ddlrtype.Items.Remove("LAST SIGNAL REPORT")
                    ' ddlrtype.Items.RemoveAt(5)
                    'ddlrtype.Items.Add("LAST SIGNAL REPORT")
                    'ddlrtype.Items(5).Value = 7
                ElseIf Session("USERROLE").ToUpper() = "USER" Then
                    oda.SelectCommand.CommandText = "select  distinct m." & vemei & "[IMIENO], u.username,m." & Vfld & "[VehicleNo] from mmm_mst_Doc d inner join mmm_mst_master m onconvert(nvarchar,m.tid)=d." & UVfld & " inner join mmm_mst_user u on u.uid=d." & Ufld & " where d.eid=" & Session("EID") & " and  d.documenttype='" & Udtype & "' and d." & Ufld & "= " & Session("UID").ToString() & "  and m." & vemei & " is not null and m." & vemei & "<>'' order by username "
                Else
                    'If IsNothing(Session("SUBUID")) Then
                    '    oda.SelectCommand.CommandText = "select  distinct m." & vemei & "[IMIENO], u.username,m." & Vfld & "[VehicleNo] from mmm_mst_Doc d inner join mmm_mst_master m on m.tid=d." & UVfld & " inner join mmm_mst_gpsdata g on g.imieno=m." & vemei & " inner join mmm_mst_user u on u.uid=d." & Ufld & " where d.eid=" & Session("EID") & " and  d.documenttype='" & Udtype & "' and d." & Ufld & " in (" & Session("UID").ToString() & ")  and imieno is not null order by username "
                    'Else
                    '    If ct <> "" Then
                    '        oda.SelectCommand.CommandText = "select  distinct m." & vemei & "[IMIENO], u.username,m." & Vfld & "[VehicleNo] from mmm_mst_Doc d inner join mmm_mst_master m on m.tid=d." & UVfld & " inner join mmm_mst_gpsdata g on g.imieno=m." & vemei & " inner join mmm_mst_user u on u.uid=d." & Ufld & " where d.eid=" & Session("EID") & " and  d.documenttype='" & Udtype & "' and d." & Ufld & " in (" & Session("SUBUID").ToString() & ") and u.fld14 in (" & ct.ToString & ")  and imieno is not null order by username "
                    '    Else
                    '        oda.SelectCommand.CommandText = "select  distinct m." & vemei & "[IMIENO], u.username,m." & Vfld & "[VehicleNo] from mmm_mst_Doc d inner join mmm_mst_master m on m.tid=d." & UVfld & " inner join mmm_mst_gpsdata g on g.imieno=m." & vemei & " inner join mmm_mst_user u on u.uid=d." & Ufld & " where d.eid=" & Session("EID") & " and  d.documenttype='" & Udtype & "' and d." & Ufld & " in (" & Session("SUBUID").ToString() & ")  and imieno is not null order by username "
                    '    End If
                    'End If
                    oda.SelectCommand.CommandType = CommandType.StoredProcedure
                    'oda.SelectCommand.CommandText = "uspGetRoleUIDNew"
                    oda.SelectCommand.CommandText = "uspGetRoleUIDNewSID"
                    oda.SelectCommand.Parameters.Clear()
                    oda.SelectCommand.Parameters.AddWithValue("uid", Session("UID"))
                    oda.SelectCommand.Parameters.AddWithValue("eid", Session("EID"))
                    If ct <> "" Then
                        oda.SelectCommand.Parameters.AddWithValue("city", ct.ToString)
                    End If
                    If IsNothing(Session("SUBUID")) Then
                        oda.SelectCommand.Parameters.AddWithValue("SUID", "0")
                    Else
                        oda.SelectCommand.Parameters.AddWithValue("SUID", Session("SUBUID"))
                    End If
                End If
                'oda.SelectCommand.CommandText = "select imieno,username,VehicleNo from ( select  distinct m." & vemei & "[IMIENO], (select username from mmm_mst_user where eid=" & Session("EID") & " and uid=d." & Ufld & ")[UserName],m." & Vfld & "[VehicleNo] from mmm_mst_Doc d inner join mmm_mst_master m on m.tid=d." & UVfld & " inner join mmm_mst_gpsdata g on g.imieno=m." & vemei & " where d.eid=" & Session("EID") & " and  d.documenttype='" & Udtype & "'  and imieno is not null) as table1 order by username  "
                ds.Clear()
            Else
                If Session("USERROLE").ToString.ToUpper = "SU" Or Session("USERROLE").ToString.ToUpper = "CADMIN" Or Session("USERROLE").ToString.ToUpper = "CORPORATEUSER" Then
                    oda.SelectCommand.CommandText = "select distinct fld12[IMIENO],fld10[VehicleName],fld1[VehicleNo.] from mmm_mst_master where eid=" & Session("EID") & "  and documenttype='vehicle'"
                Else
                    oda.SelectCommand.CommandText = "select distinct fld12[IMIENO],fld10[VehicleName],fld1[VehicleNo.] from mmm_mst_master where eid=" & Session("EID") & "  and documenttype='vehicle'"
                End If
            End If
            'oda.SelectCommand.CommandText = "select imieno,username,VehicleNo from ( select  distinct m." & vemei & "[IMIENO], (select username from mmm_mst_user where eid=" & Session("EID") & " and uid=d." & Ufld & ")[UserName],m." & Vfld & "[VehicleNo] from mmm_mst_Doc d inner join mmm_mst_master m on m.tid=d." & UVfld & " inner join mmm_mst_gpsdata g on g.imieno=m." & vemei & " where d.eid=" & Session("EID") & " and  d.documenttype='" & Udtype & "'  and imieno is not null) as table1 order by username  "
            ds.Clear()

            oda.SelectCommand.CommandTimeout = 180
            UsrVeh.Items().Clear()
            oda.Fill(ds, "vemei")
            con.Dispose()
            For i As Integer = 0 To ds.Tables("vemei").Rows.Count - 1

                UsrVeh.Items.Add(ds.Tables("vemei").Rows(i).Item(1).ToString() & "-" & ds.Tables("vemei").Rows(i).Item(2).ToString())
                UsrVeh.Items(i).Value = ds.Tables("vemei").Rows(i).Item(0).ToString()

            Next
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
    End Sub
    Protected Sub filteronCircle(ByRef id As String)
        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim con As SqlConnection = New SqlConnection(conStr)
        Dim oda As SqlDataAdapter = New SqlDataAdapter("", con)
        Dim dt As New DataTable
        Try
            City.Items.Clear()
            If Session("EID") = 32 Then
                oda.SelectCommand.CommandText = "select  distinct m1.tid,m1.fld1 from mmm_mst_master m with (nolock) inner join mmm_mst_master m1  with (nolock) on convert(nvarchar,m.tid)=m1.fld10 where m.eid=" & Session("EID") & " and m.documenttype='State' and m1.documenttype='City' and m.tid in (" & id & ") order by m1.fld1 "
            Else
                oda.SelectCommand.CommandText = "select  distinct m1.tid,m1.fld1 from mmm_mst_master m with (nolock) inner join mmm_mst_master m1 with (nolock) on convert(nvarchar,m.tid)=m1.fld11 where m.eid=54 and m.documenttype='Circle' and m1.documenttype='cluster' and m.tid in (" & id & ") order by m1.fld1 "
            End If
            oda.Fill(dt)
            For i As Integer = 0 To dt.Rows.Count - 1
                City.Items.Add(Convert.ToString(dt.Rows(i).Item("fld1")))
                City.Items(i).Value = dt.Rows(i).Item("tid").ToString
            Next

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
    End Sub
    Protected Sub UsrVeh_SelectedIndexChanged(sender As Object, e As EventArgs) Handles UsrVeh.SelectedIndexChanged
        Dim str As String = ""
        If UsrVeh.SelectedIndex < 0 Then
        Else
            imeino = UsrVeh.SelectedItem.Value
            ViewState("imei") = imeino
            If UsrVeh.SelectedItem.Text.Contains("-") Then
                Dim vehiclename As String = UsrVeh.SelectedItem.Text
                Dim arg() As String = vehiclename.Split(New Char() {"-"})
                If arg.Length > 0 Then
                    For index As Integer = 0 To arg.Length - 1
                        If index = 0 Then

                        Else
                            str = str + arg(index) & "-"
                        End If
                    Next
                    hdvehcleName.Value = str.Remove(str.Length - 1)
                    ViewState("VehicleName") = hdvehcleName.Value
                End If

            End If
            Dim numSelected As Integer = 0

            For Each li As ListItem In UsrVeh.Items
                If li.Selected = True Then
                    numSelected = numSelected + 1
                End If
            Next
            If numSelected > 1 Then
                For Each li As ListItem In UsrVeh.Items
                    li.Selected = False
                Next
                lblerror.Text = "Please Select Only One User At a Time "
            Else
                lblerror.Text = ""
            End If

        End If

    End Sub
    Protected Sub FilterUser(sender As Object, e As System.EventArgs) Handles City.SelectedIndexChanged
        Dim id As String = ""
        For i As Integer = 0 To City.Items.Count - 1
            If City.Items(i).Selected = True Then
                id = id & City.Items(i).Value & ","
            End If
        Next
        UsrVeh.Items.Clear()
        If id.Length > 0 Then
            id = Left(id, id.Length - 1)
            hdcity.Value = id
            FilterUserOnCity(id)
        End If
    End Sub


    Public Shared strPathJson As String = HttpContext.Current.Server.MapPath("DOCS/Jasontext.json")

    <WebMethod()> _
    <Script.Services.ScriptMethod()> _
    Public Shared Function GetMarkerListJSON() As String
        Dim strList As String = ""
        Dim d As String = IO.File.ReadAllText(strPathJson)
        Return d
    End Function

End Class
Public Class GeoLocation
    Dim latitude As String
    Dim longitude As String
    Dim ctime As String
    Dim speed As String
    Dim Vhname As String
    Dim ime As String

    Public Property VehiName As String
        Get
            Return Vhname
        End Get
        Set(value As String)
            Vhname = value
        End Set
    End Property
    Public Property IMEINUMBER() As String
        Get
            Return ime
        End Get
        Set(ByVal value As String)
            ime = value
        End Set
    End Property


    Public Property Lat() As String
        Get
            Return latitude
        End Get
        Set(ByVal value As String)
            latitude = value
        End Set
    End Property
    Public Property Longit() As String
        Get
            Return longitude
        End Get
        Set(ByVal value As String)
            longitude = value
        End Set
    End Property
    Public Property Speeds() As String
        Get
            Return speed
        End Get
        Set(value As String)
            speed = value
        End Set
    End Property
    Public Property Ctimeing() As String
        Get
            Return ctime
        End Get
        Set(value As String)
            ctime = value
        End Set
    End Property

End Class
