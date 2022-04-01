Imports Microsoft.VisualBasic
Imports System.Data
Imports System.Data.SqlClient
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
Imports System.Net.Security
Imports iTextSharp.text.pdf
Imports System.Xml
Partial Class ElogbookUpload
    Inherits System.Web.UI.Page
    Protected Sub btnUpload_Click(sender As Object, e As EventArgs) Handles btnUpload.Click
        Dim filename As String = ""
        Dim dt As New DataTable()
        'Session("AppData") = Nothing
        gvData.DataSource = Nothing
        gvData.DataBind()
        lblMsg.Text = "0"
        Try
            If impfile.HasFile() Then
                filename = "Elog" & Now.Day & Now.Month & Now.Year & Now.Hour & Now.Minute & Now.Minute & Now.Second & Now.Millisecond & Right(impfile.FileName, 4).ToUpper()
                impfile.PostedFile.SaveAs(Server.MapPath("~/Import/" & filename))
                filename = "Import/" & filename
                dt = GetDataFromExcel(filename)
                If dt.Rows.Count > 0 Then
                    ' lblMsg.Text = dt.Rows.Count
                    Dim ds As New DataSet()
                    ds.Tables.Add(dt)
                    Session("AppData") = ds
                    FillDocData(dt)
                End If
            End If
        Catch ex As Exception
        Finally
            If File.Exists(Server.MapPath("~/Import/" & filename)) Then
                File.Delete(Server.MapPath("~/Import/" & filename))
            End If
        End Try
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
    Public Function GetDataFromExcel(ByVal strDataFilePath As String) As DataTable
        Try
            Dim sr As New StreamReader(Server.MapPath("~/" & strDataFilePath))
            Dim fullFileStr As String = sr.ReadToEnd()
            sr.Close()
            sr.Dispose()
            Dim lines As String() = fullFileStr.Split(ControlChars.Lf)
            Dim recs As New DataTable()
            Dim sArr As String() = lines(0).Split(","c)
            For Each s As String In sArr
                recs.Columns.Add(New DataColumn(s.Trim()))
            Next
            Dim row As DataRow
            Dim finalLine As String = ""
            Dim i As Integer = 0
            For Each line As String In lines
                If i > 0 And Not String.IsNullOrEmpty(line.Trim()) Then
                    row = recs.NewRow()
                    finalLine = line.Replace(Convert.ToString(ControlChars.Cr), "").ToString
                    row.ItemArray = finalLine.Split(","c)
                    recs.Rows.Add(row)
                End If
                i = i + 1
            Next
            'DataColumn Col   = datatable.Columns.Add("Column Name", System.Type.GetType("System.Boolean"));
            'Dim col As DataColumn = recs.Columns.Add("Check All", System.Type.GetType("System.String"))
            'col.SetOrdinal(0)
            Return recs
        Catch ex As Exception
            Throw ex
        Finally
        End Try
    End Function
    Public Sub FillDocData(ByRef dt As DataTable)
        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim con As SqlConnection = New SqlConnection(conStr)
        Dim da As SqlDataAdapter = New SqlDataAdapter("", con)
        Try
            Dim ds As New DataSet
            ds = DirectCast(Session("AppData"), DataSet)
            'Dim dtA = CType(Session("tblActionField"), DataTable)
            If dt.Rows.Count > 0 Then
                'loop through all column of Excell file
                If Trim(dt.Columns(0).ToString.ToUpper) <> "VEHICLE_NO" Then
                    lblMsg.Text = lblMsg.Text & " Vehicle No does not exist in the file."
                    Exit Sub
                End If
                If Trim(dt.Columns(1).ToString.ToUpper) <> "TRIP_START_DATETIME" Then
                    lblMsg.Text = lblMsg.Text & " Trip_Start_DateTime does not exist in the file."
                    Exit Sub
                End If
                If Trim(dt.Columns(2).ToString.ToUpper) <> "TRIP_END_DATETIME" Then
                    lblMsg.Text = lblMsg.Text & " Trip_End_DateTime does not exist in the file."
                    Exit Sub
                End If
                Dim dtt As New DataTable
                Dim errcnt As Integer = 1
                lblMsg.Text = ""

                For i As Integer = 0 To dt.Rows.Count - 1
                    Try
                        Dim VehicleNum = Trim(dt.Rows(i).Item("Vehicle_no").ToString().Trim)
                        Dim Sdate As String = Trim(dt.Rows(i).Item("Trip_Start_DateTime").ToString())
                        Dim Edate As String = Trim(dt.Rows(i).Item("Trip_End_DateTime").ToString())
                        da.SelectCommand.CommandType = CommandType.Text
                        da.SelectCommand.CommandText = "select tid,fld2[IMIE] from mmm_mst_master where fld10 like '%" & Trim(VehicleNum) & "%' and fld2 is not null and fld2<>''"
                        da.Fill(ds, "imie")
                        'get imeino

                        Dim IMIENo As String = ""
                        Dim mtid As String = ""

                        If ds.Tables("imie").Rows.Count > 0 Then
                            IMIENo = ds.Tables("imie").Rows(i).Item("IMIE").ToString()
                            mtid = ds.Tables("imie").Rows(i).Item("tid").ToString()
                        Else
                            lblMsg.Text = lblMsg.Text & "Line Number " & errcnt & " No any IMEI mapped of vehicle no " & VehicleNum & ".</br>"
                            errcnt = errcnt + 1
                            Continue For
                        End If

                        'If IMIENo.ToString = "" Then
                        '    lblMsg.Text = lblMsg.Text & "Line Number " & errcnt & " No any IMEI mapped of vehicle no " & VehicleNum & ".</br>"
                        '    errcnt = errcnt + 1
                        '    Continue For
                        'End If

                        da.SelectCommand.CommandText = "select count(*) from mmm_mst_newelogbook where (Vehicle_no='" & VehicleNum & "' and  '" & getdate1(Sdate) & "' <= trip_end_Datetime and '" & getdate1(Sdate) & "' >= trip_start_Datetime) or (Vehicle_no='" & VehicleNum & "' and  '" & getdate1(Edate) & "' <= trip_end_Datetime and '" & getdate1(Edate) & "' >= trip_start_Datetime) or (Vehicle_no='" & VehicleNum & "' and  '" & getdate1(Sdate) & "' <= trip_start_Datetime and  trip_end_Datetime<='" & getdate1(Edate) & "')"

                        'da.SelectCommand.CommandText = "select count(*) from mmm_mst_newelogbook where (Vehicle_no='" & VehicleNum & "' and  '" & Trim(getdate1(Sdate)) & "' <= trip_end_Datetime and '" & Trim(getdate1(Sdate)) & "' >= trip_start_Datetime) or (Vehicle_no='" & VehicleNum & "' and  '" & Trim(getdate1(Edate)) & "' <= trip_end_Datetime and '" & Trim((getdate1(Edate)) & "' >= trip_start_Datetime) or (Vehicle_no='" & VehicleNum & "' and  '" & Trim((getdate1(Sdate)) & "' <= trip_start_Datetime and  trip_end_Datetime<='" & Trim(getdate1(Edate) & "')"
                        If con.State <> ConnectionState.Open Then
                            con.Open()
                        End If
                        Dim cnt As Integer = da.SelectCommand.ExecuteScalar()
                        If cnt > 0 Then
                            lblMsg.Text = lblMsg.Text & "Line Number " & errcnt & " Trip already exist at this period.</br>"
                            errcnt = errcnt + 1
                            Continue For
                        End If

                        ' validate 
                        ' If starttime <= dt.Rows(i).Item("Trip_Start_DateTime").ToString And endtime <= dt.Rows(i).Item("trip_end_datetime").ToString Then
                        da.SelectCommand.CommandText = "set dateformat dmy;select fld11 from mmm_mst_doc where documenttype='vrf fixed_pool' and curstatus in ('allotted','surrender','archive') and (convert(datetime,fld17)>=convert(datetime,'" & Trim(getdate(Sdate)) & "') or convert(datetime,'" & Trim(getdate(Sdate)) & "')<=convert(datetime,fld19)) and (convert(datetime,fld19)<=convert(datetime,'" & Trim(getdate(Edate)) & "') or convert(datetime,'" & Trim(getdate(Edate)) & "') >= convert(datetime,fld17)) and fld24='" & mtid & "'"
                        da.Fill(ds, "user")
                        Dim uid As String = ""
                        If ds.Tables("user").Rows.Count > 0 Then
                            uid = ds.Tables("user").Rows(0).Item("fld11").ToString
                        End If

                        da.SelectCommand.CommandText = "set dateformat mdy;select top 1 lattitude,longitude from mmm_mst_gpsdata with(nolock) where imieno='" & IMIENo & "' and DATEADD(minute,DATEDIFF(minute,0,ctime),0)>='" & Trim(getdate1(Sdate)) & "' "
                        da.Fill(ds, "startlat")
                        Dim startlat As String = ""
                        Dim startlong As String = ""
                        Dim tripstartLocation As String = ""

                        If ds.Tables("startlat").Rows.Count > 0 Then
                            startlat = ds.Tables("startlat").Rows(0).Item("lattitude").ToString
                            startlong = ds.Tables("startlat").Rows(0).Item("longitude").ToString
                            tripstartLocation = Location(startlat, startlong)
                        End If

                        da.SelectCommand.CommandText = "select top 1 lattitude,longitude from mmm_mst_gpsdata with(nolock) where imieno='" & IMIENo & "' and DATEADD(minute,DATEDIFF(minute,0,ctime),0) >= '" & Trim(getdate1(Edate)) & "' "
                        da.Fill(ds, "endlat")

                        Dim endlat As String = ""
                        Dim endlong As String = ""
                        Dim tripEndLocation As String = ""

                        If ds.Tables("endlat").Rows.Count > 0 Then
                            endlat = ds.Tables("endlat").Rows(0).Item("lattitude").ToString
                            endlong = ds.Tables("endlat").Rows(0).Item("longitude").ToString
                            tripEndLocation = Location(endlat, endlong)
                        End If
                        da.SelectCommand.CommandText = "select isnull(sum(devdist),0)[Distance] from mmm_mst_gpsdata with(nolock) where imieno='" & IMIENo & "' and ctime>= '" & Trim(getdate1(Sdate)) & "' and ctime<= '" & Trim(getdate1(Edate)) & "' and ((devdist>0.03 and speed=0) or (devdist>0 and speed>0 ))"
                        da.Fill(ds, "dist")
                        Dim dist As Double = ds.Tables("dist").Rows(0).Item("Distance").ToString
                        ds.Tables("dist").Rows.Clear()

                        da.SelectCommand.Parameters.Clear()
                        da.SelectCommand.CommandType = CommandType.StoredProcedure
                        da.SelectCommand.CommandText = "USPInsertElogUpload"
                        da.SelectCommand.Parameters.AddWithValue("@eid", 32)
                        da.SelectCommand.Parameters.AddWithValue("@vehno", VehicleNum)
                        da.SelectCommand.Parameters.AddWithValue("@IMEI", IMIENo)
                        da.SelectCommand.Parameters.AddWithValue("@TSDT", Sdate)
                        da.SelectCommand.Parameters.AddWithValue("@TEDT", Edate)
                        da.SelectCommand.Parameters.AddWithValue("@TSLoc", tripstartLocation)
                        da.SelectCommand.Parameters.AddWithValue("@TELoc", tripEndLocation)
                        da.SelectCommand.Parameters.AddWithValue("@TotDist", dist)
                        da.SelectCommand.Parameters.AddWithValue("@uid", uid)
                        da.SelectCommand.Parameters.AddWithValue("@SLat", startlat)
                        da.SelectCommand.Parameters.AddWithValue("@SLong", startlong)
                        da.SelectCommand.Parameters.AddWithValue("@ELat", endlat)
                        da.SelectCommand.Parameters.AddWithValue("@ELong", endlong)

                        'da.SelectCommand.ExecuteNonQuery()
                        da.Fill(dtt)
                        ' End If
                        errcnt = errcnt + 1
                    Catch ex As Exception
                        errcnt = errcnt + 1
                        lblMsg.Text = lblMsg.Text & "Line Number " & errcnt & " Date Format is not correct.</br>"
                        If dtt.Rows.Count > 0 Then
                            gvData.DataSource = dtt
                            gvData.DataBind()
                        End If
                    End Try
                Next
                If lblMsg.Text = "" And dtt.Rows.Count > 0 Then
                    lblMsg.Text = "Total " & errcnt - 1 & " line item inserted successfully."
                End If
                If dtt.Rows.Count > 0 Then
                    gvData.DataSource = dtt
                    gvData.DataBind()
                End If
                Using dbConnection As New SqlConnection(conStr)
                    dbConnection.Open()
                    Using s As New SqlBulkCopy(dbConnection)
                        s.DestinationTableName = "MMM_MST_NEWELOGBOOK"
                        For Each column In dt.Columns
                            s.ColumnMappings.Add(column.ToString(), column.ToString())
                        Next
                        s.WriteToServer(dt)
                    End Using
                End Using
            End If
        Catch ex As Exception
        Finally
            con.Close()
            con.Dispose()
        End Try
    End Sub
    Function getdate1(ByVal dbt As String) As String
        'Dim dt1 As DateTime = Convert.ToDateTime(dbt)
        Dim dtArr As String()
        dtArr = dbt.ToString.Split("/")
        If dtArr.Length <= 1 Then
            dtArr = dtArr(0).ToString.Split("-")
        End If
        If dtArr.GetUpperBound(0) = 2 Then
            Dim dd, mm, yy, mint As String
            mm = dtArr(0)
            dd = dtArr(1)
            yy = Right(Left(dtArr(2), 4), 4)
            mint = Left(Right(dtArr(2), 5), 5)
            Dim dt As String
            dt = yy & "-" & mm & "-" & dd & " " & mint
            Return dt
        Else
            Return "Date Format is Not Correct"
        End If
    End Function

    Function getdate(ByVal dbt As String) As String
        'Dim dt1 As DateTime = Convert.ToDateTime(dbt)
        Dim dtArr As String()
        dtArr = dbt.ToString.Split("/")
        If dtArr.Length <= 1 Then
            dtArr = dtArr(0).ToString.Split("-")
        End If
        If dtArr.GetUpperBound(0) = 2 Then
            Dim dd, mm, yy As String
            mm = dtArr(0)
            dd = dtArr(1)
            yy = Right(Left(dtArr(2), 4), 2)
            Dim dt As String
            dt = dd & "-" & mm & "-" & yy
            Return dt
        Else
            Return "Date Format is Not Correct"
        End If
    End Function
    Private Function ContainColumn(columnName As String, table As DataTable) As Boolean
        Dim ret As Boolean = False
        Dim columns As DataColumnCollection = table.Columns
        If (columns.Contains(columnName)) Then
            ret = True
        End If
        Return ret
    End Function
    Public Overloads Overrides Sub VerifyRenderingInServerForm(ByVal control As Control)
        ' Verifies that the control is rendered
    End Sub
    Protected Sub helpexport_Click(ByVal sender As Object, ByVal e As System.Web.UI.ImageClickEventArgs) Handles helpexport.Click
        Response.Redirect("~/MailAttach/TripUploader.csv")
        'Response.ContentType = "application/csv"
        'Response.AddHeader("content-disposition", "attachment;filename=TripUploader.csv")
        'Response.TransmitFile(Server.MapPath("~/MailAttach/Test.csv"))
    End Sub
    Protected Sub gvData_PageIndexChanging(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.GridViewPageEventArgs) Handles gvData.PageIndexChanging
        gvData.PageIndex = e.NewPageIndex
        gvData.DataBind()
    End Sub
    Public Function Location(lat As String, log As String) As String
        Dim constr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim con As SqlConnection = New SqlConnection(constr)
        Dim oda As SqlDataAdapter = New SqlDataAdapter("", con)
        'try Catch Block Added by Ajeet Kumar :Date::22 May=2014
        Try
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
                        locatoinr.Dispose()

                    Catch ex As Exception
                    End Try
                    Return fulladdress
                End If
            End If
            Return Nothing
        Catch ex As Exception
            Throw
        Finally
            If Not oda Is Nothing Then
                con.Dispose()
            End If
            If Not oda Is Nothing Then
                oda.Dispose()
            End If
        End Try
    End Function
End Class
