Imports System.Data
Imports System.Data.SqlClient

Partial Class GoogleMap
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        If Not IsPostBack Then
            lblLoginName.Text = "Logined User : " & Session("USERNAME").ToString()
            Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
            Dim con As SqlConnection = New SqlConnection(conStr)
            Dim oda1 As SqlDataAdapter = New SqlDataAdapter("select * from MMM_MST_LAT_USER", con)
            Dim dt1 As New DataTable
            oda1.Fill(dt1)
            For i As Integer = 0 To dt1.Rows.Count - 1
                DropDownList1.Items.Add(dt1.Rows(i).Item("UserName").ToString())
                DropDownList1.Items(i).Value = dt1.Rows(i).Item("UserID").ToString()
            Next
        End If
    End Sub

    Private Sub RenderLocationOnMap()
        If Date1.Text.Length <> 10 Or TextBox1.Text.Length <> 5 Then
            lblError.Text = "Please Select Date And Time in Defined Format"
            Exit Sub
        Else
            lblError.Text = " "
        End If

        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim con As SqlConnection = New SqlConnection(conStr)
        Dim oda As SqlDataAdapter = New SqlDataAdapter("select U.userID,Username,phonenumber,longitude,latitude,run_time,cur_date from MMM_MST_LATITUDE L LEFT OUTER JOIN  MMM_MST_LAT_USER U on U.tid=L.uid WHERE run_time is not null and l.tid in (Select max(tid) from MMM_MST_LATITUDE group by UID)", con)

        If Date1.Text.ToString() = "" And TextBox1.Text.ToString() = "" Then
        Else
            Dim s As String = "select U.userID,Username,phonenumber,longitude,latitude,run_time,cur_date from MMM_MST_LATITUDE L LEFT OUTER JOIN  MMM_MST_LAT_USER U on U.tid=L.uid WHERE run_time is not null and l.tid in (Select max(tid) from MMM_MST_LATITUDE WHERE run_time < '" & Date1.Text.ToString() & " " & TextBox1.Text & "' group by UID) "
            oda.SelectCommand.CommandText = s
        End If

        Dim dt As New DataTable
        oda.Fill(dt)
        Dim JQuertStr As String = "<script>var locations = [ "
        Dim centerUser As String = ""
        For i As Integer = 0 To dt.Rows.Count - 1
            If Session("UID").ToString() = dt.Rows(i).Item("userid").ToString() Then
                JQuertStr &= "['" & dt.Rows(i).Item("username").ToString() & "<br/>Phone : " & dt.Rows(i).Item("phonenumber").ToString() & "<br/>Time : " & dt.Rows(i).Item("cur_date").ToString() & "<br /> Latitude : " & dt.Rows(i).Item("latitude").ToString() & ", Longitude : " & dt.Rows(i).Item("longitude").ToString() & "'," & dt.Rows(i).Item("latitude") & "," & dt.Rows(i).Item("longitude") & ",'images/me.png'],"
                centerUser = dt.Rows(i).Item("latitude") & "," & dt.Rows(i).Item("longitude")
            Else
                JQuertStr &= "['" & dt.Rows(i).Item("username").ToString() & "<br/>Phone : " & dt.Rows(i).Item("phonenumber").ToString() & "<br/>Time : " & dt.Rows(i).Item("cur_date").ToString() & "<br /> Latitude : " & dt.Rows(i).Item("latitude").ToString() & ", Longitude : " & dt.Rows(i).Item("longitude").ToString() & "'," & dt.Rows(i).Item("latitude") & "," & dt.Rows(i).Item("longitude") & ",'images/staff.png'],"
                'JQuertStr &= "['" & dt.Rows(i).Item("username").ToString() & "<br/>Phone : " & dt.Rows(i).Item("phonenumber").ToString() & "<br/>Time : " & dt.Rows(i).Item("cur_date").ToString() & "'," & dt.Rows(i).Item("latitude") & "," & dt.Rows(i).Item("longitude") & ",'images/staff.png'],"

            End If
        Next
        JQuertStr = Left(JQuertStr, Len(JQuertStr) - 1)
        JQuertStr &= "];"
        JQuertStr &= " var map = new google.maps.Map(document.getElementById('map'), { zoom: 14, center: new google.maps.LatLng(" & centerUser & "),  mapTypeId: google.maps.MapTypeId.ROADMAP   }); var infowindow = new google.maps.InfoWindow();  var marker, i;  for (i = 0; i < locations.length; i++) { marker = new google.maps.Marker({ position: new google.maps.LatLng(locations[i][1], locations[i][2]), map: map,icon : locations[i][3]});  google.maps.event.addListener(marker, 'click', (function (marker, i) { return function () { infowindow.setContent(locations[i][0]);  infowindow.open(map, marker); } })(marker, i)); }</script>"

        'Page.ClientScript.re()
        Dim page As Page = HttpContext.Current.Handler
        page.ClientScript.RegisterStartupScript(Me.GetType(), "", JQuertStr)
    End Sub

    Protected Sub btnJustMe_Click(sender As Object, e As EventArgs) Handles btnJustMe.Click
        If Date1.Text.Length <> 10 Then
            lblError.Text = "Please Select Date"
            Exit Sub
        End If

        Dim abcArr() As String = Date1.Text.Split("-")
        Dim abcDate As String = abcArr(2) & "-" & abcArr(1) & "-" & Right(abcArr(0), 2)
        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim con As SqlConnection = New SqlConnection(conStr)
        Dim oda As SqlDataAdapter = New SqlDataAdapter("select distinct U.userID,Username,phonenumber,longitude,latitude,cur_date,userimage from MMM_MST_LATITUDE L LEFT OUTER JOIN  MMM_MST_LAT_USER U on U.tid=L.uid WHERE convert(nvarchar(12),run_time,5) = '" & abcDate & "'  and userid=" & DropDownList1.SelectedItem.Value, con)
        Dim dt As New DataTable
        oda.Fill(dt)

        Dim JQuertStr As String = "<script>var locations = [ "
        Dim centerUser As String = ""
        For i As Integer = 0 To dt.Rows.Count - 1
            centerUser = dt.Rows(i).Item("latitude") & "," & dt.Rows(i).Item("longitude")
            '            JQuertStr &= "['" & dt.Rows(i).Item("username").ToString() & "<br/>Phone : " & dt.Rows(i).Item("phonenumber").ToString() & "<br/>Time : " & dt.Rows(i).Item("cur_date").ToString() & "'," & dt.Rows(i).Item("latitude") & "," & dt.Rows(i).Item("longitude") & ",'images/staff.png'],"
            JQuertStr &= "['<b>" & dt.Rows(i).Item("username").ToString() & "</b> - " & dt.Rows(i).Item("phonenumber").ToString() & "<br/>Time : " & dt.Rows(i).Item("cur_date").ToString() & "<br /> Latitude : " & dt.Rows(i).Item("latitude").ToString() & " <br /> Longitude : " & dt.Rows(i).Item("longitude").ToString() & "'," & dt.Rows(i).Item("latitude") & "," & dt.Rows(i).Item("longitude") & ",'images/me.png'],"
        Next
        JQuertStr = Left(JQuertStr, Len(JQuertStr) - 1)
        JQuertStr &= "];"
        JQuertStr &= " var map = new google.maps.Map(document.getElementById('map'), { zoom: 11, center: new google.maps.LatLng(" & centerUser & "),  mapTypeId: google.maps.MapTypeId.ROADMAP   }); var infowindow = new google.maps.InfoWindow();  var marker, i;  for (i = 0; i < locations.length; i++) { marker = new google.maps.Marker({ position: new google.maps.LatLng(locations[i][1], locations[i][2]), map: map,icon : locations[i][3]});  google.maps.event.addListener(marker, 'click', (function (marker, i) { return function () { infowindow.setContent(locations[i][0]);  infowindow.open(map, marker); } })(marker, i)); }</script>"

        'Page.ClientScript.re()
        Dim page As Page = HttpContext.Current.Handler
        page.ClientScript.RegisterStartupScript(Me.GetType(), "", JQuertStr)
    End Sub

    Protected Sub btnAll_Click(sender As Object, e As EventArgs) Handles btnAll.Click
        RenderLocationOnMap()
    End Sub

End Class
