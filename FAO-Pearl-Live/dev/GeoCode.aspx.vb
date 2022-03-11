Imports System.IO
Imports System.Data.OleDb
Imports System.Data
Imports System.Net
Imports Newtonsoft.Json.Linq

Partial Class GeoCode
    Inherits System.Web.UI.Page
    Protected Sub btnGeoCode_Click(sender As Object, e As EventArgs) Handles btnGeoCode.Click


        Dim ConStr As String = ""
        Dim ext As String = Path.GetExtension(FileUpload1.FileName).ToLower()
        'string path = Server.MapPath("~/MyFolder/"+FileUpload1.FileName);  
        Dim path1 = Server.MapPath("~/Scripts/" + FileUpload1.FileName)
        FileUpload1.SaveAs(path1)
        If ext.Trim() = ".xls" Then
            ConStr = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + path1 + ";Extended Properties=""Excel 8.0;HDR=Yes;IMEX=2"""
        ElseIf ext.Trim() = ".xlsx" Then
            ConStr = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + path1 + ";Extended Properties=""Excel 12.0;HDR=Yes;IMEX=2"""
        End If

        Dim query As String = "SELECT * FROM [Sheet1$]"

        Dim conn As New OleDbConnection(ConStr)

        If conn.State = ConnectionState.Closed Then
            conn.Open()
        End If
        Dim cmd As New OleDbCommand(query, conn)

        Dim da As New OleDbDataAdapter(cmd)
        Dim ds As New DataSet()
        da.Fill(ds)
        conn.Close()

        If ds.Tables(0).Rows.Count > 0 Then
            GeoCode1(ds.Tables(0))
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
    Dim StrLocations As New StringBuilder()

    Private Sub GeoCode1(dt As DataTable)
        ' Dim con As SqlConnection = New SqlConnection(conStr)
        Try

            For i As Integer = 0 To dt.Rows.Count - 1

                Dim searchText As String = dt.Rows(i).Item("Address").ToString()
                If searchText.Trim = "" Then
                    Continue For
                End If
                Dim LatLng = GoogleGeoCode(searchText)
                If Not LatLng = "" Then
                    Dim arr As String() = LatLng.Split(",")
                    'StrLocations.Append("new mapsjs.geo.Point(" & arr(0).ToString & ", " & arr(1).ToString() & ")")
                    StrLocations.Append("" & arr(0).ToString & ", " & arr(1).ToString() & ":")
                Else
                    Try

                        Dim url As String = "http://geocoder.cit.api.here.com/6.2/geocode.json?searchtext=" & searchText & "&app_id=FhrHxdDSWojustuTPwwL&app_code=-DMrq8Tm98ut9TA3-wSnOA&gen=6"
                        Dim request As System.Net.WebRequest = WebRequest.Create(url)
                        Dim response As HttpWebResponse = request.GetResponse()
                        If response.StatusCode = HttpStatusCode.OK Then
                            Dim reader As StreamReader = New StreamReader(response.GetResponseStream())
                            Dim rawresp As String
                            rawresp = reader.ReadToEnd()
                            Dim jResults As JObject = JObject.Parse(rawresp)
                            Dim jo = JObject.Parse(jResults.ToString())
                            If jo("Response")("View").ToArray().Count = 0 Then
                                Continue For
                            End If
                            Dim Lat As JToken
                            Dim Longt As JToken

                            Try
                                Lat = jo("Response")("View").ToArray()(0)("Result").ToArray()(0)("Location")("DisplayPosition")("Latitude")
                                Longt = jo("Response")("View").ToArray()(0)("Result").ToArray()(0)("Location")("DisplayPosition")("Longitude")
                                'StrLocations.Append("new mapsjs.geo.Point(" & Lat.ToString & ", " & Longt.ToString() & ")")
                                StrLocations.Append("" & Lat.ToString & ", " & Longt.ToString() & ":")
                            Catch ex As Exception
                                Continue For
                            End Try
                        End If

                    Catch ex As Exception

                    End Try

                End If


            Next

            ''ScriptManager.RegisterClientScriptBlock(Me.Page, GetType(String), "calcHashFunction", "PlotMarkers('" & StrLocations.ToString & "');", True)
            'Dim svg As String = "var svg='<svg xmlns=""http://www.w3.org/2000/svg"" width=""28px"" height=""36px"">" +
            '"<path d=""M 19 31 C 19 32.7 16.3 34 13 34 C 9.7 34 7 32.7 7 31 C 7 29.3 9.7 28 13 28 C 16.3 28 19" +
            '" 29.3 19 31 Z"" fill=""#000"" fill-opacity="".2""/>" +
            '"<path d=""M 13 0 C 9.5 0 6.3 1.3 3.8 3.8 C 1.4 7.8 0 9.4 0 12.8 C 0 16.3 1.4 19.5 3.8 21.9 L 13 31 L 22.2" +
            '" 21.9 C 24.6 19.5 25.9 16.3 25.9 12.8 C 25.9 9.4 24.6 6.1 22.1 3.8 C 19.7 1.3 16.5 0 13 0 Z"" fill=""#fff""/>" +
            '"<path d=""M 13 2.2 C 6 2.2 2.3 7.2 2.1 12.8 C 2.1 16.1 3.1 18.4 5.2 20.5 L 13 28.2 L 20.8 20.5 C" +
            '" 22.9 18.4 23.8 16.2 23.8 12.8 C 23.6 7.07 20 2.2 13 2.2 Z"" fill=""__FILLCOLOR__""/>" +
            '"<text font-size=""12"" font-weight=""bold"" fill=""#fff"" font-family=""Nimbus Sans L,sans-serif"" x=""10"" y=""19"">__NO__</text>" +
            '"</svg>';"

            'Dim str2 = "var hidpi = ('devicePixelRatio' in window && devicePixelRatio > 1);"
            'str2 = str2 & "var mapContainer = document.getElementById('mapContainer'),"
            'str2 = str2 & " platform = new H.service.Platform({app_id: 'DemoAppId01082013GAL', app_code: 'AJKnXv84fjrb0KIHawS0Tg', useCIT: true,useHTTPS: true,}),"
            'str2 = str2 & " maptileService = platform.getMapTileService({ 'type': 'base' });"
            'str2 = str2 & " maptypes = platform.createDefaultLayers(hidpi ? 512 : 256, hidpi ? 320 : null);"
            'str2 = str2 & " map = new H.Map(mapContainer, maptypes.normal.map,{center: new H.geo.Point(21.2597, 77.5114),zoom: 4});"
            'str2 = str2 & "new H.mapevents.Behavior(new H.mapevents.MapEvents(map));"
            'str2 = str2 & "var ui = H.ui.UI.createDefault(map, maptypes);"
            'str2 = str2 & "window.addEventListener('resize', function () { map.getViewPort().resize(); });"

            'ScriptManager.RegisterClientScriptBlock(Me.Page, GetType(String), "calcHashFunction", " var str1='" & StrLocations.ToString & "'; " & svg & "  var colors = [new H.map.Icon(svg.replace(/__NO__/g, '1').replace(/__FILLCOLOR__/g, '#FF0000')), new H.map.Icon(svg.replace(/__NO__/g, '2').replace(/__FILLCOLOR__/g, '#FF0000')), new H.map.Icon(svg.replace(/__NO__/g, '3').replace(/__FILLCOLOR__/g, '#00FF00')), new H.map.Icon(svg.replace(/__NO__/g, '4').replace(/__FILLCOLOR__/g, '#0000FF')), new H.map.Icon(svg.replace(/__NO__/g, '5').replace(/__FILLCOLOR__/g, '#F0F000'))]; " & str2 & " function PlotMarkers(str) {var coordinates = str.split(':');var a = new Array(coordinates.length);for (var i = 0; i < a.length; i++) {var v = coordinates[i];if (v == '' || v == undefined){ continue; } if (v[0] == '') { continue; }a[i] = new H.map.Marker(new mapsjs.geo.Point(v[0], v[1]), {  icon: colors[Math.floor((Math.random() * 4))]});  }  map.addObjects(a);}   PlotMarkers(str1);", True)


            hdnloc.Value = StrLocations.ToString()

            'ScriptManager.RegisterClientScriptBlock(Me.Page, GetType(String), "calcHashFunction", " var str1='" & StrLocations.ToString & "';", True)



        Catch ex As Exception

        End Try
    End Sub

    Public Function GoogleGeoCode(Address As String) As String
        Dim url As String = "http://maps.google.com/maps/api/geocode/xml?address=" + Address + "&sensor=false"
        Dim request As WebRequest = WebRequest.Create(url)
        Using response As WebResponse = DirectCast(request.GetResponse(), HttpWebResponse)
            Using reader As New StreamReader(response.GetResponseStream(), Encoding.UTF8)
                Dim dsResult As New DataSet()
                dsResult.ReadXml(reader)
                If Not dsResult.Tables.Contains("Location") Then
                    Return ""
                End If
                If dsResult.Tables("Location").Rows.Count = 0 Then
                    Return ""
                End If
                Address = dsResult.Tables("Location").Rows(0).Item(0).ToString()
                Address &= "," & dsResult.Tables("Location").Rows(0).Item(1).ToString()

            End Using
        End Using
        Return Address

    End Function


End Class
