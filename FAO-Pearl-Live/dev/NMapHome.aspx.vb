Imports System.Net.HttpWebRequest
Imports System.Net
Imports System.IO
Imports System.Data
Imports System.Configuration
Imports System.Data.SqlClient
Imports System.Collections.Generic
Imports System.Xml
Imports System.Web.Services
Imports Newtonsoft.Json


Partial Class NMapHome
    Inherits System.Web.UI.Page

    Public Shared dtVechical As New DataTable
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
    Protected Sub Page_Load(sender As Object, e As System.EventArgs) Handles Me.Load

        'Session("uid") = 5897 '5906 '
        'Session("username") = "vinay kumar"
        'Session("userrole") = "su" '"CircleUser" '
        'Session("code") = "Industowers"
        'Session("userimage") = "2.jpg"
        'Session("clogo") = "hfcl.png"
        'Session("eid") = 54
        'Session("islocal") = "true"
        'Session("ipaddress") = "vinay"
        'Session("macaddress") = "vinay"
        'Session("intime") = Now
        'Session("email") = "vinay.kumar@myndsol.com"
        'Session("lid") = "25"
        'Session("headerstrip") = "hfclstrip.jpg"
        'Session("roles") = "su" '"CircleUser" ' 

        'Session("uid") = 6912
        'Session("username") = "Prashant Singh Sengar"
        'Session("userrole") = "SU" '"CircleUser" '
        'Session("code") = "RILIANCE"
        'Session("userimage") = "2.jpg"
        'Session("clogo") = "hfcl.png"
        'Session("eid") = 60
        'Session("islocal") = "true"
        'Session("ipaddress") = "vinay"
        'Session("macaddress") = "vinay"
        'Session("intime") = Now
        'Session("email") = "vinay.kumar@myndsol.com"
        'Session("lid") = "25"
        'Session("headerstrip") = "hfclstrip.jpg"
        'Session("roles") = "SU" '"CircleUser" ' 

        'Session("uid") = 7503
        'Session("username") = "Prashant Singh Sengar"
        'Session("userrole") = "SU" '"CircleUser" '
        'Session("code") = "Delhivery"
        'Session("userimage") = "2.jpg"
        'Session("clogo") = "hfcl.png"
        'Session("eid") = 72
        'Session("islocal") = "true"
        'Session("ipaddress") = "vinay"
        'Session("macaddress") = "vinay"
        'Session("intime") = Now
        'Session("email") = "vinay.kumar@myndsol.com"
        'Session("lid") = "25"
        'Session("headerstrip") = "hfclstrip.jpg"
        'Session("roles") = "SU" '"CircleUser" 


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
                oda.Fill(ds, "Entity")
                hdnRefTime.Value = Convert.ToInt32(ds.Tables("Entity").Rows(0).Item("ReloadSeconds")) * 1000
                'hdnRefTime.Value = 70000
                If Session("userrole").ToString().ToUpper() = "SU" Then
                    hdnCluster.Value = "SU"
                ElseIf Session("userrole").ToString().ToUpper() = "CORPORATEUSER" Then
                    hdnCluster.Value = "CORPORATEUSER"
                Else
                    oda.SelectCommand.CommandText = "select uid,fld1[ClusterID],rolename from mmm_ref_role_user  where eid=" & Session("EID") & " and uid= " & Session("uid") & " and rolename='" & Session("userrole") & "'"
                    oda.Fill(ds, "Cluster")
                    hdnCluster.Value = ds.Tables("Cluster").Rows(0).Item("ClusterID").ToString()
                    Session("RID") = ds.Tables("Cluster").Rows(0).Item("ClusterID").ToString()
                End If



                Dim dtt As New DataTable()
                Circle.Items.Clear()
                oda.SelectCommand.CommandText = "select distinct tid,fld1 from mmm_mst_master where DOCUMENTTYPE='Site Type' and eid=" & Session("EID") & " and fld1<>'' order by fld1"
                oda.Fill(dtt)
                For i As Integer = 0 To dtt.Rows.Count - 1
                    If dtt.Rows(i).Item("fld1").ToString.ToUpper = "HUB" Then
                        Circle.Items.Add(Convert.ToString(dtt.Rows(i).Item("fld1").ToString))
                        Circle.Items(i).Value = dtt.Rows(i).Item("tid").ToString
                        Circle.Items(i).Attributes.Add("style", "color:Orange;")
                    ElseIf dtt.Rows(i).Item("fld1").ToString.ToUpper = "BSC" Or dtt.Rows(i).Item("fld1").ToString.ToUpper = "PC" Then
                        Circle.Items.Add(Convert.ToString(dtt.Rows(i).Item("fld1").ToString))
                        Circle.Items(i).Value = dtt.Rows(i).Item("tid").ToString
                        Circle.Items(i).Attributes.Add("style", "color:lightblue;")
                    ElseIf dtt.Rows(i).Item("fld1").ToString.ToUpper = "STRATEGIC" Or dtt.Rows(i).Item("fld1").ToString.ToUpper = "DC" Then
                        Circle.Items.Add(Convert.ToString(dtt.Rows(i).Item("fld1").ToString))
                        Circle.Items(i).Value = dtt.Rows(i).Item("tid").ToString
                        Circle.Items(i).Attributes.Add("style", "color:DarkBlue;")
                    ElseIf dtt.Rows(i).Item("fld1").ToString.ToUpper = "NON STRATEGIC" Or dtt.Rows(i).Item("fld1").ToString.ToUpper = "DPC" Then
                        Circle.Items.Add(Convert.ToString(dtt.Rows(i).Item("fld1").ToString))
                        Circle.Items(i).Value = dtt.Rows(i).Item("tid").ToString
                        Circle.Items(i).Attributes.Add("style", "color:Blueviolet;")
                    End If
                    If Circle.Items.Count > 0 Then
                        Circle.Items(i).Selected = True
                        Circle.Items(i).Attributes.Add("onclick", "ShowHideMap(this)")
                    End If
                Next


                oda.SelectCommand.CommandText = "select * from mmm_mst_master with(nolock) where eid=" & Session("EID") & " and documenttype='vehicle type' "
                dtt.Clear()
                oda.Fill(dtt)
                For i As Integer = 0 To dtt.Rows.Count - 1
                    chkvtype.Items.Add(Convert.ToString(dtt.Rows(i).Item("fld1").ToString))
                    chkvtype.Items(i).Value = dtt.Rows(i).Item("tid").ToString
                    chkvtype.Items(i).Attributes.Add("onclick", "ShowHideMap(this)")
                    chkvtype.Items(i).Selected = True
                Next
                BindAll()
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




    Private Sub BindAll()
        Dim id As String = ""
        For i As Integer = 0 To Circle.Items.Count - 1

            id = id & Circle.Items(i).Value & ","

        Next
        For i As Integer = 0 To chkvtype.Items.Count - 1

            id = id & chkvtype.Items(i).Value & ","

        Next
        If id.Length > 0 Then
            id = Left(id, id.Length - 1)
            Maprendorforsite(id)
        End If
    End Sub

    Protected Sub FilterSite(sender As Object, e As System.EventArgs)
        Dim id As String = ""
        For i As Integer = 0 To Circle.Items.Count - 1
            If Circle.Items(i).Selected = True Then
                id = id & Circle.Items(i).Value & ","
            End If
        Next
        For i As Integer = 0 To chkvtype.Items.Count - 1
            If chkvtype.Items(i).Selected = True Then
                id = id & chkvtype.Items(i).Value & ","
            End If
        Next
        If id.Length > 0 Then
            id = Left(id, id.Length - 1)
            Maprendorforsite(id)
        End If
    End Sub
    Protected Function Maprendorforsite(ByVal sid As String) As String
        Try
            LstVehicle.Items.Clear()
            'Change Check Box Text Color
            If Circle.Items.Count > 0 Then
                Circle.Items(0).Attributes.Add("style", "color:lightblue;")
            End If
            If Circle.Items.Count > 1 Then
                Circle.Items(1).Attributes.Add("style", "color:Orange;")
            End If
            If Circle.Items.Count > 2 Then
                Circle.Items(2).Attributes.Add("style", "color:Blueviolet;")
            End If
            If Circle.Items.Count > 3 Then
                Circle.Items(3).Attributes.Add("style", "color:DarkBlue;")
            End If

            Dim j As Integer = 0
            Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
            Dim con As New SqlConnection(conStr)
            Dim oda As SqlDataAdapter = New SqlDataAdapter("", con)

            Dim dtImieNo As New DataTable
            Dim qry As String = ""
            If HttpContext.Current.Session("USERROLE").ToString.ToUpper = "SU" Or HttpContext.Current.Session("USERROLE").ToString.ToUpper = "CORPORATEUSER" Then
                qry = "select fld12[IMIENO] from mmm_mst_master where documenttype='vehicle' and eid=" & Session("Eid") & " and fld12<>'' and fld12<>'0'"
            Else
                qry = "select fld12[IMIENO] from mmm_mst_master inner join  dbo.split('" & HttpContext.Current.Session("RID") & "', ',') s on s.items in (select items from dbo.split(fld16, ','))  where documenttype='vehicle' and eid=" & Session("Eid") & " and fld12<>'' and fld12<>'0'"
            End If
            oda.SelectCommand.CommandText = qry
            oda.Fill(dtImieNo)
            dtVechical.Clear()

            For i As Integer = 0 To dtImieNo.Rows.Count - 1
                Dim str As String = "select convert(nvarchar,g.tid)[TID],IMIENO,max(m2.fld10)[Site_Name],DMS.IdealTime(g.imieno) IdealTime,convert(nvarchar,Lattitude)[Lat],convert(nvarchar,longitude)[Long],convert(nvarchar,ctime,101)ctime,max(m2.fld14)[Group],0 as Isvehicle, m2.fld1 [VhNo] "
                str = str & " from mmm_mst_gpsdata g with (nolock) join mmm_mst_master m2 with (nolock) on m2.fld12=g.imieno   where m2.documenttype='vehicle' and m2.fld14 in (" & sid & ")  and m2.eid=" & Session("Eid") & " group by g.tid,imieno,ctime,m2.fld1,msensor,lattitude,longitude,speed having ctime=(select max(ctime) from mmm_mst_gpsdata with (nolock) where imieno=g.imieno) and Ltrim(IMIENO) <> '' and  IMIENO='" & dtImieNo.Rows(i).Item("IMIENO") & "'"
                oda.SelectCommand.CommandText = str
                oda.SelectCommand.CommandTimeout = 180
                oda.Fill(dtVechical)
            Next

            ' Dim str As String = "select convert(nvarchar,g.tid)[TID],IMIENO,max(m2.fld10)[Site_Name],DMS.IdealTime(g.imieno) IdealTime,convert(nvarchar,Lattitude)[Lat],convert(nvarchar,longitude)[Long],convert(nvarchar,ctime,101)ctime,max(m2.fld14)[Group],0 as Isvehicle, m2.fld1 [VhNo] "
            ' If HttpContext.Current.Session("USERROLE").ToString.ToUpper = "SU" Or HttpContext.Current.Session("USERROLE").ToString.ToUpper = "CORPORATEUSER" Then
            ' str = str & " from mmm_mst_gpsdata g with (nolock) join mmm_mst_master m2 with (nolock) on m2.fld12=g.imieno   where m2.documenttype='vehicle' and m2.fld14 in (" & sid & ")  and m2.eid=54 group by g.tid,imieno,ctime,m2.fld1,msensor,lattitude,longitude,speed having ctime=(select max(ctime) from mmm_mst_gpsdata with (nolock) where imieno=g.imieno) and Ltrim(IMIENO) <> '' and  "
            'Else
            ' str = str & " from mmm_mst_gpsdata g with (nolock) join mmm_mst_master m2 with (nolock) on m2.fld12=g.imieno inner join  dbo.split('" & HttpContext.Current.Session("RID") & "', ',') s on s.items in (select items from dbo.split(m2.fld16, ','))   where m2.documenttype='vehicle' and m2.fld14 in (" & sid & ")  and m2.eid=54 group by g.tid,imieno,ctime,m2.fld1,msensor,lattitude,longitude,speed having ctime=(select max(ctime) from mmm_mst_gpsdata where imieno=g.imieno) and Ltrim(IMIENO) <> '' "
            ' End If

            'oda.SelectCommand.CommandText = str
            'oda.SelectCommand.CommandTimeout = 180
            'oda.Fill(dtVechical)

            For i As Integer = 0 To dtVechical.Rows.Count - 1
                If Convert.ToInt32(dtVechical.Rows(i).Item("IdealTime").ToString) > 10 And Convert.ToInt32(dtVechical.Rows(i).Item("IdealTime").ToString) <= 600 And Convert.ToDateTime(dtVechical.Rows(i).Item("ctime")) = DateAndTime.Today() Then
                    LstVehicle.Items.Add(dtVechical.Rows(i).Item("VhNo").ToString())
                    LstVehicle.Items(j).Text = i + 1 & " " & dtVechical.Rows(i).Item("Site_Name").ToString()
                    LstVehicle.Items(j).Value = dtVechical.Rows(i).Item("IMIENO").ToString()
                    LstVehicle.Items(j).Attributes.Add("style", "color:Black;")
                    LstVehicle.Items(j).Attributes.Add("onclick", "focusMarker(this);")
                    j = j + 1
                ElseIf Convert.ToInt32(dtVechical.Rows(i).Item("IdealTime").ToString) >= 0 And Convert.ToInt32(dtVechical.Rows(i).Item("IdealTime").ToString) <= 10 Then 'And Convert.ToDateTime(dtVechical.Rows(i).Item("ctime")) = DateAndTime.Today() Then
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
            'If apikey = "" Then
            '    jqueryInclude.Attributes.Add("type", "text/javascript")
            '    'jqueryInclude.Attributes.Add("src", "http://maps.google.com/maps/api/js?sensor=false")
            '    url = ""
            '    Page.Header.Controls.Add(jqueryInclude)
            'Else
            '    jqueryInclude.Attributes.Add("type", "text/javascript")
            '    'jqueryInclude.Attributes.Add("src", "http://www.google.com/jsapi?key=" + apikey + "")
            '    Page.Header.Controls.Add(jqueryInclude)
            '    url = "<script type='text/javascript'>google.load('maps', '4.7', { 'other_params': 'sensor=true' }); </script>"
            'End If
            oda.Dispose()
            con.Dispose()
            Return "0"
        Catch ex As Exception
            Throw
        End Try
    End Function


    Public Shared Function ConvertDataTableTojSonString(dataTable As DataTable) As [String]
        Dim serializer As New System.Web.Script.Serialization.JavaScriptSerializer()
        Dim tableRows As New List(Of Dictionary(Of [String], [Object]))()
        Dim row As Dictionary(Of [String], [Object])

        For Each dr As DataRow In dataTable.Rows
            row = New Dictionary(Of [String], [Object])()
            For Each col As DataColumn In dataTable.Columns
                row.Add(col.ColumnName, dr(col))
            Next
            tableRows.Add(row)
        Next
        serializer.MaxJsonLength = 900000000
        Dim i As Integer = tableRows.Count
        Return serializer.Serialize(tableRows)
    End Function

    'Public Shared CsvPath As String = HttpContext.Current.Server.MapPath("DOCS/CsvJson.txt")

    <WebMethod()> _
   <Script.Services.ScriptMethod()> _
    Public Shared Function GetMarkerListJSON2(IDs As String) As String
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

        Dim fStr As String = CsvStr.ToString()
        If fStr.First() = "{" Then
            fStr = fStr.Substring(1, fStr.Length - 1)
        End If

        Dim d1 As String = IO.File.ReadAllText(CsvPath)
        Return d1 & "|" & fStr

    End Function


    <WebMethod()> _
 <Script.Services.ScriptMethod()> _
    Public Shared Function GetMarkerInfo(Id As String, IsVehical As String, Ids As String) As String
        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim con As New SqlConnection(conStr)
        Dim Info As String


        If Not IsVehical = "0" Then

            Dim query As String = "select m.tid[TID],m.fld10[SiteID],m.fld11[SiteName],"
            query &= "m.fld13[Address],m1.fld1[Site],ltrim(rtrim(left(replace(m.fld21,',','        '),9)))[Lat] ,rtrim(ltrim(right(replace(m.fld21,',','        '),9)))[Long], m.fld12[Group],m.fld11[Site Name],u.UserName[OandM Head],u1.UserName[Maintenance Head], u2.UserName[Opex Manager],u3.UserName[Security Manager],u4.UserName[Zonal Head],u5.UserName[Cluster Manager],m2.fld1[Supervisor], m3.fld1[Technician], m.fld2[No of OPCOs],m.fld19[Anchor OPCO],m.fld15[Cluster] from  mmm_mst_master m with (nolock) inner join mmm_mst_user u with (nolock) on convert(nvarchar,u.uid)=m.fld23 join mmm_mst_user u1 with (nolock) on convert(nvarchar,u1.uid)=m.fld24 join mmm_mst_user u2 with (nolock) on convert(nvarchar,u2.uid)=m.fld25  join mmm_mst_user u3 with (nolock) on convert(nvarchar,u3.uid)=m.fld26  join mmm_mst_user u4 with (nolock) on convert(nvarchar,u4.uid)=m.fld27 join mmm_mst_user u5 with (nolock) on convert(nvarchar,u5.uid)=m.fld28 join mmm_mst_master m2 with (nolock) on convert(nvarchar,m2.tid)=m.fld29 join mmm_mst_master m3 with (nolock) on convert(nvarchar,m3.tid)=m.fld3 join mmm_mst_master m1 with (nolock) on convert(nvarchar,m1.tid)=m.fld12  where m.documenttype='Site' and m1.documenttype='Site Type' and m1.eid=" & HttpContext.Current.Session("Eid") & " and m.eid=" & HttpContext.Current.Session("Eid") & " and  u.eid=" & HttpContext.Current.Session("Eid") & " and "
            query &= " u1.eid=" & HttpContext.Current.Session("Eid") & " and u2.eid=" & HttpContext.Current.Session("Eid") & " and m2.eid=" & HttpContext.Current.Session("Eid") & " and m3.eid=" & HttpContext.Current.Session("Eid") & " and m.Tid=" & Id
            Dim oda As SqlDataAdapter = New SqlDataAdapter(query, con)
            Dim dt As New DataTable
            oda.Fill(dt)
            Info = "<span style='font-weight:bold;'> SiteID : " + dt.Rows(0).Item("SiteID") + "</span><br>Site : " + dt.Rows(0).Item("Site") + "<br>Site Name : " + dt.Rows(0).Item("SiteName") + "<br>OandM Head: " + dt.Rows(0).Item("OandM Head") + " <br> Maintenance Head : " + dt.Rows(0).Item("Maintenance Head") + " <br>Opex Manager : " + dt.Rows(0).Item("Opex Manager") + "<br>Security Manager : " + dt.Rows(0).Item("Security Manager") + "<br>Zonal Head : " + dt.Rows(0).Item("Zonal Head") + "<br>Cluster Manager :" + dt.Rows(0).Item("Cluster Manager") + "<br>Supervisor : " + dt.Rows(0).Item("Supervisor") + "<br>Technician : " + dt.Rows(0).Item("Technician") + "<br>No of OPCOs : " + dt.Rows(0).Item("No of OPCOs") + " <br>Anchor OPCO : " + dt.Rows(0).Item("Anchor OPCO") + ""
            Return Info
        Else
            Dim query As String = "select convert(nvarchar,g.tid)[TID],max(m2.fld10)[Site_Name],DMS.IdealTime(g.imieno) IdealTime,convert(nvarchar,Lattitude)[Lat],convert(nvarchar,longitude)[Long],convert(nvarchar,ctime,101)ctime, m2.fld1[vehicleNo], g.imieno, g.Speed"
            query &= " from mmm_mst_gpsdata g with (nolock) join mmm_mst_master m2 with (nolock) on m2.fld12=g.imieno "
            query &= "where m2.documenttype='vehicle' and m2.fld14 in (" & Ids.TrimEnd(CChar(",")) & ") and m2.eid=" & HttpContext.Current.Session("Eid") & "  and m2.fld12='" & Id & "' "
            query &= " group by g.tid,imieno,ctime,m2.fld1,msensor,lattitude,longitude,speed having ctime=(select max(ctime)"
            query &= " from mmm_mst_gpsdata with (nolock) where imieno=g.imieno) "
            Dim oda As SqlDataAdapter = New SqlDataAdapter(query, con)
            Dim dt As New DataTable
            oda.Fill(dt)

            Dim TotalHrs As Integer = Convert.ToInt32(dt.Rows(0).Item("IdealTime")) / 60
            Dim TotalMints As Integer = Convert.ToInt32(dt.Rows(0).Item("IdealTime")) Mod 60

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
        'Dim str As String = "select convert(nvarchar,g.tid)[TID],IMIENO,max(m2.fld10)[Site_Name],DMS.IdealTime(g.imieno) IdealTime,convert(nvarchar,Lattitude)[Lat],convert(nvarchar,longitude)[Long],convert(nvarchar,ctime,101)ctime,max(m2.fld14)[Group],0 as Isvehicle ,m2.fld1 [VhNo] "
        'If HttpContext.Current.Session("USERROLE").ToString.ToUpper = "SU" Or HttpContext.Current.Session("USERROLE").ToString.ToUpper = "CORPORATEUSER" Then
        '    str = str & " from mmm_mst_gpsdata g with (nolock) join mmm_mst_master m2 with (nolock) on m2.fld12=g.imieno   where m2.documenttype='vehicle' and m2.fld14 in (" & strList & ")  and m2.eid=54 group by g.tid,imieno,ctime,m2.fld1,msensor,lattitude,longitude,speed having ctime=(select max(ctime) from mmm_mst_gpsdata with (nolock) where imieno=g.imieno)"
        'Else
        '    str = str & " from mmm_mst_gpsdata g with (nolock) join mmm_mst_master m2 with (nolock) on m2.fld12=g.imieno inner join  dbo.split('" & HttpContext.Current.Session("RID") & "', ',') s on s.items in (select items from dbo.split(m2.fld16, ','))   where m2.documenttype='vehicle' and m2.fld14 in (" & strList & ")  and m2.eid=54 group by g.tid,imieno,ctime,m2.fld1,msensor,lattitude,longitude,speed having ctime=(select max(ctime) from mmm_mst_gpsdata where imieno=g.imieno)"
        'End If
        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim con As New SqlConnection(conStr)
        'Dim apikey As String = String.Empty
        Dim oda As SqlDataAdapter = New SqlDataAdapter("", con)
        Dim ds As New DataSet()
        'oda.SelectCommand.CommandText = str
        'oda.SelectCommand.CommandTimeout = 180
        'oda.Fill(ds)

        Dim dtImieNo As New DataTable
        Dim qry As String = ""
        If HttpContext.Current.Session("USERROLE").ToString.ToUpper = "SU" Or HttpContext.Current.Session("USERROLE").ToString.ToUpper = "CORPORATEUSER" Then
            qry = "select fld12[IMIENO] from mmm_mst_master where documenttype='vehicle' and eid=" & HttpContext.Current.Session("Eid") & " and fld12<>'' and fld12<>'0'"
        Else
            qry = "select fld12[IMIENO] from mmm_mst_master inner join  dbo.split('" & HttpContext.Current.Session("RID") & "', ',') s on s.items in (select items from dbo.split(fld16, ','))  where documenttype='vehicle' and eid=" & HttpContext.Current.Session("Eid") & " and fld12<>'' and fld12<>'0'"
        End If
        oda.SelectCommand.CommandText = qry
        oda.Fill(dtImieNo)
        dtVechical.Clear()

        For i As Integer = 0 To dtImieNo.Rows.Count - 1
            Dim str As String = "select convert(nvarchar,g.tid)[TID],IMIENO,max(m2.fld10)[Site_Name],DMS.IdealTime(g.imieno) IdealTime,convert(nvarchar,Lattitude)[Lat],convert(nvarchar,longitude)[Long],convert(nvarchar,ctime,101)ctime,max(m2.fld14)[Group],0 as Isvehicle, m2.fld1 [VhNo] "
            str = str & " from mmm_mst_gpsdata g with (nolock) join mmm_mst_master m2 with (nolock) on m2.fld12=g.imieno   where m2.documenttype='vehicle' and m2.fld14 in (" & strList & ")  and m2.eid=" & HttpContext.Current.Session("Eid") & " group by g.tid,imieno,ctime,m2.fld1,msensor,lattitude,longitude,speed having ctime=(select max(ctime) from mmm_mst_gpsdata with (nolock) where imieno=g.imieno) and Ltrim(IMIENO) <> '' and  IMIENO='" & dtImieNo.Rows(i).Item("IMIENO") & "'"
            oda.SelectCommand.CommandText = str
            oda.SelectCommand.CommandTimeout = 180
            oda.Fill(ds)
        Next

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



    Protected Sub Button1_Click(sender As Object, e As EventArgs)
        'Dim json As String = IO.File.ReadAllText(strPathJson)

        'Dim json1 As String = json & "]"
        'Dim table As DataTable
        'If json <> "" Then
        '    table = JsonConvert.DeserializeObject(Of DataTable)(json1) 'Getting the file into Datatable
        'End If

        'Dim CsvStr As New StringBuilder()

        'Dim colSize As Integer = table.Columns.Count

        'For Each Row As DataRow In table.Rows
        '    If table.Rows.IndexOf(Row) > 0 Then
        '        CsvStr.Append("|")
        '    End If

        '    For i As Integer = 0 To colSize - 1
        '        If i = 0 Or i = 1 Or i = 2 Or i = 4 Or i = 6 Or i = 7 Or i = 8 Or i = 21 Then
        '            CsvStr.Append(Row(i).ToString())
        '            If i < colSize - 1 Then
        '                CsvStr.Append("^")
        '            End If
        '        Else
        '            Continue For
        '        End If

        '    Next

        'Next

        'File.WriteAllText(HttpContext.Current.Server.MapPath("DOCS/csvJson.txt"), CsvStr.ToString())


    End Sub


End Class
