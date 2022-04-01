Imports System.Data
Imports System.Data.SqlClient
'Imports System.Web.UI.DataVisualization.Charting.SeriesChartType
'Imports System.Web.UI.DataVisualization.Charting
Imports System.IO
Imports System.Web.UI.Adapters.ControlAdapter
Imports iTextSharp.text
Imports System.Drawing
Imports System.Threading
Imports System.Net.Mail
Imports System.Net
Imports System.Net.HttpWebRequest
Imports System.Net.HttpWebResponse
Imports System
Imports System.Collections.Specialized
Imports System.Text
Imports System.Xml
Imports System.Collections.Generic
Imports System.Net.Security
Partial Class TripReport
    Inherits System.Web.UI.Page
    Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
    Protected Sub Page_Load(sender As Object, e As System.EventArgs) Handles Me.Load
        If Not IsPostBack Then
        End If
    End Sub
    Protected Sub btsSearch_Click(sender As Object, e As System.EventArgs) Handles btsSearch.Click
        TripCreateFromSwitch()
    End Sub
    Protected currentPageNumber As Integer = 1
    Protected currentPageNumberu As Integer = 1
    Protected currentPageNumberp As Integer = 1
    Public Sub TripCreateFromSwitch()

        Dim con As New SqlConnection(constr)
        Dim oda As SqlDataAdapter = New SqlDataAdapter("", con)
        Dim ds As New DataSet()
        Dim dt As New DataTable
        Dim dt1 As New DataTable
        lblmsg.Text = ""

        Try
            'oda.SelectCommand.CommandText = "select count(*) from MMM_MST_ELOGBOOK where convert(date,trip_start_datetime)=convert(nvarchar,getdate()-1,23) and triptype='switch'"
            'If con.State <> ConnectionState.Open Then
            '    con.Open()
            'End If
            'Dim cnt As Integer = oda.SelectCommand.ExecuteScalar()
            Dim cnt As Integer = 0
            If cnt = 0 Then
                If txtsdate.Text = "" Then
                    lblmsg.Text = "Please select Date."
                    Exit Sub
                End If
                oda.SelectCommand.CommandText = "select distinct imieno,CONVERT(nvarchar,ctime,23)[cdate] from mmm_mst_gpsdata where tripon=1 and convert(nvarchar,ctime,23)='" & txtsdate.Text & "' group by imieno,CONVERT(nvarchar,ctime,23)"
                oda.Fill(dt)
                For i As Integer = 0 To dt.Rows.Count - 1
                    oda.SelectCommand.CommandText = "CreateTripfromSwitchNew"
                    oda.SelectCommand.CommandType = CommandType.StoredProcedure
                    oda.SelectCommand.Parameters.Clear()
                    oda.SelectCommand.Parameters.AddWithValue("@imieno", dt.Rows(i).Item("imieno").ToString)
                    oda.SelectCommand.Parameters.AddWithValue("Date", dt.Rows(i).Item("cdate").ToString())
                    If con.State <> ConnectionState.Open Then
                        con.Open()
                    End If
                    oda.SelectCommand.CommandTimeout = 3600
                    oda.SelectCommand.ExecuteNonQuery()
                    oda.SelectCommand.CommandType = CommandType.Text
                    oda.SelectCommand.CommandText = "select tid,start_latitude,start_longitude,end_latitude,end_longitude from mmm_mst_elogbook where imei_no='" & dt.Rows(i).Item("imieno").ToString & "' and Trip_start_location is null and Trip_end_location is null and convert(nvarchar,trip_start_datetime,23)='" & dt.Rows(i).Item("cdate").ToString & "' and convert(nvarchar,trip_end_datetime,23)='" & dt.Rows(i).Item("cdate").ToString & "'"
                    oda.Fill(dt1)
                    For j As Integer = 0 To dt1.Rows.Count - 1
                        oda.SelectCommand.CommandText = "USPUpdateTripLocation"
                        oda.SelectCommand.CommandType = CommandType.StoredProcedure
                        oda.SelectCommand.Parameters.Clear()
                        oda.SelectCommand.Parameters.AddWithValue("@tid", dt1.Rows(j).Item("tid").ToString)
                        oda.SelectCommand.Parameters.AddWithValue("@stlocation", Location(dt1.Rows(j).Item("start_latitude"), dt1.Rows(j).Item("start_longitude")))
                        oda.SelectCommand.Parameters.AddWithValue("@edlocation", Location(dt1.Rows(j).Item("end_latitude"), dt1.Rows(j).Item("end_longitude")))
                        oda.SelectCommand.CommandTimeout = 1800
                        oda.SelectCommand.ExecuteNonQuery()
                    Next
                Next
                oda.SelectCommand.CommandType = CommandType.Text
                'oda.SelectCommand.CommandText = "insert into mmm_schedulerlog_Switch values ('Window Service END Trip Created By Switch',getdate())"
                'If con.State <> ConnectionState.Open Then
                '    con.Open()
                'End If
                'oda.SelectCommand.ExecuteNonQuery()
                lblmsg.Text = "Trip Created Successfully"
            End If
        Catch ex As Exception
            oda.SelectCommand.CommandType = CommandType.Text
            ' oda.SelectCommand.CommandText = "insert into mmm_schedulerlog_Switch values ('" & ex.ToString & "',getdate())"
            'If con.State <> ConnectionState.Open Then
            '    con.Open()
            'End If
            'oda.SelectCommand.ExecuteNonQuery()
            lblmsg.Text = "Please Try Again..."
            con.Dispose()
            con.Close()
            oda.Dispose()
        Finally
            con.Dispose()
            con.Close()
            oda.Dispose()
        End Try

    End Sub
    Public Function Location(lat As String, log As String) As String
        Dim con As New SqlConnection(conStr)
        Dim oda As SqlDataAdapter = New SqlDataAdapter("", con)
        oda.SelectCommand.CommandText = "select top 1 * from gpsLocation where Lat_start  <=" + lat + " and  lat_end >= " + lat + " and long_start <= " + log + " and long_end >= " + log + " "
        Dim locatoinr As DataTable = New DataTable()
        oda.Fill(locatoinr)
        If locatoinr.Rows.Count > 0 Then
            Return locatoinr.Rows(0).Item(1).ToString
        Else
            Dim url As String = "http://maps.googleapis.com/maps/api/geocode/xml?latlng=" & lat & "," & log & "&sensor=false"
            Dim request As HttpWebRequest = DirectCast(WebRequest.Create(url), HttpWebRequest)
            Dim response As WebResponse = request.GetResponse()
            Dim dataStream As Stream = response.GetResponseStream()
            Dim sreader As New StreamReader(dataStream)
            Dim responsereader As String = sreader.ReadToEnd()
            response.Close()
            Dim xmldoc As New XmlDocument()
            xmldoc.LoadXml(responsereader)
            If xmldoc.GetElementsByTagName("status")(0).ChildNodes(0).InnerText = "OK" Then
                oda.SelectCommand.CommandText = "gpsinsertlocation"
                oda.SelectCommand.CommandType = CommandType.StoredProcedure
                oda.SelectCommand.Parameters.AddWithValue("complete_latitude", lat)
                oda.SelectCommand.Parameters.AddWithValue("complete_longitude", log)
                oda.SelectCommand.Parameters.AddWithValue("Lat_start", Convert.ToDouble(lat.Substring(0, 5)))
                oda.SelectCommand.Parameters.AddWithValue("lat_end", Convert.ToDouble(lat.Substring(0, 5)) + 0.01)
                oda.SelectCommand.Parameters.AddWithValue("long_start", Convert.ToDouble(log.Substring(0, 5)))
                oda.SelectCommand.Parameters.AddWithValue("long_end", Convert.ToDouble(log.Substring(0, 5)) + 0.01)
                Dim fulladdress As String = String.Empty
                Try
                    If xmldoc.ChildNodes.Count > 0 Then
                        Dim SelNodesTxt As String = xmldoc.DocumentElement.Name
                        Dim Cnt As Integer = 0
                        Dim nodes As XmlNodeList = xmldoc.SelectNodes(SelNodesTxt)

                        Dim other As Int32 = 0
                        For Each node As XmlNode In nodes
                            For c As Integer = 0 To node.ChildNodes.Count - 1
                                If node.ChildNodes(c).Name = "result" Then
                                    Cnt += 1
                                    For c2 As Integer = 0 To node.ChildNodes.Item(c).ChildNodes.Count - 1
                                        If node.ChildNodes.Item(c).ChildNodes.Item(c2).Name = "address_component" Then
                                            For j As Integer = 0 To node.ChildNodes.Item(c).ChildNodes.Item(c2).ChildNodes.Count - 1
                                                If node.ChildNodes.Item(c).ChildNodes.Item(c2).ChildNodes.Count = 2 And other = 0 Then
                                                    oda.SelectCommand.Parameters.AddWithValue("other", node.ChildNodes.Item(c).ChildNodes.Item(c2).ChildNodes.Item(j).InnerText)
                                                    other = 1
                                                End If
                                                If node.ChildNodes.Item(c).ChildNodes.Item(c2).ChildNodes.Item(j).Name = "type" Then
                                                    If node.ChildNodes.Item(c).ChildNodes.Item(c2).ChildNodes.Item(j).InnerXml = "street_address" Then
                                                        oda.SelectCommand.Parameters.AddWithValue("street_address", node.ChildNodes.Item(c).ChildNodes.Item(c2).ChildNodes.Item(j - 2).InnerText) '''local address
                                                    End If

                                                    If node.ChildNodes.Item(c).ChildNodes.Item(c2).ChildNodes.Item(j).InnerXml = "floor" Then
                                                        oda.SelectCommand.Parameters.AddWithValue("floor", node.ChildNodes.Item(c).ChildNodes.Item(c2).ChildNodes.Item(j - 2).InnerText) '''local address
                                                    End If
                                                    If node.ChildNodes.Item(c).ChildNodes.Item(c2).ChildNodes.Item(j).InnerXml = "parking" Then
                                                        oda.SelectCommand.Parameters.AddWithValue("parking", node.ChildNodes.Item(c).ChildNodes.Item(c2).ChildNodes.Item(j - 2).InnerText) '''local address
                                                    End If
                                                    If node.ChildNodes.Item(c).ChildNodes.Item(c2).ChildNodes.Item(j).InnerXml = "post_box" Then
                                                        oda.SelectCommand.Parameters.AddWithValue("post_box", node.ChildNodes.Item(c).ChildNodes.Item(c2).ChildNodes.Item(j - 2).InnerText) '''local address
                                                    End If
                                                    If node.ChildNodes.Item(c).ChildNodes.Item(c2).ChildNodes.Item(j).InnerXml = "postal_town" Then
                                                        oda.SelectCommand.Parameters.AddWithValue("postal_town", node.ChildNodes.Item(c).ChildNodes.Item(c2).ChildNodes.Item(j - 2).InnerText) '''local address
                                                    End If
                                                    If node.ChildNodes.Item(c).ChildNodes.Item(c2).ChildNodes.Item(j).InnerXml = "room" Then
                                                        oda.SelectCommand.Parameters.AddWithValue("room", node.ChildNodes.Item(c).ChildNodes.Item(c2).ChildNodes.Item(j - 2).InnerText) '''local address
                                                    End If
                                                    If node.ChildNodes.Item(c).ChildNodes.Item(c2).ChildNodes.Item(j).InnerXml = "train_station" Then
                                                        oda.SelectCommand.Parameters.AddWithValue("train_station", node.ChildNodes.Item(c).ChildNodes.Item(c2).ChildNodes.Item(j - 2).InnerText) '''local address
                                                    End If
                                                    If node.ChildNodes.Item(c).ChildNodes.Item(c2).ChildNodes.Item(j).InnerXml = "establishment" Then
                                                        oda.SelectCommand.Parameters.AddWithValue("establishment_address", node.ChildNodes.Item(c).ChildNodes.Item(c2).ChildNodes.Item(j - 2).InnerText) '''local address
                                                    End If
                                                    If node.ChildNodes.Item(c).ChildNodes.Item(c2).ChildNodes.Item(j).InnerXml = "street_number" Then
                                                        oda.SelectCommand.Parameters.AddWithValue("street_number", node.ChildNodes.Item(c).ChildNodes.Item(c2).ChildNodes.Item(j - 2).InnerText) '''local address
                                                    End If
                                                    If node.ChildNodes.Item(c).ChildNodes.Item(c2).ChildNodes.Item(j).InnerXml = "bus_station" Then
                                                        oda.SelectCommand.Parameters.AddWithValue("stationaddress", node.ChildNodes.Item(c).ChildNodes.Item(c2).ChildNodes.Item(j - 2).InnerText) '''local address
                                                    End If
                                                    If node.ChildNodes.Item(c).ChildNodes.Item(c2).ChildNodes.Item(j).InnerXml = "route" Then
                                                        oda.SelectCommand.Parameters.AddWithValue("rld", node.ChildNodes.Item(c).ChildNodes.Item(c2).ChildNodes.Item(j - 2).InnerText) '''local address
                                                    End If
                                                    If node.ChildNodes.Item(c).ChildNodes.Item(c2).ChildNodes.Item(j).InnerXml = "neighborhood" Then
                                                        oda.SelectCommand.Parameters.AddWithValue("npa", node.ChildNodes.Item(c).ChildNodes.Item(c2).ChildNodes.Item(j - 2).InnerText)  ''neighborhood address
                                                    End If
                                                    If node.ChildNodes.Item(c).ChildNodes.Item(c2).ChildNodes.Item(j).InnerXml = "sublocality" Then
                                                        oda.SelectCommand.Parameters.AddWithValue("sublocalityaddress", node.ChildNodes.Item(c).ChildNodes.Item(c2).ChildNodes.Item(j - 2).InnerText)
                                                    End If
                                                    If node.ChildNodes.Item(c).ChildNodes.Item(c2).ChildNodes.Item(j).InnerXml = "locality" Then
                                                        oda.SelectCommand.Parameters.AddWithValue("locPaddre", node.ChildNodes.Item(c).ChildNodes.Item(c2).ChildNodes.Item(j - 2).InnerText)   ''locality
                                                    End If
                                                    If node.ChildNodes.Item(c).ChildNodes.Item(c2).ChildNodes.Item(j).InnerXml = "administrative_area_level_3" Then
                                                        oda.SelectCommand.Parameters.AddWithValue("admini3address", node.ChildNodes.Item(c).ChildNodes.Item(c2).ChildNodes.Item(j - 2).InnerText)   ''city
                                                    End If
                                                    If node.ChildNodes.Item(c).ChildNodes.Item(c2).ChildNodes.Item(j).InnerXml = "administrative_area_level_2" Then
                                                        oda.SelectCommand.Parameters.AddWithValue("adminpoladdress", node.ChildNodes.Item(c).ChildNodes.Item(c2).ChildNodes.Item(j - 2).InnerText)   ''city
                                                    End If
                                                    If node.ChildNodes.Item(c).ChildNodes.Item(c2).ChildNodes.Item(j).InnerXml = "administrative_area_level_1" Then
                                                        oda.SelectCommand.Parameters.AddWithValue("addressLongName", node.ChildNodes.Item(c).ChildNodes.Item(c2).ChildNodes.Item(j - 2).InnerText)   ''city
                                                        oda.SelectCommand.Parameters.AddWithValue("addShortName", node.ChildNodes.Item(c).ChildNodes.Item(c2).ChildNodes.Item(j - 1).InnerText)
                                                    End If
                                                    If node.ChildNodes.Item(c).ChildNodes.Item(c2).ChildNodes.Item(j).InnerXml = "country" Then
                                                        oda.SelectCommand.Parameters.AddWithValue("countryLong", node.ChildNodes.Item(c).ChildNodes.Item(c2).ChildNodes.Item(j - 2).InnerText)
                                                        oda.SelectCommand.Parameters.AddWithValue("countryShort", node.ChildNodes.Item(c).ChildNodes.Item(c2).ChildNodes.Item(j - 1).InnerText)
                                                    End If
                                                    If node.ChildNodes.Item(c).ChildNodes.Item(c2).ChildNodes.Item(j).InnerXml = "postal_code" Then
                                                        oda.SelectCommand.Parameters.AddWithValue("postalLong", node.ChildNodes.Item(c).ChildNodes.Item(c2).ChildNodes.Item(j - 2).InnerText)
                                                        oda.SelectCommand.Parameters.AddWithValue("postalShort", node.ChildNodes.Item(c).ChildNodes.Item(c2).ChildNodes.Item(j - 1).InnerText)
                                                    End If
                                                    If node.ChildNodes.Item(c).ChildNodes.Item(c2).ChildNodes.Item(j).InnerXml = "airport" Then
                                                        oda.SelectCommand.Parameters.AddWithValue("airport", node.ChildNodes.Item(c).ChildNodes.Item(c2).ChildNodes.Item(j - 2).InnerText) '''local address
                                                    End If
                                                    If node.ChildNodes.Item(c).ChildNodes.Item(c2).ChildNodes.Item(j).InnerXml = "point_of_interest" Then
                                                        oda.SelectCommand.Parameters.AddWithValue("point_of_interest", node.ChildNodes.Item(c).ChildNodes.Item(c2).ChildNodes.Item(j - 2).InnerText) '''local address
                                                    End If
                                                    If node.ChildNodes.Item(c).ChildNodes.Item(c2).ChildNodes.Item(j).InnerXml = "park" Then
                                                        oda.SelectCommand.Parameters.AddWithValue("park", node.ChildNodes.Item(c).ChildNodes.Item(c2).ChildNodes.Item(j - 2).InnerText) '''local address
                                                    End If
                                                    If node.ChildNodes.Item(c).ChildNodes.Item(c2).ChildNodes.Item(j).InnerXml = "intersection" Then
                                                        oda.SelectCommand.Parameters.AddWithValue("intersection", node.ChildNodes.Item(c).ChildNodes.Item(c2).ChildNodes.Item(j - 2).InnerText) '''local address
                                                    End If
                                                    If node.ChildNodes.Item(c).ChildNodes.Item(c2).ChildNodes.Item(j).InnerXml = "colloquial_area" Then
                                                        oda.SelectCommand.Parameters.AddWithValue("colloquial_area", node.ChildNodes.Item(c).ChildNodes.Item(c2).ChildNodes.Item(j - 2).InnerText) '''local address
                                                    End If
                                                    If node.ChildNodes.Item(c).ChildNodes.Item(c2).ChildNodes.Item(j).InnerXml = "premise" Then
                                                        oda.SelectCommand.Parameters.AddWithValue("premise", node.ChildNodes.Item(c).ChildNodes.Item(c2).ChildNodes.Item(j - 2).InnerText) '''local address
                                                    End If
                                                    If node.ChildNodes.Item(c).ChildNodes.Item(c2).ChildNodes.Item(j).InnerXml = "subpremise" Then
                                                        oda.SelectCommand.Parameters.AddWithValue("subpremise", node.ChildNodes.Item(c).ChildNodes.Item(c2).ChildNodes.Item(j - 2).InnerText) '''local address
                                                    End If
                                                End If
                                            Next
                                        ElseIf node.ChildNodes.Item(c).ChildNodes.Item(c2).Name = "formatted_address" Then
                                            fulladdress = node.ChildNodes.Item(c).ChildNodes.Item(c2).InnerText
                                            oda.SelectCommand.Parameters.AddWithValue("location_namer", node.ChildNodes.Item(c).ChildNodes.Item(c2).InnerText)
                                        ElseIf node.ChildNodes.Item(c).ChildNodes.Item(c2).Name = "geometry" Then
                                            For j As Integer = 0 To node.ChildNodes.Item(c).ChildNodes.Item(c2).ChildNodes.Count - 1
                                                If node.ChildNodes.Item(c).ChildNodes.Item(c2).ChildNodes.Item(j).Name = "location" Then
                                                    For k As Integer = 0 To node.ChildNodes.Item(c).ChildNodes.Item(c2).ChildNodes.Item(j).ChildNodes.Count - 1
                                                        If node.ChildNodes.Item(c).ChildNodes.Item(c2).ChildNodes.Item(j).ChildNodes.Item(k).Name = "lat" Then
                                                            oda.SelectCommand.Parameters.AddWithValue("geometrylat", node.ChildNodes.Item(c).ChildNodes.Item(c2).ChildNodes.Item(j).ChildNodes.Item(k).InnerText)
                                                        ElseIf node.ChildNodes.Item(c).ChildNodes.Item(c2).ChildNodes.Item(j).ChildNodes.Item(k).Name = "lng" Then
                                                            oda.SelectCommand.Parameters.AddWithValue("geometrylng", node.ChildNodes.Item(c).ChildNodes.Item(c2).ChildNodes.Item(j).ChildNodes.Item(k).InnerText)
                                                        End If
                                                    Next
                                                ElseIf node.ChildNodes.Item(c).ChildNodes.Item(c2).ChildNodes.Item(j).Name = "viewport" Then
                                                    For k As Integer = 0 To node.ChildNodes.Item(c).ChildNodes.Item(c2).ChildNodes.Item(j).ChildNodes.Count - 1
                                                        If node.ChildNodes.Item(c).ChildNodes.Item(c2).ChildNodes.Item(j).ChildNodes.Item(k).Name = "southwest" Then
                                                            For l As Integer = 0 To node.ChildNodes.Item(c).ChildNodes.Item(c2).ChildNodes.Item(j).ChildNodes.Item(k).ChildNodes.Count - 1
                                                                If node.ChildNodes.Item(c).ChildNodes.Item(c2).ChildNodes.Item(j).ChildNodes.Item(k).ChildNodes.Item(l).Name = "lat" Then
                                                                    oda.SelectCommand.Parameters.AddWithValue("vSWlat", node.ChildNodes.Item(c).ChildNodes.Item(c2).ChildNodes.Item(j).ChildNodes.Item(k).ChildNodes.Item(l).InnerText)
                                                                ElseIf node.ChildNodes.Item(c).ChildNodes.Item(c2).ChildNodes.Item(j).ChildNodes.Item(k).ChildNodes.Item(l).Name = "lng" Then
                                                                    oda.SelectCommand.Parameters.AddWithValue("vSWlng", node.ChildNodes.Item(c).ChildNodes.Item(c2).ChildNodes.Item(j).ChildNodes.Item(k).ChildNodes.Item(l).InnerText)
                                                                End If
                                                            Next
                                                        ElseIf node.ChildNodes.Item(c).ChildNodes.Item(c2).ChildNodes.Item(j).ChildNodes.Item(k).Name = "northeast" Then
                                                            For l As Integer = 0 To node.ChildNodes.Item(c).ChildNodes.Item(c2).ChildNodes.Item(j).ChildNodes.Item(k).ChildNodes.Count - 1
                                                                If node.ChildNodes.Item(c).ChildNodes.Item(c2).ChildNodes.Item(j).ChildNodes.Item(k).ChildNodes.Item(l).Name = "lat" Then
                                                                    oda.SelectCommand.Parameters.AddWithValue("vNElat", node.ChildNodes.Item(c).ChildNodes.Item(c2).ChildNodes.Item(j).ChildNodes.Item(k).ChildNodes.Item(l).InnerText)
                                                                ElseIf node.ChildNodes.Item(c).ChildNodes.Item(c2).ChildNodes.Item(j).ChildNodes.Item(k).ChildNodes.Item(l).Name = "lng" Then
                                                                    oda.SelectCommand.Parameters.AddWithValue("vNElng", node.ChildNodes.Item(c).ChildNodes.Item(c2).ChildNodes.Item(j).ChildNodes.Item(k).ChildNodes.Item(l).InnerText)
                                                                End If
                                                            Next
                                                        End If
                                                    Next
                                                ElseIf node.ChildNodes.Item(c).ChildNodes.Item(c2).ChildNodes.Item(j).Name = "bounds" Then
                                                    For k As Integer = 0 To node.ChildNodes.Item(c).ChildNodes.Item(c2).ChildNodes.Item(j).ChildNodes.Count - 1
                                                        If node.ChildNodes.Item(c).ChildNodes.Item(c2).ChildNodes.Item(j).ChildNodes.Item(k).Name = "southwest" Then
                                                            For l As Integer = 0 To node.ChildNodes.Item(c).ChildNodes.Item(c2).ChildNodes.Item(j).ChildNodes.Item(k).ChildNodes.Count - 1
                                                                If node.ChildNodes.Item(c).ChildNodes.Item(c2).ChildNodes.Item(j).ChildNodes.Item(k).ChildNodes.Item(l).Name = "lat" Then
                                                                    oda.SelectCommand.Parameters.AddWithValue("bSWlat", node.ChildNodes.Item(c).ChildNodes.Item(c2).ChildNodes.Item(j).ChildNodes.Item(k).ChildNodes.Item(l).InnerText)
                                                                ElseIf node.ChildNodes.Item(c).ChildNodes.Item(c2).ChildNodes.Item(j).ChildNodes.Item(k).ChildNodes.Item(l).Name = "lng" Then
                                                                    oda.SelectCommand.Parameters.AddWithValue("bSWlng", node.ChildNodes.Item(c).ChildNodes.Item(c2).ChildNodes.Item(j).ChildNodes.Item(k).ChildNodes.Item(l).InnerText)
                                                                End If
                                                            Next
                                                        ElseIf node.ChildNodes.Item(c).ChildNodes.Item(c2).ChildNodes.Item(j).ChildNodes.Item(k).Name = "northeast" Then
                                                            For l As Integer = 0 To node.ChildNodes.Item(c).ChildNodes.Item(c2).ChildNodes.Item(j).ChildNodes.Item(k).ChildNodes.Count - 1
                                                                If node.ChildNodes.Item(c).ChildNodes.Item(c2).ChildNodes.Item(j).ChildNodes.Item(k).ChildNodes.Item(l).Name = "lat" Then
                                                                    oda.SelectCommand.Parameters.AddWithValue("bNElat", node.ChildNodes.Item(c).ChildNodes.Item(c2).ChildNodes.Item(j).ChildNodes.Item(k).ChildNodes.Item(l).InnerText)
                                                                ElseIf node.ChildNodes.Item(c).ChildNodes.Item(c2).ChildNodes.Item(j).ChildNodes.Item(k).ChildNodes.Item(l).Name = "lng" Then
                                                                    oda.SelectCommand.Parameters.AddWithValue("bNElng", node.ChildNodes.Item(c).ChildNodes.Item(c2).ChildNodes.Item(j).ChildNodes.Item(k).ChildNodes.Item(l).InnerText)
                                                                End If
                                                            Next
                                                        End If
                                                    Next
                                                End If
                                            Next
                                        End If
                                    Next
                                End If
                                If Cnt = 1 Then
                                    Exit For
                                End If
                            Next
                        Next
                    End If
                    If con.State <> ConnectionState.Open Then
                        con.Open()
                    End If
                    oda.SelectCommand.ExecuteNonQuery()
                    oda.Dispose()
                    locatoinr.Dispose()
                Catch ex As Exception

                Finally
                    con.Close()
                    con.Dispose()
                    oda.Dispose()
                End Try
                Return fulladdress
            End If
        End If
        Return Nothing
    End Function
End Class

