Imports System.Data.SqlClient

Imports System.Data

Imports System.Configuration
Partial Class OnMap
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(sender As Object, e As System.EventArgs) Handles Me.Load

        If Not IsPostBack Then
            Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
            Dim con As New SqlConnection(conStr)







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

            oda.SelectCommand.CommandText = "select distinct mmm_mst_user.UserName,mmm_mst_master." & Vfld & ",mmm_mst_master." & vemei & " from mmm_mst_master,mmm_mst_doc,mmm_mst_user where mmm_mst_master.eid=32 and mmm_mst_master." & vemei & "=" & Session("IMEINO").ToString() & "  and mmm_mst_doc.ouid=mmm_mst_user.uid and  mmm_mst_master.documenttype='Vehicle' and mmm_mst_doc.ouid=" & Session("UID") & " and mmm_mst_master.tid=(select " & UVfld & " from mmm_mst_doc where documenttype='" & Udtype & "' and " & Ufld & " =  " & Session("UID") & " and curstatus='Allotted')"


            ds.Clear()
            oda.Fill(ds, "vemei")
            If ds.Tables("vemei").Rows.Count > 0 Then


                DropDownList1.Items.Add(ds.Tables("vemei").Rows(0).Item(0).ToString & " " & ds.Tables("vemei").Rows(0).Item(1).ToString)

                DropDownList1.Items(0).Value = ds.Tables("vemei").Rows(0).Item(2).ToString

            End If


            Dim r As String = "select lattitude,longitude,ctime,speed,distancetravel,recordTime from MMM_MST_GPSDATA where tid in (select max(tid) from MMM_MST_GPSDATA where IMIENO='" & Session("IMEINO").ToString() & "'  and   cTime >= '" + Request.QueryString("Start") + "' AND ctime <= '" + Request.QueryString("End") + "' group by lattitude,longitude) order by ctime  "

            Dim da1 As New SqlDataAdapter(r, con)


            Dim MaxSpeed As New SqlDataAdapter("select max(speed) as speed from MMM_MST_GPSDATA where tid in (select max(tid) from MMM_MST_GPSDATA where IMIENO='" & Session("IMEINO").ToString() & "'  and   cTime >= '" + Request.QueryString("Start") + "' AND ctime <= '" + Request.QueryString("End") + "' group by lattitude,longitude) ", con)
            Dim ds1 As DataSet = New DataSet()
            da1.Fill(ds1, "table1")
            MaxSpeed.Fill(ds1, "table2")

            Dim Distance As New SqlDataAdapter("select sum(DistAnceTravel) as DistAnceTravel ,sum(DevDist) as DevDist ,sum(DistAnother) as DistAnother  from MMM_MST_GPSDATA where tid in (select max(tid) from MMM_MST_GPSDATA where IMIENO='" & Session("IMEINO").ToString() & "'  and   cTime >= '" + Request.QueryString("Start") + "' AND ctime <= '" + Request.QueryString("End") + "' group by lattitude,longitude) ", con)
            Distance.Fill(ds1, "table3")

            Label1.Text = ds1.Tables(1).Rows(0).Item("speed")
            Label2.Text = ds1.Tables(2).Rows(0).Item("DistAnceTravel")
            Label3.Text = ds1.Tables(2).Rows(0).Item("DevDist")
            Label4.Text = ds1.Tables(2).Rows(0).Item("DistAnother")
            Dim centerUser As String
            Dim JLineCoOrdinate As String = ""


            For Each dr As DataRow In ds1.Tables(0).Rows
                JLineCoOrdinate = JLineCoOrdinate & "['Speed " & dr("speed").ToString() & " Km / Hr <br>" + dr("recordTime").ToString() + " '," & dr("lattitude").ToString() & "," & dr("longitude").ToString() & "],"


            Next

            Dim JQMAP As String = "<script>          function initialize() { var locations = [ " + JLineCoOrdinate + " ]; var map = new google.maps.Map(document.getElementById('map'), { zoom: 10,center: new google.maps.LatLng(28.5324936, 77.2512221),mapTypeId: google.maps.MapTypeId.ROADMAP  }); var infowindow = new google.maps.InfoWindow();var lineCoordinates = []; for (i = 0; i < locations.length; i++) { lineCoordinates.push(new google.maps.LatLng(locations[i][1], locations[i][2])); }    var FrPath = new google.maps.Polyline({  path: lineCoordinates,strokeColor: 'black'  });FrPath.setMap(map);for (i = 0; i < locations.length; i++) { marker = new google.maps.Marker({ position: new google.maps.LatLng(locations[i][1], locations[i][2 ]),map: map,icon:  'images/car.png', size: new google.maps.Size(1 , 1)});google.maps.event.addListener(marker, 'click', (function (marker, i) { return function () { infowindow.setContent(locations[i][0]);infowindow.open(map, marker);}})(marker, i));}} google.maps.event.addDomListener(window, 'load', initialize);</script>"


            Page.ClientScript.RegisterStartupScript(Me.GetType(), "", JQMAP)
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
    Protected Sub SEEMAP(ByVal sender As Object, ByVal e As System.EventArgs)
        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim con As New SqlConnection(conStr)


        Dim r As String = "select lattitude,longitude,ctime,speed,distancetravel,recordTime from MMM_MST_GPSDATA where tid in (select max(tid) from MMM_MST_GPSDATA where IMIENO='" & DropDownList1.SelectedValue & "'  and   cTime >= '" + Request.QueryString("Start") + "' AND ctime <= '" + Request.QueryString("End") + "' group by lattitude,longitude) order by ctime  "

        Dim da1 As New SqlDataAdapter(r, con)


        Dim MaxSpeed As New SqlDataAdapter("select max(speed) as speed from MMM_MST_GPSDATA where tid in (select max(tid) from MMM_MST_GPSDATA where IMIENO='" & DropDownList1.SelectedValue & "'  and   cTime >= '" + Request.QueryString("Start") + "' AND ctime <= '" + Request.QueryString("End") + "' group by lattitude,longitude) ", con)
        Dim ds1 As DataSet = New DataSet()
        da1.Fill(ds1, "table1")
        MaxSpeed.Fill(ds1, "table2")

        Dim Distance As New SqlDataAdapter("select sum(DistAnceTravel) as DistAnceTravel ,sum(DevDist) as DevDist ,sum(DistAnother) as DistAnother  from MMM_MST_GPSDATA where tid in (select max(tid) from MMM_MST_GPSDATA where IMIENO='" & DropDownList1.SelectedValue & "'  and   cTime >= '" + Request.QueryString("Start") + "' AND ctime <= '" + Request.QueryString("End") + "' group by lattitude,longitude) ", con)
        Distance.Fill(ds1, "table3")

        Label1.Text = ds1.Tables(1).Rows(0).Item("speed")
        Label2.Text = ds1.Tables(2).Rows(0).Item("DistAnceTravel")
        Label3.Text = ds1.Tables(2).Rows(0).Item("DevDist")
        Label4.Text = ds1.Tables(2).Rows(0).Item("DistAnother")
        Dim centerUser As String
        Dim JLineCoOrdinate As String = ""


        For Each dr As DataRow In ds1.Tables(0).Rows
            JLineCoOrdinate = JLineCoOrdinate & "['Speed " & dr("speed").ToString() & " Km / Hr <br>" + dr("recordTime").ToString() + " '," & dr("lattitude").ToString() & "," & dr("longitude").ToString() & "],"


        Next

        Dim JQMAP As String = "<script>          function initialize() { var locations = [ " + JLineCoOrdinate + " ]; var map = new google.maps.Map(document.getElementById('map'), { zoom: 10,center: new google.maps.LatLng(28.5324936, 77.2512221),mapTypeId: google.maps.MapTypeId.ROADMAP  }); var infowindow = new google.maps.InfoWindow();var lineCoordinates = []; for (i = 0; i < locations.length; i++) { lineCoordinates.push(new google.maps.LatLng(locations[i][1], locations[i][2])); }    var FrPath = new google.maps.Polyline({  path: lineCoordinates,strokeColor: 'black'  });FrPath.setMap(map);for (i = 0; i < locations.length; i++) { marker = new google.maps.Marker({ position: new google.maps.LatLng(locations[i][1], locations[i][2 ]),map: map,icon:  'images/car.png', size: new google.maps.Size(1 , 1)});google.maps.event.addListener(marker, 'click', (function (marker, i) { return function () { infowindow.setContent(locations[i][0]);infowindow.open(map, marker);}})(marker, i));}} google.maps.event.addDomListener(window, 'load', initialize);</script>"


        Page.ClientScript.RegisterStartupScript(Me.GetType(), "", JQMAP)


        con.Dispose()

    End Sub

End Class
