Imports System.Data
Imports System.Data.SqlClient
Imports System.Web.UI.Adapters.ControlAdapter
Imports iTextSharp.text
Imports System.Drawing
Imports System.IO
Imports System.Globalization
Imports System.Web.Services
Imports Newtonsoft.Json.Converters
Imports Newtonsoft.Json
Imports System.Web.Script.Serialization
Partial Class LastLocation

    Inherits System.Web.UI.Page
    Shared flag1 As Integer

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        flag1 = 1
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
                oda.SelectCommand.CommandText = "select distinct  tid,fld1 from mmm_mst_master where DOCUMENTTYPE='State' and eid=" & Session("EID") & " order by fld1"
                oda.Fill(dtt)
                For i As Integer = 0 To dtt.Rows.Count - 1
                    Circle.Items.Add(Convert.ToString(dtt.Rows(i).Item("fld1").ToString))
                    Circle.Items(i).Value = dtt.Rows(i).Item("tid").ToString
                Next
                City.Items.Clear()
                oda.SelectCommand.CommandText = "select distinct tid, fld1 from mmm_mst_master where DOCUMENTTYPE='City' and eid=" & Session("EID") & " order by fld1 "
                Dim circledata As New DataTable()
                oda.Fill(circledata)
                For i As Integer = 0 To circledata.Rows.Count - 1
                    City.Items.Add(Convert.ToString(circledata.Rows(i).Item("fld1")))
                    City.Items(i).Value = circledata.Rows(i).Item("tid").ToString
                Next

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
                'oda.SelectCommand.CommandText = "select imieno,username,VehicleNo from ( select  distinct m." & vemei & "[IMIENO], (select username from mmm_mst_user where eid=" & Session("EID") & " and uid=d." & Ufld & ")[UserName],m." & Vfld & "[VehicleNo] from mmm_mst_Doc d inner join mmm_mst_master m on m.tid=d." & UVfld & " inner join mmm_mst_gpsdata g on g.imieno=m." & vemei & " where d.eid=" & Session("EID") & " and  d.documenttype='" & Udtype & "'  and imieno is not null) as table1 order by username  "
                ds.Clear()
                oda.SelectCommand.CommandTimeout = 3000
                oda.Fill(ds, "vemei")
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
    Protected Sub btnshow_Click(sender As Object, e As ImageClickEventArgs)
        Show()
        UpdatePanel1.Update()
    End Sub

    <WebMethod()> _
    Public Shared Function ShowDynamic1(cit As String, cir As String, Imieno As String, EID As String, uRole As String, uid As String) As Grid
        Dim Addressdt As New DataTable()
        Dim GeoPointdt As New DataTable()
        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim con As SqlConnection = New SqlConnection(conStr)
        Dim oda As SqlDataAdapter = New SqlDataAdapter("", con)
        Dim dt As New DataTable
        Dim objgrid = New Grid()
        Dim Columns As New List(Of GridCol)
        Dim Query As String = " "
        Try
            If EID = "32" Then

                If cit = "" And cir = "" Then
                    Query = "select distinct (select fld1 from mmm_mst_master with(nolock) where convert(nvarchar,tid)=m4.fld10)[State],(select fld1 from mmm_mst_master with(nolock) where convert(nvarchar,tid)=u.fld14)[City], m1.fld2[Device IMEI No.],m1.fld10[Vehicle No],u.UserName[User Name],lattitude,longitude,cTime[Last Movement Date & Time]  frOm mmm_mst_master m1 with(nolock)   inner join MMM_MST_DOC d with(nolock) on d.fld24=m1.tid   inner join MMM_MST_USER u with(nolock) on u.uid=d.fld11 inner join   mmm_mst_master m4 with(nolock) on m4.fld10=u.fld13    and curstatus in ('Allotted','surrender','archive') inner join mmm_mst_gpsdata g with(nolock) on g.IMIENO= m1.fld2  WHERE m1.DOCUMENTTYPE='Vehicle' and d.documenttype='vrf fixed_pool'  and m4.documenttype='City' and m1.fld2 <>'' and g.tid in ( select max(tid) from MMM_MST_GPSDATA with(nolock) where imieno in (" & Imieno.TrimEnd(",") & ") group by imieno) "
                ElseIf cit <> "" And cir <> "" Then
                    Query = "select distinct (select fld1 from mmm_mst_master with(nolock) where convert(nvarchar,tid)=m4.fld10)[State],(select fld1 from mmm_mst_master with(nolock) where convert(nvarchar,tid)=u.fld14)[City], m1.fld2[Device IMEI No.],m1.fld10[Vehicle No],u.UserName[User Name],lattitude,longitude,cTime[Last Movement Date & Time]  frOm mmm_mst_master m1 with(nolock)   inner join MMM_MST_DOC d with(nolock) on d.fld24=m1.tid   inner join MMM_MST_USER u with(nolock) on u.uid=d.fld11 inner join   mmm_mst_master m4 with(nolock) on m4.fld10=u.fld13    and curstatus in ('Allotted','surrender','archive') inner join mmm_mst_gpsdata g with(nolock) on g.IMIENO= m1.fld2  WHERE m1.DOCUMENTTYPE='Vehicle' and d.documenttype='vrf fixed_pool'  and m4.documenttype='City' and m1.fld2 <>'' and g.tid in ( select max(tid) from MMM_MST_GPSDATA with(nolock) where imieno in (" & Imieno.TrimEnd(",") & ") group by imieno ) and m4.tid in (" & cit.ToString & ")  and m4.fld10 in (" & cir.ToString & ") "
                ElseIf cit <> "" Then
                    Query = "select distinct (select fld1 from mmm_mst_master with(nolock) where convert(nvarchar,tid)=m4.fld10)[State],(select fld1 from mmm_mst_master with(nolock) where convert(nvarchar,tid)=u.fld14)[City], m1.fld2[Device IMEI No.],m1.fld10[Vehicle No],u.UserName[User Name],lattitude,longitude,cTime[Last Movement Date & Time]  frOm mmm_mst_master m1 with(nolock)  inner join MMM_MST_DOC d with(nolock) on d.fld24=m1.tid   inner join MMM_MST_USER u with(nolock) on u.uid=d.fld11 inner join   mmm_mst_master m4 with(nolock) on m4.fld10=u.fld13    and curstatus in ('Allotted','surrender','archive') inner join mmm_mst_gpsdata g with(nolock) on g.IMIENO= m1.fld2  WHERE m1.DOCUMENTTYPE='Vehicle' and d.documenttype='vrf fixed_pool'  and m4.documenttype='City' and m1.fld2 <>'' and g.tid in ( select max(tid) from MMM_MST_GPSDATA with(nolock) where imieno in (" & Imieno.TrimEnd(",") & ") group by imieno ) and m4.tid in (" & cit.ToString & ") "
                End If

            Else

                Query = GetQueryForMileageDaily(EID, Imieno.TrimEnd(","), uRole, uid)

            End If
            Dim lo As New GPSClass()

            oda.SelectCommand.CommandText = Query
            oda.SelectCommand.CommandTimeout = 180
            oda.Fill(dt)
            GeoPointdt = dt.Copy()
            dt.Columns.Add("Current Location Address", Type.GetType("System.String"))
            For i As Integer = 0 To dt.Rows.Count - 1
                dt.Rows(i).Item("Current Location Address") = lo.Location(dt.Rows(i).Item("lattitude"), dt.Rows(i).Item("longitude"))
            Next
            dt.Columns.Remove("lattitude")
            dt.Columns.Remove("longitude")
            Addressdt = dt.Copy()
            HttpContext.Current.Session("table") = Addressdt
            Dim ds1 As New DataSet
            oda.Fill(ds1, "gpsdata")
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
        Static flag As Integer = 0
        Static TempDT As New DataTable()
        If (flag = 0 And flag1 = 1) Then
            If GeoPointdt.Columns.Contains("Address") Then
            Else
                GeoPointdt.Columns.Add("Address", GetType(String))
            End If
        End If
        If (flag = 1 And flag1 = 1) Then
            If GeoPointdt.Columns.Contains("Address") Then
            Else
                GeoPointdt.Columns.Add("Address", GetType(String))
            End If
        End If
        For index As Integer = 0 To Addressdt.Rows.Count - 1
            GeoPointdt.Rows(index)("Address") = Addressdt.Rows(index)("Current Location Address").ToString()
        Next
        Dim rows As New List(Of Dictionary(Of String, Object))()
        Dim row As Dictionary(Of String, Object)
        For Each dr As DataRow In GeoPointdt.Rows
            row = New Dictionary(Of String, Object)()
            For Each col As DataColumn In GeoPointdt.Columns
                If col.ColumnName = "Last Movement Date & Time" Then
                    'row.Add(col.ColumnName, Format(dr(col), "General Date"))
                    row.Add(col.ColumnName, Convert.ToDateTime(dr(col)).ToString("dd-MM-yyyy HH:mm tt"))
                Else
                    row.Add(col.ColumnName, dr(col))
                End If
            Next
            rows.Add(row)
        Next
        flag = 1
        Try
            For j As Integer = 0 To Addressdt.Columns.Count - 1
                Dim objgrodcol As New GridCol()
                Dim colnm = Addressdt.Columns(j).ColumnName.Replace(" ", "_").Replace(".", "").ToString().Replace("&", "and").ToString()
                objgrodcol.field = colnm
                objgrodcol.title = Addressdt.Columns(j).ColumnName
                Columns.Add(objgrodcol)
                Addressdt.Columns(j).ColumnName = colnm
            Next

            Dim serializerSettings As New JsonSerializerSettings()
            Dim json_serializer As New JavaScriptSerializer()
            serializerSettings.Converters.Add(New DataTableConverter())
            Dim jsonData As String = JsonConvert.SerializeObject(Addressdt, Newtonsoft.Json.Formatting.None, serializerSettings)
            objgrid.GridData = jsonData
            objgrid.Columns = Columns
            objgrid.IsDetailsOn = True
            objgrid.rows = rows
        Catch ex As Exception

        End Try

        Return objgrid

    End Function


    Shared Function GetQueryForMileageDaily(Eid As String, IMEINos As String, uRole As String, uid As String) As String

        Dim query As String = " Select * "

        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim con As SqlConnection = New SqlConnection(conStr)
        Dim dt As New DataTable()
        Dim oda As SqlDataAdapter = New SqlDataAdapter("select * from mmm_mst_gpsreportsettings where Reporttype ='VehiclePosition' and eid =" + Eid + " order by SequenceNo", con)
        oda.Fill(dt)
        Dim dtGrid = dt.Select("SettingType='Grid'", " SequenceNo ASC")

        If (dtGrid.Length > 0) Then
            query = " Select "
            For i As Integer = 0 To dtGrid.Length - 1
                query &= dtGrid(i).Item("DocField").ToString().Split(":")(0) + "[" + dtGrid(i).Item("DispName").ToString() + "],"
            Next

        End If

        query = query.Remove(query.LastIndexOf(","))

        Dim dtVehicle = dt.Select("SettingType='Vehicle'")
        Dim dtDoc = dt.Select("SettingType='DocumentType'")
        Dim dtpermission = dt.Select("SettingType='VehiclePermission'")
        If (dtVehicle.Length > 0) Then
            If (dtDoc.Length > 0) Then
                If (uRole <> "SU" And dtpermission.Length > 0) Then

                    query &= "   frOm mmm_mst_master Vehicle with(nolock)   inner join MMM_MST_DOC doc with(nolock) on doc." & dtDoc(0).Item("DocField") & "=Vehicle.tid   " &
     "  and doc.curstatus in ('Allotted','surrender','archive') inner join mmm_mst_gpsdata g with(nolock) on g.IMIENO= Vehicle." & dtVehicle(0).Item("DocField") &
     "inner join  dbo.split((select (select DocMapping from mmm_mst_forms where eid =" & Eid & " and isRoleDef=1 and Formname ='" & dtpermission(0).Item("VType") & "') from mmm_ref_role_user where uid =" & uid &
     " and rolename=" & uRole & "), ',') s on s.items in (select items from dbo.split(Vehicle." & dtpermission(0).Item("DocField") & ", ',')) " &
       " WHERE Vehicle.DOCUMENTTYPE='" & dtVehicle(0).Item("VType") & "' and doc.documenttype='" & dtDoc(0).Item("VType") & "' " &
" and Vehicle." & dtVehicle(0).Item("DocField") & " <>'' and g.tid in ( select max(tid) from MMM_MST_GPSDATA with(nolock) where imieno in (" & IMEINos & ") group by imieno) "
                Else
                    query &= "   frOm mmm_mst_master Vehicle with(nolock)   inner join MMM_MST_DOC doc with(nolock) on doc." & dtDoc(0).Item("DocField") & "=Vehicle.tid   " &
     "  and doc.curstatus in ('Allotted','surrender','archive') inner join mmm_mst_gpsdata g with(nolock) on g.IMIENO= Vehicle." & dtVehicle(0).Item("DocField") &
       " WHERE Vehicle.DOCUMENTTYPE='" & dtVehicle(0).Item("VType") & "' and doc.documenttype='" & dtDoc(0).Item("VType") & "' " &
" and Vehicle." & dtVehicle(0).Item("DocField") & " <>'' and g.tid in ( select max(tid) from MMM_MST_GPSDATA with(nolock) where imieno in (" & IMEINos & ") group by imieno) "
                End If

            Else
                If (dtpermission.Length > 0 And uRole <> "SU") Then
                    query &= "   frOm mmm_mst_master Vehicle with(nolock) inner join mmm_mst_gpsdata g with(nolock) on g.IMIENO= Vehicle." & dtVehicle(0).Item("DocField") &
    "inner join  dbo.split((select (select DocMapping from mmm_mst_forms where eid =" & Eid & " and isRoleDef=1 and Formname ='" & dtpermission(0).Item("VType") & "') from mmm_ref_role_user where uid =" & uid & " and rolename=" & uRole &
    "), ',') s on s.items in (select items from dbo.split(Vehicle." & dtpermission(0).Item("DocField") & ", ',')) " &
      " WHERE Vehicle.DOCUMENTTYPE='" & dtVehicle(0).Item("VType") & "'  " &
" and Vehicle." & dtVehicle(0).Item("DocField") & " <>'' and g.tid in ( select max(tid) from MMM_MST_GPSDATA with(nolock) where imieno in (" & IMEINos & ") group by imieno) "
                Else
                    query &= "   frOm mmm_mst_master Vehicle with(nolock)      " &
    "   inner join mmm_mst_gpsdata g with(nolock) on g.IMIENO= Vehicle." & dtVehicle(0).Item("DocField") &
    "inner join  dbo.split((select (select DocMapping from mmm_mst_forms where eid =" & Eid & " and isRoleDef=1 and Formname ='" & dtpermission(0).Item("VType") &
    "') from mmm_ref_role_user where uid =" & uid & " and rolename=" & uRole & "), ',') s on s.items in (select items from dbo.split(Vehicle." & dtpermission(0).Item("DocField") & ", ',')) " &
      " WHERE Vehicle.DOCUMENTTYPE='" & dtVehicle(0).Item("VType") & "'" &
" and Vehicle." & dtVehicle(0).Item("DocField") & " <>'' and g.tid in ( select max(tid) from MMM_MST_GPSDATA with(nolock) where imieno in (" & IMEINos & ") group by imieno) "
                End If
                

            End If

            Else : query &= "   frOm mmm_mst_master Vehicle with(nolock)   inner join MMM_MST_DOC doc with(nolock) on d.fld24=m1.tid   " &
 "  and curstatus in ('Allotted','surrender','archive') inner join mmm_mst_gpsdata g with(nolock) on g.IMIENO= m1.fld2  " &
   " WHERE m1.DOCUMENTTYPE='Vehicle' and d.documenttype='vrf fixed_pool' " &
" and m1.fld2 <>'' and g.tid in ( select max(tid) from MMM_MST_GPSDATA with(nolock) where imieno in (" & IMEINos & ") group by imieno) "

            End If



        Return query

    End Function

    Protected Sub Show()

        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim con As SqlConnection = New SqlConnection(conStr)
        Dim oda As SqlDataAdapter = New SqlDataAdapter("", con)
        Dim dt As New DataTable
        Dim Udtype As String
        Dim Ufld As String
        Dim UVfld As String
        Dim Vdtype As String
        Dim Vfld As String
        Dim vemei As String
        Dim status As String = ""
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
                status = ds.Tables("data").Rows(0).Item("curstatus").ToString
            End If
            Dim crsts() As String = status.Split(",")
            Dim sts As String = ""
            For i As Integer = 0 To crsts.Length - 1
                sts = sts & "'" & crsts(i) & "',"
            Next
            sts = Left(sts, sts.Length - 1)
            Dim imeinoallow As Integer = 0
            Dim Imieno As String = ""
            Dim cit As String = ""
            Dim cir As String = ""
            For i As Integer = 0 To UsrVeh.Items.Count - 1
                If UsrVeh.Items(i).Selected = True Then
                    Imieno = Imieno & "'" & UsrVeh.Items(i).Value & "',"
                    imeinoallow = imeinoallow + 1
                End If
            Next
            For i As Integer = 0 To City.Items.Count - 1
                If City.Items(i).Selected = True Then
                    cit = cit & City.Items(i).Value & ","
                End If
            Next
            For i As Integer = 0 To Circle.Items.Count - 1
                If Circle.Items(i).Selected = True Then
                    cir = cir & Circle.Items(i).Value & ","
                End If
            Next

            If Imieno.ToString = "" Then
                lblmsg.Text = "Please select any User vehicle."
                Exit Sub
            Else
                Imieno = Left(Imieno, Imieno.Length - 1)
            End If

            If cit.ToString <> "" Then
                cit = Left(cit, cit.Length - 1)
            End If
            If cir.ToString <> "" Then
                cir = Left(cir, cir.Length - 1)
            End If

            Dim dtt As New DataTable()
            Dim lo As New GPSClass()
            '  oda.SelectCommand.CommandText = "select  IMIENO,lattitude,longitude,cTime from mmm_MST_gpsdata where tid in (select max(tid) from MMM_MST_GPSDATA where imieno in (" & Imieno.ToString & ") group by IMIENO) "
            If cit = "" And cir = "" Then
                oda.SelectCommand.CommandText = "select distinct (select fld1 from mmm_mst_master with(nolock) where convert(nvarchar,tid)=m4.fld10)[State],(select fld1 from mmm_mst_master with(nolock) where convert(nvarchar,tid)=u.fld14)[City], m1.fld2[Device IMEI No.],m1.fld10[Vehicle No],u.UserName[User Name],lattitude,longitude,cTime[Last Movement Date & Time]  frOm mmm_mst_master m1 with(nolock)   inner join MMM_MST_DOC d with(nolock) on d.fld24=m1.tid   inner join MMM_MST_USER u with(nolock) on u.uid=d.fld11 inner join   mmm_mst_master m4 with(nolock) on m4.fld10=u.fld13    and curstatus in ('Allotted','surrender','archive') inner join mmm_mst_gpsdata g with(nolock) on g.IMIENO= m1.fld2  WHERE m1.DOCUMENTTYPE='Vehicle' and d.documenttype='vrf fixed_pool'  and m4.documenttype='City' and m1.fld2 <>'' and g.tid in ( select max(tid) from MMM_MST_GPSDATA with(nolock) where imieno in (" & Imieno & ") group by imieno) "
            ElseIf cit <> "" And cir <> "" Then
                oda.SelectCommand.CommandText = "select distinct (select fld1 from mmm_mst_master with(nolock) where convert(nvarchar,tid)=m4.fld10)[State],(select fld1 from mmm_mst_master with(nolock) where convert(nvarchar,tid)=u.fld14)[City], m1.fld2[Device IMEI No.],m1.fld10[Vehicle No],u.UserName[User Name],lattitude,longitude,cTime[Last Movement Date & Time]  frOm mmm_mst_master m1 with(nolock)   inner join MMM_MST_DOC d with(nolock) on d.fld24=m1.tid   inner join MMM_MST_USER u with(nolock) on u.uid=d.fld11 inner join   mmm_mst_master m4 with(nolock) on m4.fld10=u.fld13    and curstatus in ('Allotted','surrender','archive') inner join mmm_mst_gpsdata g with(nolock) on g.IMIENO= m1.fld2  WHERE m1.DOCUMENTTYPE='Vehicle' and d.documenttype='vrf fixed_pool'  and m4.documenttype='City' and m1.fld2 <>'' and g.tid in ( select max(tid) from MMM_MST_GPSDATA with(nolock) where imieno in (" & Imieno & ") group by imieno ) and m4.tid in (" & cit.ToString & ")  and m4.fld10 in (" & cir.ToString & ") "
            ElseIf cit <> "" Then
                oda.SelectCommand.CommandText = "select distinct (select fld1 from mmm_mst_master with(nolock) where convert(nvarchar,tid)=m4.fld10)[State],(select fld1 from mmm_mst_master with(nolock) where convert(nvarchar,tid)=u.fld14)[City], m1.fld2[Device IMEI No.],m1.fld10[Vehicle No],u.UserName[User Name],lattitude,longitude,cTime[Last Movement Date & Time]  frOm mmm_mst_master m1 with(nolock)  inner join MMM_MST_DOC d with(nolock) on d.fld24=m1.tid   inner join MMM_MST_USER u with(nolock) on u.uid=d.fld11 inner join   mmm_mst_master m4 with(nolock) on m4.fld10=u.fld13    and curstatus in ('Allotted','surrender','archive') inner join mmm_mst_gpsdata g with(nolock) on g.IMIENO= m1.fld2  WHERE m1.DOCUMENTTYPE='Vehicle' and d.documenttype='vrf fixed_pool'  and m4.documenttype='City' and m1.fld2 <>'' and g.tid in ( select max(tid) from MMM_MST_GPSDATA with(nolock) where imieno in (" & Imieno & ") group by imieno ) and m4.tid in (" & cit.ToString & ") "
            ElseIf cir <> "" Then
                lblmsg.Text = "Please select any City"
                GVReport.Controls.Clear()
                con.Close()
                con.Dispose()
                Exit Sub
                'oda.SelectCommand.CommandText = "select distinct (select fld1 from mmm_mst_master where tid=m4.fld10)[State],(select fld1 from mmm_mst_master where tid=u.fld14)[City], m1.fld2[Device IMEI No.],m1.fld10[Vehicle No],u.UserName[User Name],lattitude,longitude,cTime[Last Movement Date & Time]  frOm mmm_mst_master m1   inner join MMM_MST_DOC d on d.fld24=m1.tid   inner join MMM_MST_USER u on u.uid=d.fld11 inner join   mmm_mst_master m4 on m4.fld10=u.fld13    and curstatus='Allotted' inner join mmm_mst_gpsdata g on g.IMIENO= m1.fld2  WHERE m1.DOCUMENTTYPE='Vehicle' and d.documenttype='vrf fixed_pool'  and m4.documenttype='City' and m1.fld2 <>'' and g.tid in ( select max(tid) from MMM_MST_GPSDATA where imieno in (" & Imieno & ") group by imieno ) and m4.fld10 in (" & cir.ToString & ") "
            End If
            oda.SelectCommand.CommandTimeout = 180
            oda.Fill(dtt)
            dtt.Columns.Add("Current Location Address", Type.GetType("System.String"))
            For i As Integer = 0 To dtt.Rows.Count - 1
                dtt.Rows(i).Item("Current Location Address") = lo.Location(dtt.Rows(i).Item("lattitude"), dtt.Rows(i).Item("longitude"))
            Next
            dtt.Columns.Remove("lattitude")
            dtt.Columns.Remove("longitude")
            GVReport.DataSource = dtt
            GVReport.DataBind()
            ds.Dispose()
            Dim ds1 As New DataSet
            oda.Fill(ds1, "gpsdata")
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

    Public Overloads Overrides Sub VerifyRenderingInServerForm(ByVal control As Control)

    End Sub

    Protected Sub btnexportxl_Click(ByVal sender As Object, ByVal e As System.Web.UI.ImageClickEventArgs) Handles btnexportxl.Click
        Show()
        GVReport.AllowPaging = False
        GVReport.DataBind()
        GVReport.DataSource = ViewState("xlexport")
        Response.Clear()
        Response.Buffer = True
        Response.Write("<br/><div align=""center"" style=""border:1px solid red"" ><h3>" & "GPSDataReport" & "</h3></div> <br/>")
        Response.AddHeader("content-disposition", "attachment;filename=" & "GPSDataReport" & ".xls")
        Response.Charset = ""
        Response.ContentType = "application/vnd.ms-excel"
        Dim sw As New StringWriter()
        Dim hw As New HtmlTextWriter(sw)
        For i As Integer = 0 To GVReport.Rows.Count - 1
            'Apply text style to each Row 
            GVReport.Rows(i).Attributes.Add("class", "textmode")
        Next
        GVReport.RenderControl(hw)
        'style to format numbers to string 
        Dim style As String = "<style> .textmode{mso-number-format:\@;}</style>"
        Response.Write(style)
        Response.Output.Write(sw.ToString())
        Response.Flush()
        Response.End()
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

    'Protected Sub changeInindex(ByVal sender As Object, ByVal e As System.EventArgs)

    '    If ddlrtype.SelectedItem.Text.ToUpper = "SMS LOG REPORT" Then
    '        smsreports.Visible = True
    '        UsrVeh.Visible = False
    '        ddldhm.Visible = False
    '    Else
    '        smsreports.Visible = False
    '        UsrVeh.Visible = True
    '        ddldhm.Visible = True
    '    End If
    '    GVReport.DataSource = Nothing
    '    GVReport.DataBind()
    '    GVReport.EmptyDataText = ""
    '    txtsdate.Text = ""
    '    txtedate.Text = ""
    '    For Each chkitem As System.Web.UI.WebControls.ListItem In UsrVeh.Items
    '        chkitem.Selected = False
    '    Next

    '    For Each smsreport As System.Web.UI.WebControls.ListItem In smsreports.Items
    '        smsreport.Selected = False
    '    Next
    '    GVReport.Controls.Clear()
    '    CheckBox1.Checked = False
    '    UpdatePanel1.Update()
    'End Sub

    Protected Sub gvReport_PageIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles GVReport.PageIndexChanged
        Show()
    End Sub

    Protected Sub gvReport_PageIndexChanging(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.GridViewPageEventArgs) Handles GVReport.PageIndexChanging
        GVReport.PageIndex = e.NewPageIndex
    End Sub

    Protected Sub checkuncheck(sender As Object, e As System.EventArgs)
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

    Protected Sub Citycheckuncheck(sender As Object, e As System.EventArgs)
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
                FilterUserOnCity(id)
            End If
        Else
            For Each chkitem As System.Web.UI.WebControls.ListItem In City.Items
                UsrVeh.Items.Clear()
                CheckBox1.Checked = False
                chkitem.Selected = False
            Next
        End If
    End Sub

    Protected Sub checkuncheckcicle(sender As Object, e As System.EventArgs)
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
                filteronCircle(id)
            End If
        Else
            For Each chkitem As System.Web.UI.WebControls.ListItem In Circle.Items
                City.Items.Clear()
                Citycheck.Checked = False
                chkitem.Selected = False
            Next
        End If
        GVReport.Controls.Clear()
    End Sub

    Protected Sub filteronCircle(ByRef id As String)
        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim con As SqlConnection = New SqlConnection(conStr)
        Dim oda As SqlDataAdapter = New SqlDataAdapter("", con)
        Dim dt As New DataTable
        Try
            City.Items.Clear()
            oda.SelectCommand.CommandText = "select  distinct m1.tid,m1.fld1 from mmm_mst_master m inner join mmm_mst_master m1 on convert(nvarchar,m.tid)=m1.fld10 where m.eid=" & Session("EID") & " and m.documenttype='State' and m1.documenttype='City' and m.tid in (" & id & ") order by m1.fld1 "
            oda.Fill(dt)
            For i As Integer = 0 To dt.Rows.Count - 1
                City.Items.Add(Convert.ToString(dt.Rows(i).Item("fld1")))
                City.Items(i).Value = dt.Rows(i).Item("tid").ToString
            Next

            GVReport.Controls.Clear()
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

    Protected Sub checkuncheckcicle1(sender As Object, e As System.EventArgs)
        Dim id As String = ""
        For i As Integer = 0 To Circle.Items.Count - 1
            If Circle.Items(i).Selected = True Then
                id = id & Circle.Items(i).Value & ","
            End If
        Next
        City.Items.Clear()
        If id.Length > 0 Then
            id = Left(id, id.Length - 1)
            filteronCircle(id)
        End If
        GVReport.Controls.Clear()
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
            GVReport.Controls.Clear()
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

    Protected Sub FilterUser(sender As Object, e As System.EventArgs)
        Dim id As String = ""
        For i As Integer = 0 To City.Items.Count - 1
            If City.Items(i).Selected = True Then
                id = id & City.Items(i).Value & ","
            End If
        Next
        UsrVeh.Items.Clear()
        If id.Length > 0 Then
            id = Left(id, id.Length - 1)
            FilterUserOnCity(id)
        End If
        GVReport.Controls.Clear()
    End Sub

    Public Class Grid
        Public GridData As String
        Public Columns As List(Of GridCol)
        Public IsDetailsOn As Boolean
        Public ErrorMsg As String
        Public rows As New List(Of Dictionary(Of String, Object))()
    End Class

    Public Class GridCol
        Public field As String
        Public title As String
    End Class

    Public Class GPSRptSettings

        Public Shared eid As Integer = 0
        Public Shared ReportType As String = ""
        Public Shared DocType As String = ""
        Public Shared DocField As String = ""
        Public Shared DispName As String = ""
        Public Shared VType As String = ""
        Public Shared Creation As String = ""
        Public Shared IsActive As Boolean = True
        Public Shared Sequence As Integer = 0
        Public Shared SettingType As String = ""

    End Class


End Class
