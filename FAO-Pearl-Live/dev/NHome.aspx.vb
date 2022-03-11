Imports System.Data
Imports System.Data.SqlClient
Imports Microsoft.VisualBasic.FileIO
Imports System.Web.Services
Imports System.Web.Script.Serialization

Partial Class NHome
    Inherits System.Web.UI.Page

    Public Shared dtVechical As New DataTable

    Protected Sub Page_Load(sender As Object, e As System.EventArgs) Handles Me.Load
        ''for SU 
        'Session("UID") = 5897 '6231
        'Session("USERNAME") = "Prashant Singh Sengar"
        'Session("USERROLE") = "SU"
        'Session("CODE") = "INDUSTOWERS"
        'Session("USERIMAGE") = "2.jpg"
        'Session("CLOGO") = "hfcl.png"
        'Session("EID") = 54
        'Session("ISLOCAL") = "TRUE"
        'Session("IPADDRESS") = "Vinay"
        'Session("MACADDRESS") = "Vinay"
        'Session("INTIME") = Now
        'Session("EMAIL") = "vinay.kumar@myndsol.com"
        'Session("LID") = "25"
        'Session("HEADERSTRIP") = "hfclstrip.jpg"
        'Session("ROLES") = "SU"


        'Session("UID") = 5906
        'Session("USERNAME") = "Vinay Kumar"
        'Session("USERROLE") = "ClusterManager"
        'Session("CODE") = "INDUSTOWERS"
        'Session("USERIMAGE") = "2.jpg"
        'Session("CLOGO") = "hfcl.png"
        'Session("EID") = 54
        'Session("ISLOCAL") = "TRUE"
        'Session("IPADDRESS") = "Vinay"
        'Session("MACADDRESS") = "Vinay"
        'Session("INTIME") = Now
        'Session("EMAIL") = "vinay.kumar@myndsol.com"
        'Session("LID") = "25"
        'Session("HEADERSTRIP") = "hfclstrip.jpg"
        'Session("ROLES") = "ClusterManager"


        If Not IsPostBack Then
            BindLeftBar()
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
    Public Sub BindLeftBar()
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
            oda.SelectCommand.CommandText = "select * from mmm_mst_entity with(nolock) where eid=" & Session("EID") & " "
            Dim ds As New DataSet()
            oda.Fill(ds, "Entity")
            hdnRefTime.Value = Convert.ToInt32(ds.Tables("Entity").Rows(0).Item("ReloadSeconds")) * 1000
            'hdnRefTime.Value = 70000
            If Session("userrole").ToString().ToUpper() = "SU" Then
                hdnCluster.Value = "SU"
            ElseIf Session("userrole").ToString().ToUpper() = "CORPORATEUSER" Then
                hdnCluster.Value = "CORPORATEUSER"
            Else
                oda.SelectCommand.CommandText = "select uid,fld1[ClusterID],rolename from mmm_ref_role_user with(nolock) where eid=" & Session("EID") & " and uid= " & Session("uid") & " and rolename='" & Session("userrole") & "'"
                oda.Fill(ds, "Cluster")
                hdnCluster.Value = ds.Tables("Cluster").Rows(0).Item("ClusterID").ToString()
                Session("RID") = ds.Tables("Cluster").Rows(0).Item("ClusterID").ToString()
            End If

            Dim dtt As New DataTable()
            ChkSiteType.Items.Clear()
            oda.SelectCommand.CommandText = "select distinct tid,fld1 from mmm_mst_master with(nolock) where DOCUMENTTYPE='Site Type' and eid=" & Session("EID") & " and fld1<>'' order by fld1"
            oda.Fill(dtt)
            For i As Integer = 0 To dtt.Rows.Count - 1
                If dtt.Rows(i).Item("fld1").ToString.ToUpper = "HUB" Then
                    ChkSiteType.Items.Add(Convert.ToString(dtt.Rows(i).Item("fld1").ToString))
                    ChkSiteType.Items(i).Value = dtt.Rows(i).Item("tid").ToString
                    ChkSiteType.Items(i).Attributes.Add("style", "color:Orange;")
                ElseIf dtt.Rows(i).Item("fld1").ToString.ToUpper = "BSC" Then
                    ChkSiteType.Items.Add(Convert.ToString(dtt.Rows(i).Item("fld1").ToString))
                    ChkSiteType.Items(i).Value = dtt.Rows(i).Item("tid").ToString
                    ChkSiteType.Items(i).Attributes.Add("style", "color:lightblue;")
                ElseIf dtt.Rows(i).Item("fld1").ToString.ToUpper = "STRATEGIC" Then
                    ChkSiteType.Items.Add(Convert.ToString(dtt.Rows(i).Item("fld1").ToString))
                    ChkSiteType.Items(i).Value = dtt.Rows(i).Item("tid").ToString
                    ChkSiteType.Items(i).Attributes.Add("style", "color:DarkBlue;")
                ElseIf dtt.Rows(i).Item("fld1").ToString.ToUpper = "NON STRATEGIC" Then
                    ChkSiteType.Items.Add(Convert.ToString(dtt.Rows(i).Item("fld1").ToString))
                    ChkSiteType.Items(i).Value = dtt.Rows(i).Item("tid").ToString
                    ChkSiteType.Items(i).Attributes.Add("style", "color:Blueviolet;")
                End If
                ChkSiteType.Items(i).Selected = True
                ChkSiteType.Items(i).Attributes.Add("onclick", "ShowHideMap(this)")
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

            oda.SelectCommand.CommandText = "Select Distinct fld15[Role] from mmm_mst_Master with(nolock) where Eid=" & Session("Eid") & " and Documenttype='Manpower Master'"
            Dim dtManPowerType As New DataTable
            oda.Fill(dtManPowerType)
            For i As Integer = 0 To dtManPowerType.Rows.Count - 1
                chkManPower.Items.Add(Convert.ToString(dtManPowerType.Rows(i).Item("Role").ToString))
                chkManPower.Items(i).Value = dtManPowerType.Rows(i).Item("Role").ToString
                chkManPower.Items(i).Attributes.Add("onclick", "ShowHideMap(this)")
                chkManPower.Items(i).Selected = True
            Next
            BindCircle()
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

    Private Sub BindAll()
        Dim id As String = ""
        For i As Integer = 0 To chkvtype.Items.Count - 1

            id = id & chkvtype.Items(i).Value & ","

        Next
        For i As Integer = 0 To chkvtype.Items.Count - 1

            id = id & chkvtype.Items(i).Value & ","

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
            If ChkSiteType.Items.Count > 0 Then
                ChkSiteType.Items(0).Attributes.Add("style", "color:lightblue;")
            End If
            If ChkSiteType.Items.Count > 1 Then
                ChkSiteType.Items(1).Attributes.Add("style", "color:Orange;")
            End If
            If ChkSiteType.Items.Count > 2 Then
                ChkSiteType.Items(2).Attributes.Add("style", "color:Blueviolet;")
            End If
            If ChkSiteType.Items.Count > 3 Then
                ChkSiteType.Items(3).Attributes.Add("style", "color:DarkBlue;")
            End If

            Dim j As Integer = 0
            Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
            Dim con As New SqlConnection(conStr)
            Dim oda As SqlDataAdapter = New SqlDataAdapter("", con)

            Dim dtImieNo As New DataTable
            Dim qry As String = ""
            If HttpContext.Current.Session("USERROLE").ToString.ToUpper = "SU" Or HttpContext.Current.Session("USERROLE").ToString.ToUpper = "CORPORATEUSER" Then
                qry = "select fld12[IMIENO] from mmm_mst_master with(nolock) where documenttype='vehicle' and eid=" & Session("Eid") & " and fld12<>'' and fld12<>'0'"
            Else
                qry = "select fld12[IMIENO] from mmm_mst_master with(nolock) inner join  dbo.split('" & HttpContext.Current.Session("RID") & "', ',') s on s.items in (select items from dbo.split(fld16, ','))  where documenttype='vehicle' and eid=" & Session("Eid") & " and fld12<>'' and fld12<>'0'"
            End If
            oda.SelectCommand.CommandText = qry
            oda.Fill(dtImieNo)
            dtVechical.Clear()

            For i As Integer = 0 To dtImieNo.Rows.Count - 1
                Dim str As String = "select convert(nvarchar,g.tid)[TID],IMIENO,max(m2.fld10)[Site_Name],DMS.IdealTime(g.imieno) IdealTime,convert(nvarchar,Lattitude)[Lat],convert(nvarchar,longitude)[Long],convert(nvarchar,ctime,101)ctime,max(m2.fld14)[Group],0 as Isvehicle, m2.fld1 [VhNo], m2.fld16[Cluster] "
                str = str & " from mmm_mst_gpsdata g with (nolock) join mmm_mst_master m2 with (nolock) on m2.fld12=g.imieno   where m2.documenttype='vehicle' and m2.fld14 in (" & sid & ")  and m2.eid=" & Session("Eid") & " group by g.tid,imieno,ctime,m2.fld1,msensor,lattitude,longitude,speed, m2.fld16 having ctime=(select max(ctime) from mmm_mst_gpsdata with (nolock) where imieno=g.imieno) and Ltrim(IMIENO) <> '' and  IMIENO='" & dtImieNo.Rows(i).Item("IMIENO") & "'"
                oda.SelectCommand.CommandText = str
                oda.SelectCommand.CommandTimeout = 180
                oda.Fill(dtVechical)
            Next


            For i As Integer = 0 To dtVechical.Rows.Count - 1
                If Convert.ToInt32(dtVechical.Rows(i).Item("IdealTime").ToString) > 10 And Convert.ToInt32(dtVechical.Rows(i).Item("IdealTime").ToString) <= 600 And Convert.ToDateTime(dtVechical.Rows(i).Item("ctime")) = DateAndTime.Today() Then
                    LstVehicle.Items.Add(dtVechical.Rows(i).Item("VhNo").ToString())
                    LstVehicle.Items(j).Text = i + 1 & " " & dtVechical.Rows(i).Item("Site_Name").ToString()
                    LstVehicle.Items(j).Value = dtVechical.Rows(i).Item("IMIENO").ToString()
                    LstVehicle.Items(j).Attributes.Add("style", "color:Black;")
                    LstVehicle.Items(j).Attributes.Add("onclick", "FocusMarker(this);")
                    j = j + 1
                ElseIf Convert.ToInt32(dtVechical.Rows(i).Item("IdealTime").ToString) >= 0 And Convert.ToInt32(dtVechical.Rows(i).Item("IdealTime").ToString) <= 10 And Convert.ToDateTime(dtVechical.Rows(i).Item("ctime")) = DateAndTime.Today() Then
                    LstVehicle.Items.Add(dtVechical.Rows(i).Item("VhNo").ToString())
                    LstVehicle.Items(j).Text = i + 1 & " " & dtVechical.Rows(i).Item("Site_Name").ToString()
                    LstVehicle.Items(j).Value = dtVechical.Rows(i).Item("IMIENO").ToString()
                    LstVehicle.Items(j).Attributes.Add("style", "color:Green;")
                    LstVehicle.Items(j).Attributes.Add("onclick", "FocusMarker(this);")
                    j = j + 1
                ElseIf (Convert.ToInt32(dtVechical.Rows(i).Item("IdealTime").ToString) > 600 And Convert.ToInt32(dtVechical.Rows(i).Item("IdealTime").ToString) <= 1440) Or (Convert.ToDateTime(dtVechical.Rows(i).Item("ctime")) = DateAndTime.Today() And Convert.ToInt32(dtVechical.Rows(i).Item("IdealTime").ToString) > 1440) Then
                    LstVehicle.Items.Add(dtVechical.Rows(i).Item("VhNo").ToString())
                    LstVehicle.Items(j).Text = i + 1 & " " & dtVechical.Rows(i).Item("Site_Name").ToString()
                    LstVehicle.Items(j).Value = dtVechical.Rows(i).Item("IMIENO").ToString()
                    LstVehicle.Items(j).Attributes.Add("style", "color:#B86A84;")
                    LstVehicle.Items(j).Attributes.Add("onclick", "FocusMarker(this);")
                    j = j + 1
                Else
                    LstVehicle.Items.Add(dtVechical.Rows(i).Item("VhNo").ToString())
                    LstVehicle.Items(j).Text = i + 1 & " " & dtVechical.Rows(i).Item("Site_Name").ToString()
                    LstVehicle.Items(j).Value = dtVechical.Rows(i).Item("IMIENO").ToString()
                    LstVehicle.Items(j).Attributes.Add("style", "color:Red;")
                    LstVehicle.Items(j).Attributes.Add("onclick", "FocusMarker(this);")
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

    Public Sub BindCircle()
        Try
            Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
            Dim con As New SqlConnection(conStr)
            Dim da As SqlDataAdapter = New SqlDataAdapter("", con)
            Dim qry = "Select Tid[Value], fld1[Text] from MMM_MST_MASTER with(nolock) where Eid=" & Session("Eid") & " and documenttype='Circle' and IsAuth=1 "
            da.SelectCommand.CommandText = qry
            Dim dt As New DataTable()
            da.Fill(dt)
            ddlCircle.DataSource = dt
            ddlCircle.DataTextField = "Text"
            ddlCircle.DataValueField = "Value"
            ddlCircle.DataBind()
            ddlCircle.Items.Insert(0, "Select Circle")
            ddlCircle.Items(0).Value = "0"
        Catch ex As Exception

        End Try
    End Sub
    <WebMethod()> _
<Script.Services.ScriptMethod()> _
    Public Shared Function GetCluster(CircleID As String) As String
        Dim strJson As String = ""
        Try
            Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
            Dim con As New SqlConnection(conStr)
            Dim da As SqlDataAdapter = New SqlDataAdapter("", con)
            Dim qry = "Select Tid[Value], fld1[Text] from MMM_MST_MASTER with(nolock) where Eid=" & HttpContext.Current.Session("Eid") & " and documenttype='Cluster' and IsAuth=1 and fld11='" & CircleID & "' "

            If Not HttpContext.Current.Session("USERROLE").ToString.ToUpper = "SU" And Not HttpContext.Current.Session("USERROLE").ToString.ToUpper = "CORPORATEUSER" Then
                qry &= " and tid IN (ISNull((Select Tid from MMM_MST_MASTER with(nolock) where Eid=54 and documenttype='Cluster' "
                qry &= " and IsAuth=1 and fld11='" & CircleID & "' and Tid in(select fld1 from mmm_ref_role_user "
                qry &= " where eid=" & HttpContext.Current.Session("EID") & " and uid=" & HttpContext.Current.Session("UID") & " and roleName='" & HttpContext.Current.Session("USERROLE") & "')),0))"
            End If

            da.SelectCommand.CommandText = qry
            Dim dt As New DataTable()
            da.Fill(dt)
            strJson = New JavaScriptSerializer().Serialize(From dr As DataRow In dt.Rows Select dt.Columns.Cast(Of DataColumn)().ToDictionary(Function(col) col.ColumnName, Function(col) dr(col)))
        Catch ex As Exception

        End Try
        Return strJson
    End Function

    <WebMethod()> _
  <Script.Services.ScriptMethod()> _
    Public Shared Function CircleCounts(CircleId As Integer) As String
        Dim strJson As String = ""
        Try
            Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
            Dim con As New SqlConnection(conStr)
            Dim da As SqlDataAdapter = New SqlDataAdapter("", con)
            Dim qry = "Declare @Circle int= " & IIf(CircleId = 0, "null", CircleId.ToString)
            qry &= " Select c.Tid, c.fld1[Circle] "
            qry &= " ,(Select Count(*) from MMM_MST_MASTER with(nolock) where Eid=54 and documenttype='Cluster' and IsAuth=1 and convert(varchar,fld11)=convert(varchar,c.Tid))[Clusters] "
            qry &= " ,(Select Count(*) from MMM_MST_MASTER with(nolock) where Eid=54 and documenttype='Manpower Master' and IsAuth=1 and convert(varchar,fld11)=convert(varchar,c.Tid))[Manpowers] "
            qry &= " ,(Select Count(*) from MMM_MST_MASTER with(nolock) where Eid=54 and documenttype='Site' and IsAuth=1 and convert(varchar,fld1)=convert(varchar,c.Tid))[Sites] "
            qry &= " ,(Select Count(*) from MMM_MST_MASTER with(nolock) where Eid=54 and documenttype='Vehicle' and IsAuth=1 and convert(varchar,fld19)=convert(varchar,c.Tid))[Vehicles] "
            qry &= " ,dbo.GetStr(c.fld10,1) Lat, dbo.GetStr(c.fld10,2) [Long] "
            qry &= " from MMM_MST_MASTER c with(nolock) where c.Eid=54 and c.documenttype='Circle' and c.IsAuth=1 "
            qry &= " and c.Tid=Coalesce(@Circle,c.Tid) "
            da.SelectCommand.CommandText = qry
            Dim dt As New DataTable()
            da.Fill(dt)
            strJson = New JavaScriptSerializer().Serialize(From dr As DataRow In dt.Rows Select dt.Columns.Cast(Of DataColumn)().ToDictionary(Function(col) col.ColumnName, Function(col) dr(col)))
        Catch ex As Exception

        End Try
        Return strJson
    End Function

    <WebMethod()> _
    <Script.Services.ScriptMethod()> _
    Public Shared Function GetMarkers(CircleId As Integer, Clusters() As String) As Marker
        Dim objMarker As New Marker()
        Try
            objMarker.Sites = GetSites(CircleId, Clusters)
            objMarker.ManPowers = GetManPower(CircleId, Clusters)
            objMarker.Vehicles = GetVehicles(CircleId, Clusters)
            objMarker.Success = True
        Catch ex As Exception

        Finally

        End Try
        Return objMarker
    End Function

    Public Shared Function GetSites(CircleID As Integer, Clusters() As String) As String
        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim con As New SqlConnection(conStr)
        Dim da As SqlDataAdapter = New SqlDataAdapter("", con)
        Dim strSite As New StringBuilder()
        Try

            Dim str = String.Join("','", Clusters)
            Dim dt As New DataTable()
            Dim qry = "Select Tid,fld12[SiteType],dbo.GetStr(fld21,1)[Lat], dbo.GetStr(fld21,2)[Long] from mmm_mst_Master where fld1 in ('" & CircleID & "') and IsAuth=1 and fld21 Not like '%NA%'"
            qry &= " and fld15 in ('" & str & "') and documenttype='Site'"

            da.SelectCommand.CommandText = qry
            da.Fill(dt)
            strSite.Append(ToCsv(dt, "^", "|"))
        Catch ex As Exception
            da.Dispose()
            con.Dispose()
        Finally

        End Try
        Return strSite.ToString()
    End Function

    Public Shared Function GetManPower(CircleID As Integer, Clusters() As String) As String
        Dim con As New SqlConnection(ConfigurationManager.ConnectionStrings("conStr").ConnectionString)
        Dim da As SqlDataAdapter = New SqlDataAdapter("", con)
        Dim strMan As New StringBuilder()
        Try
            Dim dt As New DataTable()
            'Dim ClustArr = Convert.ToString(HttpContext.Current.Session("RID")).Split(",")
            Dim str = String.Join("','", Clusters)
            Dim qry = "Select Distinct m.fld1[userID] ,m.fld10[User], m.fld15[Role] from mmm_mst_Master m with(nolock) "
            'If Not HttpContext.Current.Session("USERROLE").ToString.ToUpper = "SU" And Not HttpContext.Current.Session("USERROLE").ToString.ToUpper = "CORPORATEUSER" Then
            qry &= " Join (Select Tid, fld15[Cluster], fld10[SiteID] from mmm_mst_Master with(nolock) where Eid=" & HttpContext.Current.Session("Eid") & " and Documenttype='Site' and IsAuth=1 "
            qry &= " and fld15 IN('" & str & "') ) s on s.Tid=m.fld12 "
            'End If
            qry &= " where Eid=" & HttpContext.Current.Session("Eid") & " and Documenttype='Manpower Master' and m.fld1<>'0' and fld11='" & CircleID & "'"
            da.SelectCommand.CommandText = qry
            da.Fill(dt)

            For i As Integer = 0 To dt.Rows.Count - 1
                da.SelectCommand.CommandText = "Select top 1 Lattitude, Longitude from mmm_mst_gpsdata with(nolock) where IMIENO='" & dt.Rows(i).Item("UserID") & "' order by Ctime desc"
                Dim dtMan As New DataTable()
                da.Fill(dtMan)
                If dtMan.Rows.Count > 0 Then
                    strMan.Append(dt.Rows(i).Item("UserID") & "^" & dt.Rows(i).Item("Role") & "^" & dtMan.Rows(0).Item("Lattitude") & "^" & dtMan.Rows(0).Item("Longitude") & "|")
                End If
            Next
        Catch ex As Exception

        End Try
        Return strMan.ToString()
    End Function

    Public Shared Function GetVehicles(CircleID As Integer, Clusters() As String) As String
        Dim CsvStr As New StringBuilder()
        Try
            Dim colSize As Integer = dtVechical.Columns.Count - 1
            Dim str = String.Join(",", Clusters)


            For Each Row As DataRow In dtVechical.Rows

                Dim str2 = Row.Item("Cluster").ToString.Split(",")
                Dim c As IEnumerable(Of String) = Clusters.Intersect(str2)

                If c.Count = 0 Then
                    Continue For
                End If

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
        Catch ex As Exception

        End Try
        Return CsvStr.ToString()
    End Function

    Public Shared Function ToCsv(dt As DataTable, ColSeperator As String, RowSeperator As String) As String
        Try
            'var arr = dt.AsEnumerable().Select(row => string.Join(ColSeperator, row.ItemArray)).ToArray();
            Dim arr = dt.AsEnumerable().Select(Function(row) String.Join(ColSeperator, row.ItemArray)).ToArray()
            Dim s = String.Join(RowSeperator, arr)
            Return s
        Catch ex As Exception
        End Try
        Return ""
    End Function

    <WebMethod()> _
<Script.Services.ScriptMethod()> _
    Public Shared Function GetInfo(ID As String, Flag As Integer, Ids As String) As String
        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim con As New SqlConnection(conStr)
        Dim da As SqlDataAdapter = New SqlDataAdapter("", con)
        Try
            Dim Info = ""
            If Flag = 0 Then
                Dim query As String = "select m.tid[TID],m.fld10[SiteID],m.fld11[SiteName],"
                query &= "m.fld13[Address],m1.fld1[Site],dbo.GetStr(m.fld21,1)[Lat] ,dbo.GetStr(m.fld21,2)[Long], m.fld12[Group],m.fld11[Site Name],u.UserName[OandM Head],u1.UserName[Maintenance Head], u2.UserName[Opex Manager],u3.UserName[Security Manager],u4.UserName[Zonal Head],u5.UserName[Cluster Manager],m2.fld1[Supervisor], m3.fld1[Technician], m.fld2[No of OPCOs],m.fld19[Anchor OPCO],m.fld15[Cluster] from  mmm_mst_master m with (nolock) inner join mmm_mst_user u with (nolock) on convert(nvarchar,u.uid)=m.fld23 join mmm_mst_user u1 with (nolock) on convert(nvarchar,u1.uid)=m.fld24 join mmm_mst_user u2 with (nolock) on convert(nvarchar,u2.uid)=m.fld25  join mmm_mst_user u3 with (nolock) on convert(nvarchar,u3.uid)=m.fld26  join mmm_mst_user u4 with (nolock) on convert(nvarchar,u4.uid)=m.fld27 join mmm_mst_user u5 with (nolock) on convert(nvarchar,u5.uid)=m.fld28 join mmm_mst_master m2 with (nolock) on convert(nvarchar,m2.tid)=m.fld29 join mmm_mst_master m3 with (nolock) on convert(nvarchar,m3.tid)=m.fld3 join mmm_mst_master m1 with (nolock) on convert(nvarchar,m1.tid)=m.fld12  where m.documenttype='Site' and m1.documenttype='Site Type' and m1.eid=" & HttpContext.Current.Session("Eid") & " and m.eid=" & HttpContext.Current.Session("Eid") & " and  u.eid=" & HttpContext.Current.Session("Eid") & " and "
                query &= " u1.eid=" & HttpContext.Current.Session("Eid") & " and u2.eid=" & HttpContext.Current.Session("Eid") & " and m2.eid=" & HttpContext.Current.Session("Eid") & " and m3.eid=" & HttpContext.Current.Session("Eid") & " and m.Tid=" & ID
                Dim oda As SqlDataAdapter = New SqlDataAdapter(query, con)
                Dim dt As New DataTable
                oda.Fill(dt)
                Info = "<span style='font-weight:bold;'> SiteID : " + dt.Rows(0).Item("SiteID") + "</span><br>Site : " + dt.Rows(0).Item("Site") + "<br>Site Name : " + dt.Rows(0).Item("SiteName") + "<br>OandM Head: " + dt.Rows(0).Item("OandM Head") + " <br> Maintenance Head : " + dt.Rows(0).Item("Maintenance Head") + " <br>Opex Manager : " + dt.Rows(0).Item("Opex Manager") + "<br>Security Manager : " + dt.Rows(0).Item("Security Manager") + "<br>Zonal Head : " + dt.Rows(0).Item("Zonal Head") + "<br>Cluster Manager :" + dt.Rows(0).Item("Cluster Manager") + "<br>Supervisor : " + dt.Rows(0).Item("Supervisor") + "<br>Technician : " + dt.Rows(0).Item("Technician") + "<br>No of OPCOs : " + dt.Rows(0).Item("No of OPCOs") + " <br>Anchor OPCO : " + dt.Rows(0).Item("Anchor OPCO") + ""
                Return Info
            ElseIf Flag = 1 Then
                Dim Query = "Select top 1 m.fld10[User Name], m.fld15[Role],dms.udf_split('MASTER-Circle-fld1', m.fld11) [Circle],dms.udf_split('MASTER-Site-fld10', m.fld12) [Site ID],m.fld13[Primary No],m.fld14[Data Number] "
                Query &= " ,(Select top 1 Lattitude from mmm_mst_Gpsdata with(nolock) where IMIENO=m.fld1)Lattitude,(Select top 1 Longitude from mmm_mst_Gpsdata with(nolock) where IMIENO=m.fld1)Longitude,(Select top 1 convert(varchar(30), ctime) from mmm_mst_Gpsdata with(nolock) where IMIENO=m.fld1)[Last record time] "
                Query &= "  from mmm_mst_Master m with(nolock) where Eid=" & HttpContext.Current.Session("Eid") & " and Documenttype='Manpower Master' and fld1='" & ID & "' "
                Dim oda As SqlDataAdapter = New SqlDataAdapter(Query, con)
                Dim dtInfo As New DataTable
                oda.Fill(dtInfo)
                Dim InfoStr As New StringBuilder()
                If dtInfo.Rows.Count > 0 Then
                    For i As Integer = 0 To dtInfo.Columns.Count - 1
                        InfoStr.Append("<b>" & dtInfo.Columns(i).ColumnName & "</b> : " & dtInfo.Rows(0).Item(dtInfo.Columns(i).ColumnName) & "<br>")
                    Next
                End If
                Return InfoStr.ToString()
            ElseIf Flag = 2 Then
                Dim query As String = "select convert(nvarchar,g.tid)[TID],max(m2.fld10)[Site_Name],DMS.IdealTime(g.imieno) IdealTime,convert(nvarchar,Lattitude)[Lat],convert(nvarchar,longitude)[Long],convert(nvarchar,ctime,101)ctime, m2.fld1[vehicleNo], g.imieno, g.Speed"
                query &= " from mmm_mst_gpsdata g with (nolock) join mmm_mst_master m2 with (nolock) on m2.fld12=g.imieno "
                query &= "where m2.documenttype='vehicle' and m2.fld14 in (" & Ids.TrimEnd(CChar(",")) & ") and m2.eid=" & HttpContext.Current.Session("Eid") & "  and m2.fld12='" & ID & "' "
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

        Catch ex As Exception

        End Try
        Return ""
    End Function

    <WebMethod()> _
<Script.Services.ScriptMethod()> _
    Public Shared Function SearchSites(SearchText As String, CircleId As Integer, Clusters() As String, Flag As Integer) As String
        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim con As New SqlConnection(conStr)
        Dim da As SqlDataAdapter = New SqlDataAdapter("", con)
        Try
            Dim strcluster As String = String.Join("','", Clusters)
            Dim qry = "Select Tid[Value], fld11[Text] from mmm_mst_Master with(nolock) where Eid=" & HttpContext.Current.Session("EID") & " and DocumentType='Site' and fld1='" & CircleId & "' and fld15 IN('" & strcluster & "') "
            If Flag = 0 Then
                qry &= " and fld11 like '%" & SearchText & "%'"
            Else
                qry &= " and fld10 like '%" & SearchText & "%'"
            End If
            da.SelectCommand.CommandText = qry
            Dim dt As New DataTable()
            da.Fill(dt)
            Dim strJson = New JavaScriptSerializer().Serialize(From dr As DataRow In dt.Rows Select dt.Columns.Cast(Of DataColumn)().ToDictionary(Function(col) col.ColumnName, Function(col) dr(col)))
            Return strJson
        Catch ex As Exception

        End Try
        Return ""
    End Function

    <WebMethod()> _
<Script.Services.ScriptMethod()> _
    Public Shared Function SearchManPower(SearchText As String, CircleId As Integer, Clusters() As String) As String
        Dim con As New SqlConnection(ConfigurationManager.ConnectionStrings("conStr").ConnectionString)
        Dim da As SqlDataAdapter = New SqlDataAdapter("", con)
        Dim strMan As New StringBuilder()
        Try
            Dim dt As New DataTable()
            Dim str = String.Join("','", Clusters)
            Dim qry = "Select Distinct m.fld1[userID] ,m.fld10[User] from mmm_mst_Master m with(nolock) "
            If Not HttpContext.Current.Session("USERROLE").ToString.ToUpper = "SU" And Not HttpContext.Current.Session("USERROLE").ToString.ToUpper = "CORPORATEUSER" Then
                qry &= " Join (Select Tid, fld15[Cluster], fld10[SiteID] from mmm_mst_Master with(nolock) where Eid=" & HttpContext.Current.Session("Eid") & " and Documenttype='Site' and IsAuth=1 "
                qry &= " and fld15 IN('" & str & "') ) s on s.Tid=m.fld12 "
            End If
            qry &= " where Eid=" & HttpContext.Current.Session("Eid") & " and Documenttype='Manpower Master' and m.fld1<>'0' and fld10 like'%" & SearchText & "%'"
            da.SelectCommand.CommandText = qry
            da.Fill(dt)

            For i As Integer = 0 To dt.Rows.Count - 1
                da.SelectCommand.CommandText = "Select top 1 Lattitude, Longitude from mmm_mst_gpsdata with(nolock) where IMIENO='" & dt.Rows(i).Item("UserID") & "' order by Ctime desc"
                Dim dtMan As New DataTable()
                da.Fill(dtMan)
                If dtMan.Rows.Count > 0 Then
                    strMan.Append(dt.Rows(i).Item("UserID") & "^" & dt.Rows(i).Item("User") & "|")
                End If
            Next
        Catch ex As Exception

        End Try
        Return strMan.ToString()
    End Function

    <WebMethod()> _
<Script.Services.ScriptMethod()> _
    Public Shared Function RefreshVehicle(Ids As String, CircleId As String, Clusters() As String) As String
        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim con As New SqlConnection(conStr)
        Dim oda As SqlDataAdapter = New SqlDataAdapter("", con)
        Dim CsvStr As New StringBuilder()
        Try
            Ids = Ids.Substring(0, Ids.Length - 1)
            Dim arr = Ids.Split(",")
            Dim sid As String = String.Join("','", arr)

            Dim dtImieNo As New DataTable
            Dim qry As String = ""
            If HttpContext.Current.Session("USERROLE").ToString.ToUpper = "SU" Or HttpContext.Current.Session("USERROLE").ToString.ToUpper = "CORPORATEUSER" Then
                qry = "select fld12[IMIENO] from mmm_mst_master with(nolock) where documenttype='vehicle' and eid=" & HttpContext.Current.Session("Eid") & " and fld12<>'' and fld12<>'0'"
            Else
                qry = "select fld12[IMIENO] from mmm_mst_master with(nolock) inner join  dbo.split('" & HttpContext.Current.Session("RID") & "', ',') s on s.items in (select items from dbo.split(fld16, ','))  where documenttype='vehicle' and eid=" & HttpContext.Current.Session("Eid") & " and fld12<>'' and fld12<>'0'"
            End If
            oda.SelectCommand.CommandText = qry
            oda.Fill(dtImieNo)
            dtVechical.Clear()

            For i As Integer = 0 To dtImieNo.Rows.Count - 1
                Dim str As String = "select convert(nvarchar,g.tid)[TID],IMIENO,max(m2.fld10)[Site_Name],DMS.IdealTime(g.imieno) IdealTime,convert(nvarchar,Lattitude)[Lat],convert(nvarchar,longitude)[Long],convert(nvarchar,ctime,101)ctime,max(m2.fld14)[Group],0 as Isvehicle, m2.fld1 [VhNo], m2.fld16[Cluster] "
                str = str & " from mmm_mst_gpsdata g with (nolock) join mmm_mst_master m2 with (nolock) on m2.fld12=g.imieno   where m2.documenttype='vehicle' and m2.fld14 in ('" & sid & "')  and m2.eid=" & HttpContext.Current.Session("Eid") & " group by g.tid,imieno,ctime,m2.fld1,msensor,lattitude,longitude,speed, m2.fld16 having ctime=(select max(ctime) from mmm_mst_gpsdata with (nolock) where imieno=g.imieno) and Ltrim(IMIENO) <> '' and  IMIENO='" & dtImieNo.Rows(i).Item("IMIENO") & "'"
                oda.SelectCommand.CommandText = str
                oda.SelectCommand.CommandTimeout = 180
                oda.Fill(dtVechical)
            Next
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
        Catch ex As Exception

        End Try
        Return CsvStr.ToString()
    End Function

    <WebMethod()> _
<Script.Services.ScriptMethod()> _
    Public Shared Function RefreshManpower(CircleID As Integer, Clusters() As String) As String
        Return GetManPower(CircleID, Clusters)
    End Function
End Class

Public Class Marker
    Public Property Sites As Object
    Public Property Vehicles As Object
    Public Property ManPowers As Object
    Public Property Success As Boolean
    Public Property Message As String
End Class
