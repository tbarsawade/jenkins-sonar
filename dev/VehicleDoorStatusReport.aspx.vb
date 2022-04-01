Imports System.Data.SqlClient
Imports System.Data
Imports System.IO
Imports System.Xml
Imports System.Net

Partial Class VehicleDoorStatusReport
    Inherits System.Web.UI.Page
    Dim imeino As String = ""
    Protected Sub Page_Load(sender As Object, e As System.EventArgs) Handles Me.Load
        If Not IsPostBack Then
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
                    'lblveh.Text = "User-Vehicle"
                    oda.SelectCommand.CommandText = "select distinct  tid,fld1 from mmm_mst_master where DOCUMENTTYPE='State' and eid=" & Session("EID") & " order by fld1"
                Else
                    lblCircle.Text = "Circle"
                    lblcity.Text = "Cluster"
                    'lblveh.Text = "VehicleName-VehicleNo."
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
                'oda.SelectCommand.CommandText = "select imieno,username,VehicleNo from ( select  distinct m." & vemei & "[IMIENO], (select username from mmm_mst_user where eid=" & Session("EID") & " and uid=d." & Ufld & ")[UserName],m." & Vfld & "[VehicleNo] from mmm_mst_Doc d inner join mmm_mst_master m on m.tid=d." & UVfld & " inner join mmm_mst_gpsdata g on g.imieno=m." & vemei & " where d.eid=" & Session("EID") & " and  d.documenttype='" & Udtype & "'  and imieno is not null) as table1 order by username  "
                ds.Clear()
                oda.SelectCommand.CommandTimeout = 180
                oda.Fill(ds, "vemei")
                For i As Integer = 0 To ds.Tables("vemei").Rows.Count - 1
                    UsrVeh.Items.Add(ds.Tables("vemei").Rows(i).Item(1).ToString() & "-" & ds.Tables("vemei").Rows(i).Item(2).ToString())
                    UsrVeh.Items(i).Value = ds.Tables("vemei").Rows(i).Item(0).ToString()
                    ' UsrVeh.Items(i).Attributes.Add("onclick", "GetUsers123(" + ds.Tables("vemei").Rows(i).Item(0).ToString() + ",this)")
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
    Protected Sub btnshow_Click(sender As Object, e As System.EventArgs) Handles btnshow.Click
        Show()
    End Sub
    Protected Sub Show()
        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim con As SqlConnection = New SqlConnection(conStr)
        Dim oda As SqlDataAdapter = New SqlDataAdapter("", con)
        Try
            If ViewState("imei") IsNot Nothing Then
                imeino = ViewState("imei").ToString()
                hdnimei.Value = imeino
            End If

            oda.SelectCommand.CommandText = "select  IMIENO,ctime,igstatus,speed,tripon,lattitude,longitude from  mmm_mst_gpsdata with (nolock) where imieno='" & imeino.ToString & "' and DATEADD(minute,DATEDIFF(minute,0,ctime),0)>='" & txtfrom.Text.ToString & "'  and DATEADD(minute,DATEDIFF(minute,0,ctime),0)<='" & txtTo.Text.ToString & "' order by ctime"
            Dim ds As New DataSet
            oda.SelectCommand.CommandTimeout = 180
            oda.Fill(ds, "data")
            Dim tbl As New DataTable
            tbl.Columns.Add("IMEINO", GetType(String))
            tbl.Columns.Add("Door Opening Time", GetType(String))
            tbl.Columns.Add("Door Closing Time", GetType(String))
            tbl.Columns.Add("Duration Opened(HH:MM)", GetType(String))
            tbl.Columns.Add("Location", GetType(String))
            ' tbl.Columns.Add("Status", GetType(String))
            lblmsg.Text = ""
            GVReport1.Controls.Clear()
            If CDate(Left(txtfrom.Text, 10)) > CDate(txtTo.Text) Then
                lblmsg.Text = "Start Date should not be greater than from End Date."
                Exit Sub
            End If

            If CDate(txtfrom.Text) < CDate("2014-08-01") Then
                lblmsg.Text = "Data for this report is available from 2nd August 2014 Onward."
                Exit Sub
            End If
            Dim FrDate As String = ""
            Dim ToDate As String = ""
            Dim dist As Decimal = 0
            Dim rw As DataRow
            For i As Integer = 0 To ds.Tables("data").Rows.Count - 1
                If ds.Tables("data").Rows(i).Item("tripon").ToString() = "0" Then
                    If FrDate.ToString = "" Then
                        FrDate = ds.Tables("data").Rows(i).Item("ctime").ToString()
                    End If
                    rw = tbl.NewRow
                   

                    ToDate = ds.Tables("data").Rows(i).Item("ctime").ToString()
                    If i = ds.Tables("data").Rows.Count - 1 Then
                        rw(0) = ds.Tables("data").Rows(i).Item("IMIENO").ToString
                        rw(1) = FrDate.ToString()
                        rw(2) = ToDate.ToString()
                        Dim mint As Integer = DateDiff(DateInterval.Minute, CDate(FrDate), CDate(ToDate)).ToString
                        Dim hours As Integer = mint \ 60
                        Dim minutes As Integer = mint - (hours * 60)
                        Dim timeElapsed As String = CType(hours, String) & ":" & CType(minutes, String)
                        rw(3) = timeElapsed.ToString

                        ' for location
                        Dim latlong As String = ds.Tables("data").Rows(i).Item("lattitude").ToString & "," & ds.Tables("data").Rows(i).Item("longitude").ToString
                        Dim Exlocation As String = ""
                        Exlocation = Location(latlong)
                        rw(4) = Exlocation

                        tbl.Rows.Add(rw)
                    End If
                Else
                    ToDate = ds.Tables("data").Rows(i).Item("ctime").ToString()
                    If i = ds.Tables("data").Rows.Count - 1 And FrDate.ToString = "" Then

                    Else

                        If FrDate <> "" Then
                           
                            rw = tbl.NewRow
                            rw(0) = ds.Tables("data").Rows(i).Item("IMIENO").ToString
                            rw(1) = FrDate.ToString()
                            rw(2) = ToDate.ToString()
                            Dim mint As Integer = DateDiff(DateInterval.Minute, CDate(FrDate), CDate(ToDate)).ToString
                            Dim hours As Integer = mint \ 60
                            Dim minutes As Integer = mint - (hours * 60)
                            Dim timeElapsed As String = CType(hours, String) & ":" & CType(minutes, String)
                            rw(3) = timeElapsed.ToString
                            ' for location
                            Dim latlong As String = ds.Tables("data").Rows(i).Item("lattitude").ToString & "," & ds.Tables("data").Rows(i).Item("longitude").ToString
                            Dim Exlocation As String = ""
                            Exlocation = Location(latlong)
                            rw(4) = Exlocation
                            tbl.Rows.Add(rw)
                            FrDate = ""
                        End If
                    End If
                End If
                'Insert in data table and change status of the devstatus filed to  new one
                'now initilize all values
            Next
            
            GVReport1.DataSource = tbl
            GVReport1.DataBind()
        Catch ex As Exception
        Finally
            con.Close()
            con.Dispose()
        End Try
    End Sub
    Public Function Location(latlong As String) As String
        Dim constr As String = "server=172.17.109.152;initial catalog=DMS;uid=DMS;pwd=Ztsu93#u"
        Dim con As New SqlConnection(constr)
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
    Public Overloads Overrides Sub VerifyRenderingInServerForm(ByVal control As Control)

    End Sub
    Protected Sub btnexportxl_Click(ByVal sender As Object, ByVal e As System.Web.UI.ImageClickEventArgs) Handles btnexportxl.Click
        Show()
        GVReport1.AllowPaging = False
        GVReport1.DataBind()
        GVReport1.DataSource = ViewState("xlexport")

        Response.Clear()
        Response.Buffer = True

        Response.Write("<br/><div align=""center"" style=""border:1px solid red"" ><h3>" & "Vehicle Door Opening Report" & "</h3></div> <br/>")
        Response.AddHeader("content-disposition", "attachment;filename=VehicleDOReport.xls")
        Response.Charset = ""
        Response.ContentType = "application/vnd.ms-excel"
        Dim sw As New StringWriter()
        Dim hw As New HtmlTextWriter(sw)
        For i As Integer = 0 To GVReport1.Rows.Count - 1
            'Apply text style to each Row 
            GVReport1.Rows(i).Attributes.Add("class", "textmode")
        Next
        GVReport1.RenderControl(hw)
        'style to format numbers to string 
        Dim style As String = "<style> .textmode{mso-number-format:\@;}</style>"
        Response.Write(style)
        Response.Output.Write(sw.ToString())
        Response.Flush()
        Response.End()
    End Sub

    Protected Sub OnMap(ByVal sender As Object, ByVal e As System.EventArgs)
        Dim OmMap As ImageButton = TryCast(sender, ImageButton)
        Dim row As GridViewRow = DirectCast(OmMap.NamingContainer, GridViewRow)
        ' Response.Redirect("ShowMap.aspx?IMIE=" & txtIMEI.Text & "&Start=" + row.Cells(3).ToString() + "&End=" + row.Cells(4).ToString())
    End Sub
    Protected Sub GVReport_RowDataBound(sender As Object, e As System.Web.UI.WebControls.GridViewRowEventArgs) Handles GVReport1.RowDataBound

    End Sub
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
                ' hdcir.Value = id
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
            'hdcir.Value = id
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
                ' hdcity.Value = id
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
                oda.SelectCommand.CommandText = "select  distinct m1.tid,m1.fld1 from mmm_mst_master m inner join mmm_mst_master m1 on convert(nvarchar,m.tid)=m1.fld10 where m.eid=" & Session("EID") & " and m.documenttype='State' and m1.documenttype='City' and m.tid in (" & id & ") order by m1.fld1 "
            Else
                oda.SelectCommand.CommandText = "select  distinct m1.tid,m1.fld1 from mmm_mst_master m inner join mmm_mst_master m1 on convert(nvarchar,m.tid)=m1.fld11 where m.eid=54 and m.documenttype='Circle' and m1.documenttype='cluster' and m.tid in (" & id & ") order by m1.fld1 "
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
            hdnimei.Value = imeino.ToString
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
                    'hdvehcleName.Value = str.Remove(str.Length - 1)
                    'ViewState("VehicleName") = hdvehcleName.Value
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
                lblmsg.Text = "Please Select Only One User At a Time "
            Else
                lblmsg.Text = ""
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
            ' hdcity.Value = id
            FilterUserOnCity(id)
        End If
    End Sub
End Class
