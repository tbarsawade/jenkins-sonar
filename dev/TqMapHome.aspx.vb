Imports System.Data
Imports System.Data.SqlClient
Imports Microsoft.VisualBasic.FileIO
Imports System.Web.Services

Partial Class TqMapHome
    Inherits System.Web.UI.Page
    Public Shared dtVechical As New DataTable

    Protected Sub Page_Load(sender As Object, e As System.EventArgs) Handles Me.Load
        'GenerateJson()

        ''for traqueur
        'Session("UID") = 6231 '6231
        'Session("USERNAME") = "Prashant Singh Sengar"
        'Session("USERROLE") = "SU"
        'Session("CODE") = "traqueur"
        'Session("USERIMAGE") = "2.jpg"
        'Session("CLOGO") = "hfcl.png"
        'Session("EID") = 58
        'Session("ISLOCAL") = "TRUE"
        'Session("IPADDRESS") = "Vinay"
        'Session("MACADDRESS") = "Vinay"
        'Session("INTIME") = Now
        'Session("EMAIL") = "vinay.kumar@myndsol.com"
        'Session("LID") = "25"
        'Session("HEADERSTRIP") = "hfclstrip.jpg"
        'Session("ROLES") = "SU"

        ' ''Sales
        'Session("UID") = 7502
        'Session("USERNAME") = "Prashant Singh Sengar"
        'Session("USERROLE") = "SU"
        'Session("CODE") = "Sales"
        'Session("USERIMAGE") = "2.jpg"
        'Session("CLOGO") = "IndusTowerLogo.png"
        'Session("EID") = 71
        'Session("ISLOCAL") = "TRUE"
        'Session("IPADDRESS") = "Vinay"
        'Session("MACADDRESS") = "Vinay"
        'Session("INTIME") = Now
        'Session("EMAIL") = "vinay.kumar@myndsol.com"
        'Session("LID") = "25"
        'Session("HEADERSTRIP") = "hfclstrip.jpg"
        'Session("ROLES") = "SU"


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
            oda.Fill(ds, "Entity")
            hdnRefTime.Value = Convert.ToInt32(ds.Tables("Entity").Rows(0).Item("ReloadSeconds")) * 1000
            'hdnRefTime.Value = 30000
            If Session("Eid") = 71 Then
                hdnCluster.Value = "SU"
                infoChart.Visible = False
                lblveh.Text = "Search Employee"
                VehSearch.Attributes("placeholder") = "Employee Name"
                If Session("userrole").ToString().ToUpper() = "SU" Then

                Else
                    oda.SelectCommand.CommandText = "select uid,fld2[Vehicle],rolename,isnull(fld1,0)[Company] from mmm_ref_role_user  where eid=" & Session("EID") & " and uid= " & Session("uid") & " and rolename='" & Session("userrole") & "'"
                    oda.Fill(ds, "Vehicle")
                    If (ds.Tables("Vehicle").Rows.Count > 0) Then
                        Session("RID") = Convert.ToString(ds.Tables("Vehicle").Rows(0).Item("Vehicle"))
                        Session("CompIDs") = ds.Tables("Vehicle").Rows(0).Item("Company").ToString()
                    Else
                        hdnCluster.Value = "0"
                        Session("RID") = "0"
                        Session("CompIDs") = "0"
                    End If
                End If
            Else

                lblveh.Text = "Search Vehicle"
                VehSearch.Attributes("placeholder") = "Type Vehicle Name"
                If Session("userrole").ToString().ToUpper() = "SU" Then
                    hdnCluster.Value = "SU"
                ElseIf Session("userrole").ToString().ToUpper() = "CORPORATEUSER" Then
                    hdnCluster.Value = "CORPORATEUSER"
                Else
                    oda.SelectCommand.CommandText = "select uid,fld2[Vehicle],rolename,isnull(fld1,0)[Company] from mmm_ref_role_user  where eid=" & Session("EID") & " and uid= " & Session("uid") & " and rolename='" & Session("userrole") & "'"
                    oda.Fill(ds, "Vehicle")
                    If (ds.Tables("Vehicle").Rows.Count > 0) Then
                        hdnCluster.Value = ds.Tables("Vehicle").Rows(0).Item("Vehicle").ToString()
                        Session("RID") = ds.Tables("Vehicle").Rows(0).Item("Vehicle").ToString()
                        Session("CompIDs") = ds.Tables("Vehicle").Rows(0).Item("Company").ToString()
                    Else
                        hdnCluster.Value = "0"
                        Session("RID") = "0"
                        Session("CompIDs") = "0"
                    End If
                End If

            End If

            'hdnCluster.Value = "SU"

            Dim dtt As New DataTable()

            oda.SelectCommand.CommandText = "select * from mmm_mst_master with(nolock) where eid=" & Session("EID") & " and documenttype='vehicle type' "
            dtt.Clear()
            oda.Fill(dtt)

            'For i As Integer = 0 To dtt.Rows.Count - 1
            '    chkvtype.Items.Add(Convert.ToString(dtt.Rows(i).Item("fld1").ToString))
            '    chkvtype.Items(i).Value = dtt.Rows(i).Item("tid").ToString
            '    chkvtype.Items(i).Attributes.Add("onclick", "ShowHideMap(this)")
            '    chkvtype.Items(i).Selected = True
            'Next
            'BindAll()
            If Session("Eid") = 71 Then
                BindVehicle()
            Else
                Maprendorforsite("0")
            End If

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
    Protected Function Maprendorforsite(ByVal sid As String) As String
        Try
            LstVehicle.Items.Clear()
            'Change Check Box Text Color
            Dim apikey As String = String.Empty
            Dim j As Integer = 0
            Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
            Dim con As New SqlConnection(conStr)
            Dim oda As SqlDataAdapter = New SqlDataAdapter("", con)

            Dim str As String = "select max(convert(nvarchar,g.tid))[TID],IMIENO,max(m2.fld10)[Site_Name],DMS.IdealTime(g.imieno) IdealTime,convert(nvarchar,Lattitude)[Lat],convert(nvarchar,longitude)[Long],convert(nvarchar,ctime,101)ctime,max(m2.fld14)[Group],0 as Isvehicle, m2.fld1 [VhNo] "
            If HttpContext.Current.Session("USERROLE").ToString.ToUpper = "SU" Or HttpContext.Current.Session("USERROLE").ToString.ToUpper = "CORPORATEUSER" Then
                str = str & " from mmm_mst_gpsdata g with (nolock) join mmm_mst_master m2 with (nolock) on m2.fld12=g.imieno   where m2.documenttype='vehicle' and m2.eid=" & HttpContext.Current.Session("Eid") & " group by imieno,ctime,m2.fld1,lattitude,longitude having ctime=(select max(ctime) from mmm_mst_gpsdata with (nolock) where imieno=g.imieno) and Ltrim(IMIENO) <> '' "
            Else
                'str = str & " from mmm_mst_gpsdata g with (nolock) join mmm_mst_master m2 with (nolock) on m2.fld12=g.imieno inner join  dbo.split('" & HttpContext.Current.Session("RID") & "', ',') s on s.items in (select items from dbo.split(m2.fld16, ','))   where m2.documenttype='vehicle' and m2.fld14 in (" & sid & ")  and m2.eid=54 group by g.tid,imieno,ctime,m2.fld1,msensor,lattitude,longitude,speed having ctime=(select max(ctime) from mmm_mst_gpsdata where imieno=g.imieno) and Ltrim(IMIENO) <> '' "
                str = str & " from mmm_mst_gpsdata g with (nolock) join mmm_mst_master m2 with (nolock) on m2.fld12=g.imieno   where m2.documenttype='vehicle'  and m2.tid in (" & HttpContext.Current.Session("RID") & ") and m2.eid=" & HttpContext.Current.Session("Eid") & " group by imieno,ctime,m2.fld1,lattitude,longitude having ctime=(select max(ctime) from mmm_mst_gpsdata with (nolock) where imieno=g.imieno) and Ltrim(IMIENO) <> '' "
            End If
            oda.SelectCommand.CommandText = str
            oda.SelectCommand.CommandTimeout = 180
            dtVechical.Clear()
            oda.Fill(dtVechical)

            For i As Integer = 0 To dtVechical.Rows.Count - 1
                If Convert.ToInt32(dtVechical.Rows(i).Item("IdealTime").ToString) > 10 And Convert.ToInt32(dtVechical.Rows(i).Item("IdealTime").ToString) <= 600 And Convert.ToDateTime(dtVechical.Rows(i).Item("ctime")) = DateAndTime.Today() Then
                    LstVehicle.Items.Add(dtVechical.Rows(i).Item("VhNo").ToString())
                    LstVehicle.Items(j).Text = i + 1 & " " & dtVechical.Rows(i).Item("Site_Name").ToString()
                    LstVehicle.Items(j).Value = dtVechical.Rows(i).Item("IMIENO").ToString()
                    LstVehicle.Items(j).Attributes.Add("style", "color:Black;")
                    LstVehicle.Items(j).Attributes.Add("onclick", "focusMarker(this);")
                    j = j + 1
                ElseIf Convert.ToInt32(dtVechical.Rows(i).Item("IdealTime").ToString) >= 0 And Convert.ToInt32(dtVechical.Rows(i).Item("IdealTime").ToString) <= 10 And Convert.ToDateTime(dtVechical.Rows(i).Item("ctime")) = DateAndTime.Today() Then
                    LstVehicle.Items.Add(dtVechical.Rows(i).Item("VhNo").ToString())
                    LstVehicle.Items(j).Text = i + 1 & " " & dtVechical.Rows(i).Item("Site_Name").ToString()
                    LstVehicle.Items(j).Value = dtVechical.Rows(i).Item("IMIENO").ToString()
                    LstVehicle.Items(j).Attributes.Add("style", "color:Green;")
                    LstVehicle.Items(j).Attributes.Add("onclick", "focusMarker(this);")
                    j = j + 1
                ElseIf (Convert.ToInt32(dtVechical.Rows(i).Item("IdealTime").ToString) > 600 And Convert.ToInt32(dtVechical.Rows(i).Item("IdealTime").ToString) <= 1440) Or (Convert.ToDateTime(dtVechical.Rows(i).Item("ctime")) = DateAndTime.Today() And Convert.ToInt32(dtVechical.Rows(i).Item("IdealTime").ToString) > 1440) Then
                    LstVehicle.Items.Add(dtVechical.Rows(i).Item("VhNo").ToString())
                    LstVehicle.Items(j).Text = i + 1 & " " & dtVechical.Rows(i).Item("Site_Name").ToString()
                    LstVehicle.Items(j).Value = dtVechical.Rows(i).Item("IMIENO").ToString()
                    LstVehicle.Items(j).Attributes.Add("style", "color:#B86A84;")
                    LstVehicle.Items(j).Attributes.Add("onclick", "focusMarker(this);")
                    j = j + 1
                Else
                    LstVehicle.Items.Add(dtVechical.Rows(i).Item("VhNo").ToString())
                    LstVehicle.Items(j).Text = i + 1 & " " & dtVechical.Rows(i).Item("Site_Name").ToString()
                    LstVehicle.Items(j).Value = dtVechical.Rows(i).Item("IMIENO").ToString()
                    LstVehicle.Items(j).Attributes.Add("style", "color:Red;")
                    LstVehicle.Items(j).Attributes.Add("onclick", "focusMarker(this);")
                    j = j + 1
                End If
                'End If
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
                url = "<script type='text/javascript'>google.load('maps', '4.7', { 'other_params': 'sensor=true' }); </script>"
            End If
            oda.Dispose()
            con.Dispose()
            Return "0"
        Catch ex As Exception
            Throw
        End Try
    End Function


    'Public Shared CsvPath As String = HttpContext.Current.Server.MapPath("DOCS/tqSites.txt")

    <WebMethod()> _
   <Script.Services.ScriptMethod()> _
    Public Shared Function GetMarkerList(IDs As String) As String
        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim con As New SqlConnection(conStr)

        Dim CsvPath As String = HttpContext.Current.Server.MapPath("Scripts/CsvJson_" & HttpContext.Current.Session("Eid") & ".txt")
        Dim CsvStr As New StringBuilder()
        CsvStr.Clear()

        Dim colSize As Integer = dtVechical.Columns.Count - 1

        For Each Row As DataRow In dtVechical.Rows
            If dtVechical.Rows.IndexOf(Row) > 0 Then
                CsvStr.Append("|")
            End If

            For i As Integer = 0 To colSize - 1
                CsvStr.Append(Row(i).ToString())
                If i < colSize - 1 Then
                    CsvStr.Append("^")
                End If
            Next
        Next
        Dim dtSite As New DataTable
        If Not HttpContext.Current.Session("Eid") = 71 Then
            Dim strQuery As String = ""
            If HttpContext.Current.Session("USERROLE").ToString.ToUpper = "SU" Or HttpContext.Current.Session("USERROLE").ToString.ToUpper = "CORPORATEUSER" Then
                strQuery &= "select [tid],[Site LatLong] , [Site ID],[Site Name] from v" & HttpContext.Current.Session("Eid") & "Site where isauth=1"
            Else
                strQuery &= "select [tid],[Site LatLong] , [Site ID],[Site Name] from v" & HttpContext.Current.Session("Eid") & "Site where Company in (" & HttpContext.Current.Session("CompIDs") & ") and isauth=1"
            End If
            Dim oda As SqlDataAdapter = New SqlDataAdapter(strQuery, con)
            oda.Fill(dtSite)
        End If
        Dim d1 = ""
        Dim colSizeSite As Integer = dtSite.Columns.Count

        For Each Row As DataRow In dtSite.Rows
            If dtSite.Rows.IndexOf(Row) > 0 Then
                d1 &= "|"
            End If

            For i As Integer = 0 To colSizeSite - 1
                If (dtSite.Columns(i).ColumnName = "Site LatLong") Then
                    Dim latlongarr As String() = Row(i).ToString().Split(",")
                    d1 &= latlongarr(0) & "^"
                    d1 &= latlongarr(1) & "^"
                Else
                    d1 &= Row(i).ToString()
                    If i < colSizeSite - 1 Then
                        d1 &= "^"
                    End If
                End If
            Next
        Next

        'Dim d1 As String = IO.File.ReadAllText(CsvPath)

        Return d1 & "|" & CsvStr.ToString()

    End Function

    <WebMethod()> _
<Script.Services.ScriptMethod()> _
    Public Shared Function GetMarkerInfo(Id As String, IsVehical As String, Ids As String) As String
        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim con As New SqlConnection(conStr)
        Dim Info As String


        If HttpContext.Current.Session("Eid") = 71 Then
            Dim query As String = "select convert(nvarchar,g.tid)[TID],dms.udf_split('STATIC-USER-UserName',fld1)[User_Name],DMS.IdealTime(g.imieno) IdealTime,convert(nvarchar,Lattitude)[Lat], convert(nvarchar,longitude)[Long],convert(nvarchar,ctime,101)ctime, m2.fld1[vehicleNo], g.imieno, g.Speed "
            query &= " from mmm_mst_gpsdata g with (nolock) join mmm_mst_master m2 with (nolock) on m2.fld10=g.imieno "
            query &= "where m2.documenttype='IMEI Master'  and m2.eid=71 and m2.fld10='" & Id & "' "
            query &= " group by g.tid,imieno,ctime,m2.fld1,lattitude,longitude,speed having ctime=(select max(ctime)"
            query &= " from mmm_mst_gpsdata with (nolock) where imieno=g.imieno) "
            Dim oda As SqlDataAdapter = New SqlDataAdapter(query, con)
            Dim dt As New DataTable
            oda.Fill(dt)
            Dim t As New TimeSpan(0, Convert.ToInt32(dt.Rows(0).Item("IdealTime")), 0)
            Dim TotalHrs As Integer = t.Hours
            Dim TotalMints As Integer = t.Minutes
            Dim hr As String
            Dim mm As String
            hr = If(TotalHrs < 10, "0" & TotalHrs.ToString(), TotalHrs.ToString())
            mm = If(TotalMints < 10, "0" & TotalMints.ToString(), TotalMints.ToString())
            Dim dipTime As String = hr & ":" & mm

            Info = "<span style='font-weight:bold;'>IMEINO : " + dt.Rows(0).Item("imieno") + "</span> <br>User Name : " + dt.Rows(0).Item("User_Name") + "<br>Speed : " + dt.Rows(0).Item("Speed").ToString() + " Km/h <br>Ideal Time : " + dipTime.ToString() + "(HH:MM) <br>Last Record Time : " + dt.Rows(0).Item("ctime").ToString() + " <br> Lattitude : " + dt.Rows(0).Item("Lat").ToString() + "<br>Longitude : " + dt.Rows(0).Item("Long").ToString() + ""

            Return Info
        End If


        If Not IsVehical = "0" Then

            Dim query As String = "select fld10[SiteID],fld11[SiteName], fld13[SiteAddress]"
            query &= " from MMM_MST_MASTER where eid=58 and documenttype='site' and tid=" & Id

            Dim oda As SqlDataAdapter = New SqlDataAdapter(query, con)
            Dim dt As New DataTable
            oda.Fill(dt)
            Info = "<span style='font-weight:bold;'> SiteID : " + dt.Rows(0).Item("SiteID") + "</span><br>Site Name : " + dt.Rows(0).Item("SiteName") + "<br>Address: " + dt.Rows(0).Item("SiteAddress") + ""
            Return Info
        Else
            Dim query As String = "select convert(nvarchar,g.tid)[TID],max(m2.fld10)[Site_Name],DMS.IdealTime(g.imieno) IdealTime,convert(nvarchar,Lattitude)[Lat],convert(nvarchar,longitude)[Long],convert(nvarchar,ctime,101)ctime, m2.fld1[vehicleNo], g.imieno, g.Speed"
            query &= " from mmm_mst_gpsdata g with (nolock) join mmm_mst_master m2 with (nolock) on m2.fld12=g.imieno "
            query &= "where m2.documenttype='vehicle'  and m2.eid=" & HttpContext.Current.Session("Eid") & "  and m2.fld12='" & Id & "' "
            query &= " group by g.tid,imieno,ctime,m2.fld1,msensor,lattitude,longitude,speed having ctime=(select max(ctime)"
            query &= " from mmm_mst_gpsdata with (nolock) where imieno=g.imieno) "
            Dim oda As SqlDataAdapter = New SqlDataAdapter(query, con)
            Dim dt As New DataTable
            oda.Fill(dt)

            Dim t As New TimeSpan(0, Convert.ToInt32(dt.Rows(0).Item("IdealTime")), 0)

            'Dim TotalHrs As Integer = Convert.ToInt32(dt.Rows(0).Item("IdealTime")) / 60
            'Dim TotalMints As Integer = Convert.ToInt32(dt.Rows(0).Item("IdealTime")) Mod 60
            Dim TotalHrs As Integer = t.Hours
            Dim TotalMints As Integer = t.Minutes

            Dim hr As String
            Dim mm As String


            hr = If(TotalHrs < 10, "0" & TotalHrs.ToString(), TotalHrs.ToString())
            mm = If(TotalMints < 10, "0" & TotalMints.ToString(), TotalMints.ToString())

            Dim dipTime As String = hr & ":" & mm

            Info = "<span style='font-weight:bold;'>IMEINO : " + dt.Rows(0).Item("imieno") + "</span> <br>Vehicle Name : " + dt.Rows(0).Item("Site_Name") + " <br>Vehicle No : " + dt.Rows(0).Item("vehicleNo").ToString() + " <br>Speed : " + dt.Rows(0).Item("Speed").ToString() + " Km/h <br>Ideal Time : " + dipTime.ToString() + "(HH:MM) <br>Last Record Time : " + dt.Rows(0).Item("ctime").ToString() + " <br> Lattitude : " + dt.Rows(0).Item("Lat").ToString() + "<br>Longitude : " + dt.Rows(0).Item("Long").ToString() + ""

            Return Info

        End If

    End Function

    <System.Web.Services.WebMethod(EnableSession:=True)> _
    <Script.Services.ScriptMethod()> _
    Public Shared Function RefreshVechicals(IDs As String) As String
        Dim strList As String = ""
        Dim CsvStr As New StringBuilder()
        CsvStr.Clear()

        strList = IDs.TrimEnd(CChar(","))

        Dim str As String = ""

        If HttpContext.Current.Session("Eid") = 71 Then
            str = "select max(convert(nvarchar,g.tid))[TID],IMIENO,dms.udf_split('STATIC-USER-UserName',fld1)[User_Name],DMS.IdealTime(g.imieno) IdealTime,convert(nvarchar,Lattitude)[Lat],convert(nvarchar,longitude)[Long],convert(nvarchar,ctime,101)ctime,0[Group],0 as Isvehicle, m2.fld1 [VhNo] "
            If HttpContext.Current.Session("USERROLE").ToString.ToUpper = "SU" Then
                str = str & " from mmm_mst_gpsdata g with (nolock) join mmm_mst_master m2 with (nolock) on m2.fld10=g.imieno   where m2.documenttype='IMEI Master' and m2.eid=" & HttpContext.Current.Session("Eid") & " group by imieno,ctime,m2.fld1,lattitude,longitude having ctime=(select max(ctime) from mmm_mst_gpsdata with (nolock) where imieno=g.imieno) and Ltrim(IMIENO) <> '' "
            Else
                str = str & " from mmm_mst_gpsdata g with (nolock) join mmm_mst_master m2 with (nolock) on m2.fld10=g.imieno where m2.documenttype='IMEI Master'  and m2.fld11 in (Select fld1 from mmm_mst_master where Tid in(" & HttpContext.Current.Session("CompIDs") & ")) and m2.eid=" & HttpContext.Current.Session("Eid") & " group by imieno,ctime,m2.fld1,lattitude,longitude having ctime=(select max(ctime) from mmm_mst_gpsdata with (nolock) where imieno=g.imieno) and Ltrim(IMIENO) <> '' "
            End If
        Else
            str = "select convert(nvarchar,g.tid)[TID],IMIENO,max(m2.fld10)[Site_Name],DMS.IdealTime(g.imieno) IdealTime,convert(nvarchar,Lattitude)[Lat],convert(nvarchar,longitude)[Long],convert(nvarchar,ctime,101)ctime,max(m2.fld14)[Group],0 as Isvehicle ,m2.fld1 [VhNo] "
            If HttpContext.Current.Session("USERROLE").ToString.ToUpper = "SU" Or HttpContext.Current.Session("USERROLE").ToString.ToUpper = "CORPORATEUSER" Then
                str = str & " from mmm_mst_gpsdata g with (nolock) join mmm_mst_master m2 with (nolock) on m2.fld12=g.imieno   where m2.documenttype='vehicle' and  m2.eid=" & HttpContext.Current.Session("Eid") & " group by g.tid,imieno,ctime,m2.fld1,msensor,lattitude,longitude,speed having ctime=(select max(ctime) from mmm_mst_gpsdata with (nolock) where imieno=g.imieno)"
            Else
                str = str & " from mmm_mst_gpsdata g with (nolock) join mmm_mst_master m2 with (nolock) on m2.fld12=g.imieno inner join  dbo.split('" & HttpContext.Current.Session("RID") & "', ',') s on s.items in (select items from dbo.split(m2.fld16, ','))   where m2.documenttype='vehicle' "
                If Not strList.Trim() = "" Then
                    str = str & "  and m2.fld14 in (" & strList & ")  "
                End If
                str = str & " and m2.eid=" & HttpContext.Current.Session("Eid") & " group by g.tid,imieno,ctime,m2.fld1,msensor,lattitude,longitude,speed having ctime=(select max(ctime) from mmm_mst_gpsdata where imieno=g.imieno)"
            End If
        End If


        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim con As New SqlConnection(conStr)
        Dim apikey As String = String.Empty
        Dim oda As SqlDataAdapter = New SqlDataAdapter("", con)
        Dim ds As New DataSet()
        oda.SelectCommand.CommandText = str
        oda.SelectCommand.CommandTimeout = 180
        oda.Fill(ds)
        Dim colSize As Integer = ds.Tables(0).Columns.Count
        For Each Row As DataRow In ds.Tables(0).Rows
            If ds.Tables(0).Rows.IndexOf(Row) > 0 Then
                CsvStr.Append("|")
            End If

            For i As Integer = 0 To colSize - 1
                CsvStr.Append(Row(i).ToString())
                If i < colSize - 1 Then
                    CsvStr.Append("^")
                End If
            Next
        Next
        Return CsvStr.ToString()

    End Function

    Public Sub BindVehicle()
        Try
            LstVehicle.Items.Clear()
            'Change Check Box Text Color
            Dim apikey As String = String.Empty
            Dim j As Integer = 0
            Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
            Dim con As New SqlConnection(conStr)
            Dim oda As SqlDataAdapter = New SqlDataAdapter("", con)

            Dim str As String = "select max(convert(nvarchar,g.tid))[TID],IMIENO,dms.udf_split('STATIC-USER-UserName',fld1)[User_Name],DMS.IdealTime(g.imieno) IdealTime,convert(nvarchar,Lattitude)[Lat],convert(nvarchar,longitude)[Long],convert(nvarchar,ctime,101)ctime,0[Group],0 as Isvehicle, m2.fld1 [VhNo] "
            If HttpContext.Current.Session("USERROLE").ToString.ToUpper = "SU" Or HttpContext.Current.Session("USERROLE").ToString.ToUpper = "CORPORATEUSER" Then
                str = str & " from mmm_mst_gpsdata g with (nolock) join mmm_mst_master m2 with (nolock) on m2.fld10=g.imieno   where m2.documenttype='IMEI Master' and m2.eid=" & HttpContext.Current.Session("Eid") & " group by imieno,ctime,m2.fld1,lattitude,longitude having ctime=(select max(ctime) from mmm_mst_gpsdata with (nolock) where imieno=g.imieno) and Ltrim(IMIENO) <> '' "
            Else
                str = str & " from mmm_mst_gpsdata g with (nolock) join mmm_mst_master m2 with (nolock) on m2.fld10=g.imieno where m2.documenttype='IMEI Master'  and m2.fld11 in (Select fld1 from mmm_mst_master where Tid in(" & HttpContext.Current.Session("CompIDs") & ")) and m2.eid=" & HttpContext.Current.Session("Eid") & " group by imieno,ctime,m2.fld1,lattitude,longitude having ctime=(select max(ctime) from mmm_mst_gpsdata with (nolock) where imieno=g.imieno) and Ltrim(IMIENO) <> '' "
            End If
            oda.SelectCommand.CommandText = str
            oda.SelectCommand.CommandTimeout = 180
            dtVechical.Clear()
            oda.Fill(dtVechical)
            For i As Integer = 0 To dtVechical.Rows.Count - 1
                If Convert.ToInt32(dtVechical.Rows(i).Item("IdealTime").ToString) > 10 And Convert.ToInt32(dtVechical.Rows(i).Item("IdealTime").ToString) <= 600 And Convert.ToDateTime(dtVechical.Rows(i).Item("ctime")) = DateAndTime.Today() Then
                    LstVehicle.Items.Add(dtVechical.Rows(i).Item("VhNo").ToString())
                    LstVehicle.Items(j).Text = i + 1 & " " & dtVechical.Rows(i).Item("User_Name").ToString()
                    LstVehicle.Items(j).Value = dtVechical.Rows(i).Item("IMIENO").ToString()
                    LstVehicle.Items(j).Attributes.Add("style", "color:Black;")
                    LstVehicle.Items(j).Attributes.Add("onclick", "focusMarker(this);")
                    j = j + 1
                ElseIf Convert.ToInt32(dtVechical.Rows(i).Item("IdealTime").ToString) >= 0 And Convert.ToInt32(dtVechical.Rows(i).Item("IdealTime").ToString) <= 10 And Convert.ToDateTime(dtVechical.Rows(i).Item("ctime")) = DateAndTime.Today() Then
                    LstVehicle.Items.Add(dtVechical.Rows(i).Item("VhNo").ToString())
                    LstVehicle.Items(j).Text = i + 1 & " " & dtVechical.Rows(i).Item("User_Name").ToString()
                    LstVehicle.Items(j).Value = dtVechical.Rows(i).Item("IMIENO").ToString()
                    LstVehicle.Items(j).Attributes.Add("style", "color:Green;")
                    LstVehicle.Items(j).Attributes.Add("onclick", "focusMarker(this);")
                    j = j + 1
                ElseIf (Convert.ToInt32(dtVechical.Rows(i).Item("IdealTime").ToString) > 600 And Convert.ToInt32(dtVechical.Rows(i).Item("IdealTime").ToString) <= 1440) Or (Convert.ToDateTime(dtVechical.Rows(i).Item("ctime")) = DateAndTime.Today() And Convert.ToInt32(dtVechical.Rows(i).Item("IdealTime").ToString) > 1440) Then
                    LstVehicle.Items.Add(dtVechical.Rows(i).Item("VhNo").ToString())
                    LstVehicle.Items(j).Text = i + 1 & " " & dtVechical.Rows(i).Item("User_Name").ToString()
                    LstVehicle.Items(j).Value = dtVechical.Rows(i).Item("IMIENO").ToString()
                    LstVehicle.Items(j).Attributes.Add("style", "color:#B86A84;")
                    LstVehicle.Items(j).Attributes.Add("onclick", "focusMarker(this);")
                    j = j + 1
                Else
                    LstVehicle.Items.Add(dtVechical.Rows(i).Item("VhNo").ToString())
                    LstVehicle.Items(j).Text = i + 1 & " " & dtVechical.Rows(i).Item("User_Name").ToString()
                    LstVehicle.Items(j).Value = dtVechical.Rows(i).Item("IMIENO").ToString()
                    LstVehicle.Items(j).Attributes.Add("style", "color:Red;")
                    LstVehicle.Items(j).Attributes.Add("onclick", "focusMarker(this);")
                    j = j + 1
                End If
                'End If
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
                url = "<script type='text/javascript'>google.load('maps', '4.7', { 'other_params': 'sensor=true' }); </script>"
            End If
            oda.Dispose()
            con.Dispose()
        Catch ex As Exception

        End Try
    End Sub


    Private Sub GenerateJson()

        Dim strB As New StringBuilder()

        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim con As New SqlConnection(conStr)

        Dim str As String = "Select Tid,ltrim(rtrim(left(replace(fld20,',','        '),9)))[Lat] ,rtrim(ltrim(right(replace(fld20,',','        '),9)))[Long],fld10[Site ID], fld11[Site Name], fld12[Site Type], fld13[Site Address], fld1[Company] from MMM_MST_MASTER where eid=58 and documenttype='site' and fld1=650808"
        Dim oda As SqlDataAdapter = New SqlDataAdapter(str, con)
        Dim dt As New DataTable
        oda.Fill(dt)

        If dt.Rows.Count > 0 Then

            For i As Integer = 0 To dt.Rows.Count - 1
                strB.Append(dt.Rows(i).Item("Tid") & "^")
                strB.Append(dt.Rows(i).Item("Lat") & "^")
                strB.Append(dt.Rows(i).Item("Long") & "^")
                strB.Append(dt.Rows(i).Item("Site ID") & "^")
                strB.Append(dt.Rows(i).Item("Site Name") & "|")
            Next

        End If

        Dim d = strB.ToString()

    End Sub

End Class
