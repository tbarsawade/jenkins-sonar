Imports Microsoft.VisualBasic
Imports System.Net
Imports System.IO
Imports System.Xml
Imports System.Data.SqlClient
Imports System.Data
Imports AjaxControlToolkit
Public Class GPSClass
    Public Function Location(lat As String, log As String) As String
        Dim constr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim con As SqlConnection = New SqlConnection(constr)
        Dim oda As SqlDataAdapter = New SqlDataAdapter("", con)
        Try
            oda.SelectCommand.CommandText = "select top 1 * from gpsLocation where Lat_start  <=" + lat + " and  lat_end >= " + lat + " and long_start <= " + log + " and long_end >= " + log + " "
            Dim locatoinr As DataTable = New DataTable()
            oda.Fill(locatoinr)
            If locatoinr.Rows.Count > 0 Then
                con.Dispose()
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
                        con.Dispose()
                    Catch ex As Exception
                    Finally

                    End Try
                    Return fulladdress
                End If
            End If
            Return Nothing
        Catch ex As Exception
            Throw
        Finally
            If Not con Is Nothing Then
                con.Close()
                con.Dispose()
            End If
            If Not oda Is Nothing Then
                oda.Dispose()
            End If
        End Try
    End Function
    Public Function LocationOnlyAPI(lat As String, log As String) As String
        Dim constr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim con As SqlConnection = New SqlConnection(constr)
        ' Dim oda As SqlDataAdapter = New SqlDataAdapter("", con)
        Try
            'oda.SelectCommand.CommandText = "select top 1 * from gpsLocation where Lat_start  <=" + lat + " and  lat_end >= " + lat + " and long_start <= " + log + " and long_end >= " + log + " "
            'Dim locatoinr As DataTable = New DataTable()
            'oda.Fill(locatoinr)
            'If locatoinr.Rows.Count > 0 Then
            '    con.Dispose()
            '    Return locatoinr.Rows(0).Item(1).ToString
            'Else ' Commented By Pallavi to get location only from API ( 5 August 2015)
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
                'oda.SelectCommand.CommandText = "gpsinsertlocation"
                'oda.SelectCommand.CommandType = CommandType.StoredProcedure
                'oda.SelectCommand.Parameters.AddWithValue("complete_latitude", lat)
                'oda.SelectCommand.Parameters.AddWithValue("complete_longitude", log)
                'oda.SelectCommand.Parameters.AddWithValue("Lat_start", Convert.ToDouble(lat.Substring(0, 5)))
                'oda.SelectCommand.Parameters.AddWithValue("lat_end", Convert.ToDouble(lat.Substring(0, 5)) + 0.01)
                'oda.SelectCommand.Parameters.AddWithValue("long_start", Convert.ToDouble(log.Substring(0, 5)))
                'oda.SelectCommand.Parameters.AddWithValue("long_end", Convert.ToDouble(log.Substring(0, 5)) + 0.01) Commented by pallavi coz do not need to insert this  locations 
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
                                                    ' oda.SelectCommand.Parameters.AddWithValue("other", node.ChildNodes.Item(c).ChildNodes.Item(c2).ChildNodes.Item(j).InnerText)
                                                    other = 1
                                                End If
                                                If node.ChildNodes.Item(c).ChildNodes.Item(c2).ChildNodes.Item(j).Name = "type" Then
                                                    If node.ChildNodes.Item(c).ChildNodes.Item(c2).ChildNodes.Item(j).InnerXml = "street_address" Then
                                                        '  oda.SelectCommand.Parameters.AddWithValue("street_address", node.ChildNodes.Item(c).ChildNodes.Item(c2).ChildNodes.Item(j - 2).InnerText) '''local address
                                                    End If

                                                    If node.ChildNodes.Item(c).ChildNodes.Item(c2).ChildNodes.Item(j).InnerXml = "floor" Then
                                                        ' oda.SelectCommand.Parameters.AddWithValue("floor", node.ChildNodes.Item(c).ChildNodes.Item(c2).ChildNodes.Item(j - 2).InnerText) '''local address
                                                    End If
                                                    If node.ChildNodes.Item(c).ChildNodes.Item(c2).ChildNodes.Item(j).InnerXml = "parking" Then
                                                        ' oda.SelectCommand.Parameters.AddWithValue("parking", node.ChildNodes.Item(c).ChildNodes.Item(c2).ChildNodes.Item(j - 2).InnerText) '''local address
                                                    End If
                                                    If node.ChildNodes.Item(c).ChildNodes.Item(c2).ChildNodes.Item(j).InnerXml = "post_box" Then
                                                        '  oda.SelectCommand.Parameters.AddWithValue("post_box", node.ChildNodes.Item(c).ChildNodes.Item(c2).ChildNodes.Item(j - 2).InnerText) '''local address
                                                    End If
                                                    If node.ChildNodes.Item(c).ChildNodes.Item(c2).ChildNodes.Item(j).InnerXml = "postal_town" Then

                                                        'oda.SelectCommand.Parameters.AddWithValue("postal_town", node.ChildNodes.Item(c).ChildNodes.Item(c2).ChildNodes.Item(j - 2).InnerText) '''local address
                                                    End If
                                                    If node.ChildNodes.Item(c).ChildNodes.Item(c2).ChildNodes.Item(j).InnerXml = "room" Then
                                                        ' oda.SelectCommand.Parameters.AddWithValue("room", node.ChildNodes.Item(c).ChildNodes.Item(c2).ChildNodes.Item(j - 2).InnerText) '''local address
                                                    End If
                                                    If node.ChildNodes.Item(c).ChildNodes.Item(c2).ChildNodes.Item(j).InnerXml = "train_station" Then
                                                        ' oda.SelectCommand.Parameters.AddWithValue("train_station", node.ChildNodes.Item(c).ChildNodes.Item(c2).ChildNodes.Item(j - 2).InnerText) '''local address
                                                    End If
                                                    If node.ChildNodes.Item(c).ChildNodes.Item(c2).ChildNodes.Item(j).InnerXml = "establishment" Then
                                                        ' oda.SelectCommand.Parameters.AddWithValue("establishment_address", node.ChildNodes.Item(c).ChildNodes.Item(c2).ChildNodes.Item(j - 2).InnerText) '''local address
                                                    End If
                                                    If node.ChildNodes.Item(c).ChildNodes.Item(c2).ChildNodes.Item(j).InnerXml = "street_number" Then
                                                        '  oda.SelectCommand.Parameters.AddWithValue("street_number", node.ChildNodes.Item(c).ChildNodes.Item(c2).ChildNodes.Item(j - 2).InnerText) '''local address
                                                    End If
                                                    If node.ChildNodes.Item(c).ChildNodes.Item(c2).ChildNodes.Item(j).InnerXml = "bus_station" Then
                                                        ' oda.SelectCommand.Parameters.AddWithValue("stationaddress", node.ChildNodes.Item(c).ChildNodes.Item(c2).ChildNodes.Item(j - 2).InnerText) '''local address
                                                    End If
                                                    If node.ChildNodes.Item(c).ChildNodes.Item(c2).ChildNodes.Item(j).InnerXml = "route" Then
                                                        'oda.SelectCommand.Parameters.AddWithValue("rld", node.ChildNodes.Item(c).ChildNodes.Item(c2).ChildNodes.Item(j - 2).InnerText) '''local address
                                                    End If
                                                    If node.ChildNodes.Item(c).ChildNodes.Item(c2).ChildNodes.Item(j).InnerXml = "neighborhood" Then
                                                        '  oda.SelectCommand.Parameters.AddWithValue("npa", node.ChildNodes.Item(c).ChildNodes.Item(c2).ChildNodes.Item(j - 2).InnerText)  ''neighborhood address
                                                    End If
                                                    If node.ChildNodes.Item(c).ChildNodes.Item(c2).ChildNodes.Item(j).InnerXml = "sublocality" Then
                                                        ' oda.SelectCommand.Parameters.AddWithValue("sublocalityaddress", node.ChildNodes.Item(c).ChildNodes.Item(c2).ChildNodes.Item(j - 2).InnerText)
                                                    End If
                                                    If node.ChildNodes.Item(c).ChildNodes.Item(c2).ChildNodes.Item(j).InnerXml = "locality" Then
                                                        ' oda.SelectCommand.Parameters.AddWithValue("locPaddre", node.ChildNodes.Item(c).ChildNodes.Item(c2).ChildNodes.Item(j - 2).InnerText)   ''locality
                                                    End If
                                                    If node.ChildNodes.Item(c).ChildNodes.Item(c2).ChildNodes.Item(j).InnerXml = "administrative_area_level_3" Then
                                                        '  oda.SelectCommand.Parameters.AddWithValue("admini3address", node.ChildNodes.Item(c).ChildNodes.Item(c2).ChildNodes.Item(j - 2).InnerText)   ''city
                                                    End If
                                                    If node.ChildNodes.Item(c).ChildNodes.Item(c2).ChildNodes.Item(j).InnerXml = "administrative_area_level_2" Then
                                                        ' oda.SelectCommand.Parameters.AddWithValue("adminpoladdress", node.ChildNodes.Item(c).ChildNodes.Item(c2).ChildNodes.Item(j - 2).InnerText)   ''city
                                                    End If
                                                    If node.ChildNodes.Item(c).ChildNodes.Item(c2).ChildNodes.Item(j).InnerXml = "administrative_area_level_1" Then
                                                        ' oda.SelectCommand.Parameters.AddWithValue("addressLongName", node.ChildNodes.Item(c).ChildNodes.Item(c2).ChildNodes.Item(j - 2).InnerText)   ''city
                                                        ' oda.SelectCommand.Parameters.AddWithValue("addShortName", node.ChildNodes.Item(c).ChildNodes.Item(c2).ChildNodes.Item(j - 1).InnerText)
                                                    End If
                                                    If node.ChildNodes.Item(c).ChildNodes.Item(c2).ChildNodes.Item(j).InnerXml = "country" Then
                                                        '   oda.SelectCommand.Parameters.AddWithValue("countryLong", node.ChildNodes.Item(c).ChildNodes.Item(c2).ChildNodes.Item(j - 2).InnerText)
                                                        '   oda.SelectCommand.Parameters.AddWithValue("countryShort", node.ChildNodes.Item(c).ChildNodes.Item(c2).ChildNodes.Item(j - 1).InnerText)
                                                    End If
                                                    If node.ChildNodes.Item(c).ChildNodes.Item(c2).ChildNodes.Item(j).InnerXml = "postal_code" Then
                                                        ' oda.SelectCommand.Parameters.AddWithValue("postalLong", node.ChildNodes.Item(c).ChildNodes.Item(c2).ChildNodes.Item(j - 2).InnerText)
                                                        ' oda.SelectCommand.Parameters.AddWithValue("postalShort", node.ChildNodes.Item(c).ChildNodes.Item(c2).ChildNodes.Item(j - 1).InnerText)
                                                    End If
                                                    If node.ChildNodes.Item(c).ChildNodes.Item(c2).ChildNodes.Item(j).InnerXml = "airport" Then
                                                        ' oda.SelectCommand.Parameters.AddWithValue("airport", node.ChildNodes.Item(c).ChildNodes.Item(c2).ChildNodes.Item(j - 2).InnerText) '''local address
                                                    End If
                                                    If node.ChildNodes.Item(c).ChildNodes.Item(c2).ChildNodes.Item(j).InnerXml = "point_of_interest" Then
                                                        ' oda.SelectCommand.Parameters.AddWithValue("point_of_interest", node.ChildNodes.Item(c).ChildNodes.Item(c2).ChildNodes.Item(j - 2).InnerText) '''local address
                                                    End If
                                                    If node.ChildNodes.Item(c).ChildNodes.Item(c2).ChildNodes.Item(j).InnerXml = "park" Then
                                                        ' oda.SelectCommand.Parameters.AddWithValue("park", node.ChildNodes.Item(c).ChildNodes.Item(c2).ChildNodes.Item(j - 2).InnerText) '''local address
                                                    End If
                                                    If node.ChildNodes.Item(c).ChildNodes.Item(c2).ChildNodes.Item(j).InnerXml = "intersection" Then
                                                        ' oda.SelectCommand.Parameters.AddWithValue("intersection", node.ChildNodes.Item(c).ChildNodes.Item(c2).ChildNodes.Item(j - 2).InnerText) '''local address
                                                    End If
                                                    If node.ChildNodes.Item(c).ChildNodes.Item(c2).ChildNodes.Item(j).InnerXml = "colloquial_area" Then
                                                        ' oda.SelectCommand.Parameters.AddWithValue("colloquial_area", node.ChildNodes.Item(c).ChildNodes.Item(c2).ChildNodes.Item(j - 2).InnerText) '''local address
                                                    End If
                                                    If node.ChildNodes.Item(c).ChildNodes.Item(c2).ChildNodes.Item(j).InnerXml = "premise" Then
                                                        ' oda.SelectCommand.Parameters.AddWithValue("premise", node.ChildNodes.Item(c).ChildNodes.Item(c2).ChildNodes.Item(j - 2).InnerText) '''local address
                                                    End If
                                                    If node.ChildNodes.Item(c).ChildNodes.Item(c2).ChildNodes.Item(j).InnerXml = "subpremise" Then
                                                        'oda.SelectCommand.Parameters.AddWithValue("subpremise", node.ChildNodes.Item(c).ChildNodes.Item(c2).ChildNodes.Item(j - 2).InnerText) '''local address
                                                    End If
                                                End If
                                            Next
                                        ElseIf node.ChildNodes.Item(c).ChildNodes.Item(c2).Name = "formatted_address" Then
                                            fulladdress = node.ChildNodes.Item(c).ChildNodes.Item(c2).InnerText
                                            '  oda.SelectCommand.Parameters.AddWithValue("location_namer", node.ChildNodes.Item(c).ChildNodes.Item(c2).InnerText)
                                        ElseIf node.ChildNodes.Item(c).ChildNodes.Item(c2).Name = "geometry" Then
                                            For j As Integer = 0 To node.ChildNodes.Item(c).ChildNodes.Item(c2).ChildNodes.Count - 1
                                                If node.ChildNodes.Item(c).ChildNodes.Item(c2).ChildNodes.Item(j).Name = "location" Then
                                                    For k As Integer = 0 To node.ChildNodes.Item(c).ChildNodes.Item(c2).ChildNodes.Item(j).ChildNodes.Count - 1
                                                        If node.ChildNodes.Item(c).ChildNodes.Item(c2).ChildNodes.Item(j).ChildNodes.Item(k).Name = "lat" Then
                                                            ' oda.SelectCommand.Parameters.AddWithValue("geometrylat", node.ChildNodes.Item(c).ChildNodes.Item(c2).ChildNodes.Item(j).ChildNodes.Item(k).InnerText)
                                                        ElseIf node.ChildNodes.Item(c).ChildNodes.Item(c2).ChildNodes.Item(j).ChildNodes.Item(k).Name = "lng" Then
                                                            ' oda.SelectCommand.Parameters.AddWithValue("geometrylng", node.ChildNodes.Item(c).ChildNodes.Item(c2).ChildNodes.Item(j).ChildNodes.Item(k).InnerText)
                                                        End If
                                                    Next
                                                ElseIf node.ChildNodes.Item(c).ChildNodes.Item(c2).ChildNodes.Item(j).Name = "viewport" Then
                                                    For k As Integer = 0 To node.ChildNodes.Item(c).ChildNodes.Item(c2).ChildNodes.Item(j).ChildNodes.Count - 1
                                                        If node.ChildNodes.Item(c).ChildNodes.Item(c2).ChildNodes.Item(j).ChildNodes.Item(k).Name = "southwest" Then
                                                            For l As Integer = 0 To node.ChildNodes.Item(c).ChildNodes.Item(c2).ChildNodes.Item(j).ChildNodes.Item(k).ChildNodes.Count - 1
                                                                If node.ChildNodes.Item(c).ChildNodes.Item(c2).ChildNodes.Item(j).ChildNodes.Item(k).ChildNodes.Item(l).Name = "lat" Then
                                                                    ' oda.SelectCommand.Parameters.AddWithValue("vSWlat", node.ChildNodes.Item(c).ChildNodes.Item(c2).ChildNodes.Item(j).ChildNodes.Item(k).ChildNodes.Item(l).InnerText)
                                                                ElseIf node.ChildNodes.Item(c).ChildNodes.Item(c2).ChildNodes.Item(j).ChildNodes.Item(k).ChildNodes.Item(l).Name = "lng" Then
                                                                    ' oda.SelectCommand.Parameters.AddWithValue("vSWlng", node.ChildNodes.Item(c).ChildNodes.Item(c2).ChildNodes.Item(j).ChildNodes.Item(k).ChildNodes.Item(l).InnerText)
                                                                End If
                                                            Next
                                                        ElseIf node.ChildNodes.Item(c).ChildNodes.Item(c2).ChildNodes.Item(j).ChildNodes.Item(k).Name = "northeast" Then
                                                            For l As Integer = 0 To node.ChildNodes.Item(c).ChildNodes.Item(c2).ChildNodes.Item(j).ChildNodes.Item(k).ChildNodes.Count - 1
                                                                If node.ChildNodes.Item(c).ChildNodes.Item(c2).ChildNodes.Item(j).ChildNodes.Item(k).ChildNodes.Item(l).Name = "lat" Then
                                                                    ' oda.SelectCommand.Parameters.AddWithValue("vNElat", node.ChildNodes.Item(c).ChildNodes.Item(c2).ChildNodes.Item(j).ChildNodes.Item(k).ChildNodes.Item(l).InnerText)
                                                                ElseIf node.ChildNodes.Item(c).ChildNodes.Item(c2).ChildNodes.Item(j).ChildNodes.Item(k).ChildNodes.Item(l).Name = "lng" Then
                                                                    '  oda.SelectCommand.Parameters.AddWithValue("vNElng", node.ChildNodes.Item(c).ChildNodes.Item(c2).ChildNodes.Item(j).ChildNodes.Item(k).ChildNodes.Item(l).InnerText)
                                                                End If
                                                            Next
                                                        End If
                                                    Next
                                                ElseIf node.ChildNodes.Item(c).ChildNodes.Item(c2).ChildNodes.Item(j).Name = "bounds" Then
                                                    For k As Integer = 0 To node.ChildNodes.Item(c).ChildNodes.Item(c2).ChildNodes.Item(j).ChildNodes.Count - 1
                                                        If node.ChildNodes.Item(c).ChildNodes.Item(c2).ChildNodes.Item(j).ChildNodes.Item(k).Name = "southwest" Then
                                                            For l As Integer = 0 To node.ChildNodes.Item(c).ChildNodes.Item(c2).ChildNodes.Item(j).ChildNodes.Item(k).ChildNodes.Count - 1
                                                                If node.ChildNodes.Item(c).ChildNodes.Item(c2).ChildNodes.Item(j).ChildNodes.Item(k).ChildNodes.Item(l).Name = "lat" Then
                                                                    '  oda.SelectCommand.Parameters.AddWithValue("bSWlat", node.ChildNodes.Item(c).ChildNodes.Item(c2).ChildNodes.Item(j).ChildNodes.Item(k).ChildNodes.Item(l).InnerText)
                                                                ElseIf node.ChildNodes.Item(c).ChildNodes.Item(c2).ChildNodes.Item(j).ChildNodes.Item(k).ChildNodes.Item(l).Name = "lng" Then
                                                                    'oda.SelectCommand.Parameters.AddWithValue("bSWlng", node.ChildNodes.Item(c).ChildNodes.Item(c2).ChildNodes.Item(j).ChildNodes.Item(k).ChildNodes.Item(l).InnerText)
                                                                End If
                                                            Next
                                                        ElseIf node.ChildNodes.Item(c).ChildNodes.Item(c2).ChildNodes.Item(j).ChildNodes.Item(k).Name = "northeast" Then
                                                            For l As Integer = 0 To node.ChildNodes.Item(c).ChildNodes.Item(c2).ChildNodes.Item(j).ChildNodes.Item(k).ChildNodes.Count - 1
                                                                If node.ChildNodes.Item(c).ChildNodes.Item(c2).ChildNodes.Item(j).ChildNodes.Item(k).ChildNodes.Item(l).Name = "lat" Then
                                                                    ' oda.SelectCommand.Parameters.AddWithValue("bNElat", node.ChildNodes.Item(c).ChildNodes.Item(c2).ChildNodes.Item(j).ChildNodes.Item(k).ChildNodes.Item(l).InnerText)
                                                                ElseIf node.ChildNodes.Item(c).ChildNodes.Item(c2).ChildNodes.Item(j).ChildNodes.Item(k).ChildNodes.Item(l).Name = "lng" Then
                                                                    'oda.SelectCommand.Parameters.AddWithValue("bNElng", node.ChildNodes.Item(c).ChildNodes.Item(c2).ChildNodes.Item(j).ChildNodes.Item(k).ChildNodes.Item(l).InnerText)
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
                    ' oda.SelectCommand.ExecuteNonQuery()
                    ' oda.Dispose()
                    ' locatoinr.Dispose()
                    con.Dispose()
                Catch ex As Exception
                Finally

                End Try
                Return fulladdress
            End If
            ' End If
            Return Nothing
        Catch ex As Exception
            Throw
        Finally
            If Not con Is Nothing Then
                con.Close()
                con.Dispose()
            End If
            'If Not oda Is Nothing Then
            '    '  oda.Dispose()
            'End If
        End Try
    End Function
    Public Function MakeMyTrip(ByVal Uid As String, ByVal Trip_start As String, ByVal TripEnd As String, ByRef label As Label, ByRef gvData As GridView, ByRef btnActUserSave As Button, ByVal eid As String, ByRef uppanel As UpdatePanel, ByRef popup As ModalPopupExtender, ByVal tid As String, ByVal vehicle As String, ByVal imei As String) As String
        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim con As SqlConnection = New SqlConnection(conStr)
        Dim oda1 As New SqlDataAdapter("", con)
        'Old Logic
        ' oda1.SelectCommand.CommandText = "select sum(devdist) from MMM_MST_GPSDATA where IMIENO='" & imei & "'  and cTime    >= '" + Trip_start + "' AND cTime     <= '" + TripEnd + "' "
        'New change 05 April 2014
        oda1.SelectCommand.CommandText = "select isnull(sum(devdist),0) from mmm_mst_gpsdata where IMIENO='" & imei & "'  and cTime >= '" + Trip_start + "' AND ctime <= '" + TripEnd + "' and ((devdist>0.03 and speed=0) or (devdist>0 and speed>0 )) group by imieno"
        Dim sum As DataTable = New DataTable()
        Try
            oda1.Fill(sum)
            If sum.Rows.Count > 0 Then

                If btnActUserSave.Text = "Save" Then
                    Dim oda As SqlDataAdapter = New SqlDataAdapter("uspInsertELOGBOOK", con)
                    oda.SelectCommand.CommandType = CommandType.StoredProcedure
                    oda.SelectCommand.Parameters.AddWithValue("Trip_Start_DateTime", Trip_start)
                    oda.SelectCommand.Parameters.AddWithValue("Trip_end_DateTime", TripEnd)
                    oda.SelectCommand.Parameters.AddWithValue("vehicle_no", vehicle)
                    oda.SelectCommand.Parameters.AddWithValue("eid", eid)
                    oda1.SelectCommand.CommandText = "select lattitude,longitude,ctime,speed from MMM_MST_GPSDATA where tid in (select max(tid) from MMM_MST_GPSDATA where IMIENO='" & imei & "'  and   cTime >= '" + Trip_start + "' AND   ctime <= '" + TripEnd + "' group by lattitude,longitude) order by ctime"
                    Dim dtlatlong As DataTable = New DataTable()
                    oda1.Fill(dtlatlong)
                    If dtlatlong.Rows.Count > 0 Then
                        oda.SelectCommand.Parameters.AddWithValue("Trip_Start_Location", Location(dtlatlong.Rows(0).ItemArray(0).ToString(), dtlatlong.Rows(0).ItemArray(1).ToString()))
                        oda.SelectCommand.Parameters.AddWithValue("Trip_End_Location", Location(dtlatlong.Rows(dtlatlong.Rows.Count - 1).ItemArray(0).ToString(), dtlatlong.Rows(dtlatlong.Rows.Count - 1).ItemArray(1).ToString()))
                        oda.SelectCommand.Parameters.AddWithValue("Start_latitude", dtlatlong.Rows(0).ItemArray(0).ToString())
                        oda.SelectCommand.Parameters.AddWithValue("Start_longitude", dtlatlong.Rows(0).ItemArray(1).ToString())
                        oda.SelectCommand.Parameters.AddWithValue("End_latitude", dtlatlong.Rows(dtlatlong.Rows.Count - 1).ItemArray(0).ToString())
                        oda.SelectCommand.Parameters.AddWithValue("End_longitude", dtlatlong.Rows(dtlatlong.Rows.Count - 1).ItemArray(1).ToString())
                    Else
                        label.Text = " Ensure that data is not sufficient to make a trip "
                        Return Nothing
                        Exit Function
                    End If
                    oda.SelectCommand.Parameters.AddWithValue("IMEI_no", imei)
                    oda.SelectCommand.Parameters.AddWithValue("Total_Distance", sum.Rows(0).ItemArray(0).ToString())
                    oda.SelectCommand.Parameters.AddWithValue("uid", Uid)
                    oda.SelectCommand.Parameters.AddWithValue("TripType", "Automatic")
                    If con.State <> ConnectionState.Open Then
                        con.Open()
                    End If
                    Dim ir As Integer = oda.SelectCommand.ExecuteNonQuery()

                    '   oda.SelectCommand.CommandText = "select * from MMM_MST_ELOGBOOK inner join MMM_MST_USER on MMM_MST_ELOGBOOK.uid=MMM_MST_USER.uid  where MMM_MST_ELOGBOOK.uid=" + System.Web.HttpContext.Current.Session("UID").ToString() + ""
                    oda.SelectCommand.CommandText = "select vehicle_no,IMEI_NO,convert(nvarchar,Trip_Start_DateTime,120)[Trip_Start_DateTime],convert(nvarchar,Trip_end_DateTime,120)[Trip_end_DateTime],Trip_Start_Location,Trip_End_Location,Total_Distance,tid,UserName,Triptype,Islock=CASE islock  when 0 THEN 'Unlocked'WHEN 1 THEN 'Locked' END, case e.isauth when 0 then 'Pending for approval' else 'Approved' end[Status] from MMM_MST_ELOGBOOK e , MMM_MST_USER u where   e.uid=u.uid and e.uid=" + Uid + " and  Trip_Start_DateTime  >= '" + Trip_start + "' AND Trip_End_DateTime <= '" + TripEnd + "'"
                    oda.SelectCommand.CommandType = CommandType.Text
                    Dim ds As New DataSet()
                    oda.Fill(ds, "data")
                    gvData.DataSource = ds.Tables("data")
                    gvData.DataBind()
                    For i As Integer = 0 To gvData.Rows.Count - 1
                        Dim Gridapprove As ImageButton = CType(gvData.Rows(i).FindControl("btnapprove"), ImageButton)
                        Dim GridDelete As ImageButton = CType(gvData.Rows(i).FindControl("btnDelete"), ImageButton)
                        If HttpContext.Current.Session("USERROLE").ToString.ToUpper = "USER" Then
                            Gridapprove.Visible = True
                            GridDelete.Visible = True
                        Else
                            Gridapprove.Visible = False
                            GridDelete.Visible = False
                        End If
                    Next
                    For i As Integer = 0 To gvData.Rows.Count - 1
                        Dim GridbtnEdit As ImageButton = CType(gvData.Rows(i).FindControl("btnEdit"), ImageButton)
                        If HttpContext.Current.Session("USERROLE").ToString.ToUpper <> "SU" And HttpContext.Current.Session("USERROLE").ToString.ToUpper <> "USER" Then
                            GridbtnEdit.Visible = False
                        Else
                            GridbtnEdit.Visible = True
                        End If
                    Next

                    label.Visible = True
                    label.Text = " Trip Created successfully "
                    uppanel.Update()
                    popup.Hide()
                    If ir = 1 Then
                        Return ir.ToString
                    Else
                        Return Nothing
                    End If

                    con.Close()
                    con.Dispose()
                    oda1.Dispose()
                ElseIf btnActUserSave.Text = "Update" Then
                    Dim chkstrin As String = ""
                    Dim oda As SqlDataAdapter = New SqlDataAdapter("uspUpdateELOGBOOK", con)
                    oda.SelectCommand.CommandType = CommandType.StoredProcedure
                    oda.SelectCommand.Parameters.AddWithValue("tid", tid)
                    'Dim oda1 As SqlDataAdapter = New SqlDataAdapter("select lattitude,longitude,ctime,speed,distancetravel from MMM_MST_GPSDATA where tid in (select max(tid) from MMM_MST_GPSDATA where IMIENO='" & imei & "'  and   cTime >= '" & Trip_start & "' AND   ctime <= '" + TripEnd + "' group by lattitude,longitude) order by ctime ", con)
                    oda1.SelectCommand.CommandText = "select lattitude,longitude,ctime,speed from MMM_MST_GPSDATA where tid in (select max(tid) from MMM_MST_GPSDATA where IMIENO='" & imei & "'  and   cTime >= '" & Trip_start & "' AND   ctime <= '" + TripEnd + "' group by lattitude,longitude) order by ctime"
                    Dim dtlatlong As DataTable = New DataTable()
                    oda1.Fill(dtlatlong)
                    If dtlatlong.Rows.Count > 0 Then
                        oda.SelectCommand.Parameters.AddWithValue("Trip_Start_Location", Location(dtlatlong.Rows(0).ItemArray(0).ToString(), dtlatlong.Rows(0).ItemArray(1).ToString()))
                        oda.SelectCommand.Parameters.AddWithValue("Trip_End_Location", Location(dtlatlong.Rows(dtlatlong.Rows.Count - 1).ItemArray(0).ToString(), dtlatlong.Rows(dtlatlong.Rows.Count - 1).ItemArray(1).ToString()))
                        oda.SelectCommand.Parameters.AddWithValue("Start_latitude", dtlatlong.Rows(0).ItemArray(0).ToString())
                        oda.SelectCommand.Parameters.AddWithValue("Start_longitude", dtlatlong.Rows(0).ItemArray(1).ToString())
                        oda.SelectCommand.Parameters.AddWithValue("End_latitude", dtlatlong.Rows(dtlatlong.Rows.Count - 1).ItemArray(0).ToString())
                        oda.SelectCommand.Parameters.AddWithValue("End_longitude", dtlatlong.Rows(dtlatlong.Rows.Count - 1).ItemArray(1).ToString())
                    Else
                        label.Text = " Ensure that data is not sufficient to make a trip "
                        Return Nothing
                        Exit Function
                    End If
                    oda.SelectCommand.Parameters.AddWithValue("Trip_Start_DateTime", Trip_start)
                    oda.SelectCommand.Parameters.AddWithValue("Trip_end_DateTime", TripEnd)
                    oda.SelectCommand.Parameters.AddWithValue("vehicle_no", vehicle)
                    oda.SelectCommand.Parameters.AddWithValue("eid", System.Web.HttpContext.Current.Session("EID"))
                    oda.SelectCommand.Parameters.AddWithValue("IMEI_no", imei)
                    oda.SelectCommand.Parameters.AddWithValue("uid", Uid)
                    oda.SelectCommand.Parameters.AddWithValue("Total_Distance", sum.Rows(0).ItemArray(0).ToString())
                    If con.State <> ConnectionState.Open Then
                        con.Open()
                    End If
                    Dim ir As Integer = oda.SelectCommand.ExecuteNonQuery()
                    ' oda.SelectCommand.CommandText = "select vehicle_no,IMEI_NO,Trip_Start_DateTime,Trip_end_DateTime,Trip_Start_Location,Trip_End_Location,Total_Distance,tid,UserName,MMM_MST_ELOGBOOK.Uid from MMM_MST_ELOGBOOK inner join MMM_MST_USER on  MMM_MST_ELOGBOOK.uid=MMM_MST_USER.uid where MMM_MST_ELOGBOOK.uid=" + System.Web.HttpContext.Current.Session("UID").ToString() + ""
                    'oda.SelectCommand.CommandText = "select vehicle_no,IMEI_NO,convert(nvarchar,Trip_Start_DateTime,120)[Trip_Start_DateTime],convert(nvarchar,Trip_end_DateTime,120)[Trip_end_DateTime],Trip_Start_Location,Trip_End_Location,Total_Distance,tid,UserName,TripType from MMM_MST_ELOGBOOK e , MMM_MST_USER u where   e.uid=u.uid and e.uid=" + System.Web.HttpContext.Current.Session("UID").ToString() + " and  Trip_Start_DateTime  >= '" + Trip_start + "' AND Trip_End_DateTime <= '" + TripEnd + "'"
                    oda.SelectCommand.CommandText = "select vehicle_no,IMEI_NO,convert(nvarchar,Trip_Start_DateTime,120)[Trip_Start_DateTime],convert(nvarchar,Trip_end_DateTime,120)[Trip_end_DateTime],Trip_Start_Location,Trip_End_Location,Total_Distance,tid,UserName,Triptype,Islock=CASE islock  when 0 THEN 'Unlocked'WHEN 1 THEN 'Locked' END, case e.isauth when 0 then 'Pending for approval' else 'Approved' end[Status] from MMM_MST_ELOGBOOK e , MMM_MST_USER u where   e.uid=u.uid and tid=" & tid & " "
                    oda.SelectCommand.CommandType = CommandType.Text
                    Dim ds As New DataSet()
                    oda.Fill(ds, "data")
                    gvData.DataSource = ds.Tables("data")
                    gvData.DataBind()

                    For i As Integer = 0 To gvData.Rows.Count - 1
                        Dim Gridapprove As ImageButton = CType(gvData.Rows(i).FindControl("btnapprove"), ImageButton)
                        Dim GridDelete As ImageButton = CType(gvData.Rows(i).FindControl("btnDelete"), ImageButton)
                        If HttpContext.Current.Session("USERROLE").ToString.ToUpper = "USER" Then
                            Gridapprove.Visible = True
                            GridDelete.Visible = True
                        Else
                            Gridapprove.Visible = False
                            GridDelete.Visible = False
                        End If
                    Next
                    For i As Integer = 0 To gvData.Rows.Count - 1
                        Dim GridbtnEdit As ImageButton = CType(gvData.Rows(i).FindControl("btnEdit"), ImageButton)
                        If HttpContext.Current.Session("USERROLE").ToString.ToUpper <> "SU" And HttpContext.Current.Session("USERROLE").ToString.ToUpper <> "USER" Then
                            GridbtnEdit.Visible = False
                        Else
                            GridbtnEdit.Visible = True
                        End If
                    Next

                    label.Visible = True
                    label.Text = "Trip Updated Successfully"
                    uppanel.Update()
                    popup.Hide()
                    If ir = 1 Then
                        Return ir.ToString
                    Else
                        Return Nothing
                    End If
                    oda1.Dispose()
                    con.Close()
                    con.Dispose()
                    oda.Dispose()
                End If
            Else
                label.Text = "Data is not sufficient to make a trip "
                Return Nothing
                Exit Function
            End If

        Catch ex As Exception
        Finally
            con.Dispose()
            oda1.Dispose()
        End Try
        Return Nothing
    End Function
    Public Function ManualTrip(ByVal Uid As String, ByVal Trip_start As String, ByVal TripEnd As String, ByRef label As Label, ByRef gvData As GridView, ByRef btnActUserSave As Button, ByVal tid As String, ByVal vehicle As String, ByVal StartLocation As String, ByVal EndLocation As String, ByVal Totaldistance As String, ByVal imei As String) As String
        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim con As SqlConnection = New SqlConnection(conStr)
        Try
            If btnActUserSave.Text = "Save" Then
                Dim oda As SqlDataAdapter = New SqlDataAdapter("uspInsertELOGBOOK", con)
                oda.SelectCommand.CommandType = CommandType.StoredProcedure
                oda.SelectCommand.Parameters.AddWithValue("Trip_Start_DateTime", Trip_start)
                oda.SelectCommand.Parameters.AddWithValue("Trip_end_DateTime", TripEnd)
                oda.SelectCommand.Parameters.AddWithValue("vehicle_no", vehicle)
                oda.SelectCommand.Parameters.AddWithValue("eid", System.Web.HttpContext.Current.Session("EID"))
                oda.SelectCommand.Parameters.AddWithValue("uid", Uid)
                oda.SelectCommand.Parameters.AddWithValue("Total_Distance", Totaldistance)
                oda.SelectCommand.Parameters.AddWithValue("Trip_Start_Location", StartLocation)
                oda.SelectCommand.Parameters.AddWithValue("Trip_End_Location", EndLocation)
                oda.SelectCommand.Parameters.AddWithValue("TripType", "Manual")
                oda.SelectCommand.Parameters.AddWithValue("IMEI_no", imei)
                If con.State <> ConnectionState.Open Then
                    con.Open()
                End If
                Dim ir As Integer = oda.SelectCommand.ExecuteNonQuery()
                oda.SelectCommand.CommandText = "select vehicle_no,IMEI_NO,convert(nvarchar,Trip_Start_DateTime,120)[Trip_Start_DateTime],convert(nvarchar,Trip_end_DateTime,120)[Trip_end_DateTime],Trip_Start_Location,Trip_End_Location,Total_Distance,tid,UserName,Triptype,Islock=CASE islock  when 0 THEN 'Unlocked'WHEN 1 THEN 'Locked' END, case e.isauth when 0 then 'Pending for approval' else 'Approved' end[Status] from MMM_MST_ELOGBOOK e , MMM_MST_USER u where   e.uid=u.uid and e.uid=" + Uid + " and  Trip_Start_DateTime  >= '" + Trip_start + "' AND Trip_End_DateTime <= '" + TripEnd + "' and TripType='Manual'"
                oda.SelectCommand.CommandType = CommandType.Text
                Dim show As DataTable = New DataTable()
                oda.Fill(show)
                gvData.DataSource = show
                gvData.DataBind()
                For i As Integer = 0 To gvData.Rows.Count - 1
                    Dim Gridapprove As ImageButton = CType(gvData.Rows(i).FindControl("btnapprove"), ImageButton)
                    Dim GridDelete As ImageButton = CType(gvData.Rows(i).FindControl("btnDelete"), ImageButton)
                    If HttpContext.Current.Session("USERROLE").ToString.ToUpper = "USER" Then
                        Gridapprove.Visible = True
                        GridDelete.Visible = True
                    Else
                        Gridapprove.Visible = False
                        GridDelete.Visible = False
                    End If
                Next
                For i As Integer = 0 To gvData.Rows.Count - 1
                    Dim GridbtnEdit As ImageButton = CType(gvData.Rows(i).FindControl("btnEdit"), ImageButton)
                    If HttpContext.Current.Session("USERROLE").ToString.ToUpper <> "SU" And HttpContext.Current.Session("USERROLE").ToString.ToUpper <> "USER" Then
                        GridbtnEdit.Visible = False
                    Else
                        GridbtnEdit.Visible = True
                    End If
                Next

                label.Visible = True
                label.Text = "Manual trip created Successfully"
                If ir = 1 Then
                    Return ir.ToString
                End If
                con.Close()
                con.Dispose()
                oda.Dispose()
            ElseIf btnActUserSave.Text = "Update" Then
                Dim chkstrin As String = ""
                Dim oda As SqlDataAdapter = New SqlDataAdapter("uspUpdateELOGBOOK", con)
                oda.SelectCommand.CommandType = CommandType.StoredProcedure
                oda.SelectCommand.Parameters.AddWithValue("tid", tid)
                oda.SelectCommand.Parameters.AddWithValue("Trip_Start_DateTime", Trip_start)
                oda.SelectCommand.Parameters.AddWithValue("Trip_end_DateTime", TripEnd)
                oda.SelectCommand.Parameters.AddWithValue("vehicle_no", vehicle)
                oda.SelectCommand.Parameters.AddWithValue("IMEI_no", imei)
                oda.SelectCommand.Parameters.AddWithValue("eid", System.Web.HttpContext.Current.Session("EID"))
                oda.SelectCommand.Parameters.AddWithValue("uid", Uid)
                oda.SelectCommand.Parameters.AddWithValue("Trip_Start_Location", StartLocation)
                oda.SelectCommand.Parameters.AddWithValue("Trip_End_Location", EndLocation)
                oda.SelectCommand.Parameters.AddWithValue("Total_Distance", Totaldistance)
                If con.State <> ConnectionState.Open Then
                    con.Open()
                End If
                Dim ir As Integer = oda.SelectCommand.ExecuteNonQuery()
                '            oda.SelectCommand.CommandText = "select vehicle_no,IMEI_NO,convert(nvarchar,Trip_Start_DateTime,120)[Trip_Start_DateTime],convert(nvarchar,Trip_end_DateTime,120)[Trip_end_DateTime],Trip_Start_Location,Trip_End_Location,Total_Distance,tid,UserName,Triptype from MMM_MST_ELOGBOOK e , MMM_MST_USER u where   e.uid=u.uid and e.uid=" + System.Web.HttpContext.Current.Session("UID").ToString() + " and  Trip_Start_DateTime  >= '" + Trip_start + "' AND Trip_End_DateTime <= '" + TripEnd + "' and TripType='Manual'"
                oda.SelectCommand.CommandText = "select vehicle_no,IMEI_NO,convert(nvarchar,Trip_Start_DateTime,120)[Trip_Start_DateTime],convert(nvarchar,Trip_end_DateTime,120)[Trip_end_DateTime],Trip_Start_Location,Trip_End_Location,Total_Distance,tid,UserName,Triptype,Islock=CASE islock  when 0 THEN 'Unlocked'WHEN 1 THEN 'Locked' END, case e.isauth when 0 then 'Pending for approval' else 'Approved' end[Status] from MMM_MST_ELOGBOOK e , MMM_MST_USER u where   e.uid=u.uid and tid=" & tid & " "
                oda.SelectCommand.CommandType = CommandType.Text
                Dim show As DataTable = New DataTable()
                oda.Fill(show)
                gvData.DataSource = show
                gvData.DataBind()
                For i As Integer = 0 To gvData.Rows.Count - 1
                    Dim Gridapprove As ImageButton = CType(gvData.Rows(i).FindControl("btnapprove"), ImageButton)
                    Dim GridDelete As ImageButton = CType(gvData.Rows(i).FindControl("btnDelete"), ImageButton)
                    If HttpContext.Current.Session("USERROLE").ToString.ToUpper = "USER" Then
                        Gridapprove.Visible = True
                        GridDelete.Visible = True
                    Else
                        Gridapprove.Visible = False
                        GridDelete.Visible = False
                    End If
                Next
                For i As Integer = 0 To gvData.Rows.Count - 1
                    Dim GridbtnEdit As ImageButton = CType(gvData.Rows(i).FindControl("btnEdit"), ImageButton)
                    If HttpContext.Current.Session("USERROLE").ToString.ToUpper <> "SU" And HttpContext.Current.Session("USERROLE").ToString.ToUpper <> "USER" Then
                        GridbtnEdit.Visible = False
                    Else
                        GridbtnEdit.Visible = True
                    End If
                Next

                label.Visible = True
                label.Text = "Trip Updated Successfully"
                con.Close()
                con.Dispose()
                oda.Dispose()
                If ir = 1 Then
                    Return ir.ToString
                End If
            End If
        Catch ex As Exception
        Finally
            con.Dispose()
        End Try
        Return Nothing
    End Function
End Class
