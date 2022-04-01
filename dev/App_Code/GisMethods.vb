Imports Microsoft.VisualBasic
Imports System.IO
Imports System.Data.SqlClient
Imports System.Data.OleDb
Imports System.Data
Imports System.Net
Imports Newtonsoft.Json.Linq
Imports System.Web.Script.Serialization
Imports System.Xml.Serialization
Imports System.Xml

Public Class GisMethods

    Shared conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString


    Public Shared Function GoogleGeoCodeFreeText1(Address As String) As ReturnAddress
        Dim objAddress As New ReturnAddress()
        Try
            Dim url As String = "http://maps.google.com/maps/api/geocode/xml?address=" + Address + "&sensor=false"
            Dim request As WebRequest = WebRequest.Create(url)
            Using response As WebResponse = DirectCast(request.GetResponse(), HttpWebResponse)
                Using reader As New StreamReader(response.GetResponseStream(), Encoding.UTF8)
                    Dim dsResult As New DataSet()
                    dsResult.ReadXml(reader)
                    If Not dsResult.Tables.Contains("Location") Then
                        objAddress.Success = False
                        Return objAddress
                    End If
                    If dsResult.Tables("Location").Rows.Count = 0 Then
                        objAddress.Success = False
                        Return objAddress
                    End If
                    objAddress.Latt = Convert.ToDouble(dsResult.Tables("Location").Rows(0).Item(0).ToString())
                    objAddress.Longt = Convert.ToDouble(dsResult.Tables("Location").Rows(0).Item(1).ToString())
                    objAddress.Success = True
                End Using
            End Using
            Return objAddress
        Catch ex As Exception
            objAddress.Success = False
            Return objAddress
        End Try
    End Function


    Public Shared Function ExecuteReverseGeoCoding(EID As String, DOCID As Integer, DocumentType As String) As String
        Dim ret As String = "Setting Not Found"
        Dim ds As New DataSet()
        Try
            Using con = New SqlConnection(conStr)
                Using da = New SqlDataAdapter("getAddressForRevGeoPoint", con)
                    da.SelectCommand.CommandType = CommandType.StoredProcedure
                    da.SelectCommand.Parameters.AddWithValue("@EID", EID)
                    da.SelectCommand.Parameters.AddWithValue("@DOCID", DOCID)
                    da.SelectCommand.Parameters.AddWithValue("@DocumentType", DocumentType)
                    da.Fill(ds)
                End Using
            End Using
            If (ds.Tables(0).Rows.Count > 0) Then
                Dim tableName As String = ""
                Dim Address As String = ""
                Dim FieldMapping As String = ""
                Dim LatLong As String = ""
                Dim GCodes As New ReturnAddress()
                tableName = Convert.ToString(ds.Tables(0).Rows(0).Item("tableName"))
                Address = Convert.ToString(ds.Tables(0).Rows(0).Item("Address"))
                FieldMapping = Convert.ToString(ds.Tables(0).Rows(0).Item("fieldmapping"))
                GCodes = GoogleGeoCodeFreeText1(Address)
                If (GCodes.Success) Then
                    LatLong = GCodes.Latt & "," & GCodes.Longt
                    Dim Sb As New StringBuilder()
                    Sb.Append("UPDATE ").Append(tableName).Append(" SET ").Append(FieldMapping).Append("='").Append(LatLong).Append("' WHERE EID=").Append(EID).Append(" AND tid=").Append(DOCID)
                    Dim StrQuery As String = Sb.ToString.Trim()
                    Using con1 = New SqlConnection(conStr)
                        Using da1 = New SqlDataAdapter(Sb.ToString.Trim, con1)
                            con1.Open()
                            Dim Count = da1.SelectCommand.ExecuteNonQuery()
                        End Using
                    End Using
                    ret = "Geo Point Uodated Successfully."
                Else
                    ret = "Geo Point Not Found"
                End If
            End If
        Catch ex As Exception
        End Try
        Return ret
    End Function



    Public Function GoogleGeoCodeFreeText(Address As String) As ReturnAddress
        Dim objAddress As New ReturnAddress()
        Try
            Dim url As String = "http://maps.google.com/maps/api/geocode/xml?address=" + Address + "&sensor=false"
            Dim request As WebRequest = WebRequest.Create(url)
            Using response As WebResponse = DirectCast(request.GetResponse(), HttpWebResponse)
                Using reader As New StreamReader(response.GetResponseStream(), Encoding.UTF8)
                    Dim dsResult As New DataSet()
                    dsResult.ReadXml(reader)
                    If Not dsResult.Tables.Contains("Location") Then
                        objAddress.Success = False
                        Return objAddress
                    End If
                    If dsResult.Tables("Location").Rows.Count = 0 Then
                        objAddress.Success = False
                        Return objAddress
                    End If
                    objAddress.Latt = Convert.ToDouble(dsResult.Tables("Location").Rows(0).Item(0).ToString())
                    objAddress.Longt = Convert.ToDouble(dsResult.Tables("Location").Rows(0).Item(1).ToString())
                    objAddress.Success = True
                End Using
            End Using
            Return objAddress
        Catch ex As Exception
            objAddress.Success = False
            Return objAddress
        End Try
    End Function

    Public Function HereGeoCodeFreeText(Address As String) As ReturnAddress
        Dim objAddress As New ReturnAddress()
        Try
            Dim url As String = "http://geocoder.cit.api.here.com/6.2/geocode.json?searchtext=" & Address & "&app_id=FhrHxdDSWojustuTPwwL&app_code=-DMrq8Tm98ut9TA3-wSnOA&gen=6"
            Dim request As System.Net.WebRequest = WebRequest.Create(url)
            Dim response As HttpWebResponse = request.GetResponse()
            If response.StatusCode = HttpStatusCode.OK Then
                Dim reader As StreamReader = New StreamReader(response.GetResponseStream())
                Dim rawresp As String
                rawresp = reader.ReadToEnd()
                Dim jResults As JObject = JObject.Parse(rawresp)
                Dim jo = JObject.Parse(jResults.ToString())
                If jo("Response")("View").ToArray().Count = 0 Then
                    objAddress.Success = False
                    Return objAddress
                End If
                Dim Lat As JToken
                Dim Longt As JToken
                Try
                    Lat = jo("Response")("View").ToArray()(0)("Result").ToArray()(0)("Location")("DisplayPosition")("Latitude")
                    Longt = jo("Response")("View").ToArray()(0)("Result").ToArray()(0)("Location")("DisplayPosition")("Longitude")
                    objAddress.Latt = Convert.ToDouble(Lat)
                    objAddress.Longt = Convert.ToDouble(Longt)
                Catch ex As Exception
                    objAddress.Success = False
                    Return objAddress
                End Try
            Else
                objAddress.Success = False
                Return objAddress
            End If
            Return objAddress
        Catch ex As Exception
            objAddress.Success = False
            Return objAddress
        End Try
    End Function

    Public Function GetDataFromExcel(ByVal strDataFilePath As String) As DataTable
        Try
            Dim sr As New StreamReader(HttpContext.Current.Server.MapPath("~/" & strDataFilePath))
            Dim fullFileStr As String = sr.ReadToEnd()
            sr.Close()
            sr.Dispose()
            Dim lines As String() = fullFileStr.Split(ControlChars.Lf)
            Dim recs As New DataTable()
            Dim sArr As String() = lines(0).Split("|"c)
            For Each s As String In sArr
                recs.Columns.Add(New DataColumn(s.Trim()))
            Next
            Dim row As DataRow
            Dim finalLine As String = ""
            Dim i As Integer = 0
            For Each line As String In lines
                If i > 0 And Not String.IsNullOrEmpty(line.Trim()) Then
                    row = recs.NewRow()
                    finalLine = line.Replace(Convert.ToString(ControlChars.Cr), "")
                    row.ItemArray = finalLine.Split("|"c)
                    recs.Rows.Add(row)
                End If
                i = i + 1
            Next
            Return recs
        Catch ex As Exception
            Throw ex
        End Try
    End Function

    Public Function GoogleAddress(Latt As Double, Longt As Double) As ReturnAddress
        Dim objAddress As New ReturnAddress()
        Try
            If Not IsNumeric(Latt) Or Not IsNumeric(Longt) Then
                objAddress.Success = False
                objAddress.Message = "Ivalid Latlong."
                Return objAddress
            End If
            Dim url As String = "http://maps.googleapis.com/maps/api/geocode/json?latlng=" & Latt.ToString() & "," & Longt.ToString() & "&sensor=true"
            Dim request As WebRequest = WebRequest.Create(url)
            Using response As WebResponse = DirectCast(request.GetResponse(), HttpWebResponse)

                Dim reader As StreamReader = New StreamReader(response.GetResponseStream())
                Dim rawresp As String
                rawresp = reader.ReadToEnd()
                Dim jResults As JObject = JObject.Parse(rawresp)
                Dim jo = JObject.Parse(jResults.ToString())

                'JavaScriptSerializer jss = new JavaScriptSerializer()
                Dim jss As New JavaScriptSerializer()
                ' Dim d = jss.Deserialize(Of GoogleGeoCodeResponse)(jResults.ToString())
                'Dim val = GetValue("administrative_area_level_1", jo)
                'Using reader As New StreamReader(response.GetResponseStream(), Encoding.UTF8)
                Dim dsResult As New DataSet()
                dsResult.ReadXml(reader)
                If Not dsResult.Tables.Contains("Location") Then
                    objAddress.Success = False
                    Return objAddress
                End If
                If dsResult.Tables("Location").Rows.Count = 0 Then
                    objAddress.Success = False
                    Return objAddress
                End If
                objAddress.Latt = Convert.ToDouble(dsResult.Tables("Location").Rows(0).Item(0).ToString())
                objAddress.Longt = Convert.ToDouble(dsResult.Tables("Location").Rows(0).Item(1).ToString())
                objAddress.Success = True
                'End Using
            End Using
            Return objAddress
        Catch ex As Exception
            objAddress.Success = False
            Return objAddress
        End Try
    End Function

    Public Function HereAddress(Latt As Double, Longt As Double) As ReturnAddress
        Dim objAddress As New ReturnAddress()
        Try
            If Not IsNumeric(Latt) Or Not IsNumeric(Longt) Then
                objAddress.Success = False
                objAddress.Message = "Ivalid Latlong."
                Return objAddress
            End If
            Dim url As String = "http://reverse.geocoder.cit.api.here.com/6.2/reversegeocode.xml?prox=" & Latt.ToString() & "," & Longt.ToString() & "&mode=retrieveAreas&gen=6&app_id=DemoAppId01082013GAL&app_code=AJKnXv84fjrb0KIHawS0Tg"
            Dim request As System.Net.WebRequest = WebRequest.Create(url)
            Dim response As HttpWebResponse = request.GetResponse()
            If response.StatusCode = HttpStatusCode.OK Then
                Dim reader As StreamReader = New StreamReader(response.GetResponseStream())
                Dim dsResult As New DataSet()
                dsResult.ReadXml(reader)

                Dim rawresp As String
                rawresp = reader.ReadToEnd()
                Dim jResults As JObject = JObject.Parse(rawresp)
                Dim jo = JObject.Parse(jResults.ToString())
                If jo("Response")("View").ToArray().Count = 0 Then
                    objAddress.Success = False
                    Return objAddress
                End If
                Dim Lat As JToken
                Dim Lng As JToken
                Try
                    Lat = jo("Response")("View").ToArray()(0)("Result").ToArray()(0)("Location")("DisplayPosition")("Latitude")
                    Lng = jo("Response")("View").ToArray()(0)("Result").ToArray()(0)("Location")("DisplayPosition")("Longitude")
                    objAddress.Latt = Convert.ToDouble(Lat)
                    objAddress.Longt = Convert.ToDouble(Lng)
                Catch ex As Exception
                    objAddress.Success = False
                    Return objAddress
                End Try
            Else
                objAddress.Success = False
                Return objAddress
            End If

            Return objAddress
        Catch ex As Exception
            objAddress.Success = False
            objAddress.Message = ex.Message
            Return objAddress
        End Try
    End Function

    Public Function IsPointInPolygon(PolyPoints As String, StrPoint As String) As Boolean
        Dim result As Boolean = False
        Dim polygon = CreatePloyCol(PolyPoints)
        Dim Point As New geopoint()
        Try
            Dim arr = StrPoint.Trim.Split(",")
            Point.X = arr(0)
            Point.Y = arr(1)
            Dim j As Integer = polygon.Count() - 1
            For i As Integer = 0 To polygon.Count() - 1
                If polygon(i).Y < Point.Y AndAlso polygon(j).Y >= Point.Y OrElse polygon(j).Y < Point.Y AndAlso polygon(i).Y >= Point.Y Then
                    If polygon(i).X + (Point.Y - polygon(i).Y) / (polygon(j).Y - polygon(i).Y) * (polygon(j).X - polygon(i).X) < Point.X Then
                        result = Not result
                    End If
                End If
                j = i
            Next
        Catch ex As Exception
            Return False
        End Try

        Return result
    End Function

    Private Function CreatePloyCol(points As String) As List(Of geopoint)
        Dim polygon As New List(Of geopoint)
        Dim arr = points.Split(",")
        Dim obj As geopoint
        For i As Integer = 0 To arr.Count - 1

            Try
                If i < arr.Length - 1 Then
                    If arr(i).Trim <> "" And arr(i + 1).Trim <> "" Then
                        obj = New geopoint()
                        obj.X = arr(i)
                        obj.Y = arr(i + 1)
                        polygon.Add(obj)
                    End If
                End If
            Catch ex As Exception

            End Try

            i = i + 1
        Next
        Return polygon
    End Function

    Public Function distance(Lat1 As Double, Long1 As Double, Lat2 As Double, Long2 As Double, unit As Char) As Double
        Dim dDistance As Double = [Double].MinValue
        Dim dLat1InRad As Double = Lat1 * (Math.PI / 180.0)
        Dim dLong1InRad As Double = Long1 * (Math.PI / 180.0)
        Dim dLat2InRad As Double = Lat2 * (Math.PI / 180.0)
        Dim dLong2InRad As Double = Long2 * (Math.PI / 180.0)

        Dim dLongitude As Double = dLong2InRad - dLong1InRad
        Dim dLatitude As Double = dLat2InRad - dLat1InRad

        ' Intermediate result a.
        Dim a As Double = Math.Pow(Math.Sin(dLatitude / 2.0), 2.0) + Math.Cos(dLat1InRad) * Math.Cos(dLat2InRad) * Math.Pow(Math.Sin(dLongitude / 2.0), 2.0)

        ' Intermediate result c (great circle distance in Radians).
        Dim c As Double = 2.0 * Math.Asin(Math.Sqrt(a))

        ' Distance.
        ' const Double kEarthRadiusMiles = 3956.0;
        Const kEarthRadiusKms As [Double] = 6376.5
        dDistance = kEarthRadiusKms * c

        Return dDistance
    End Function


    Public Shared Function HereDistanceMatrix(Sources As String, Destinations As String, DestinationCounts As Integer) As PossiblePaths
        Dim ResponseMatrix As Response = Nothing
        Dim ReturnObj As New PossiblePaths()
        Try
            Dim url As String = "http://route.st.nlp.nokia.com/routing/6.2/calculatematrix.json?app_id=DemoAppId01082013GAL&app_code=AJKnXv84fjrb0KIHawS0Tg&mode=fastest;car;traffic:enabled" & Sources & Destinations & ""
            Dim request As System.Net.WebRequest = WebRequest.Create(url)
            Dim response As HttpWebResponse = request.GetResponse()
            If response.StatusCode = HttpStatusCode.OK Then
                Dim reader As StreamReader = New StreamReader(response.GetResponseStream())
                Dim rawresp As String
                rawresp = reader.ReadToEnd()
                Dim jResults As JObject = JObject.Parse(rawresp)
                Dim jo = JObject.Parse(jResults.ToString())
                Dim ser As New System.Web.Script.Serialization.JavaScriptSerializer
                ResponseMatrix = ser.Deserialize(Of Response)(jo("Response").ToString())

                Dim iList As New List(Of DistanceMatrix)()
                For i As Short = 1 To ResponseMatrix.MatrixEntry.Length - 1
                    Dim ob As New DistanceMatrix()
                    ob.StartIndex = ResponseMatrix.MatrixEntry(i).StartIndex
                    ob.DestinationIndex = ResponseMatrix.MatrixEntry(i).DestinationIndex
                    ob.Distance = ResponseMatrix.MatrixEntry(i).Route.Summary.Distance
                    ob.BaseTime = ResponseMatrix.MatrixEntry(i).Route.Summary.BaseTime
                    iList.Add(ob)
                Next

                Dim iList1 = iList

                Dim dsPrev As New DistanceMatrix()
                dsPrev.StartIndex = 0
                dsPrev.DestinationIndex = 0
                Dim startPoint = 0
                Dim DistPathList As New List(Of DistanceMatrix)
                Dim TimePathList As New List(Of DistanceMatrix)


                For i As Integer = 0 To DestinationCounts - 1
                    Dim result As List(Of DistanceMatrix)
                    If i = 0 Then
                        result = iList.FindAll(Function(p As DistanceMatrix) p.StartIndex = 0)
                    Else
                        result = iList.FindAll(Function(p As DistanceMatrix) p.StartIndex = startPoint)
                    End If

                    Dim retObj = GetShortestPathByDistance(result.ToList(), startPoint, dsPrev)
                    DistPathList.Add(retObj.Clone())
                    If i = 0 Then
                        iList = iList.FindAll(Function(p As DistanceMatrix) Not p.StartIndex = 0)
                        iList = iList.FindAll(Function(p As DistanceMatrix) Not p.DestinationIndex = 0)
                    Else
                        iList = iList.FindAll(Function(p As DistanceMatrix) Not p.StartIndex = startPoint)
                        iList = iList.FindAll(Function(p As DistanceMatrix) Not p.DestinationIndex = startPoint)
                    End If
                    dsPrev = retObj
                    startPoint = retObj.DestinationIndex
                Next
                ReturnObj.DistancePath = DistPathList

                startPoint = 0
                iList = iList1
                dsPrev.StartIndex = 0
                dsPrev.DestinationIndex = 0
                dsPrev.BaseTime = 0

                For i As Integer = 0 To DestinationCounts - 1
                    Dim result As List(Of DistanceMatrix)
                    If i = 0 Then
                        result = iList.FindAll(Function(p As DistanceMatrix) p.StartIndex = 0)

                    Else
                        result = iList.FindAll(Function(p As DistanceMatrix) p.StartIndex = startPoint)

                    End If

                    Dim retObj = GetShortestPathByTime(result.ToList(), startPoint, dsPrev)
                    TimePathList.Add(retObj.Clone())
                    If i = 0 Then
                        iList = iList.FindAll(Function(p As DistanceMatrix) Not p.StartIndex = 0)
                        iList = iList.FindAll(Function(p As DistanceMatrix) Not p.DestinationIndex = 0)
                    Else
                        iList = iList.FindAll(Function(p As DistanceMatrix) Not p.StartIndex = startPoint)
                        iList = iList.FindAll(Function(p As DistanceMatrix) Not p.DestinationIndex = retObj.DestinationIndex)
                    End If
                    dsPrev = retObj
                    startPoint = retObj.DestinationIndex
                Next
                ReturnObj.TimePath = TimePathList

                Return ReturnObj
            End If
        Catch ex As Exception
            Return ReturnObj
        End Try
        Return ReturnObj
    End Function

    Public Shared Function GetShortestPathByDistance(ByVal iList As List(Of DistanceMatrix), start As Integer, startObj As DistanceMatrix) As DistanceMatrix
        Dim retObj As New DistanceMatrix()
        retObj.Distance = 10000000000
        'Dim ob1 = DirectCast(iList.Item(start), DistanceMatrix)
        Dim ob1 = startObj
        For i As Short = 0 To iList.Count - 1
            Dim ob = DirectCast(iList.Item(i), DistanceMatrix)
            If Not ob.DestinationIndex = start And Not ob.DestinationIndex = ob1.DestinationIndex Then
                If ob.Distance < retObj.Distance Then
                    retObj = ob
                End If
            End If
        Next
        Return retObj
    End Function

    Public Shared Function GetShortestPathByTime(ByVal iList As List(Of DistanceMatrix), start As Integer, startObj As DistanceMatrix) As DistanceMatrix
        Dim retObj As New DistanceMatrix()
        retObj.BaseTime = 10000000000
        'Dim ob1 = DirectCast(iList.Item(start), DistanceMatrix)
        Dim ob1 = startObj
        For i As Short = 0 To iList.Count - 1
            Dim ob = DirectCast(iList.Item(i), DistanceMatrix)
            If Not ob.DestinationIndex = start And Not ob.DestinationIndex = ob1.DestinationIndex Then
                If ob.BaseTime < retObj.BaseTime Then
                    retObj = ob
                End If
            End If
        Next
        Return retObj
    End Function


    Public Shared Function GetHereDistanceTimeMatrix(Sources As String, Destinations As String, VehicleType As String, Optional ByVal mode As String = "fastest", Optional ByVal traffic As String = "enabled") As List(Of DistanceMatrix)
        Dim ResponseMatrix As Response = Nothing
        Dim ReturnObj As New PossiblePaths()
        Dim iList As New List(Of DistanceMatrix)()
        Try
            Dim url As String = "http://route.st.nlp.nokia.com/routing/6.2/calculatematrix.json?app_id=DemoAppId01082013GAL&app_code=AJKnXv84fjrb0KIHawS0Tg&mode=" & mode & ";" & VehicleType & ";traffic:" & traffic & Sources & Destinations & ""
            Dim request As System.Net.WebRequest = WebRequest.Create(url)
            Dim response As HttpWebResponse = request.GetResponse()
            If response.StatusCode = HttpStatusCode.OK Then
                Dim reader As StreamReader = New StreamReader(response.GetResponseStream())
                Dim rawresp As String
                rawresp = reader.ReadToEnd()
                Dim jResults As JObject = JObject.Parse(rawresp)
                Dim jo = JObject.Parse(jResults.ToString())
                Dim ser As New System.Web.Script.Serialization.JavaScriptSerializer
                ResponseMatrix = ser.Deserialize(Of Response)(jo("Response").ToString())

                For i As Short = 1 To ResponseMatrix.MatrixEntry.Length - 1
                    Dim ob As New DistanceMatrix()
                    ob.StartIndex = ResponseMatrix.MatrixEntry(i).StartIndex
                    ob.DestinationIndex = ResponseMatrix.MatrixEntry(i).DestinationIndex
                    ob.Distance = ResponseMatrix.MatrixEntry(i).Route.Summary.Distance
                    ob.BaseTime = ResponseMatrix.MatrixEntry(i).Route.Summary.BaseTime
                    iList.Add(ob)
                Next
                Return iList
            End If
        Catch ex As Exception

        End Try
        Return iList
    End Function

    Public Function sendNotification(regId As String, Msg As String, Optional ByVal applicationID As String = "AIzaSyCqaT5MDZIX24NTvlXM8OWnW5lK1LQrIIo", Optional ByVal SENDER_ID As String = "947513119499") As String
        Dim tRequest As WebRequest
        tRequest = WebRequest.Create("https://android.googleapis.com/gcm/send")
        tRequest.Method = "post"
        tRequest.ContentType = "application/x-www-form-urlencoded;charset=UTF-8"
        tRequest.Headers.Add(String.Format("Authorization: key={0}", applicationID))
        tRequest.Headers.Add(String.Format("Sender: id={0}", SENDER_ID))
        'Dim postData = "{ 'registration_id': [ '" + regId + "' ], 'data': {'message': '" + Msg + "'}}"
        Dim postData As String = "collapse_key=score_update&time_to_live=108&delay_while_idle=1&data.message=" + Msg + "&data.time=" + System.DateTime.Now.ToString() + "&registration_id=" & regId + ""
        Console.WriteLine(postData)
        Dim byteArray As [Byte]() = Encoding.UTF8.GetBytes(postData)
        tRequest.ContentLength = byteArray.Length
        Dim dataStream As Stream = tRequest.GetRequestStream()
        dataStream.Write(byteArray, 0, byteArray.Length)
        dataStream.Close()
        Dim tResponse As WebResponse = tRequest.GetResponse()
        dataStream = tResponse.GetResponseStream()
        Dim tReader As New StreamReader(dataStream)
        Dim sResponseFromServer As [String] = tReader.ReadToEnd()
        tReader.Close()
        dataStream.Close()
        tResponse.Close()
        Return sResponseFromServer
    End Function

    ' prev
    'Public Function sendNotification(regId As String, Msg As String) As String
    '    Dim applicationID = "AIzaSyCqaT5MDZIX24NTvlXM8OWnW5lK1LQrIIo"
    '    Dim SENDER_ID = "947513119499"
    '    Dim tRequest As WebRequest
    '    tRequest = WebRequest.Create("https://android.googleapis.com/gcm/send")
    '    tRequest.Method = "post"
    '    tRequest.ContentType = " application/x-www-form-urlencoded;charset=UTF-8"
    '    tRequest.Headers.Add(String.Format("Authorization: key={0}", applicationID))

    '    tRequest.Headers.Add(String.Format("Sender: id={0}", SENDER_ID))

    '    'Dim postData = "{ 'registration_id': [ '" + regId + "' ], 'data': {'message': '" + Msg + "'}}"
    '    Dim postData As String = (Convert.ToString("collapse_key=score_update&time_to_live=108&delay_while_idle=1&data.message=" + Msg + "&data.time=" + System.DateTime.Now.ToString() + "&registration_id=") & regId) + ""

    '    Console.WriteLine(postData)

    '    Dim byteArray As [Byte]() = Encoding.UTF8.GetBytes(postData)
    '    tRequest.ContentLength = byteArray.Length

    '    Dim dataStream As Stream = tRequest.GetRequestStream()
    '    dataStream.Write(byteArray, 0, byteArray.Length)
    '    dataStream.Close()

    '    Dim tResponse As WebResponse = tRequest.GetResponse()

    '    dataStream = tResponse.GetResponseStream()

    '    Dim tReader As New StreamReader(dataStream)

    '    Dim sResponseFromServer As [String] = tReader.ReadToEnd()
    '    tReader.Close()
    '    dataStream.Close()
    '    tResponse.Close()
    '    Return sResponseFromServer
    'End Function


End Class

Public Class ReturnAddress
    Public Property Success As Boolean
    Public Property Message As String
    Public Property Latt As Double
    Public Property Longt As Double
    Public Property Address As String
    Public Property Country As String
    Public Property State As String
    Public Property City As String
    Public Property District As String
    Public Property Area As String
    Public Property locality As String
    Public Property political As String
    Public Property Street As String
    Public Property PostalCode As Integer
    Public Property AccuracyLevel As String
End Class

Public Class Summary
    Public Property Distance As Double
    Public Property BaseTime As Double
End Class

Public Class Route
    Public Property Summary As Summary
End Class

Public Class MatrixEntry
    Public Property StartIndex As Integer
    Public Property DestinationIndex As Integer
    Public Property Route As Route
End Class

Public Class Response
    Public Property MatrixEntry() As MatrixEntry()
End Class

Public Class DistanceMatrix
    Public Property StartIndex As Integer
    Public Property DestinationIndex As Integer
    Public Property Distance As Double
    Public Property BaseTime As Double
    Public Property AdditionalData As Object
    Public Function Clone() As DistanceMatrix
        Return DirectCast(Me.MemberwiseClone(), DistanceMatrix)
    End Function
End Class

Public Class PossiblePaths
    Public Property DistancePath As List(Of DistanceMatrix)
    Public Property TimePath As List(Of DistanceMatrix)
    Public Property StartLatt As Double
    Public Property StartLongt As Double
End Class

Public Class geopoint
    Public X As Decimal
    Public Y As Decimal
End Class