Imports System.Data
Imports System.Data.SqlClient

Partial Class PhoneReport
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(sender As Object, e As System.EventArgs) Handles Me.Load

        Dim conStr As [String] = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim con As New SqlConnection(conStr)
        Dim da1 As New SqlDataAdapter("select distinct IMIENO from [DMS].[MMM_MST_GPSDATA] where DevType='PHONE'", con)
        Dim ds As New DataSet()
        da1.Fill(ds)

        If Not IsPostBack Then

            For Each dr As DataRow In ds.Tables(0).Rows

                DropDownList1.Items.Add(dr(0).ToString())


            Next
        End If

        con.Dispose()
        da1.Dispose()
        ds.Dispose()
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
        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim con As New SqlConnection(conStr)
        Dim apikey As String = String.Empty
        Dim oda As SqlDataAdapter = New SqlDataAdapter("", con)
        '  oda.SelectCommand.CommandText = "select * from mmm_mst_entity where eid=" & Session("EID").ToString() & " "
        oda.SelectCommand.CommandText = "select * from mmm_mst_entity where eid=32"

        Dim ds As New DataSet()
        oda.Fill(ds, "data")
        If ds.Tables("data").Rows.Count > 0 Then
            apikey = ds.Tables("data").Rows(0).Item("APIkey").ToString
        End If

        Dim r As String = "select lattitude,longitude,ctime,speed,recordTime from MMM_MST_GPSDATA where tid in (select max(tid) from MMM_MST_GPSDATA where IMIENO='" & DropDownList1.SelectedItem.Value & "'  and   cTime >= '" + Date1.Text + " " + TextBox1.Text + "' AND ctime <= '" + Date2.Text + " " + TextBox2.Text + "' group by lattitude,longitude) order by ctime "
        oda.SelectCommand.CommandText = r
        oda.Fill(ds, "table1")
        oda.SelectCommand.CommandText = "select max(speed) as speed from MMM_MST_GPSDATA where tid in (select max(tid) from MMM_MST_GPSDATA where IMIENO='" & DropDownList1.SelectedItem.Value & "'  and   cTime >= '" + Date1.Text + " " + TextBox1.Text + "' AND ctime <= '" + Date2.Text + " " + TextBox2.Text + "' group by lattitude,longitude) "
        oda.Fill(ds, "speedtable")


        oda.SelectCommand.CommandText = "select sum(DistAnceTravel) as DistAnceTravel ,sum(DevDist) as DevDist ,sum(DistAnother) as DistAnother from MMM_MST_GPSDATA where tid in (select max(tid) from MMM_MST_GPSDATA where IMIENO='" & DropDownList1.SelectedItem.Value & "'  and   cTime >= '" + Date1.Text + " " + TextBox1.Text + "' AND ctime <= '" + Date2.Text + " " + TextBox2.Text + "' group by lattitude,longitude) "
        oda.Fill(ds, "table3")

        Label1.Text = ds.Tables("speedtable").Rows(0).Item("speed")
        Label2.Text = ds.Tables("table3").Rows(0).Item("DistAnceTravel")
        Label3.Text = ds.Tables("table3").Rows(0).Item("DevDist")
        Label4.Text = ds.Tables("table3").Rows(0).Item("DistAnother")
        Dim centerUser As String = String.Empty
        Dim JLineCoOrdinate As String = ""

        Dim inii As String

        inii = ds.Tables("table1").Rows.Count.ToString


        'For Each dr As DataRow In ds.Tables("table1").Rows

        '    JLineCoOrdinate = JLineCoOrdinate & "['Speed " & dr("speed").ToString() & " Km / Hr <br>RecordTime " + dr("recordTime").ToString() + "<br>Ctime " + dr("ctime").ToString() + " '," & dr("lattitude").ToString() & "," & dr("longitude").ToString() & ",],"

        'Next
        Dim icon As String = ""
        For i As Integer = 0 To ds.Tables("table1").Rows.Count - 1
            If i = 0 Or i = ds.Tables("table1").Rows.Count - 1 Then
                icon = ""
            Else
                icon = " images/car.png"
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


        Dim JQMAP As String = " " + url + " <script>     function initialize() { var locations = [ " + JLineCoOrdinate + " ]; var map = new google.maps.Map(document.getElementById('map'), { zoom: 12,center: new google.maps.LatLng(" & ds.Tables("table1").Rows(cLocation).Item("lattitude").ToString() & "," & ds.Tables("table1").Rows(cLocation).Item("longitude").ToString() & "),mapTypeId: google.maps.MapTypeId.ROADMAP  }); var infowindow = new google.maps.InfoWindow();var lineCoordinates = []; for (i = 0; i < locations.length; i++) { lineCoordinates.push(new google.maps.LatLng(locations[i][1], locations[i][2])); }    var FrPath = new google.maps.Polyline({  path: lineCoordinates,strokeColor: 'black'  });FrPath.setMap(map);for (i = 0; i < locations.length; i++) { marker = new google.maps.Marker({ position: new google.maps.LatLng(locations[i][1], locations[i][2 ]),map: map,icon:  locations[i][3], size: new google.maps.Size(1 , 1)});google.maps.event.addListener(marker, 'click', (function (marker, i) { return function () { infowindow.setContent(locations[i][0]);infowindow.open(map, marker);}})(marker, i));}} google.maps.event.addDomListener(window, 'load', initialize);</script>"
        Page.ClientScript.RegisterStartupScript(Me.GetType(), "", JQMAP)

        oda.Dispose()
        ds.Dispose()
        con.Dispose()
    End Sub

End Class
