' NOTE: You can use the "Rename" command on the context menu to change the class name "DMSNewService" in code, svc and config file together.
Imports System.Data
Imports System.Data.SqlClient
Imports System.Net
Imports System.IO
Imports System.Xml.Serialization
Imports System.Xml
Imports System.Web.Hosting
Imports System.Random
Public Class DMSNewService
    Implements IDMSNewService
    Public Function MissedCall(who As String, ChannelID As String, Circle As String, Optr As String, QualityScore As String, DateTime As String) As String Implements IDMSNewService.MissedCall
        Dim conStr As String '= ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        conStr = "server=172.17.109.152;initial catalog=DMS_SEQUEL;uid=DMSSEQUEL;pwd=S@urabh123;Connect Timeout=200; pooling='true'; Max Pool Size=200"
        Dim con As SqlConnection = New SqlConnection(conStr)
        Dim oda As New SqlDataAdapter("", con)
        Try
            oda.SelectCommand.CommandText = "USPinserMissedCall"
            oda.SelectCommand.CommandType = CommandType.StoredProcedure
            oda.SelectCommand.Parameters.Clear()
            oda.SelectCommand.Parameters.AddWithValue("mcallno", who)
            oda.SelectCommand.Parameters.AddWithValue("cno", ChannelID)
            oda.SelectCommand.Parameters.AddWithValue("cir", Circle)
            oda.SelectCommand.Parameters.AddWithValue("opr", Optr)
            oda.SelectCommand.Parameters.AddWithValue("dt", DateTime)
            If con.State <> ConnectionState.Open Then
                con.Open()
            End If
            oda.SelectCommand.ExecuteNonQuery()
        Catch ex As System.Exception
            oda.SelectCommand.CommandText = "INSERT_ERRORLOG"
            oda.SelectCommand.CommandType = CommandType.StoredProcedure
            oda.SelectCommand.Parameters.Clear()
            oda.SelectCommand.Parameters.AddWithValue("@ERRORMSG", ex.ToString())
            oda.SelectCommand.Parameters.AddWithValue("@ALERTNAME", "MissedCallService")
            oda.SelectCommand.Parameters.AddWithValue("@EID", 9855)
            If con.State <> ConnectionState.Open Then
                con.Open()
            End If
            oda.SelectCommand.ExecuteNonQuery()
        Finally
            con.Close()
            oda.Dispose()
            con.Dispose()
        End Try
        Return "Successful"
    End Function

    Public Function GetReport(UID As Integer, EID As Integer) As String Implements IDMSNewService.GetReport
        Dim ConStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim con As New SqlConnection(ConStr)
        Dim oda As New SqlDataAdapter("", con)
        Dim ds As New DataSet
        oda.SelectCommand.CommandType = CommandType.Text
        Dim obj As New MailUtill(EID:=EID)

        Try
            oda.SelectCommand.CommandText = "select * from mmm_print_template where eid=" & EID & " and draft='Service' and sendtype='WS'"
            oda.Fill(ds, "data")
            For i = 0 To ds.Tables("data").Rows.Count - 1
                Dim EMailto As String = ds.Tables("data").Rows(i).Item("emailto").ToString
                Dim exlsub As String = ds.Tables("data").Rows(i).Item("ExlSubject").ToString()
                Dim strmsgBdy As String = ds.Tables("data").Rows(i).Item("ExlMailBody").ToString()
                Dim qry As String = ds.Tables("data").Rows(i).Item("qry").ToString
                Dim cc As String = ds.Tables("data").Rows(i).Item("cc").ToString()
                Dim bcc As String = ds.Tables("data").Rows(i).Item("bcc").ToString()
                Dim msg As String = ""
                qry = Replace(qry, "{UID}", UID)
                oda.SelectCommand.CommandText = qry
                oda.Fill(ds, "rpt")
                If strmsgBdy.Contains("LATLONG") Then
                    ds.Tables("rpt").Columns.Add("Location Address", Type.GetType("System.String"))
                    For j As Integer = 0 To ds.Tables("rpt").Rows.Count - 1
                        If ds.Tables("rpt").Rows(j).Item("GPS Location") = "" Then
                            ds.Tables("rpt").Rows(j).Item("Location Address") = "No Location Found"
                        Else
                            ds.Tables("rpt").Rows(j).Item("Location Address") = Location(ds.Tables("rpt").Rows(j).Item("GPS Location"))
                            ds.Tables("rpt").Rows(j).Item("Location Address") = ds.Tables("rpt").Rows(j).Item("Location Address").ToString.Replace(",", " ")
                        End If

                    Next
                    ds.Tables("rpt").Columns.Remove("GPS Location")
                    strmsgBdy = strmsgBdy.Replace("LATLONG", "")
                End If

                If ds.Tables("rpt").Rows.Count > 0 Then
                    Dim MailTable As New StringBuilder()
                    MailTable.Append("<table border=""1"" width=""100%"">")
                    MailTable.Append("<tr style=""background-color:#990000"" Font-Bold=""True""> ")
                    For l As Integer = 0 To ds.Tables("rpt").Columns.Count - 1
                        MailTable.Append("<td >" & ds.Tables("rpt").Columns(l).ColumnName & "</td>")
                    Next
                    For m As Integer = 0 To ds.Tables("rpt").Rows.Count - 1 ' binding the tr tab in table
                        MailTable.Append("</tr><tr>") ' for row records
                        For t As Integer = 0 To ds.Tables("rpt").Columns.Count - 1
                            MailTable.Append("<td>" & ds.Tables("rpt").Rows(m).Item(t).ToString() & " </td>")
                        Next
                        MailTable.Append("</tr>")
                    Next
                    MailTable.Append("</table>")
                    If strmsgBdy.Contains("@body") Then
                        strmsgBdy = Replace(strmsgBdy, "@body", MailTable.ToString())
                        msg = strmsgBdy
                    Else
                        msg = MailTable.ToString()
                    End If
                    msg = strmsgBdy
                    obj.SendMail(ToMail:=EMailto, Subject:=exlsub, MailBody:=msg, CC:=cc, BCC:=bcc)
                    ds.Tables("rpt").Rows.Clear()
                    ds.Tables("rpt").Columns.Clear()
                    ds.Tables("rpt").Clear()
                    strmsgBdy = ""
                End If
            Next

        Catch ex As Exception
            oda.SelectCommand.CommandText = "INSERT_ERRORLOG"
            oda.SelectCommand.CommandType = CommandType.StoredProcedure
            oda.SelectCommand.Parameters.Clear()
            oda.SelectCommand.Parameters.AddWithValue("@ERRORMSG", ex.ToString())
            oda.SelectCommand.Parameters.AddWithValue("@ALERTNAME", "GetReport_NewService")
            oda.SelectCommand.Parameters.AddWithValue("@EID", EID)
            If con.State <> ConnectionState.Open Then
                con.Open()
            End If
            oda.SelectCommand.ExecuteNonQuery()

        Finally
            con.Close()
            con.Dispose()
            oda.Dispose()
        End Try
        Return "Success"
    End Function
    Public Function Location(latlong As String) As String
        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim con As SqlConnection = New SqlConnection(conStr)
        Dim oda As SqlDataAdapter = New SqlDataAdapter("", con)
        Try
            Dim crsts() As String = latlong.Split(",")
            Dim lat As String = crsts(0)
            Dim log As String = crsts(1)

            oda.SelectCommand.CommandText = "select top 1 * from gpsLocation with(nolock) where Lat_start  <=" + lat + " and  lat_end >= " + lat + " and long_start <= " + log + " and long_end >= " + log + " "
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
    Function JobScheduler(ByVal FTid As Integer) As Boolean
        Dim b As Boolean = False
        Try
            Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
            Dim con As SqlConnection = New SqlConnection(conStr)
            Dim time1 As DateTime = Now.AddMinutes(-5).ToShortTimeString()
            Dim time2 As DateTime = Now.AddMinutes(+5).ToShortTimeString()
            Dim da As New SqlDataAdapter("select HH,MM,JobScheduleTime from mmm_mst_Jobscheduler where tid=" & FTid, con)
            'Dim da As New SqlDataAdapter("select HH,MM,ScheduleTime from oit_mst_inputfile where tid=44" & FTid, con)
            Dim dt As New DataTable()
            da.Fill(dt)
            Dim i As Integer = 0
            ' Dim date1f As String 
            If dt.Rows.Count = 1 Then
                If (dt.Rows(0).Item("JobScheduleTime").ToString() <> "") Then
                    'Dim schedule As String = "*|*|*|*|*"
                    Dim schedule As String = dt.Rows(0).Item("JobScheduleTime").ToString()
                    Dim str_array As [String]() = schedule.Split("|")
                    Dim stringa As [String] = str_array(0)
                    Dim stringb As [String] = str_array(1)
                    Dim stringc As [String] = str_array(2)
                    Dim stringd As [String] = str_array(3)
                    Dim stringe As [String] = str_array(4)
                    Dim currentTime As System.DateTime = System.DateTime.Now
                    Dim currentdate As String = currentTime.Date.Day
                    Dim str_datte As [String]() = stringb.Split(",")
                    Dim str_hours As [String]() = stringd.Split(",")
                    Dim str_mintus As [String]() = stringe.Split(",")

                    If Trim(stringa) = "*" And Trim(stringb) = "*" And Trim(stringc) = "*" And Trim(stringd) = "*" And Trim(stringe) = "*" Then
                        b = True
                        Return b
                        ' Exit For
                    End If

                    If stringb <> "*" Then
                        For j As Integer = 0 To str_datte.Length - 1
                            Dim A As [String] = str_datte(j)
                            If A.Contains("-") Then
                                Dim o As [String]() = A.Split("-")
                                Dim f As [String] = o(0)
                                Dim g As [String] = o(1)
                                If (f <= currentdate And g >= currentdate) Then
                                    For n As Integer = 0 To str_hours.Length - 1
                                        For m As Integer = 0 To str_mintus.Length - 1
                                            Dim x As DateTime = (Convert.ToDateTime(str_hours(n) & ":" & str_mintus(m) & ":" & "00").ToShortTimeString)
                                            If x <= time2 And x >= time1 Then
                                                b = True
                                                Exit For
                                            End If
                                        Next
                                    Next
                                End If
                            Else
                                If ((currentdate = A)) Then
                                    For n As Integer = 0 To str_hours.Length - 1
                                        For m As Integer = 0 To str_mintus.Length - 1
                                            Dim x As DateTime = (Convert.ToDateTime(str_hours(n) & ":" & str_mintus(m) & ":" & "00").ToShortTimeString)
                                            If x <= time2 And x >= time1 Then
                                                b = True
                                                Exit For
                                            End If
                                        Next
                                    Next
                                End If
                            End If
                        Next
                    ElseIf ((currentdate = stringb) Or (stringb.Trim() = "*") Or (stringd.Trim() <> "") Or (stringe.Trim() <> "")) Then
                        For n As Integer = 0 To str_hours.Length - 1
                            For m As Integer = 0 To str_mintus.Length - 1
                                Dim x As DateTime = (Convert.ToDateTime(str_hours(n) & ":" & str_mintus(m) & ":" & "00").ToShortTimeString)
                                '  Dim x As DateTime = (Convert.ToDateTime(stringd & ":" & stringe & ":" & "00").ToShortTimeString)
                                If x <= time2 And x >= time1 Then
                                    b = True
                                    Exit For
                                End If
                            Next
                        Next
                    ElseIf ((stringa.Trim() = "*") Or (stringb.Trim() = "*") Or (stringc.Trim() = "*") Or (stringd.Trim() = "*") Or (stringe.Trim() = "*")) Then
                        b = True
                        'Exit For
                    End If
                End If
            End If
            con.Close()
            con.Dispose()
            da.Dispose()
            dt.Dispose()
            Return b
        Catch ex As Exception
            ' UpdateErrorLog("TryCatch", "TC", "Exception found in Scheduler(Ftid) MSG - " & ex.Message.ToString, "1", "TC")
            ' Label1.Text = "Exception found in cushman_varroc automation()"
        End Try
    End Function

    Public Function MissedCallDoc(skey As String) As String Implements IDMSNewService.MissedCallDoc
        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim con As SqlConnection = New SqlConnection(conStr)
        Dim oda As SqlDataAdapter = New SqlDataAdapter("", con)
        Dim ds As New DataSet()
        Try
            If Not skey = "MCD@123" Then
                Return "Invalid key"
            End If
            oda.SelectCommand.CommandText = "select * from  MMM_MST_JobScheduler with(nolock) where EID=66 and isOnOff=1 "
            oda.SelectCommand.CommandType = CommandType.Text
            oda.Fill(ds, "rpt")
            For d As Integer = 0 To ds.Tables("rpt").Rows.Count - 1
                If JobScheduler(ds.Tables("rpt").Rows(d).Item("tid")) = True Then
                    Dim obj As New MissedCallDocumentCreation
                    obj.CreateDocument()
                    oda.SelectCommand.CommandText = "update MMM_MST_JobScheduler set LastJobScheduledDate=getdate() where tid=" & ds.Tables("rpt").Rows(d).Item("tid") & ""
                    If con.State <> ConnectionState.Open Then
                        con.Open()
                    End If
                    oda.SelectCommand.ExecuteNonQuery()
                End If
            Next
            Return "Done"
        Catch ex As Exception
            oda.SelectCommand.Parameters.Clear()
            oda.SelectCommand.CommandText = "INSERT_ERRORLOG"
            oda.SelectCommand.CommandType = CommandType.StoredProcedure
            oda.SelectCommand.Parameters.AddWithValue("@ERRORMSG", ex.ToString())
            oda.SelectCommand.Parameters.AddWithValue("@ALERTNAME", "MissedCallScheduledBasedDoc")
            oda.SelectCommand.Parameters.AddWithValue("@EID", 66)
            If con.State <> ConnectionState.Open Then
                con.Open()
            End If
            oda.SelectCommand.ExecuteNonQuery()
        Finally
            con.Close()
            con.Dispose()
        End Try
        Return ""
    End Function

    Public Function MailAlertGeofenceBased() As String Implements IDMSNewService.MailAlertGeofenceBased        'Pallavi
        Dim constr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim con As SqlConnection = New SqlConnection(constr)
        Dim da As SqlDataAdapter = New SqlDataAdapter(" ", con)
        Dim ds As New DataSet()
        Dim Outside As Integer = 0
        Dim Inside As Integer = 0
        Dim InTime As String = ""
        Dim OutTime As String = ""
        Dim InMsg As String = ""
        Dim InMsgCode As String = ""
        Dim OutMsg As String = ""
        Dim OutMsgCode As String = ""
        Try
            da.SelectCommand.CommandText = "select * from mmm_mst_reportscheduler with(nolock) where AlertType='GPS Mail Alert' and  isactive=0  order by hh,mm,ordering"
            da.Fill(ds, "Scheduler")
            For i As Integer = 0 To ds.Tables("Scheduler").Rows.Count - 1 ' Report Loop Start
                Dim dtEmail As New DataTable()
                dtEmail.Columns.Add("Inside")
                dtEmail.Columns.Add("InTime")
                dtEmail.Columns.Add("EmailInMsg")
                dtEmail.Columns.Add("EmailInSubject")
                dtEmail.Columns.Add("Outside")
                dtEmail.Columns.Add("OutTime")
                dtEmail.Columns.Add("EmailOutMsg")
                dtEmail.Columns.Add("EmailOutSubject")
                dtEmail.Columns.Add("EmailAddress")
                dtEmail.Columns.Add("EmailAddressCC")
                dtEmail.Columns.Add("EmailAddressBCC")
                Dim Emailmsgbody As String = ds.Tables("Scheduler").Rows(i).Item("msgbody")
                Dim Eid As Integer = Convert.ToInt32(ds.Tables("Scheduler").Rows(i).Item("Eid"))
                da.SelectCommand.CommandText = "Select * from mmm_mst_Entity with(nolock) where Eid=" & Eid
                da.Fill(ds, "Entity")
                Dim EntityName As String = Convert.ToString(ds.Tables("Entity").Rows(i).Item("Name"))
                Dim Sitedoc As String = Convert.ToString(ds.Tables("Entity").Rows(i).Item("SiteDoc"))
                Dim SiteNamefld As String = Convert.ToString(ds.Tables("Entity").Rows(i).Item("SiteNamefld"))
                Dim SiteFencefld As String = Convert.ToString(ds.Tables("Entity").Rows(i).Item("SiteFencefld"))
                Dim SiteMobilefld As String = Convert.ToString(ds.Tables("Entity").Rows(i).Item("SiteMobilefld"))
                Dim SiteCompanyfld As String = Convert.ToString(ds.Tables("Entity").Rows(i).Item("SiteCompanyfld"))
                Dim SiteAddressfld As String = Convert.ToString(ds.Tables("Entity").Rows(i).Item("SiteAddressfld"))
                Dim SiteCustNmfld As String = Convert.ToString(ds.Tables("Entity").Rows(i).Item("SiteCustNmfld"))
                Dim ClusterCodefld As String = Convert.ToString(ds.Tables("Entity").Rows(i).Item("ClusterCodefld"))
                Dim VehicleDoc As String = Convert.ToString(ds.Tables("Entity").Rows(i).Item("VehicleDoc"))
                Dim VehicleNofld As String = Convert.ToString(ds.Tables("Entity").Rows(i).Item("VehicleNofld"))
                Dim IMEIfld As String = Convert.ToString(ds.Tables("Entity").Rows(i).Item("IMEIfld"))
                Dim VehicleCompanyfld As String = Convert.ToString(ds.Tables("Entity").Rows(i).Item("VehicleCompanyfld"))
                Dim ExtendedSettings As String = Convert.ToString(ds.Tables("Entity").Rows(i).Item("ExtendedSetting"))
                Dim VehicleUserName As String = Convert.ToString(ds.Tables("Entity").Rows(i).Item("VehicleUserNmfld"))
                Dim SiteEmailIDfld As String = Convert.ToString(ds.Tables("Entity").Rows(i).Item("SiteEmailfld"))
                Dim SiteEmailCCfld As String = Convert.ToString(ds.Tables("Entity").Rows(i).Item("SiteEmailCCfld"))
                Dim SiteEmailBCCfld As String = Convert.ToString(ds.Tables("Entity").Rows(i).Item("SiteEmailBCCfld"))
                Dim SiteEmailType As String = Convert.ToString(ds.Tables("Scheduler").Rows(i).Item("GPSEmailType"))
                If Not String.IsNullOrEmpty(ExtendedSettings) Then
                    Try
                        da.SelectCommand.CommandText = "Select  Tid from MMM_MST_Master with(nolock) where EId=" & Eid & " and Documenttype='" & (ExtendedSettings.Split("_"))(0).ToString & "' and IsAuth=1 "
                        da.Fill(ds, "Company")
                        For c As Integer = 0 To ds.Tables("Company").Rows.Count - 1 'Company Loop Start
                            da.SelectCommand.CommandText = "Select  Tid," & VehicleNofld & ", " & IMEIfld & ", " & VehicleCompanyfld & "," & VehicleUserName & " from  MMM_MST_MASTER with(nolock) where Documenttype='" & VehicleDoc & "' and Eid=" & Eid & " and IsAuth=1 and " & VehicleCompanyfld & "='" & ds.Tables("Company").Rows(c).Item("Tid") & "'"
                            da.Fill(ds, "Vehicles")
                            da.SelectCommand.CommandText = "select  Tid, " & SiteNamefld & "," & SiteFencefld & "," & SiteMobilefld & "," & SiteCompanyfld & "," & SiteEmailIDfld & " from MMM_MST_Master with(nolock) where EId=" & Eid & " and Documenttype='" & Sitedoc & "' and IsAuth=1 and " & SiteCompanyfld & "='" & ds.Tables("Company").Rows(c).Item("Tid") & "' and  isnull(" & SiteFencefld & ",' ') <> ' ' and " & SiteFencefld & " <> ',' and len(" & SiteFencefld & ")>2 "
                            da.Fill(ds, "Sites")
                            For v As Integer = 0 To ds.Tables("Vehicles").Rows.Count - 1 'Vehicle Loop Start
                                dtEmail.Clear()
                                Dim IMEI As String = ds.Tables("Vehicles").Rows(v).Item(IMEIfld)
                                Dim phone As String = ""
                                Dim EmailID As String = ""
                                Dim VehicleUserNm As String = Convert.ToString(ds.Tables("Vehicles").Rows(v).Item(VehicleUserName))
                                InTime = ""
                                OutTime = ""
                                Inside = 0
                                Outside = 0
                                For s As Integer = 0 To ds.Tables("Sites").Rows.Count - 1 'Site Loop Start
                                    phone = Convert.ToString(ds.Tables("Sites").Rows(s).Item(SiteMobilefld))
                                    EmailID = Convert.ToString(ds.Tables("Sites").Rows(s).Item(SiteEmailIDfld))
                                    Emailmsgbody = ds.Tables("Scheduler").Rows(i).Item("msgbody")
                                    If (Regex.IsMatch(EmailID, "\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*")) Then
                                        Dim SITEID As String = ds.Tables("Sites").Rows(s).Item("Tid")
                                        Dim arr() As String = ds.Tables("Sites").Rows(s).Item(SiteFencefld).ToString.Split(",")
                                        Dim fence As String
                                        If (arr.Length < 3) Then
                                            fence = "200"
                                        Else : fence = arr(2)
                                        End If
                                        da.SelectCommand.CommandText = "select isnull( max(convert(varchar,ctime,22)),'')[ctime] from mmm_mst_gpsdata with(nolock)" &
                                            " where imieno='" & ds.Tables("Vehicles").Rows(v).Item(IMEIfld) & "' and DATEADD(minute,DATEDIFF(minute,0,ctime),0)>=DATEADD(minute,DATEDIFF(minute,0,'" &
                                            ds.Tables("Scheduler").Rows(i).Item("lastscheduleddate").ToString & "'),0) and DATEADD(minute,DATEDIFF(minute,0,ctime),0)<=DATEADD(minute,DATEDIFF(minute,0,GETDATE()),0) " &
                                            " and  dms.CalculateDistanceInMeter (" & arr(0) & "," & arr(1) & ",lattitude,longitude) < " & arr(2)

                                        If con.State <> ConnectionState.Open Then
                                            con.Open()
                                        End If

                                        Dim ctime As String = Convert.ToString(da.SelectCommand.ExecuteScalar())
                                        con.Close()
                                        If ctime <> Nothing And ctime <> "" Then 'If exist within fence ctime is not empty

                                            da.SelectCommand.CommandText = "Select [FenceTidEmail] from  mmm_mst_master with(nolock) where Eid=" & Eid & " and documenttype='" & VehicleDoc & "' and " & VehicleCompanyfld & "='" & ds.Tables("Company").Rows(c).Item("Tid") & "' and " & IMEIfld & "='" & IMEI & "'"

                                            If con.State <> ConnectionState.Open Then
                                                con.Open()
                                            End If

                                            Dim FenceIDEmail As String = Convert.ToString(da.SelectCommand.ExecuteScalar())

                                            If (SiteEmailType <> "Out") Then
                                                If String.IsNullOrEmpty(FenceIDEmail) Then
                                                    Inside = Inside + 1
                                                    InTime = ctime
                                                    Dim qry As String = "update mmm_mst_master set FenceTidEmail='" & SITEID & "' where Eid=" & Eid & " and documenttype='" & VehicleDoc & "' and " & VehicleCompanyfld & "='" & ds.Tables("Company").Rows(c).Item("Tid") & "' and " & IMEIfld & "='" & IMEI & "' "
                                                    Dim cmd As New SqlCommand(qry, con)
                                                    If con.State <> ConnectionState.Open Then
                                                        con.Open()
                                                    End If
                                                    Dim rowAff = cmd.ExecuteNonQuery()
                                                    con.Close()

                                                    Dim dr = dtEmail.NewRow()
                                                    dr.Item("Inside") = Inside + 1
                                                    dr.Item("InTime") = ctime
                                                    dr.Item("EmailInSubject") = "Mail Alert!!"
                                                    Emailmsgbody = Emailmsgbody.Replace("@Name", "(" & VehicleUserNm & ")")
                                                    Emailmsgbody = Emailmsgbody.Replace("@ReportType", "reached")
                                                    Emailmsgbody = Emailmsgbody.Replace("@SiteName", Convert.ToString(ds.Tables("Sites").Rows(s).Item(SiteNamefld)))
                                                    Emailmsgbody = Emailmsgbody.Replace("@Location", "")
                                                    Emailmsgbody = Emailmsgbody.Replace("@Datetime", InTime)
                                                    Emailmsgbody = Emailmsgbody.Replace("@EntityName", EntityName & " Team")
                                                    dr.Item("EmailInMsg") = Emailmsgbody
                                                    dr.Item("EmailAddress") = EmailID
                                                    dtEmail.Rows.Add(dr)

                                                End If
                                            End If

                                        Else ' If ctime is nothing or empty means out of fence
                                            da.SelectCommand.CommandText = "select isnull( max(convert(varchar,ctime,22)),'')[ctime] from mmm_mst_gpsdata with(nolock)" &
                                           " where imieno='" & ds.Tables("Vehicles").Rows(v).Item(IMEIfld) & "' and DATEADD(minute,DATEDIFF(minute,0,ctime),0)>=DATEADD(minute,DATEDIFF(minute,0,'" & ds.Tables("Scheduler").Rows(i).Item("lastscheduleddate").ToString & "'),0) and DATEADD(minute,DATEDIFF(minute,0,ctime),0)<=DATEADD(minute,DATEDIFF(minute,0,GETDATE()),0) " &
                                           " and  dms.CalculateDistanceInMeter (" & arr(0) & "," & arr(1) & ",lattitude,longitude) > " & arr(2)

                                            If con.State <> ConnectionState.Open Then
                                                con.Open()
                                            End If

                                            Dim ctimeout As String = Convert.ToString(da.SelectCommand.ExecuteScalar())
                                            If (ctimeout <> Nothing And ctimeout <> "") Then

                                                If (SiteEmailType <> "In") Then
                                                    da.SelectCommand.CommandText = "Select [FenceTidEmail] from  mmm_mst_master with(nolock) where Eid=" & Eid & " and documenttype='" & VehicleDoc & "' and " & VehicleCompanyfld & "='" & ds.Tables("Company").Rows(c).Item("Tid") & "' and " & IMEIfld & "='" & IMEI & "'"
                                                    If con.State <> ConnectionState.Open Then
                                                        con.Open()
                                                    End If
                                                    Dim FenceIDEmail As String = Convert.ToString(da.SelectCommand.ExecuteScalar())
                                                    If Not String.IsNullOrEmpty(FenceIDEmail) Then
                                                        Outside = Outside + 1
                                                        OutTime = DateTime.Now
                                                        Dim qry As String = "update mmm_mst_master set FenceTidEmail=null where Eid=" & Eid & " and documenttype='" & VehicleDoc & "' and " & VehicleCompanyfld & "='" & ds.Tables("Company").Rows(c).Item("Tid") & "' and " & IMEIfld & "='" & IMEI & "' "
                                                        Dim cmd As New SqlCommand(qry, con)
                                                        If con.State <> ConnectionState.Open Then
                                                            con.Open()
                                                        End If
                                                        Dim rowAff = cmd.ExecuteNonQuery()
                                                        con.Close()
                                                        Dim dr = dtEmail.NewRow()
                                                        dr.Item("Outside") = Outside + 1
                                                        dr.Item("OutTime") = ctime
                                                        dr.Item("EmailOutSubject") = "Mail Alert!!"
                                                        Emailmsgbody = Emailmsgbody.Replace("@Name", "(" & VehicleUserNm & ")")
                                                        Emailmsgbody = Emailmsgbody.Replace("@ReportType", "departed")
                                                        Emailmsgbody = Emailmsgbody.Replace("@SiteName", Convert.ToString(ds.Tables("Sites").Rows(s).Item(SiteNamefld)))
                                                        Emailmsgbody = Emailmsgbody.Replace("@Location", "")
                                                        Emailmsgbody = Emailmsgbody.Replace("@Datetime", OutTime)
                                                        Emailmsgbody = Emailmsgbody.Replace("@EntityName", EntityName & " Team")
                                                        dr.Item("EmailOutMsg") = Emailmsgbody
                                                        dr.Item("EmailAddress") = EmailID
                                                        dtEmail.Rows.Add(dr)

                                                    End If
                                                End If

                                            End If
                                        End If
                                    End If
                                Next  'Sites Next

                                Dim mailutil As MailUtill = New MailUtill(Eid)

                                For z As Integer = 0 To dtEmail.Rows.Count - 1
                                    If Not IsDBNull(dtEmail.Rows(z).Item("InTime")) Then
                                        mailutil.SendMail(dtEmail.Rows(z).Item("EmailAddress").ToString(), dtEmail.Rows(z).Item("EmailInSubject").ToString(), dtEmail.Rows(z).Item("EmailInMsg").ToString(), dtEmail.Rows(z).Item("EmailAddressCC").ToString(), "", dtEmail.Rows(z).Item("EmailAddressBCC").ToString())
                                    Else
                                        mailutil.SendMail(dtEmail.Rows(z).Item("EmailAddress").ToString(), dtEmail.Rows(z).Item("EmailOutSubject").ToString(), dtEmail.Rows(z).Item("EmailOutMsg").ToString(), dtEmail.Rows(z).Item("EmailAddressCC").ToString(), "", dtEmail.Rows(z).Item("EmailAddressBCC").ToString())
                                    End If
                                Next

                                dtEmail.Clear()
                            Next 'vehicle Next

                            da.SelectCommand.CommandText = "update mmm_mst_reportscheduler set lastscheduleddate=getdate() where eid=" & Eid & " and tid=" & ds.Tables("Scheduler").Rows(i).Item("tid").ToString() & ""
                            If con.State <> ConnectionState.Open Then
                                con.Open()
                            End If
                            da.SelectCommand.ExecuteNonQuery()
                            con.Close()

                        Next 'Company Next
                    Catch ex As Exception
                    Finally
                        da.SelectCommand.CommandText = "update mmm_mst_reportscheduler set lastscheduleddate=getdate() where eid=" & Eid & " and tid=" & ds.Tables("Scheduler").Rows(i).Item("tid").ToString() & ""
                        If con.State <> ConnectionState.Open Then
                            con.Open()
                        End If
                        da.SelectCommand.ExecuteNonQuery()
                        con.Close()
                    End Try
                Else 'Extended settings not available
                    Try

                        If (ClusterCodefld <> "") Then
                            da.SelectCommand.CommandText = "Select  Tid, " & VehicleNofld & ", " & IMEIfld & " ," & VehicleUserName & ",dms.udf_split('MASTER-Cluster Master-fld1'," & ClusterCodefld & ") [" & ClusterCodefld.TrimEnd() & "] from  MMM_MST_Master with(nolock) where Documenttype='" & VehicleDoc & "' and Eid=" & Eid & " and IsAuth=1 "
                        Else
                            da.SelectCommand.CommandText = "Select  Tid, " & VehicleNofld & ", " & IMEIfld & " ," & VehicleUserName & " from  MMM_MST_Master with(nolock) where Documenttype='" & VehicleDoc & "' and Eid=" & Eid & " and IsAuth=1 "
                        End If

                        If (ds.Tables.Contains("Vehicles")) Then
                            ds.Tables("Vehicles").Clear()
                        End If
                        da.Fill(ds, "Vehicles")
                        If (SiteAddressfld <> "" And SiteCustNmfld <> "" And SiteEmailCCfld <> "" And SiteEmailBCCfld <> "") Then
                            da.SelectCommand.CommandText = "select  Tid, " & SiteNamefld & "," & SiteFencefld & "," & SiteMobilefld & "," & SiteEmailIDfld & "," & SiteAddressfld & "," & SiteEmailCCfld & "," & SiteEmailBCCfld & ",dms.udf_split('MASTER-Customer-fld10'," & SiteCustNmfld & ") [" & SiteCustNmfld & "] from MMM_MST_Master with(nolock) where EId=" & Eid & " and Documenttype='" & Sitedoc & "' and IsAuth=1 and    isnull(" & SiteFencefld & ",' ') <> ' ' and " & SiteFencefld & " <> ',' and len(" & SiteFencefld & ")>2 "
                        Else
                            da.SelectCommand.CommandText = "select  Tid, " & SiteNamefld & "," & SiteFencefld & "," & SiteMobilefld & "," & SiteEmailIDfld & " from MMM_MST_Master with(nolock) where EId=" & Eid & " and Documenttype='" & Sitedoc & "' and IsAuth=1 and    isnull(" & SiteFencefld & ",' ') <> ' ' and " & SiteFencefld & " <> ',' and len(" & SiteFencefld & ")>2 "
                        End If
                        If (ds.Tables.Contains("Sites")) Then
                            ds.Tables("Sites").Clear()
                        End If
                        da.Fill(ds, "Sites")

                        For v As Integer = 0 To ds.Tables("Vehicles").Rows.Count - 1 'Vehicle loop Start
                            Dim IMEI As String = ds.Tables("Vehicles").Rows(v).Item(IMEIfld)
                            Dim phone As String = ""
                            Dim EmailID As String = ""
                            Dim VehicleUserNm As String = Convert.ToString(ds.Tables("Vehicles").Rows(v).Item(VehicleUserName))
                            '28.5909595,77.1825745 Site lat Long used For testing securitas emails
                            '    If (IMEI = "352423060319041") Then 'Vehicle IMEI used for testing securitas emails
                            For s As Integer = 0 To ds.Tables("Sites").Rows.Count - 1 'Site Loop Start
                                InTime = ""
                                OutTime = ""
                                Inside = 0
                                Outside = 0
                                phone = Convert.ToString(ds.Tables("Sites").Rows(s).Item(SiteMobilefld))
                                Emailmsgbody = ds.Tables("Scheduler").Rows(i).Item("msgbody")
                                'EmailID = "rohan.arora@sequelmynd.com, ramco.support@securitas-india.com" 'commented by pallavi (lines for testing email scheduler)
                                EmailID = Convert.ToString(ds.Tables("Sites").Rows(s).Item(SiteEmailIDfld))
                                If (Regex.IsMatch(EmailID, "\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*")) Then
                                    Dim SITEID As String = ds.Tables("Sites").Rows(s).Item("Tid")
                                    Dim arr() As String = ds.Tables("Sites").Rows(s).Item(SiteFencefld).ToString.Split(",")
                                    Dim fence As String
                                    If (arr.Length < 2) Then
                                        Continue For
                                    End If
                                    If (arr.Length < 3) Then
                                        fence = "200"
                                    ElseIf arr(2) = "" Or arr(2) = Nothing Then
                                        fence = "200"
                                    Else : fence = arr(2)
                                    End If

                                    da.SelectCommand.CommandText = "select max(convert(varchar,ctime,22))[ctime] from mmm_mst_gpsdata with(nolock)" &
                                        " where imieno='" & ds.Tables("Vehicles").Rows(v).Item(IMEIfld) & "' and DATEADD(minute,DATEDIFF(minute,0,ctime),0)>=DATEADD(minute,DATEDIFF(minute,0,'" & ds.Tables("Scheduler").Rows(i).Item("lastscheduleddate").ToString & "'),0) and DATEADD(minute,DATEDIFF(minute,0,ctime),0)<=DATEADD(minute,DATEDIFF(minute,0,GETDATE()),0) " &
                                        " and  dms.CalculateDistanceInMeter (" & arr(0) & "," & arr(1) & ",lattitude,longitude) < " & fence & "  "

                                    If con.State <> ConnectionState.Open Then
                                        con.Open()
                                    End If

                                    Dim ctime As String = Convert.ToString(da.SelectCommand.ExecuteScalar())
                                    con.Close()

                                    If ctime <> Nothing Or ctime <> "" Then  ' If exist within fence ,ctime is not empty

                                        da.SelectCommand.CommandText = "Select [FenceTidEmail] from  mmm_mst_master with(nolock) where Eid=" & Eid & " and documenttype='" & VehicleDoc & "'  and " & IMEIfld & "='" & IMEI & "'"
                                        If con.State <> ConnectionState.Open Then
                                            con.Open()
                                        End If

                                        Dim FenceIDEmail As String = Convert.ToString(da.SelectCommand.ExecuteScalar())
                                        If (SiteEmailType <> "Out") Then

                                            If String.IsNullOrEmpty(FenceIDEmail) Then
                                                Inside = Inside + 1
                                                InTime = ctime
                                                Dim qry As String = "update mmm_mst_master  set FenceTidEmail='" & SITEID & "' where Eid=" & Eid & " and documenttype='" & VehicleDoc & "'  and " & IMEIfld & "='" & IMEI & "' "
                                                Dim cmd As New SqlCommand(qry, con)
                                                If con.State <> ConnectionState.Open Then
                                                    con.Open()
                                                End If
                                                Dim rowAff = cmd.ExecuteNonQuery()
                                                con.Close()
                                                Dim dr = dtEmail.NewRow()
                                                dr.Item("Inside") = Inside + 1
                                                dr.Item("InTime") = ctime
                                                dr.Item("EmailInSubject") = "Mail Alert!!"

                                                If (SiteCustNmfld = "") Then
                                                    Emailmsgbody = Emailmsgbody.Replace("@Name", "(" & VehicleUserNm & ")")
                                                Else
                                                    Emailmsgbody = Emailmsgbody.Replace("@Name", "(" & VehicleUserNm & ")" & "(" & ds.Tables("Vehicles").Rows(v).Item(ClusterCodefld).ToString() & ")")
                                                End If

                                                Emailmsgbody = Emailmsgbody.Replace("@ReportType", "reached")
                                                Emailmsgbody = Emailmsgbody.Replace("@SiteName", "(" & Convert.ToString(ds.Tables("Sites").Rows(s).Item(SiteNamefld)) & ")")

                                                If (SiteAddressfld <> "" And SiteCustNmfld <> "") Then
                                                    Emailmsgbody = Emailmsgbody.Replace("@Location", " " & ds.Tables("Sites").Rows(s).Item(SiteCustNmfld) & "," & ds.Tables("Sites").Rows(s).Item(SiteAddressfld))
                                                Else
                                                    Emailmsgbody = Emailmsgbody.Replace("@Location", "")
                                                End If

                                                Emailmsgbody = Emailmsgbody.Replace("@Datetime", InTime)
                                                Emailmsgbody = Emailmsgbody.Replace("@EntityName", EntityName & " Team")
                                                dr.Item("EmailInMsg") = Emailmsgbody

                                                If (Regex.IsMatch(Convert.ToString(ds.Tables("Sites").Rows(s).Item(SiteEmailCCfld)).ToString(), "\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*")) Then
                                                    dr.Item("EmailAddressCC") = Convert.ToString(ds.Tables("Sites").Rows(s).Item(SiteEmailCCfld)).ToString()
                                                Else
                                                    dr.Item("EmailAddressCC") = ""
                                                End If

                                                If (Regex.IsMatch(Convert.ToString(ds.Tables("Sites").Rows(s).Item(SiteEmailBCCfld)).ToString(), "\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*")) Then
                                                    dr.Item("EmailAddressBCC") = Convert.ToString(ds.Tables("Sites").Rows(s).Item(SiteEmailBCCfld)).ToString()
                                                Else
                                                    dr.Item("EmailAddressBCC") = ""
                                                End If

                                                dr.Item("EmailAddress") = EmailID
                                                dtEmail.Rows.Add(dr)
                                            End If
                                        End If

                                    Else

                                        da.SelectCommand.CommandText = "select isnull( min(convert(varchar,ctime,22)),'')[ctime] from mmm_mst_gpsdata with(nolock)" &
                                         " where imieno='" & ds.Tables("Vehicles").Rows(v).Item(IMEIfld) & "' and DATEADD(minute,DATEDIFF(minute,0,ctime),0)>=DATEADD(minute,DATEDIFF(minute,0,'" & ds.Tables("Scheduler").Rows(i).Item("lastscheduleddate").ToString & "'),0) and DATEADD(minute,DATEDIFF(minute,0,ctime),0)<=DATEADD(minute,DATEDIFF(minute,0,GETDATE()),0) " &
                                         " and  dms.CalculateDistanceInMeter (" & arr(0) & "," & arr(1) & ",lattitude,longitude) > " & fence

                                        If con.State <> ConnectionState.Open Then
                                            con.Open()
                                        End If

                                        Dim ctimeout As String = Convert.ToString(da.SelectCommand.ExecuteScalar())
                                        If (ctimeout <> Nothing And ctimeout <> "") Then
                                            If (SiteEmailType <> "In") Then 'If ctime is empty or nothing means out of fence
                                                da.SelectCommand.CommandText = "Select [FenceTidEmail] from  mmm_mst_master where Eid=" & Eid & " and documenttype='" & VehicleDoc & "'  and " & IMEIfld & "='" & IMEI & "'"
                                                If con.State <> ConnectionState.Open Then
                                                    con.Open()
                                                End If
                                                Dim FenceIDEmail As String = Convert.ToString(da.SelectCommand.ExecuteScalar())

                                                If Not String.IsNullOrEmpty(FenceIDEmail) Then
                                                    Outside = Outside + 1
                                                    OutTime = DateTime.Now
                                                    Dim qry As String = "update mmm_mst_master set FenceTidEmail=null where Eid=" & Eid & " and documenttype='" & VehicleDoc & "'  and " & IMEIfld & "='" & IMEI & "' "
                                                    Dim cmd As New SqlCommand(qry, con)
                                                    If con.State <> ConnectionState.Open Then
                                                        con.Open()
                                                    End If
                                                    Dim rowAff = cmd.ExecuteNonQuery()
                                                    con.Close()
                                                    Dim dr = dtEmail.NewRow()
                                                    dr.Item("Outside") = Outside + 1
                                                    dr.Item("OutTime") = ctime
                                                    dr.Item("EmailOutSubject") = "Mail Alert!!"
                                                    If (SiteCustNmfld = "") Then
                                                        Emailmsgbody = Emailmsgbody.Replace("@Name", "(" & VehicleUserNm & ")")
                                                    Else
                                                        Emailmsgbody = Emailmsgbody.Replace("@Name", " " & VehicleUserNm & " " & "(" & ds.Tables("Vehicles").Rows(v).Item(ClusterCodefld).ToString() & ")")
                                                    End If
                                                    Emailmsgbody = Emailmsgbody.Replace("@ReportType", "departed")

                                                    If (SiteAddressfld <> "") Then
                                                        Emailmsgbody = Emailmsgbody.Replace("@Location", "," & ds.Tables("Sites").Rows(s).Item(SiteAddressfld))
                                                    Else
                                                        Emailmsgbody = Emailmsgbody.Replace("@Location", "")
                                                    End If

                                                    Emailmsgbody = Emailmsgbody.Replace("@SiteName", "(" & Convert.ToString(ds.Tables("Sites").Rows(s).Item(SiteNamefld)) & ")")
                                                    Emailmsgbody = Emailmsgbody.Replace("@Location", "")
                                                    Emailmsgbody = Emailmsgbody.Replace("@Datetime", ctimeout)
                                                    Emailmsgbody = Emailmsgbody.Replace("@EntityName", EntityName & " Team")
                                                    dr.Item("EmailOutMsg") = Emailmsgbody

                                                    If (Regex.IsMatch(Convert.ToString(ds.Tables("Sites").Rows(s).Item(SiteEmailCCfld)).ToString(), "\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*")) Then
                                                        dr.Item("EmailAddressCC") = Convert.ToString(ds.Tables("Sites").Rows(s).Item(SiteEmailCCfld)).ToString()
                                                    Else
                                                        dr.Item("EmailAddressCC") = ""
                                                    End If

                                                    If (Regex.IsMatch(Convert.ToString(ds.Tables("Sites").Rows(s).Item(SiteEmailBCCfld)).ToString(), "\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*")) Then
                                                        dr.Item("EmailAddressBCC") = Convert.ToString(ds.Tables("Sites").Rows(s).Item(SiteEmailBCCfld)).ToString()
                                                    Else
                                                        dr.Item("EmailAddressBCC") = ""
                                                    End If

                                                    dr.Item("EmailAddress") = EmailID
                                                    dtEmail.Rows.Add(dr)

                                                End If
                                            End If
                                        End If
                                    End If
                                End If
                            Next 'Sites Next
                            '  End If

                            Dim mailutil As MailUtill = New MailUtill(Eid)
                            For z As Integer = 0 To dtEmail.Rows.Count - 1
                                If Not IsDBNull(dtEmail.Rows(z).Item("InTime")) Then
                                    mailutil.SendMail(dtEmail.Rows(z).Item("EmailAddress").ToString(), dtEmail.Rows(z).Item("EmailInSubject").ToString(), dtEmail.Rows(z).Item("EmailInMsg").ToString(), dtEmail.Rows(z).Item("EmailAddressCC").ToString(), "", dtEmail.Rows(z).Item("EmailAddressBCC").ToString())
                                Else
                                    mailutil.SendMail(dtEmail.Rows(z).Item("EmailAddress").ToString(), dtEmail.Rows(z).Item("EmailOutSubject").ToString(), dtEmail.Rows(z).Item("EmailOutMsg").ToString(), dtEmail.Rows(z).Item("EmailAddressCC").ToString(), "", dtEmail.Rows(z).Item("EmailAddressbCC").ToString())
                                End If
                            Next
                            dtEmail.Clear()
                        Next 'vehicle Next

                        da.SelectCommand.CommandText = "update mmm_mst_reportscheduler set lastscheduleddate=getdate() where eid=" & Eid & " and tid=" & ds.Tables("Scheduler").Rows(i).Item("tid").ToString() & ""
                        If con.State <> ConnectionState.Open Then
                            con.Open()
                        End If
                        con.Close()
                    Catch ex As Exception
                    Finally
                        da.SelectCommand.CommandText = "update mmm_mst_reportscheduler set lastscheduleddate=getdate() where eid=" & Eid & " and tid=" & ds.Tables("Scheduler").Rows(i).Item("tid").ToString() & ""
                        If con.State <> ConnectionState.Open Then
                            con.Open()
                        End If
                        da.SelectCommand.ExecuteNonQuery()
                        con.Close()
                    End Try
                End If 'Extended settings End

            Next 'Report Next

        Catch ex As Exception


        End Try

        Return ""
    End Function

    Private Function GetJson(ByVal dt As DataTable) As String
        Dim Jserializer As New System.Web.Script.Serialization.JavaScriptSerializer()
        Dim rowsList As New List(Of Dictionary(Of String, Object))()
        Dim row As Dictionary(Of String, Object)
        For Each dr As DataRow In dt.Rows
            row = New Dictionary(Of String, Object)()
            For Each col As DataColumn In dt.Columns
                row.Add(col.ColumnName, dr(col))
            Next
            rowsList.Add(row)
        Next
        Jserializer.MaxJsonLength = 2147483647
        Return Jserializer.Serialize(rowsList)
    End Function
    Public Function GetSiteData(Skey As String, Eid As Integer, ltlng As String) As String Implements IDMSNewService.GetSiteData
        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim con As SqlConnection = New SqlConnection(conStr)
        Dim qry As String = ""
        Dim ltlg() As String
        ltlg = ltlng.Split(",")
        Dim lt = ltlg(0)
        Dim lng = ltlg(1)
        Try
            If Not Skey = "Y@FA" Then
                Return "Invalid key"
            End If
            'fld3 for sitlatlong
            '356307047664379
            qry = " ;with cte as( "
            qry &= " Select Tid,fld1,fld2,dms.CalculateDistanceInMeter((select  top 1 *  from dms.split(fld3,',')),(select  top 1 *  from dms.split(fld3,',') order by items desc)," & lt & "," & lng & ") dist "
            qry &= " from mmm_mst_Master where Eid='" & Eid & "' and documenttype='site master' and fld3 is not null) "
            qry &= " Select  top 1 isnull(TID,0)[TID],fld1[SiteID],fld2[SiteName] from cte order by dist "
            Dim da As SqlDataAdapter = New SqlDataAdapter(qry, con)
            Dim dt As New DataTable
            da.Fill(dt)
            If dt.Rows.Count > 0 Then
                Return GetJson(dt)
            Else
                Return "Data not found.."
            End If
        Catch ex As Exception
            Return ""
        Finally

        End Try
    End Function
End Class
