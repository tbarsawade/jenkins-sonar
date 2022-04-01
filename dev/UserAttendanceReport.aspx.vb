Imports System.Data
Imports System.Data.SqlClient
Imports System.Web.UI.Adapters.ControlAdapter
Imports iTextSharp.text
Imports System.Drawing
Imports System.IO
Imports System.Xml
Imports System.Net

Partial Class UserAttendanceReport
    Inherits System.Web.UI.Page
    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Not IsPostBack Then
            Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
            Dim con As SqlConnection = New SqlConnection(conStr)
            Dim username As String = ""
            Dim vehimei As String = ""
            Dim oda As SqlDataAdapter = New SqlDataAdapter("", con)
            Try
                oda.SelectCommand.CommandText = "select fld1[StudentName],fld11[CardCSN] from mmm_mst_master with(nolock) where documenttype='Student Master' and eid=" & Session("EID") & " "
                Dim ds As New DataSet()
                oda.Fill(ds, "data")
                For i As Integer = 0 To ds.Tables("data").Rows.Count - 1
                    If ds.Tables("data").Rows(i).Item("CardCSN").ToString() = "" Then
                        Continue For
                    End If
                    UsrVeh.Items.Add(ds.Tables("data").Rows(i).Item("StudentName").ToString() & "-" & ds.Tables("data").Rows(i).Item("CardCSN").ToString())
                Next
            Catch ex As Exception
            Finally
                con.Close()
                oda.Dispose()
                con.Dispose()
            End Try
        End If
    End Sub
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
    Protected Sub btnshow_Click(sender As Object, e As ImageClickEventArgs) Handles btnshow.Click
        Show()
        UpdatePanel1.Update()
    End Sub
    Protected Sub Show()
        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim con As SqlConnection = New SqlConnection(conStr)
        Dim oda As SqlDataAdapter = New SqlDataAdapter("", con)
        Dim dt As New DataTable
        Dim username As String = ""
        Dim vehimei As String = ""
        Dim cardno As String = ""
        Try
            If txtsdate.Text = "" Or txtedate.Text = "" Then
                lblmsg.Text = "Please select start/end date."
                Exit Sub
            End If
            If CDate(Left(txtsdate.Text, 10)) > CDate(txtedate.Text) Then
                lblmsg.Text = "Start Date should not be greater than from End Date."
                Exit Sub
            End If

            If CDate(txtsdate.Text) < CDate("2015-05-01") Then
                lblmsg.Text = "Data for this report is available."
                Exit Sub
            End If
            For i As Integer = 0 To UsrVeh.Items.Count - 1
                If UsrVeh.Items(i).Selected = True Then
                    Dim satuts() As String = UsrVeh.Items(i).ToString.Split("-")
                    cardno = cardno & "'" & satuts(1) & "',"
                End If
            Next
            If cardno.ToString = "" Then
                lblmsg.Text = "Please select any User Card."
                Exit Sub
            Else
                cardno = Left(cardno, cardno.Length - 1)
                oda.SelectCommand.CommandText = "select  m.fld1[StudentName],m.fld10[RollNo],CONVERT(varchar(10),g.ctime, 7) [Date],g.ctime,g.imieno,g.ibuttonid,convert(nvarchar,g.lattitude) + ',' + convert(nvarchar,g.longitude)[Latlong] from  mmm_mst_master m with(nolock) inner join mmm_mst_gpsdata g with(nolock) on m.fld11=g.ibuttonid where m.documenttype='Student Master' and m.eid=" & Session("EID") & " and g.ibuttonid in ( " + cardno + ") and  convert(date,g.ctime)>='" + txtsdate.Text + "' and convert(date,g.ctime)<='" + txtedate.Text + "' order by m.fld11,g.ctime"
                Dim common As New DataTable
                oda.SelectCommand.CommandTimeout = 180
                oda.Fill(common)
                Dim tbl As New DataTable
                tbl.Columns.Add("Date", GetType(String))
                tbl.Columns.Add("User / Student Name", GetType(String))
                tbl.Columns.Add("Roll No", GetType(String))
                tbl.Columns.Add("In Punch Time", GetType(String))
                tbl.Columns.Add("In Punch Address", GetType(String))
                tbl.Columns.Add("Out Punch Time", GetType(String))
                tbl.Columns.Add("Out Punch Address", GetType(String))
                GVReport.Controls.Clear()
                For i As Integer = 0 To common.Rows.Count - 1
                    Dim intime As Integer = i
                    Dim Attdate As String = common.Rows(i).Item("Date").ToString
                    Dim Sname As String = common.Rows(i).Item("StudentName").ToString
                    Dim cardnum As String = common.Rows(i).Item("RollNo").ToString
                    Dim InpunchTime As String = common.Rows(i).Item("ctime").ToString
                    Dim InAddress As String = Location(common.Rows(i).Item("Latlong").ToString)
                    Dim NxtCN As String = ""
                    If i < common.Rows.Count - 1 Then
                        i += 1
                        NxtCN = common.Rows(i).Item("RollNo").ToString
                    End If

                    Dim OutpunchTime As String = ""
                    Dim OutAddress As String = ""
                    Dim rw As DataRow
                    rw = tbl.NewRow
                    If cardnum = NxtCN Then
                        OutpunchTime = common.Rows(i).Item("ctime").ToString
                        OutAddress = Location(common.Rows(i).Item("Latlong").ToString)
                        rw(0) = Attdate
                        rw(1) = Sname
                        rw(2) = cardnum
                        rw(3) = InpunchTime
                        rw(4) = InAddress
                        rw(5) = OutpunchTime
                        rw(6) = OutAddress

                    Else
                        OutpunchTime = "NA"
                        OutAddress = "NA"
                        rw(0) = Attdate
                        rw(1) = Sname
                        rw(2) = cardnum
                        rw(3) = InpunchTime
                        rw(4) = InAddress
                        rw(5) = OutpunchTime
                        rw(6) = OutAddress

                    End If
                    tbl.Rows.Add(rw)
                Next
                If tbl.Rows.Count > 1 Then
                    GVReport.DataSource = tbl
                    GVReport.DataBind()
                    lblmsg.Text = ""
                    ViewState("xlexport") = tbl
                Else
                    lblmsg.Text = "Data Not Found."
                End If
            End If

        Catch ex As Exception
        Finally
            con.Close()
            oda.Dispose()
            con.Dispose()
        End Try
       
    End Sub
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
    Protected Sub ToPdf(ByVal newDataSet As DataSet)
        Dim PDFData As New System.IO.MemoryStream()
        Dim newDocument As New iTextSharp.text.Document(PageSize.A4.Rotate(), 10, 10, 10, 10)
        Dim newPdfWriter As iTextSharp.text.pdf.PdfWriter = iTextSharp.text.pdf.PdfWriter.GetInstance(newDocument, PDFData)
        newDocument.Open()
        For Page As Integer = 0 To newDataSet.Tables.Count - 1
            Dim totalColumns As Integer = newDataSet.Tables(Page).Columns.Count
            Dim newPdfTable As New iTextSharp.text.pdf.PdfPTable(totalColumns)
            newPdfTable.DefaultCell.Padding = 4
            newPdfTable.WidthPercentage = 100
            newPdfTable.DefaultCell.HorizontalAlignment = iTextSharp.text.Element.ALIGN_LEFT
            newPdfTable.DefaultCell.VerticalAlignment = iTextSharp.text.Element.ALIGN_MIDDLE
            newPdfTable.HeaderRows = 1
            newPdfTable.DefaultCell.BorderWidth = 1
            newPdfTable.DefaultCell.BorderColor = New iTextSharp.text.BaseColor(193, 211, 236)
            newPdfTable.DefaultCell.BackgroundColor = New iTextSharp.text.BaseColor(255, 255, 255)
            For i As Integer = 0 To totalColumns - 1
                newPdfTable.AddCell(New Phrase(newDataSet.Tables(Page).Columns(i).ColumnName, FontFactory.GetFont("Tahoma", 10, iTextSharp.text.Font.BOLD, New iTextSharp.text.BaseColor(21, 66, 157))))
            Next
            For Each record As DataRow In newDataSet.Tables(Page).Rows
                For i As Integer = 0 To totalColumns - 1
                    newPdfTable.DefaultCell.BorderColor = New iTextSharp.text.BaseColor(193, 211, 236)
                    newPdfTable.AddCell(New Phrase(record(i).ToString, FontFactory.GetFont("Tahoma", 9, iTextSharp.text.Font.NORMAL, New iTextSharp.text.BaseColor(80, 80, 80))))
                Next
            Next
            newDocument.Add(New Phrase(Environment.NewLine))
            newDocument.Add(newPdfTable)
            newDocument.Add(New Phrase(Environment.NewLine))
            newDocument.Add(New Phrase(Environment.NewLine))
            newDocument.Add(New Phrase(Environment.NewLine))
            newDocument.Add(New Phrase(Environment.NewLine))
            'If Page < newDataSet.Tables.Count Then
            '    newDocument.NewPage()
            'End If
        Next
        newDocument.Add(New Phrase("Created By: " & Session("USERNAME"), FontFactory.GetFont("Tahoma", 8, iTextSharp.text.Font.BOLD, New iTextSharp.text.BaseColor(80, 80, 80))))
        newDocument.Add(New Phrase(Environment.NewLine))
        newDocument.Add(New Phrase("Printed Date: " & DateTime.Now.ToString(), FontFactory.GetFont("Tahoma", 8, iTextSharp.text.Font.BOLD, New iTextSharp.text.BaseColor(80, 80, 80))))
        newDocument.Close()
        Response.ContentType = "application/pdf"
        Response.Cache.SetCacheability(System.Web.HttpCacheability.[Public])
        Response.AppendHeader("Content-Type", "application/pdf")
        Response.AppendHeader("Content-Disposition", "attachment; filename=GpsDataReport")
        Response.OutputStream.Write(PDFData.GetBuffer(), 0, PDFData.GetBuffer().Length)
        Response.OutputStream.Flush()
        Response.OutputStream.Close()
    End Sub
    Public Overloads Overrides Sub VerifyRenderingInServerForm(ByVal control As Control)
    End Sub
    Protected Sub btnexportxl_Click(ByVal sender As Object, ByVal e As System.Web.UI.ImageClickEventArgs) Handles btnexportxl.Click

        GVReport.AllowPaging = False
        GVReport.DataSource = ViewState("xlexport")
        GVReport.DataBind()


        Response.Clear()
        Response.Buffer = True

        Response.Write("<br/><div align=""center"" style=""border:1px solid red"" ><h3>" & "GPSDataReport" & "</h3></div> <br/>")
        Response.AddHeader("content-disposition", "attachment;filename=" & "GPSDataReport" & ".xls")
        Response.Charset = ""
        Response.ContentType = "application/vnd.ms-excel"
        Dim sw As New StringWriter()
        Dim hw As New HtmlTextWriter(sw)

        'style to format numbers to string 
        GVReport.RenderControl(hw)
        Dim style As String = "<style> .textmode{mso-number-format:\@;}</style>"
        Response.Write(style)
        Response.Output.Write(sw.ToString())
        Response.Flush()
        Response.End()
    End Sub
    Protected Sub btnExportPDF_Click(sender As Object, e As ImageClickEventArgs) Handles btnExportPDF.Click
        ToPdf(ViewState("pdf"))
    End Sub
    Function getdate(ByVal dbt As String) As String
        Dim dtArr() As String
        dtArr = Split(dbt, "/")
        If dtArr.GetUpperBound(0) = 2 Then
            Dim dd, mm, yy As String
            dd = dtArr(0)
            mm = dtArr(1)
            yy = dtArr(2)
            Dim dt As String
            dt = yy & "-" & mm & "-" & dd
            Return dt
        Else
            Return Now.Date
        End If
    End Function


    Protected Sub gvReport_PageIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles GVReport.PageIndexChanged
        Show()
    End Sub
    Protected Sub gvReport1_PageIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles GVReport1.PageIndexChanged
        Show()
    End Sub
    Protected Sub gvReport_PageIndexChanging(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.GridViewPageEventArgs) Handles GVReport.PageIndexChanging
        GVReport.PageIndex = e.NewPageIndex
    End Sub
    Protected Sub gvReport1_PageIndexChanging(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.GridViewPageEventArgs) Handles GVReport1.PageIndexChanging
        GVReport1.PageIndex = e.NewPageIndex
    End Sub
    Protected Sub checkuncheck(sender As Object, e As System.EventArgs) Handles CheckBox1.CheckedChanged
        If CheckBox1.Checked = True Then

            For Each chkitem As System.Web.UI.WebControls.ListItem In UsrVeh.Items
                chkitem.Selected = True
            Next
            'For Each smsreport As System.Web.UI.WebControls.ListItem In smsreports.Items
            '    smsreport.Selected = True
            'Next
            'UpdatePanel1.Update()
        Else
            For Each chkitem As System.Web.UI.WebControls.ListItem In UsrVeh.Items
                chkitem.Selected = False
            Next
            'For Each smsreport As System.Web.UI.WebControls.ListItem In smsreports.Items
            '    smsreport.Selected = False
            'Next
        End If
        UpdatePanel1.Update()
    End Sub



    Protected Sub txtsdate_TextChanged(sender As Object, e As System.EventArgs) Handles txtsdate.TextChanged


    End Sub




End Class
