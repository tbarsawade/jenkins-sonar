Imports System.Data
Imports System.Data.SqlClient
Imports System.Web.UI.Adapters.ControlAdapter
Imports iTextSharp.text
Imports System.Drawing
Imports System.IO
Imports System.Globalization
Partial Class GPSDataReport
    Inherits System.Web.UI.Page
    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
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
                If Session("EID") = 32 Then
                    lblvehn.Text = "User-Vehicle"
                    ddldhm.Items.Add("One Hourly")
                    ddldhm.Items.Add("Two Hourly")
                    ddlrtype.Items.Add("No Signal")
                    ddlrtype.Items.Add("Pending Trip Approval")
                    ddlrtype.Items.Add("Consolidated Trip Report")
                    If Session("USERROLE") = "SU" Or Session("USERROLE") = "FCAGGN" Or Session("USERROLE") = "BNK" Or Session("USERROLE") = "CADMIN" Or Session("USERROLE") = "FCANHQ" Then
                        'oda.SelectCommand.CommandText = "select  distinct m." & vemei & "[IMIENO], (select username from mmm_mst_user where eid=" & Session("EID") & " and uid=d." & Ufld & ")[UserName],m." & Vfld & "[VehicleNo] from mmm_mst_Doc d inner join mmm_mst_master m on m.tid=d." & UVfld & " inner join mmm_mst_gpsdata g on g.imieno=m." & vemei & " where d.eid=" & Session("EID") & " and  d.documenttype='" & Udtype & "'  and imieno is not null order by username "
                        oda.SelectCommand.CommandText = "select  distinct m." & vemei & "[IMIENO], u.username,m." & Vfld & "[VehicleNo] from mmm_mst_Doc d inner join mmm_mst_master m on convert(nvarchar,m.tid)=d." & UVfld & "  inner join mmm_mst_user u on convert(nvarchar,u.uid)=d." & Ufld & " where d.eid=" & Session("EID") & " and  d.documenttype='" & Udtype & "'  and  m." & vemei & " is not null and  m." & vemei & " <>'' order by username "
                    ElseIf Session("USERROLE").ToString.ToUpper() = "USER" Then
                        oda.SelectCommand.CommandText = "select  distinct m." & vemei & "[IMIENO], u.username,m." & Vfld & "[VehicleNo] from mmm_mst_Doc d inner join mmm_mst_master m on convert(nvarchar,m.tid)=d." & UVfld & " inner join mmm_mst_gpsdata g on g.imieno=m." & vemei & " inner join mmm_mst_user u on convert(nvarchar,u.uid)=d." & Ufld & " where d.eid=" & Session("EID") & " and  d.documenttype='" & Udtype & "' and d." & Ufld & "= " & Session("UID").ToString() & "  and imieno is not null order by username "
                    ElseIf Session("USERROLE").ToString.ToUpper() = "VENDOR" Then
                        oda.SelectCommand.CommandText = "select distinct m." & vemei & "[IMIENO], m." & Vfld & "[VehicleNo] from  mmm_mst_user u inner join mmm_mst_master m on m.fld12=convert(nvarchar,u.extid) where m.documenttype='vehicle' and extid=" & Session("EXTID") & " and m." & vemei & " is not null and m." & vemei & "<>''"
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
                ElseIf Session("EID") = 66 Or Session("EID") = 63 Or Session("EID") = 71 Then
                    lblvehn.Text = "PhoneName-IMEI"
                    If Session("USERROLE").ToString.ToUpper = "SU" Or Session("USERROLE").ToString.ToUpper = "CADMIN" Or Session("USERROLE").ToString.ToUpper = "CORPORATEUSER" Then
                        oda.SelectCommand.CommandText = "select " & vemei & "[IMEI]," & Vfld & "[PhoneUserName] from mmm_mst_master with(nolock) where documenttype='" & Vdtype & "' and eid=" & Session("EID") & ""
                    Else
                        oda.SelectCommand.CommandText = "select " & vemei & "[IMEI]," & Vfld & "[PhoneUserName] from mmm_mst_master with(nolock) where documenttype='" & Vdtype & "' and eid=" & Session("EID") & ""
                    End If
                ElseIf Session("EID") = 56 Then
                    ddldhm.Items.Add("One Hourly")
                    oda.SelectCommand.CommandText = "select fld2[IMEI],fld1[VehicleName],fld10[VehicleNo] from mmm_mst_master with(nolock) where documenttype='vehicle' and eid=" & Session("EID") & " and fld2<>''"
                Else
                    lblvehn.Text = "VehicleName-VehicleNo."
                    If Session("USERROLE").ToString.ToUpper = "SU" Or Session("USERROLE").ToString.ToUpper = "CADMIN" Or Session("USERROLE").ToString.ToUpper = "CORPORATEUSER" Then
                        oda.SelectCommand.CommandText = "select fld12[IMEI],fld10[VehicleName],fld1[VehicleNo] from mmm_mst_master where documenttype='vehicle' and eid=" & Session("EID") & " and len(fld12)=15 and fld12<>''"
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
                oda.SelectCommand.CommandTimeout = 300
                oda.Fill(ds, "vemei")
                For i As Integer = 0 To ds.Tables("vemei").Rows.Count - 1
                    If Session("USERROLE").ToString.ToUpper = "VENDOR" Then
                        UsrVeh.Items.Add(ds.Tables("vemei").Rows(i).Item(1).ToString())
                    Else
                        If Session("EID") = 66 Or Session("EID") = 63 Or Session("EID") = 71 Then
                            UsrVeh.Items.Add(ds.Tables("vemei").Rows(i).Item(1).ToString() & "-" & ds.Tables("vemei").Rows(i).Item(0).ToString())
                        Else
                            UsrVeh.Items.Add(ds.Tables("vemei").Rows(i).Item(1).ToString() & "-" & ds.Tables("vemei").Rows(i).Item(2).ToString())
                        End If
                    End If
                    UsrVeh.Items(i).Value = ds.Tables("vemei").Rows(i).Item(0).ToString()
                Next
            Catch ex As Exception
            Finally
                con.Close()
                oda.Dispose()
                con.Dispose()
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
    Protected Sub btnshow_Click(sender As Object, e As ImageClickEventArgs) Handles btnshow.Click
        Show()
        UpdatePanel1.Update()
    End Sub
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
        oda.SelectCommand.CommandText = "select* from mmm_mst_entity where eid=" & Session("EID") & " "
        Dim ds As New DataSet()
        oda.Fill(ds, "data")
        Try
            If ds.Tables("data").Rows.Count > 0 Then
                Udtype = ds.Tables("data").Rows(0).Item("uvdtype").ToString
                Ufld = ds.Tables("data").Rows(0).Item("uvuserfield").ToString
                UVfld = ds.Tables("data").Rows(0).Item("uvvehiclefield").ToString
                Vdtype = ds.Tables("data").Rows(0).Item("VIDType").ToString
                Vfld = ds.Tables("data").Rows(0).Item("vivehiclefield").ToString
                vemei = ds.Tables("data").Rows(0).Item("viimeifield").ToString
                status = ds.Tables("data").Rows(0).Item("curstatus").ToString
            End If
            If ddlrtype.SelectedItem.Text.ToUpper = "" Then
            End If
            Dim crsts() As String = status.Split(",")
            Dim sts As String = ""
            For i As Integer = 0 To crsts.Length - 1
                sts = sts & "'" & crsts(i) & "',"
            Next
            sts = Left(sts, sts.Length - 1)
            Dim imeinoallow As Integer = 0
            Dim Imieno As String = ""
            For i As Integer = 0 To UsrVeh.Items.Count - 1
                If UsrVeh.Items(i).Selected = True Then
                    Imieno = Imieno & "'" & UsrVeh.Items(i).Value & "',"
                    imeinoallow = imeinoallow + 1
                End If
            Next
            'If imeinoallow > 20 And ddlrtype.SelectedItem.Text.ToUpper = "LAST LOCATION" Then
            '    lblmsg.Text = "Please select maximum 20 imei no."
            '    Exit Sub
            'Else
            '    lblmsg.Text = ""
            'End If
            If Imieno.ToString = "" Then
                lblmsg.Text = "Please select any User vehicle."
                Exit Sub
            Else
                Imieno = Left(Imieno, Imieno.Length - 1)
            End If
            If ddlrtype.SelectedItem.Text.ToUpper <> "LAST SIGNAL REPORT" Or ddlrtype.SelectedItem.Text.ToUpper <> "LAST LOCATION" Then
                If (ddlrtype.SelectedItem.Text.ToUpper = "NO SIGNAL" And (txtsdate.Text = "" Or txtedate.Text = "")) Or (ddlrtype.SelectedItem.Text.ToUpper = "TRIP VS MILEAGE" And (txtsdate.Text = "" Or txtedate.Text = "")) Or (ddlrtype.SelectedItem.Text.ToUpper = "NO TRIP" And (txtsdate.Text = "" Or txtedate.Text = "")) Then
                    lblmsg.Text = "Please Select date range"
                    Exit Sub
                End If
                If ddlrtype.SelectedItem.Text.ToUpper = "SELECT" Then
                    lblmsg.Text = "Please Select Report Type"
                    Exit Sub
                End If
                Dim crrdate As String = Date.Now.ToString("yyyy-MM-dd HH:mm")
                If TxtStime.Text = "" Then
                    TxtStime.Text = "00:00"
                ElseIf txtetime.Text = "" Then
                    txtetime.Text = "23:59"
                Else
                    If CDate(txtsdate.Text) > CDate(txtedate.Text) Then
                        lblmsg.Text = "Date selection is not correct "
                        Exit Sub
                    End If
                    If DateTime.Parse(txtsdate.Text) > DateTime.Parse(crrdate) Then
                        lblmsg.Text = "Future start date is not allowed "
                        Exit Sub
                    ElseIf DateTime.Parse(txtedate.Text) > DateTime.Parse(crrdate) Then
                        lblmsg.Text = "Future end date is not allowed "
                        Exit Sub
                    End If
                End If
            End If
            Dim sdate As String = txtsdate.Text & " " & TxtStime.Text
            Dim edate As String = txtedate.Text & " " & txtetime.Text & ":59"
            Dim dtt As New DataTable()
            If txtsdate.Text = "" And txtedate.Text = "" Then
                If ddlrtype.SelectedItem.Text.ToUpper = "IDLE REPORT" And ddldhm.SelectedItem.Text.ToUpper = "DAILY" Then
                ElseIf ddlrtype.SelectedItem.Text.ToUpper = "MILEAGE REPORT" And ddldhm.SelectedItem.Text.ToUpper = "DAILY" Then
                    'Old Code
                    'oda.SelectCommand.CommandText = "select distinct max(g.tid)[TId],'Automatic'[TripType],g.IMIENO, m." & Vfld & "[VehicleNo],(select fld1 from mmm_mst_master where tid=m.fld18)[Circle],sum(devdist)[Total KM],convert(nvarchar,g.ctime,5)[Date] from mmm_mst_master m inner join mmm_mst_gpsdata g on g.imieno=m.fld2 where m.documenttype='" & Udtype & "' and m.eid=" & Session("EID") & " and Devtype='TELTRONIKA' and g.imieno<>'' and speed<>0.00 and g.imieno in (" & Imieno & ")  group by g.imieno,m.fld10,m.fld18,convert(nvarchar,g.ctime,5),m.fld2 having count(g.imieno)>0 order by g.IMIENO,convert(nvarchar,g.ctime,5)"
                    'New Change
                    oda.SelectCommand.CommandText = "select distinct max(g.tid)[TId],'Automatic'[TripType],g.IMIENO, m." & Vfld & "[VehicleNo],(select fld1 from mmm_mst_master where tid=m.fld18)[Circle],isnull(sum(devdist),0)[Total KM],convert(nvarchar,g.ctime,5)[Date] from mmm_mst_master m inner join mmm_mst_gpsdata g on g.imieno=m.fld2 where m.documenttype='" & Udtype & "' and m.eid=" & Session("EID") & " and Devtype='TELTRONIKA' and g.imieno<>'' and g.imieno in (" & Imieno & ") and ((devdist>0.03 and speed=0) or (devdist>0 and speed>0))  group by g.imieno,m.fld10,m.fld18,convert(nvarchar,g.ctime,5),m.fld2 having count(g.imieno)>0 order by g.IMIENO,convert(nvarchar,g.ctime,5)"
                ElseIf ddlrtype.SelectedItem.Text.ToUpper = "PENDING TRIP APPROVAL" And ddldhm.SelectedItem.Text.ToUpper = "DAILY" Then
                    oda.SelectCommand.CommandText = "select U.Username[User Name],E.vehicle_no[Vehicle Number],E.trip_start_datetime[Trip Start DateTime],E.trip_end_datetime[Trip End DateTime],E.trip_start_location[Trip Start Location],E.trip_end_location[Trip End Location],E.Total_Distance[Km. Run],TripType[Trip Type] from mmm_mst_elogbook E left outer join mmm_mst_user u on E.uid=u.uid where E.eid=" & Session("EID") & " and E.imei_no  in  (" & Imieno & ")  and (E.isauth=0 and E.trip_end_datetime is not null)"
                ElseIf ddlrtype.SelectedItem.Text.ToUpper = "LAST SIGNAL REPORT" Then
                    oda.SelectCommand.Parameters.Clear()
                    oda.SelectCommand.CommandType = CommandType.StoredProcedure
                    oda.SelectCommand.CommandText = "usproclastrecord"
                    oda.SelectCommand.Parameters.AddWithValue("imeino", Imieno.ToString)
                    oda.SelectCommand.Parameters.AddWithValue("eid", Session("eid").ToString)
                End If
                'ElseIf txtsdate.Text = "" And txtedate.Text = "" Then
                '    If ddlrtype.SelectedItem.Text.ToUpper = "LAST SIGNAL REPORT" Then
                '        oda.SelectCommand.Parameters.Clear()
                '        oda.SelectCommand.CommandType = CommandType.StoredProcedure
                '        oda.SelectCommand.CommandText = "usproclastrecord"
                '        oda.SelectCommand.Parameters.AddWithValue("imeino", Imieno.ToString)
                '        oda.SelectCommand.Parameters.AddWithValue("eid", Session("eid").ToString)
                '    End If
            Else
                If ddlrtype.SelectedItem.Text.ToUpper = "IDLE REPORT" And ddldhm.SelectedItem.Text.ToUpper = "DAILY" Then
                ElseIf ddlrtype.SelectedItem.Text.ToUpper = "MILEAGE REPORT" And ddldhm.SelectedItem.Text.ToUpper = "DAILY" Then
                    'Old Code
                    'oda.SelectCommand.CommandText = "select distinct max(g.tid)[TId],'Automatic'[TripType], g.IMIENO[IMEINO], m.FLD10[VehicleNo],(select fld1 from mmm_mst_master where tid=m.fld18)[Circle],sum(devdist)[TotalKM],convert(nvarchar,g.ctime,5)[Date],(select * from  GETUSERNAMEBYIMEINO(g.IMIENO))[UserName] from mmm_mst_master m inner join mmm_mst_gpsdata g on g.imieno=m.fld2  where m.documenttype='VEHICLE' and m.eid=" & Session("EID") & "  and g.imieno in (" + Imieno + ") and g.cTime >= '" + sdate + "' AND g.ctime <= '" + edate + "' and g.devdist >0.000   group by convert(nvarchar,g.ctime,5), g.imieno,m.fld10,m.fld18,m.fld2 order by UserName,date"
                    'New Change
                    If Session("EID") = 32 Or Session("EID") = 56 Then
                        oda.SelectCommand.CommandText = "set dateformat ymd;select distinct max(g.tid)[TId],'Automatic'[TripType], g.IMIENO[IMEINO], m.FLD10[VehicleNo],(select fld1 from mmm_mst_master with(nolock) where tid=m.fld18)[Circle],isnull(sum(devdist),0)[TotalKM],(select * from  GETUSERNAMEBYIMEINO29APR(g.IMIENO,convert(date,g.ctime),convert(date,g.ctime)))[UserName],convert(nvarchar,convert(date,g.ctime),106)[Date] from mmm_mst_master m with (nolock) inner join mmm_mst_gpsdata g with (nolock) on g.imieno=m.fld2  where m.documenttype='VEHICLE' and m.eid=" & Session("EID") & "  and g.imieno in (" + Imieno + ") and convert(datetime,g.ctime) >= '" + sdate + " ' AND convert(datetime,g.ctime) <= convert(datetime,'" + edate + "')  and ((devdist>0.03 and speed=0) or (devdist>0 and speed>0))  group by convert(date,g.ctime), g.imieno,m.fld10,m.fld18,m.fld2 order by UserName,date"
                    ElseIf Session("EID") = 66 Or Session("EID") = 63 Or Session("EID") = 71 Then
                        Dim str As String = ""
                        str = "set dateformat ymd;select distinct max(g.tid)[TId],'Automatic'[TripType],m." & vemei & "[IMEINo],m." & Vfld & "[Phone Name],''[Circle],convert(int,round(isnull(sum(devdist),0),0))[TotalKM],convert(int,max(speed))[Max. Speed],convert(nvarchar,convert(date,g.ctime),106)[Date] from mmm_mst_master m with (nolock) inner join mmm_mst_gpsdata g with (nolock) on g.imieno=m." & vemei & "  where m.documenttype='" & Vdtype & "' and m.eid=" & Session("EID") & "  and g.imieno in (" + Imieno + ") and convert(datetime,g.ctime) >= '" + sdate + " ' AND convert(datetime,g.ctime) <= convert(datetime,'" + edate + "') and ((devdist>0.03 and speed=0) or (devdist>0 and speed>0))  group by convert(nvarchar,convert(date,g.ctime),106),g.imieno,m." & vemei & ",m." & Vfld & ""
                        str = str.Replace("m.dms.", "dms.")
                        oda.SelectCommand.CommandText = str
                        'ElseIf Session("EID") = 71 Then
                        'oda.SelectCommand.CommandText = "set dateformat ymd;select distinct max(g.tid)[TId],'Automatic'[TripType],u.FLD10[EMP Code],u.fld16[IMEINo.],dms.udf_split('MASTER-Company-fld1',u.fld17)[Company],convert(int,round(isnull(sum(devdist),0),0))[TotalKM],convert(int,max(speed))[Max. Speed],convert(nvarchar,convert(date,g.ctime),106)[Date] from mmm_mst_user u with (nolock) inner join mmm_mst_gpsdata g with (nolock) on g.imieno=u.fld16  where u.eid=" & Session("EID") & "  and g.imieno in (" + Imieno + ") and convert(datetime,g.ctime) >= '" + sdate + " ' AND convert(datetime,g.ctime) <= convert(datetime,'" + edate + "') and ((devdist>0.03 and speed=0) or (devdist>0 and speed>0))  group by convert(nvarchar,convert(date,g.ctime),106),g.imieno,u.fld10,u.fld17,u.fld16"
                    Else
                        'oda.SelectCommand.CommandText = "select distinct max(g.tid)[TId],'Automatic'[TripType], g.IMIENO[IMEINO], m.FLD1[VehicleNo],m.fld10[Vehicle Name],(select fld1 from mmm_mst_master with(nolock) where convert(nvarchar,tid)=m.fld19 and eid=m.eid)[Circle],(select fld1 from mmm_mst_master with(nolock) where convert(nvarchar,tid)=m.fld16 and eid=m.eid)[Cluster],convert(int,round(isnull(sum(devdist),0),0))[TotalKM],convert(nvarchar,g.ctime,106)[Date] from mmm_mst_master m with (nolock) inner join mmm_mst_gpsdata g with (nolock) on g.imieno=m.fld12  where m.documenttype='VEHICLE' and m.eid=" & Session("EID") & "  and g.imieno in (" + Imieno + ") and convert(date,g.ctime) >= '" + txtsdate.Text + " ' AND convert(date,g.ctime) < dateadd(day,1,convert(date,'" + txtedate.Text + "')) and ((devdist>0.03 and speed=0) or (devdist>0 and speed>0))  group by convert(nvarchar,g.ctime,106), g.imieno,m.fld10,m.fld19,m.fld12,m.fld16,m.fld1,m.eid"
                        oda.SelectCommand.CommandText = "set dateformat ymd;select distinct max(g.tid)[TId],'Automatic'[TripType],m.FLD1[VehicleNo],m.fld10[Vehicle Name],(select fld1 from mmm_mst_master with(nolock) where convert(nvarchar,tid)=m.fld19 and eid=m.eid)[Circle],convert(int,round(isnull(sum(devdist),0),0))[TotalKM],convert(int,max(speed))[Max. Speed],convert(nvarchar,convert(date,g.ctime),106)[Date] from mmm_mst_master m with (nolock) inner join mmm_mst_gpsdata g with (nolock) on g.imieno=m.fld12  where m.documenttype='VEHICLE' and m.eid=" & Session("EID") & "  and g.imieno in (" + Imieno + ") and convert(datetime,g.ctime) >= '" + sdate + " ' AND convert(datetime,g.ctime) <= convert(datetime,'" + edate + "') and ((devdist>0.03 and speed=0) or (devdist>0 and speed>0))  group by convert(nvarchar,convert(date,g.ctime),106),g.imieno,m.fld10,m.fld19,m.fld12,m.fld1,m.eid"
                    End If
                ElseIf ddlrtype.SelectedItem.Text.ToUpper = "MILEAGE REPORT" And ddldhm.SelectedItem.Text.ToUpper = "HALF HOURLY" Then
                    oda.SelectCommand.CommandText = "select g.IMIENO[IMEINO], max(m.FLD10)[VehicleNo],max(m1.fld1)[Circle],sum(g.devdist)[TotalKM],max(g.ctime)[Date],(select * from  GETUSERNAMEBYIMEINO(g.IMIENO))[UserName],(select top 1 lattitude from mmm_mst_gpsdata where imieno=g.imieno and ctime=max(g.ctime))[Lat],(select top 1 longitude from mmm_mst_gpsdata where imieno=g.imieno and ctime=max(g.ctime))[Long]  from mmm_mst_master m inner join mmm_mst_gpsdata g on g.imieno=m.fld2 inner join mmm_mst_master m1 on m1.tid=m.fld18  where m.documenttype='VEHICLE' and m1.documenttype='Circle office' and m.eid=32  and g.imieno in (" + Imieno + ") and convert(date,ctime)='" & txtsdate.Text & "' and DATEPART(MINUTE, CAST(cTime as TIME)) IN (0, 30) group by g.IMIENO,g.cTime ORDER BY CAST(ctime as TIME)"
                ElseIf ddlrtype.SelectedItem.Text.ToUpper = "MILEAGE REPORT" And ddldhm.SelectedItem.Text.ToUpper = "ONE HOURLY" Then
                    If imeinoallow > 12 Then
                        lblmsg.Text = "Please select less than 12 Vehicles."
                        Exit Sub
                    End If
                    oda.SelectCommand.CommandText = "select distinct  g.IMIENO[IMEINO], max(m.FLD10)[VehicleNo],max(m1.fld1)[Circle],(select isnull(sum(devdist),0) from mmm_mst_gpsdata where imieno=g.imieno and DATEADD(minute,DATEDIFF(minute,0,ctime),0)>='" & txtsdate.Text & " 00:00' and DATEADD(minute,DATEDIFF(minute,0,ctime),0)<='" & txtsdate.Text & " 01:00' and ((devdist>0.03 and speed=0) or (devdist>0 and speed>0)))[TotalKM],max(g.ctime)[Date],(select * from  GETUSERNAMEBYIMEINO(g.IMIENO))[UserName],(select top 1 lattitude from mmm_mst_gpsdata where imieno=g.imieno and ctime=max(g.ctime))[Lat],(select top 1 longitude from mmm_mst_gpsdata where imieno=g.imieno and ctime=max(g.ctime))[Long]  from mmm_mst_master m inner join mmm_mst_gpsdata g on g.imieno=m.fld2 inner join mmm_mst_master m1 on m1.tid=m.fld18  where m.documenttype='VEHICLE' and m1.documenttype='Circle office' and m.eid=" & Session("EID") & "  and g.imieno in (" + Imieno + ") and Dateadd(minute,DATEDIFF(minute,0,ctime),0)>='" & txtsdate.Text & " 00:00' and Dateadd(minute,DATEDIFF(minute,0,ctime),0)<='" & txtsdate.Text & " 01:00' group by g.IMIENO union select distinct  g.IMIENO[IMEINO], max(m.FLD10)[VehicleNo],max(m1.fld1)[Circle],(select isnull(sum(devdist),0) from mmm_mst_gpsdata where imieno=g.imieno and DATEADD(minute,DATEDIFF(minute,0,ctime),0)>='" & txtsdate.Text & " 01:01' and DATEADD(minute,DATEDIFF(minute,0,ctime),0)<='" & txtsdate.Text & " 02:00' and ((devdist>0.03 and speed=0) or (devdist>0 and speed>0)))[TotalKM],max(g.ctime)[Date],(select * from  GETUSERNAMEBYIMEINO(g.IMIENO))[UserName],(select top 1 lattitude from mmm_mst_gpsdata where imieno=g.imieno and ctime=max(g.ctime))[Lat],(select top 1 longitude from mmm_mst_gpsdata where imieno=g.imieno and ctime=max(g.ctime))[Long]  from mmm_mst_master m inner join mmm_mst_gpsdata g on g.imieno=m.fld2 inner join mmm_mst_master m1 on m1.tid=m.fld18  where m.documenttype='VEHICLE' and m1.documenttype='Circle office' and m.eid=" & Session("EID") & "  and g.imieno in (" + Imieno + ") and Dateadd(minute,DATEDIFF(minute,0,ctime),0)>='" & txtsdate.Text & " 01:01' and Dateadd(minute,DATEDIFF(minute,0,ctime),0)<='" & txtsdate.Text & " 02:00' group by g.IMIENO union select distinct  g.IMIENO[IMEINO], max(m.FLD10)[VehicleNo],max(m1.fld1)[Circle],(select isnull(sum(devdist),0) from mmm_mst_gpsdata where imieno=g.imieno and DATEADD(minute,DATEDIFF(minute,0,ctime),0)>='" & txtsdate.Text & " 02:01' and DATEADD(minute,DATEDIFF(minute,0,ctime),0)<='" & txtsdate.Text & " 03:00' and ((devdist>0.03 and speed=0) or (devdist>0 and speed>0)))[TotalKM],max(g.ctime)[Date],(select * from  GETUSERNAMEBYIMEINO(g.IMIENO))[UserName],(select top 1 lattitude from mmm_mst_gpsdata where imieno=g.imieno and ctime=max(g.ctime))[Lat],(select top 1 longitude from mmm_mst_gpsdata where imieno=g.imieno and ctime=max(g.ctime))[Long]  from mmm_mst_master m inner join mmm_mst_gpsdata g on g.imieno=m.fld2 inner join mmm_mst_master m1 on m1.tid=m.fld18  where m.documenttype='VEHICLE' and m1.documenttype='Circle office' and m.eid=" & Session("EID") & "  and g.imieno in (" + Imieno + ") and Dateadd(minute,DATEDIFF(minute,0,ctime),0)>='" & txtsdate.Text & " 02:01' and Dateadd(minute,DATEDIFF(minute,0,ctime),0)<='" & txtsdate.Text & " 03:00' group by g.IMIENO union select distinct  g.IMIENO[IMEINO], max(m.FLD10)[VehicleNo],max(m1.fld1)[Circle],(select isnull(sum(devdist),0) from mmm_mst_gpsdata where imieno=g.imieno and DATEADD(minute,DATEDIFF(minute,0,ctime),0)>='" & txtsdate.Text & " 03:01' and DATEADD(minute,DATEDIFF(minute,0,ctime),0)<='" & txtsdate.Text & " 04:00' and ((devdist>0.03 and speed=0) or (devdist>0 and speed>0)))[TotalKM],max(g.ctime)[Date],(select * from  GETUSERNAMEBYIMEINO(g.IMIENO))[UserName],(select top 1 lattitude from mmm_mst_gpsdata where imieno=g.imieno and ctime=max(g.ctime)),(select top 1 longitude from mmm_mst_gpsdata where imieno=g.imieno and ctime=max(g.ctime))  from mmm_mst_master m inner join mmm_mst_gpsdata g on g.imieno=m.fld2 inner join mmm_mst_master m1 on m1.tid=m.fld18  where m.documenttype='VEHICLE' and m1.documenttype='Circle office' and m.eid=" & Session("EID") & "  and g.imieno in (" + Imieno + ") and Dateadd(minute,DATEDIFF(minute,0,ctime),0)>='" & txtsdate.Text & " 03:01' and Dateadd(minute,DATEDIFF(minute,0,ctime),0)<='" & txtsdate.Text & " 04:00' group by g.IMIENO union select distinct  g.IMIENO[IMEINO], max(m.FLD10)[VehicleNo],max(m1.fld1)[Circle],(select isnull(sum(devdist),0) from mmm_mst_gpsdata where imieno=g.imieno and DATEADD(minute,DATEDIFF(minute,0,ctime),0)>='" & txtsdate.Text & " 04:01' and DATEADD(minute,DATEDIFF(minute,0,ctime),0)<='" & txtsdate.Text & " 05:00' and ((devdist>0.03 and speed=0) or (devdist>0 and speed>0)))[TotalKM],max(g.ctime)[Date],(select * from  GETUSERNAMEBYIMEINO(g.IMIENO))[UserName],(select top 1 lattitude from mmm_mst_gpsdata where imieno=g.imieno and ctime=max(g.ctime)),(select top 1 longitude from mmm_mst_gpsdata where imieno=g.imieno and ctime=max(g.ctime))  from mmm_mst_master m inner join mmm_mst_gpsdata g on g.imieno=m.fld2 inner join mmm_mst_master m1 on m1.tid=m.fld18  where m.documenttype='VEHICLE' and m1.documenttype='Circle office' and m.eid=" & Session("EID") & "  and g.imieno in (" + Imieno + ") and Dateadd(minute,DATEDIFF(minute,0,ctime),0)>='" & txtsdate.Text & " 04:01' and Dateadd(minute,DATEDIFF(minute,0,ctime),0)<='" & txtsdate.Text & " 05:00' group by g.IMIENO union select distinct  g.IMIENO[IMEINO], max(m.FLD10)[VehicleNo],max(m1.fld1)[Circle],(select isnull(sum(devdist),0) from mmm_mst_gpsdata where imieno=g.imieno and DATEADD(minute,DATEDIFF(minute,0,ctime),0)>='" & txtsdate.Text & " 05:01' and DATEADD(minute,DATEDIFF(minute,0,ctime),0)<='" & txtsdate.Text & " 06:00' and ((devdist>0.03 and speed=0) or (devdist>0 and speed>0)))[TotalKM],max(g.ctime)[Date],(select * from  GETUSERNAMEBYIMEINO(g.IMIENO))[UserName],(select top 1 lattitude from mmm_mst_gpsdata where imieno=g.imieno and ctime=max(g.ctime)),(select top 1 longitude from mmm_mst_gpsdata where imieno=g.imieno and ctime=max(g.ctime))  from mmm_mst_master m inner join mmm_mst_gpsdata g on g.imieno=m.fld2 inner join mmm_mst_master m1 on m1.tid=m.fld18  where m.documenttype='VEHICLE' and m1.documenttype='Circle office' and m.eid=" & Session("EID") & "  and g.imieno in (" + Imieno + ") and Dateadd(minute,DATEDIFF(minute,0,ctime),0)>='" & txtsdate.Text & " 05:01' and Dateadd(minute,DATEDIFF(minute,0,ctime),0)<='" & txtsdate.Text & " 06:00' group by g.IMIENO union select distinct  g.IMIENO[IMEINO], max(m.FLD10)[VehicleNo],max(m1.fld1)[Circle],(select isnull(sum(devdist),0) from mmm_mst_gpsdata where imieno=g.imieno and DATEADD(minute,DATEDIFF(minute,0,ctime),0)>='" & txtsdate.Text & " 06:01' and DATEADD(minute,DATEDIFF(minute,0,ctime),0)<='" & txtsdate.Text & " 07:00' and ((devdist>0.03 and speed=0) or (devdist>0 and speed>0)))[TotalKM],max(g.ctime)[Date],(select * from  GETUSERNAMEBYIMEINO(g.IMIENO))[UserName],(select top 1 lattitude from mmm_mst_gpsdata where imieno=g.imieno and ctime=max(g.ctime)),(select top 1 longitude from mmm_mst_gpsdata where imieno=g.imieno and ctime=max(g.ctime))  from mmm_mst_master m inner join mmm_mst_gpsdata g on g.imieno=m.fld2 inner join mmm_mst_master m1 on m1.tid=m.fld18  where m.documenttype='VEHICLE' and m1.documenttype='Circle office' and m.eid=" & Session("EID") & "  and g.imieno in (" + Imieno + ") and Dateadd(minute,DATEDIFF(minute,0,ctime),0)>='" & txtsdate.Text & " 06:01' and Dateadd(minute,DATEDIFF(minute,0,ctime),0)<='" & txtsdate.Text & " 07:00' group by g.IMIENO union select distinct  g.IMIENO[IMEINO], max(m.FLD10)[VehicleNo],max(m1.fld1)[Circle],(select isnull(sum(devdist),0) from mmm_mst_gpsdata where imieno=g.imieno and DATEADD(minute,DATEDIFF(minute,0,ctime),0)>='" & txtsdate.Text & " 07:01' and DATEADD(minute,DATEDIFF(minute,0,ctime),0)<='" & txtsdate.Text & " 08:00' and ((devdist>0.03 and speed=0) or (devdist>0 and speed>0)))[TotalKM],max(g.ctime)[Date],(select * from  GETUSERNAMEBYIMEINO(g.IMIENO))[UserName],(select top 1 lattitude from mmm_mst_gpsdata where imieno=g.imieno and ctime=max(g.ctime)),(select top 1 longitude from mmm_mst_gpsdata where imieno=g.imieno and ctime=max(g.ctime))  from mmm_mst_master m inner join mmm_mst_gpsdata g on g.imieno=m.fld2 inner join mmm_mst_master m1 on m1.tid=m.fld18  where m.documenttype='VEHICLE' and m1.documenttype='Circle office' and m.eid=" & Session("EID") & "  and g.imieno in (" + Imieno + ") and Dateadd(minute,DATEDIFF(minute,0,ctime),0)>='" & txtsdate.Text & " 07:01' and Dateadd(minute,DATEDIFF(minute,0,ctime),0)<='" & txtsdate.Text & " 08:00' group by g.IMIENO union select distinct  g.IMIENO[IMEINO], max(m.FLD10)[VehicleNo],max(m1.fld1)[Circle],(select isnull(sum(devdist),0) from mmm_mst_gpsdata where imieno=g.imieno and DATEADD(minute,DATEDIFF(minute,0,ctime),0)>='" & txtsdate.Text & " 08:01' and DATEADD(minute,DATEDIFF(minute,0,ctime),0)<='" & txtsdate.Text & " 09:00' and ((devdist>0.03 and speed=0) or (devdist>0 and speed>0)))[TotalKM],max(g.ctime)[Date],(select * from  GETUSERNAMEBYIMEINO(g.IMIENO))[UserName],(select top 1 lattitude from mmm_mst_gpsdata where imieno=g.imieno and ctime=max(g.ctime)),(select top 1 longitude from mmm_mst_gpsdata where imieno=g.imieno and ctime=max(g.ctime))  from mmm_mst_master m inner join mmm_mst_gpsdata g on g.imieno=m.fld2 inner join mmm_mst_master m1 on m1.tid=m.fld18  where m.documenttype='VEHICLE' and m1.documenttype='Circle office' and m.eid=" & Session("EID") & "  and g.imieno in (" + Imieno + ") and Dateadd(minute,DATEDIFF(minute,0,ctime),0)>='" & txtsdate.Text & " 08:01' and Dateadd(minute,DATEDIFF(minute,0,ctime),0)<='" & txtsdate.Text & " 09:00' group by g.IMIENO union select distinct  g.IMIENO[IMEINO], max(m.FLD10)[VehicleNo],max(m1.fld1)[Circle],(select isnull(sum(devdist),0) from mmm_mst_gpsdata where imieno=g.imieno and DATEADD(minute,DATEDIFF(minute,0,ctime),0)>='" & txtsdate.Text & " 09:01' and DATEADD(minute,DATEDIFF(minute,0,ctime),0)<='" & txtsdate.Text & " 10:00' and ((devdist>0.03 and speed=0) or (devdist>0 and speed>0)))[TotalKM],max(g.ctime)[Date],(select * from  GETUSERNAMEBYIMEINO(g.IMIENO))[UserName],(select top 1 lattitude from mmm_mst_gpsdata where imieno=g.imieno and ctime=max(g.ctime)),(select top 1 longitude from mmm_mst_gpsdata where imieno=g.imieno and ctime=max(g.ctime))  from mmm_mst_master m inner join mmm_mst_gpsdata g on g.imieno=m.fld2 inner join mmm_mst_master m1 on m1.tid=m.fld18  where m.documenttype='VEHICLE' and m1.documenttype='Circle office' and m.eid=" & Session("EID") & "  and g.imieno in (" + Imieno + ") and Dateadd(minute,DATEDIFF(minute,0,ctime),0)>='" & txtsdate.Text & " 09:01' and Dateadd(minute,DATEDIFF(minute,0,ctime),0)<='" & txtsdate.Text & " 10:00' group by g.IMIENO union select distinct  g.IMIENO[IMEINO], max(m.FLD10)[VehicleNo],max(m1.fld1)[Circle],(select isnull(sum(devdist),0) from mmm_mst_gpsdata where imieno=g.imieno and DATEADD(minute,DATEDIFF(minute,0,ctime),0)>='" & txtsdate.Text & " 10:01' and DATEADD(minute,DATEDIFF(minute,0,ctime),0)<='" & txtsdate.Text & " 11:00' and ((devdist>0.03 and speed=0) or (devdist>0 and speed>0)))[TotalKM],max(g.ctime)[Date],(select * from  GETUSERNAMEBYIMEINO(g.IMIENO))[UserName],(select top 1 lattitude from mmm_mst_gpsdata where imieno=g.imieno and ctime=max(g.ctime)),(select top 1 longitude from mmm_mst_gpsdata where imieno=g.imieno and ctime=max(g.ctime))  from mmm_mst_master m inner join mmm_mst_gpsdata g on g.imieno=m.fld2 inner join mmm_mst_master m1 on m1.tid=m.fld18  where m.documenttype='VEHICLE' and m1.documenttype='Circle office' and m.eid=" & Session("EID") & "  and g.imieno in (" + Imieno + ") and Dateadd(minute,DATEDIFF(minute,0,ctime),0)>='" & txtsdate.Text & " 10:01' and Dateadd(minute,DATEDIFF(minute,0,ctime),0)<='" & txtsdate.Text & " 11:00' group by g.IMIENO union select distinct  g.IMIENO[IMEINO], max(m.FLD10)[VehicleNo],max(m1.fld1)[Circle],(select isnull(sum(devdist),0) from mmm_mst_gpsdata where imieno=g.imieno and DATEADD(minute,DATEDIFF(minute,0,ctime),0)>='" & txtsdate.Text & " 11:01' and DATEADD(minute,DATEDIFF(minute,0,ctime),0)<='" & txtsdate.Text & " 12:00' and ((devdist>0.03 and speed=0) or (devdist>0 and speed>0)))[TotalKM],max(g.ctime)[Date],(select * from  GETUSERNAMEBYIMEINO(g.IMIENO))[UserName],(select top 1 lattitude from mmm_mst_gpsdata where imieno=g.imieno and ctime=max(g.ctime)),(select top 1 longitude from mmm_mst_gpsdata where imieno=g.imieno and ctime=max(g.ctime))  from mmm_mst_master m inner join mmm_mst_gpsdata g on g.imieno=m.fld2 inner join mmm_mst_master m1 on m1.tid=m.fld18  where m.documenttype='VEHICLE' and m1.documenttype='Circle office' and m.eid=" & Session("EID") & "  and g.imieno in (" + Imieno + ") and Dateadd(minute,DATEDIFF(minute,0,ctime),0)>='" & txtsdate.Text & " 11:01' and Dateadd(minute,DATEDIFF(minute,0,ctime),0)<='" & txtsdate.Text & " 12:00' group by g.IMIENO union select distinct  g.IMIENO[IMEINO], max(m.FLD10)[VehicleNo],max(m1.fld1)[Circle],(select isnull(sum(devdist),0) from mmm_mst_gpsdata where imieno=g.imieno and DATEADD(minute,DATEDIFF(minute,0,ctime),0)>='" & txtsdate.Text & " 12:01' and DATEADD(minute,DATEDIFF(minute,0,ctime),0)<='" & txtsdate.Text & " 13:00' and ((devdist>0.03 and speed=0) or (devdist>0 and speed>0)))[TotalKM],max(g.ctime)[Date],(select * from  GETUSERNAMEBYIMEINO(g.IMIENO))[UserName],(select top 1 lattitude from mmm_mst_gpsdata where imieno=g.imieno and ctime=max(g.ctime)),(select top 1 longitude from mmm_mst_gpsdata where imieno=g.imieno and ctime=max(g.ctime))  from mmm_mst_master m inner join mmm_mst_gpsdata g on g.imieno=m.fld2 inner join mmm_mst_master m1 on m1.tid=m.fld18  where m.documenttype='VEHICLE' and m1.documenttype='Circle office' and m.eid=" & Session("EID") & "  and g.imieno in (" + Imieno + ") and Dateadd(minute,DATEDIFF(minute,0,ctime),0)>='" & txtsdate.Text & " 12:01' and Dateadd(minute,DATEDIFF(minute,0,ctime),0)<='" & txtsdate.Text & " 13:00' group by g.IMIENO union select distinct  g.IMIENO[IMEINO], max(m.FLD10)[VehicleNo],max(m1.fld1)[Circle],(select isnull(sum(devdist),0) from mmm_mst_gpsdata where imieno=g.imieno and DATEADD(minute,DATEDIFF(minute,0,ctime),0)>='" & txtsdate.Text & " 13:01' and DATEADD(minute,DATEDIFF(minute,0,ctime),0)<='" & txtsdate.Text & " 14:00' and ((devdist>0.03 and speed=0) or (devdist>0 and speed>0)))[TotalKM],max(g.ctime)[Date],(select * from  GETUSERNAMEBYIMEINO(g.IMIENO))[UserName],(select top 1 lattitude from mmm_mst_gpsdata where imieno=g.imieno and ctime=max(g.ctime)),(select top 1 longitude from mmm_mst_gpsdata where imieno=g.imieno and ctime=max(g.ctime))  from mmm_mst_master m inner join mmm_mst_gpsdata g on g.imieno=m.fld2 inner join mmm_mst_master m1 on m1.tid=m.fld18  where m.documenttype='VEHICLE' and m1.documenttype='Circle office' and m.eid=" & Session("EID") & "  and g.imieno in (" + Imieno + ") and Dateadd(minute,DATEDIFF(minute,0,ctime),0)>='" & txtsdate.Text & " 13:01' and Dateadd(minute,DATEDIFF(minute,0,ctime),0)<='" & txtsdate.Text & " 14:00' group by g.IMIENO union select distinct  g.IMIENO[IMEINO], max(m.FLD10)[VehicleNo],max(m1.fld1)[Circle],(select isnull(sum(devdist),0) from mmm_mst_gpsdata where imieno=g.imieno and DATEADD(minute,DATEDIFF(minute,0,ctime),0)>='" & txtsdate.Text & " 14:01' and DATEADD(minute,DATEDIFF(minute,0,ctime),0)<='" & txtsdate.Text & " 15:00' and ((devdist>0.03 and speed=0) or (devdist>0 and speed>0)))[TotalKM],max(g.ctime)[Date],(select * from  GETUSERNAMEBYIMEINO(g.IMIENO))[UserName],(select top 1 lattitude from mmm_mst_gpsdata where imieno=g.imieno and ctime=max(g.ctime)),(select top 1 longitude from mmm_mst_gpsdata where imieno=g.imieno and ctime=max(g.ctime))  from mmm_mst_master m inner join mmm_mst_gpsdata g on g.imieno=m.fld2 inner join mmm_mst_master m1 on m1.tid=m.fld18  where m.documenttype='VEHICLE' and m1.documenttype='Circle office' and m.eid=" & Session("EID") & "  and g.imieno in (" + Imieno + ") and Dateadd(minute,DATEDIFF(minute,0,ctime),0)>='" & txtsdate.Text & " 14:01' and Dateadd(minute,DATEDIFF(minute,0,ctime),0)<='" & txtsdate.Text & " 15:00' group by g.IMIENO union select distinct  g.IMIENO[IMEINO], max(m.FLD10)[VehicleNo],max(m1.fld1)[Circle],(select isnull(sum(devdist),0) from mmm_mst_gpsdata where imieno=g.imieno and DATEADD(minute,DATEDIFF(minute,0,ctime),0)>='" & txtsdate.Text & " 15:01' and DATEADD(minute,DATEDIFF(minute,0,ctime),0)<='" & txtsdate.Text & " 16:00' and ((devdist>0.03 and speed=0) or (devdist>0 and speed>0)))[TotalKM],max(g.ctime)[Date],(select * from  GETUSERNAMEBYIMEINO(g.IMIENO))[UserName],(select top 1 lattitude from mmm_mst_gpsdata where imieno=g.imieno and ctime=max(g.ctime)),(select top 1 longitude from mmm_mst_gpsdata where imieno=g.imieno and ctime=max(g.ctime))  from mmm_mst_master m inner join mmm_mst_gpsdata g on g.imieno=m.fld2 inner join mmm_mst_master m1 on m1.tid=m.fld18  where m.documenttype='VEHICLE' and m1.documenttype='Circle office' and m.eid=" & Session("EID") & "  and g.imieno in (" + Imieno + ") and Dateadd(minute,DATEDIFF(minute,0,ctime),0)>='" & txtsdate.Text & " 15:01' and Dateadd(minute,DATEDIFF(minute,0,ctime),0)<='" & txtsdate.Text & " 16:00' group by g.IMIENO union select distinct  g.IMIENO[IMEINO], max(m.FLD10)[VehicleNo],max(m1.fld1)[Circle],(select isnull(sum(devdist),0) from mmm_mst_gpsdata where imieno=g.imieno and DATEADD(minute,DATEDIFF(minute,0,ctime),0)>='" & txtsdate.Text & " 16:01' and DATEADD(minute,DATEDIFF(minute,0,ctime),0)<='" & txtsdate.Text & " 17:00' and ((devdist>0.03 and speed=0) or (devdist>0 and speed>0)))[TotalKM],max(g.ctime)[Date],(select * from  GETUSERNAMEBYIMEINO(g.IMIENO))[UserName],(select top 1 lattitude from mmm_mst_gpsdata where imieno=g.imieno and ctime=max(g.ctime)),(select top 1 longitude from mmm_mst_gpsdata where imieno=g.imieno and ctime=max(g.ctime))  from mmm_mst_master m inner join mmm_mst_gpsdata g on g.imieno=m.fld2 inner join mmm_mst_master m1 on m1.tid=m.fld18  where m.documenttype='VEHICLE' and m1.documenttype='Circle office' and m.eid=" & Session("EID") & "  and g.imieno in (" + Imieno + ") and Dateadd(minute,DATEDIFF(minute,0,ctime),0)>='" & txtsdate.Text & " 16:01' and Dateadd(minute,DATEDIFF(minute,0,ctime),0)<='" & txtsdate.Text & " 17:00' group by g.IMIENO union select distinct  g.IMIENO[IMEINO], max(m.FLD10)[VehicleNo],max(m1.fld1)[Circle],(select isnull(sum(devdist),0) from mmm_mst_gpsdata where imieno=g.imieno and DATEADD(minute,DATEDIFF(minute,0,ctime),0)>='" & txtsdate.Text & " 17:01' and DATEADD(minute,DATEDIFF(minute,0,ctime),0)<='" & txtsdate.Text & " 18:00' and ((devdist>0.03 and speed=0) or (devdist>0 and speed>0)))[TotalKM],max(g.ctime)[Date],(select * from  GETUSERNAMEBYIMEINO(g.IMIENO))[UserName],(select top 1 lattitude from mmm_mst_gpsdata where imieno=g.imieno and ctime=max(g.ctime)),(select top 1 longitude from mmm_mst_gpsdata where imieno=g.imieno and ctime=max(g.ctime))  from mmm_mst_master m inner join mmm_mst_gpsdata g on g.imieno=m.fld2 inner join mmm_mst_master m1 on m1.tid=m.fld18  where m.documenttype='VEHICLE' and m1.documenttype='Circle office' and m.eid=" & Session("EID") & "  and g.imieno in (" + Imieno + ") and Dateadd(minute,DATEDIFF(minute,0,ctime),0)>='" & txtsdate.Text & " 17:01' and Dateadd(minute,DATEDIFF(minute,0,ctime),0)<='" & txtsdate.Text & " 18:00' group by g.IMIENO union select distinct  g.IMIENO[IMEINO], max(m.FLD10)[VehicleNo],max(m1.fld1)[Circle],(select isnull(sum(devdist),0) from mmm_mst_gpsdata where imieno=g.imieno and DATEADD(minute,DATEDIFF(minute,0,ctime),0)>='" & txtsdate.Text & " 18:01' and DATEADD(minute,DATEDIFF(minute,0,ctime),0)<='" & txtsdate.Text & " 19:00' and ((devdist>0.03 and speed=0) or (devdist>0 and speed>0)))[TotalKM],max(g.ctime)[Date],(select * from  GETUSERNAMEBYIMEINO(g.IMIENO))[UserName],(select top 1 lattitude from mmm_mst_gpsdata where imieno=g.imieno and ctime=max(g.ctime)),(select top 1 longitude from mmm_mst_gpsdata where imieno=g.imieno and ctime=max(g.ctime))  from mmm_mst_master m inner join mmm_mst_gpsdata g on g.imieno=m.fld2 inner join mmm_mst_master m1 on m1.tid=m.fld18  where m.documenttype='VEHICLE' and m1.documenttype='Circle office' and m.eid=" & Session("EID") & "  and g.imieno in (" + Imieno + ") and Dateadd(minute,DATEDIFF(minute,0,ctime),0)>='" & txtsdate.Text & " 18:01' and Dateadd(minute,DATEDIFF(minute,0,ctime),0)<='" & txtsdate.Text & " 19:00' group by g.IMIENO union select distinct  g.IMIENO[IMEINO], max(m.FLD10)[VehicleNo],max(m1.fld1)[Circle],(select isnull(sum(devdist),0) from mmm_mst_gpsdata where imieno=g.imieno and DATEADD(minute,DATEDIFF(minute,0,ctime),0)>='" & txtsdate.Text & " 19:01' and DATEADD(minute,DATEDIFF(minute,0,ctime),0)<='" & txtsdate.Text & " 20:00' and ((devdist>0.03 and speed=0) or (devdist>0 and speed>0)))[TotalKM],max(g.ctime)[Date],(select * from  GETUSERNAMEBYIMEINO(g.IMIENO))[UserName],(select top 1 lattitude from mmm_mst_gpsdata where imieno=g.imieno and ctime=max(g.ctime)),(select top 1 longitude from mmm_mst_gpsdata where imieno=g.imieno and ctime=max(g.ctime))  from mmm_mst_master m inner join mmm_mst_gpsdata g on g.imieno=m.fld2 inner join mmm_mst_master m1 on m1.tid=m.fld18  where m.documenttype='VEHICLE' and m1.documenttype='Circle office' and m.eid=" & Session("EID") & "  and g.imieno in (" + Imieno + ") and Dateadd(minute,DATEDIFF(minute,0,ctime),0)>='" & txtsdate.Text & " 19:01' and Dateadd(minute,DATEDIFF(minute,0,ctime),0)<='" & txtsdate.Text & " 20:00' group by g.IMIENO union select distinct  g.IMIENO[IMEINO], max(m.FLD10)[VehicleNo],max(m1.fld1)[Circle],(select isnull(sum(devdist),0) from mmm_mst_gpsdata where imieno=g.imieno and DATEADD(minute,DATEDIFF(minute,0,ctime),0)>='" & txtsdate.Text & " 20:01' and DATEADD(minute,DATEDIFF(minute,0,ctime),0)<='" & txtsdate.Text & " 21:00' and ((devdist>0.03 and speed=0) or (devdist>0 and speed>0)))[TotalKM],max(g.ctime)[Date],(select * from  GETUSERNAMEBYIMEINO(g.IMIENO))[UserName],(select top 1 lattitude from mmm_mst_gpsdata where imieno=g.imieno and ctime=max(g.ctime)),(select top 1 longitude from mmm_mst_gpsdata where imieno=g.imieno and ctime=max(g.ctime))  from mmm_mst_master m inner join mmm_mst_gpsdata g on g.imieno=m.fld2 inner join mmm_mst_master m1 on m1.tid=m.fld18  where m.documenttype='VEHICLE' and m1.documenttype='Circle office' and m.eid=" & Session("EID") & "  and g.imieno in (" + Imieno + ") and Dateadd(minute,DATEDIFF(minute,0,ctime),0)>='" & txtsdate.Text & " 20:01' and Dateadd(minute,DATEDIFF(minute,0,ctime),0)<='" & txtsdate.Text & " 21:00' group by g.IMIENO union select distinct  g.IMIENO[IMEINO], max(m.FLD10)[VehicleNo],max(m1.fld1)[Circle],(select isnull(sum(devdist),0) from mmm_mst_gpsdata where imieno=g.imieno and DATEADD(minute,DATEDIFF(minute,0,ctime),0)>='" & txtsdate.Text & " 21:01' and DATEADD(minute,DATEDIFF(minute,0,ctime),0)<='" & txtsdate.Text & " 22:00' and ((devdist>0.03 and speed=0) or (devdist>0 and speed>0)))[TotalKM],max(g.ctime)[Date],(select * from  GETUSERNAMEBYIMEINO(g.IMIENO))[UserName],(select top 1 lattitude from mmm_mst_gpsdata where imieno=g.imieno and ctime=max(g.ctime)),(select top 1 longitude from mmm_mst_gpsdata where imieno=g.imieno and ctime=max(g.ctime))  from mmm_mst_master m inner join mmm_mst_gpsdata g on g.imieno=m.fld2 inner join mmm_mst_master m1 on m1.tid=m.fld18  where m.documenttype='VEHICLE' and m1.documenttype='Circle office' and m.eid=" & Session("EID") & "  and g.imieno in (" + Imieno + ") and Dateadd(minute,DATEDIFF(minute,0,ctime),0)>='" & txtsdate.Text & " 21:01' and Dateadd(minute,DATEDIFF(minute,0,ctime),0)<='" & txtsdate.Text & " 22:00' group by g.IMIENO union select distinct  g.IMIENO[IMEINO], max(m.FLD10)[VehicleNo],max(m1.fld1)[Circle],(select isnull(sum(devdist),0) from mmm_mst_gpsdata where imieno=g.imieno and DATEADD(minute,DATEDIFF(minute,0,ctime),0)>='" & txtsdate.Text & " 22:01' and DATEADD(minute,DATEDIFF(minute,0,ctime),0)<='" & txtsdate.Text & " 23:00' and ((devdist>0.03 and speed=0) or (devdist>0 and speed>0)))[TotalKM],max(g.ctime)[Date],(select * from  GETUSERNAMEBYIMEINO(g.IMIENO))[UserName],(select top 1 lattitude from mmm_mst_gpsdata where imieno=g.imieno and ctime=max(g.ctime)),(select top 1 longitude from mmm_mst_gpsdata where imieno=g.imieno and ctime=max(g.ctime))  from mmm_mst_master m inner join mmm_mst_gpsdata g on g.imieno=m.fld2 inner join mmm_mst_master m1 on m1.tid=m.fld18  where m.documenttype='VEHICLE' and m1.documenttype='Circle office' and m.eid=" & Session("EID") & "  and g.imieno in (" + Imieno + ") and Dateadd(minute,DATEDIFF(minute,0,ctime),0)>='" & txtsdate.Text & " 22:01' and Dateadd(minute,DATEDIFF(minute,0,ctime),0)<='" & txtsdate.Text & " 23:00' group by g.IMIENO union select distinct  g.IMIENO[IMEINO], max(m.FLD10)[VehicleNo],max(m1.fld1)[Circle],(select isnull(sum(devdist),0) from mmm_mst_gpsdata where imieno=g.imieno and DATEADD(minute,DATEDIFF(minute,0,ctime),0)>='" & txtsdate.Text & " 23:01' and DATEADD(minute,DATEDIFF(minute,0,ctime),0)<='" & txtsdate.Text & " 23:59' and ((devdist>0.03 and speed=0) or (devdist>0 and speed>0)))[TotalKM],max(g.ctime)[Date],(select * from  GETUSERNAMEBYIMEINO(g.IMIENO))[UserName],(select top 1 lattitude from mmm_mst_gpsdata where imieno=g.imieno and ctime=max(g.ctime)),(select top 1 longitude from mmm_mst_gpsdata where imieno=g.imieno and ctime=max(g.ctime))  from mmm_mst_master m inner join mmm_mst_gpsdata g on g.imieno=m.fld2 inner join mmm_mst_master m1 on m1.tid=m.fld18  where m.documenttype='VEHICLE' and m1.documenttype='Circle office' and m.eid=" & Session("EID") & "  and g.imieno in (" + Imieno + ") and DATEADD(minute,DATEDIFF(minute,0,g.ctime),0)>='" & txtsdate.Text & " 23:01' and DATEADD(minute,DATEDIFF(minute,0,g.ctime),0)<='" & txtsdate.Text & " 23:59' group by g.IMIENO "
                ElseIf ddlrtype.SelectedItem.Text.ToUpper = "MILEAGE REPORT" And ddldhm.SelectedItem.Text.ToUpper = "TWO HOURLY" Then
                    If imeinoallow > 12 Then
                        lblmsg.Text = "Please select less than 12 Vehicles."
                        Exit Sub
                    End If
                    oda.SelectCommand.CommandText = "select distinct  g.IMIENO[IMEINO], max(m.FLD10)[VehicleNo],max(m1.fld1)[Circle],(select isnull(sum(devdist),0) from mmm_mst_gpsdata where imieno=g.imieno and DATEADD(minute,DATEDIFF(minute,0,ctime),0)>='" & txtsdate.Text & " 00:01' and DATEADD(minute,DATEDIFF(minute,0,ctime),0)<='" & txtsdate.Text & " 02:00' and ((devdist>0.03 and speed=0) or (devdist>0 and speed>0)))[TotalKM],max(g.ctime)[Date],(select * from  GETUSERNAMEBYIMEINO(g.IMIENO))[UserName],(select top 1 lattitude from mmm_mst_gpsdata where imieno=g.imieno and ctime=max(g.ctime))[Lat],(select top 1 longitude from mmm_mst_gpsdata where imieno=g.imieno and ctime=max(g.ctime))[Long]  from mmm_mst_master m inner join mmm_mst_gpsdata g on g.imieno=m.fld2 inner join mmm_mst_master m1 on m1.tid=m.fld18  where m.documenttype='VEHICLE' and m1.documenttype='Circle office' and m.eid=32  and g.imieno in (" + Imieno + ") and g.DATEADD(minute,DATEDIFF(minute,0,ctime),0)>='" & txtsdate.Text & " 00:01' and g.DATEADD(minute,DATEDIFF(minute,0,ctime),0)<='" & txtsdate.Text & " 02:00' group by g.IMIENO union select distinct  g.IMIENO[IMEINO], max(m.FLD10)[VehicleNo],max(m1.fld1)[Circle],(select isnull(sum(devdist),0) from mmm_mst_gpsdata where imieno=g.imieno and DATEADD(minute,DATEDIFF(minute,0,ctime),0)>='" & txtsdate.Text & " 02:00' and DATEADD(minute,DATEDIFF(minute,0,ctime),0)<='" & txtsdate.Text & " 04:00' and ((devdist>0.03 and speed=0) or (devdist>0 and speed>0)))[TotalKM],max(g.ctime)[Date],(select * from  GETUSERNAMEBYIMEINO(g.IMIENO))[UserName],(select top 1 lattitude from mmm_mst_gpsdata where imieno=g.imieno and ctime=max(g.ctime)),(select top 1 longitude from mmm_mst_gpsdata where imieno=g.imieno and ctime=max(g.ctime))  from mmm_mst_master m inner join mmm_mst_gpsdata g on g.imieno=m.fld2 inner join mmm_mst_master m1 on m1.tid=m.fld18  where m.documenttype='VEHICLE' and m1.documenttype='Circle office' and m.eid=32  and g.imieno in (" + Imieno + ") and g.DATEADD(minute,DATEDIFF(minute,0,ctime),0)>='" & txtsdate.Text & " 02:00' and g.DATEADD(minute,DATEDIFF(minute,0,ctime),0)<='" & txtsdate.Text & " 04:00' group by g.IMIENO union select distinct  g.IMIENO[IMEINO], max(m.FLD10)[VehicleNo],max(m1.fld1)[Circle],(select isnull(sum(devdist),0) from mmm_mst_gpsdata where imieno=g.imieno and DATEADD(minute,DATEDIFF(minute,0,ctime),0)>='" & txtsdate.Text & " 04:00' and DATEADD(minute,DATEDIFF(minute,0,ctime),0)<='" & txtsdate.Text & " 06:00' and ((devdist>0.06 and speed=0) or (devdist>0 and speed>0)))[TotalKM],max(g.ctime)[Date],(select * from  GETUSERNAMEBYIMEINO(g.IMIENO))[UserName],(select top 1 lattitude from mmm_mst_gpsdata where imieno=g.imieno and ctime=max(g.ctime)),(select top 1 longitude from mmm_mst_gpsdata where imieno=g.imieno and ctime=max(g.ctime))  from mmm_mst_master m inner join mmm_mst_gpsdata g on g.imieno=m.fld2 inner join mmm_mst_master m1 on m1.tid=m.fld18  where m.documenttype='VEHICLE' and m1.documenttype='Circle office' and m.eid=32  and g.imieno in (" + Imieno + ") and g.DATEADD(minute,DATEDIFF(minute,0,ctime),0)>='" & txtsdate.Text & " 04:00' and g.DATEADD(minute,DATEDIFF(minute,0,ctime),0)<='" & txtsdate.Text & " 06:00' group by g.IMIENO union select distinct  g.IMIENO[IMEINO], max(m.FLD10)[VehicleNo],max(m1.fld1)[Circle],(select isnull(sum(devdist),0) from mmm_mst_gpsdata where imieno=g.imieno and DATEADD(minute,DATEDIFF(minute,0,ctime),0)>='" & txtsdate.Text & " 06:00' and DATEADD(minute,DATEDIFF(minute,0,ctime),0)<='" & txtsdate.Text & " 08:00' and ((devdist>0.06 and speed=0) or (devdist>0 and speed>0)))[TotalKM],max(g.ctime)[Date],(select * from  GETUSERNAMEBYIMEINO(g.IMIENO))[UserName],(select top 1 lattitude from mmm_mst_gpsdata where imieno=g.imieno and ctime=max(g.ctime)),(select top 1 longitude from mmm_mst_gpsdata where imieno=g.imieno and ctime=max(g.ctime))  from mmm_mst_master m inner join mmm_mst_gpsdata g on g.imieno=m.fld2 inner join mmm_mst_master m1 on m1.tid=m.fld18  where m.documenttype='VEHICLE' and m1.documenttype='Circle office' and m.eid=32  and g.imieno in (" + Imieno + ") and g.DATEADD(minute,DATEDIFF(minute,0,ctime),0)>='" & txtsdate.Text & " 06:00' and g.DATEADD(minute,DATEDIFF(minute,0,ctime),0)<='" & txtsdate.Text & " 08:00' group by g.IMIENO union select distinct  g.IMIENO[IMEINO], max(m.FLD10)[VehicleNo],max(m1.fld1)[Circle],(select isnull(sum(devdist),0) from mmm_mst_gpsdata where imieno=g.imieno and DATEADD(minute,DATEDIFF(minute,0,ctime),0)>='" & txtsdate.Text & " 08:00' and DATEADD(minute,DATEDIFF(minute,0,ctime),0)<='" & txtsdate.Text & " 10:00' and ((devdist>0.06 and speed=0) or (devdist>0 and speed>0)))[TotalKM],max(g.ctime)[Date],(select * from  GETUSERNAMEBYIMEINO(g.IMIENO))[UserName],(select top 1 lattitude from mmm_mst_gpsdata where imieno=g.imieno and ctime=max(g.ctime)),(select top 1 longitude from mmm_mst_gpsdata where imieno=g.imieno and ctime=max(g.ctime))  from mmm_mst_master m inner join mmm_mst_gpsdata g on g.imieno=m.fld2 inner join mmm_mst_master m1 on m1.tid=m.fld18  where m.documenttype='VEHICLE' and m1.documenttype='Circle office' and m.eid=32  and g.imieno in (" + Imieno + ") and g.DATEADD(minute,DATEDIFF(minute,0,ctime),0)>='" & txtsdate.Text & " 08:00' and g.DATEADD(minute,DATEDIFF(minute,0,ctime),0)<='" & txtsdate.Text & " 10:00' group by g.IMIENO union select distinct  g.IMIENO[IMEINO], max(m.FLD10)[VehicleNo],max(m1.fld1)[Circle],(select isnull(sum(devdist),0) from mmm_mst_gpsdata where imieno=g.imieno and DATEADD(minute,DATEDIFF(minute,0,ctime),0)>='" & txtsdate.Text & " 10:00' and DATEADD(minute,DATEDIFF(minute,0,ctime),0)<='" & txtsdate.Text & " 12:00' and ((devdist>0.06 and speed=0) or (devdist>0 and speed>0)))[TotalKM],max(g.ctime)[Date],(select * from  GETUSERNAMEBYIMEINO(g.IMIENO))[UserName],(select top 1 lattitude from mmm_mst_gpsdata where imieno=g.imieno and ctime=max(g.ctime)),(select top 1 longitude from mmm_mst_gpsdata where imieno=g.imieno and ctime=max(g.ctime))  from mmm_mst_master m inner join mmm_mst_gpsdata g on g.imieno=m.fld2 inner join mmm_mst_master m1 on m1.tid=m.fld18  where m.documenttype='VEHICLE' and m1.documenttype='Circle office' and m.eid=32  and g.imieno in (" + Imieno + ") and g.DATEADD(minute,DATEDIFF(minute,0,ctime),0)>='" & txtsdate.Text & " 10:00' and g.DATEADD(minute,DATEDIFF(minute,0,ctime),0)<='" & txtsdate.Text & " 12:00' group by g.IMIENO union select distinct  g.IMIENO[IMEINO], max(m.FLD10)[VehicleNo],max(m1.fld1)[Circle],(select isnull(sum(devdist),0) from mmm_mst_gpsdata where imieno=g.imieno and DATEADD(minute,DATEDIFF(minute,0,ctime),0)>='" & txtsdate.Text & " 12:00' and DATEADD(minute,DATEDIFF(minute,0,ctime),0)<='" & txtsdate.Text & " 14:00' and ((devdist>0.06 and speed=0) or (devdist>0 and speed>0)))[TotalKM],max(g.ctime)[Date],(select * from  GETUSERNAMEBYIMEINO(g.IMIENO))[UserName],(select top 1 lattitude from mmm_mst_gpsdata where imieno=g.imieno and ctime=max(g.ctime)),(select top 1 longitude from mmm_mst_gpsdata where imieno=g.imieno and ctime=max(g.ctime))  from mmm_mst_master m inner join mmm_mst_gpsdata g on g.imieno=m.fld2 inner join mmm_mst_master m1 on m1.tid=m.fld18  where m.documenttype='VEHICLE' and m1.documenttype='Circle office' and m.eid=32  and g.imieno in (" + Imieno + ") and g.DATEADD(minute,DATEDIFF(minute,0,ctime),0)>='" & txtsdate.Text & " 12:00' and g.DATEADD(minute,DATEDIFF(minute,0,ctime),0)<='" & txtsdate.Text & " 14:00' group by g.IMIENO union select distinct  g.IMIENO[IMEINO], max(m.FLD10)[VehicleNo],max(m1.fld1)[Circle],(select isnull(sum(devdist),0) from mmm_mst_gpsdata where imieno=g.imieno and DATEADD(minute,DATEDIFF(minute,0,ctime),0)>='" & txtsdate.Text & " 14:00' and DATEADD(minute,DATEDIFF(minute,0,ctime),0)<='" & txtsdate.Text & " 16:00' and ((devdist>0.06 and speed=0) or (devdist>0 and speed>0)))[TotalKM],max(g.ctime)[Date],(select * from  GETUSERNAMEBYIMEINO(g.IMIENO))[UserName],(select top 1 lattitude from mmm_mst_gpsdata where imieno=g.imieno and ctime=max(g.ctime)),(select top 1 longitude from mmm_mst_gpsdata where imieno=g.imieno and ctime=max(g.ctime))  from mmm_mst_master m inner join mmm_mst_gpsdata g on g.imieno=m.fld2 inner join mmm_mst_master m1 on m1.tid=m.fld18  where m.documenttype='VEHICLE' and m1.documenttype='Circle office' and m.eid=32  and g.imieno in (" + Imieno + ") and g.DATEADD(minute,DATEDIFF(minute,0,ctime),0)>='" & txtsdate.Text & " 14:00' and g.DATEADD(minute,DATEDIFF(minute,0,ctime),0)<='" & txtsdate.Text & " 16:00' group by g.IMIENO union select distinct  g.IMIENO[IMEINO], max(m.FLD10)[VehicleNo],max(m1.fld1)[Circle],(select isnull(sum(devdist),0) from mmm_mst_gpsdata where imieno=g.imieno and DATEADD(minute,DATEDIFF(minute,0,ctime),0)>='" & txtsdate.Text & " 16:00' and DATEADD(minute,DATEDIFF(minute,0,ctime),0)<='" & txtsdate.Text & " 18:00' and ((devdist>0.06 and speed=0) or (devdist>0 and speed>0)))[TotalKM],max(g.ctime)[Date],(select * from  GETUSERNAMEBYIMEINO(g.IMIENO))[UserName],(select top 1 lattitude from mmm_mst_gpsdata where imieno=g.imieno and ctime=max(g.ctime)),(select top 1 longitude from mmm_mst_gpsdata where imieno=g.imieno and ctime=max(g.ctime))  from mmm_mst_master m inner join mmm_mst_gpsdata g on g.imieno=m.fld2 inner join mmm_mst_master m1 on m1.tid=m.fld18  where m.documenttype='VEHICLE' and m1.documenttype='Circle office' and m.eid=32  and g.imieno in (" + Imieno + ") and g.DATEADD(minute,DATEDIFF(minute,0,ctime),0)>='" & txtsdate.Text & " 16:00' and g.DATEADD(minute,DATEDIFF(minute,0,ctime),0)<='" & txtsdate.Text & " 18:00' group by g.IMIENO union select distinct  g.IMIENO[IMEINO], max(m.FLD10)[VehicleNo],max(m1.fld1)[Circle],(select isnull(sum(devdist),0) from mmm_mst_gpsdata where imieno=g.imieno and DATEADD(minute,DATEDIFF(minute,0,ctime),0)>='" & txtsdate.Text & " 18:00' and DATEADD(minute,DATEDIFF(minute,0,ctime),0)<='" & txtsdate.Text & " 20:00' and ((devdist>0.06 and speed=0) or (devdist>0 and speed>0)))[TotalKM],max(g.ctime)[Date],(select * from  GETUSERNAMEBYIMEINO(g.IMIENO))[UserName],(select top 1 lattitude from mmm_mst_gpsdata where imieno=g.imieno and ctime=max(g.ctime)),(select top 1 longitude from mmm_mst_gpsdata where imieno=g.imieno and ctime=max(g.ctime))  from mmm_mst_master m inner join mmm_mst_gpsdata g on g.imieno=m.fld2 inner join mmm_mst_master m1 on m1.tid=m.fld18  where m.documenttype='VEHICLE' and m1.documenttype='Circle office' and m.eid=32  and g.imieno in (" + Imieno + ") and g.DATEADD(minute,DATEDIFF(minute,0,ctime),0)>='" & txtsdate.Text & " 18:00' and g.DATEADD(minute,DATEDIFF(minute,0,ctime),0)<='" & txtsdate.Text & " 20:00' group by g.IMIENO union select distinct  g.IMIENO[IMEINO], max(m.FLD10)[VehicleNo],max(m1.fld1)[Circle],(select isnull(sum(devdist),0) from mmm_mst_gpsdata where imieno=g.imieno and DATEADD(minute,DATEDIFF(minute,0,ctime),0)>='" & txtsdate.Text & " 20:00' and DATEADD(minute,DATEDIFF(minute,0,ctime),0)<='" & txtsdate.Text & " 22:00' and ((devdist>0.06 and speed=0) or (devdist>0 and speed>0)))[TotalKM],max(g.ctime)[Date],(select * from  GETUSERNAMEBYIMEINO(g.IMIENO))[UserName],(select top 1 lattitude from mmm_mst_gpsdata where imieno=g.imieno and ctime=max(g.ctime)),(select top 1 longitude from mmm_mst_gpsdata where imieno=g.imieno and ctime=max(g.ctime))  from mmm_mst_master m inner join mmm_mst_gpsdata g on g.imieno=m.fld2 inner join mmm_mst_master m1 on m1.tid=m.fld18  where m.documenttype='VEHICLE' and m1.documenttype='Circle office' and m.eid=32  and g.imieno in (" + Imieno + ") and DATEADD(minute,DATEDIFF(minute,0,GETDATE()),0)>='" & txtsdate.Text & " 20:00' and DATEADD(minute,DATEDIFF(minute,0,GETDATE()),0)<='" & txtsdate.Text & " 22:00' group by g.IMIENO union select distinct  g.IMIENO[IMEINO], max(m.FLD10)[VehicleNo],max(m1.fld1)[Circle],(select isnull(sum(devdist),0) from mmm_mst_gpsdata where imieno=g.imieno and DATEADD(minute,DATEDIFF(minute,0,GETDATE()),0)>='" & txtsdate.Text & " 22:00' and DATEADD(minute,DATEDIFF(minute,0,GETDATE()),0)<='" & txtsdate.Text & " 23:59' and ((devdist>0.06 and speed=0) or (devdist>0 and speed>0)))[TotalKM],max(g.ctime)[Date],(select * from  GETUSERNAMEBYIMEINO(g.IMIENO))[UserName],(select top 1 lattitude from mmm_mst_gpsdata where imieno=g.imieno and ctime=max(g.ctime)),(select top 1 longitude from mmm_mst_gpsdata where imieno=g.imieno and ctime=max(g.ctime))  from mmm_mst_master m inner join mmm_mst_gpsdata g on g.imieno=m.fld2 inner join mmm_mst_master m1 on m1.tid=m.fld18  where m.documenttype='VEHICLE' and m1.documenttype='Circle office' and m.eid=32  and g.imieno in (" + Imieno + ") and DATEADD(minute,DATEDIFF(minute,0,GETDATE()),0)>='" & txtsdate.Text & " 22:00' and DATEADD(minute,DATEDIFF(minute,0,GETDATE()),0)<='" & txtsdate.Text & " 23:59' group by g.IMIENO"
                ElseIf ddlrtype.SelectedItem.Text.ToUpper = "TRIP VS MILEAGE" Then
                    'New Change
                    oda.SelectCommand.CommandText = "select r1.VehicleNo,r1.IMEINO,r1.Circle,r1.TotalKM,r1.date1[date],r2.distance[Trip Distance KM],r2.start[Trip start],r2.Trip_end_datetime[Trip end datetime],r1.UserName from (select distinct g.IMIENO[IMEINO], m.FLD10[VehicleNo],(select fld1 from mmm_mst_master where tid=m.fld18)[Circle],sum(devdist)[TotalKM],convert(nvarchar,g.ctime,5)[Date1] ,(select * from  GETUSERNAMEBYIMEINO(g.IMIENO))[UserName] from mmm_mst_master m inner join mmm_mst_gpsdata g on g.imieno=m.fld2  where m.documenttype='VEHICLE' and m.eid=" & Session("EID") & "  and g.imieno in (" + Imieno + ") and g.cTime >= '" & sdate & "' AND g.ctime <= '" & edate & "' and ((devdist>0.03 and speed=0) or (devdist>0 and speed>0))  group by g.imieno,m.fld10,m.fld18,convert(nvarchar,g.ctime,5),m.fld2 ) r1 left outer join  ( select imei_no[imeino1],Total_distance[distance],Trip_start_datetime[start],Trip_end_datetime from MMM_mst_elogbook where imei_no in (" + Imieno + ") and Trip_start_datetime >='" & sdate & "' and  '" & edate & "'>=Trip_start_datetime and  eid=" & Session("EID") & ") as r2 on r1.IMEINO=r2.imeino1 and convert(date,r2.start,120)= convert(date,r1.date1,5) order by [UserName],[date]"
                ElseIf ddlrtype.SelectedItem.Text.ToUpper = "NO SIGNAL" And ddldhm.SelectedItem.Text.ToUpper = "DAILY" Then
                    oda.SelectCommand.Parameters.Clear()
                    oda.SelectCommand.CommandType = CommandType.StoredProcedure
                    oda.SelectCommand.CommandText = "NOSIGNALBYI"
                    oda.SelectCommand.Parameters.AddWithValue("imieno", Imieno.ToString)
                    oda.SelectCommand.Parameters.AddWithValue("sdatetime", sdate)
                    oda.SelectCommand.Parameters.AddWithValue("edatetime", edate)
                    'ElseIf ddlrtype.SelectedItem.Text.ToUpper = "LAST SIGNAL REPORT" Then
                    '    oda.SelectCommand.Parameters.Clear()
                    '    oda.SelectCommand.CommandType = CommandType.StoredProcedure
                    '    oda.SelectCommand.CommandText = "usproclastrecord"
                    '    oda.SelectCommand.Parameters.AddWithValue("imeino", Imieno.ToString)
                    '    oda.SelectCommand.Parameters.AddWithValue("eid", Session("eid").ToString)
                ElseIf ddlrtype.SelectedItem.Text.ToUpper = "PENDING TRIP APPROVAL" And ddldhm.SelectedItem.Text.ToUpper = "DAILY" Then
                    oda.SelectCommand.CommandText = "select U.Username[User Name],E.vehicle_no[Vehicle Number],E.trip_start_datetime[Trip Start DateTime],E.trip_end_datetime[Trip End DateTime],E.trip_start_location[Trip Start Location],E.trip_end_location[Trip End Location],E.Total_Distance[Km. Run],TripType[Trip Type] from mmm_mst_elogbook E left outer join mmm_mst_user u on E.uid=u.uid where E.eid=" & Session("EID") & " and E.imei_no  in  (" & Imieno & ")  and (E.trip_start_datetime>'" & txtsdate.Text & " " & TxtStime.Text & "' and E.trip_start_datetime<'" & txtedate.Text & " " & txtetime.Text & "' and E.isauth=0 or E.trip_end_datetime>'" & txtsdate.Text & " " & TxtStime.Text & "' and E.trip_end_datetime<'" & txtedate.Text & " " & txtetime.Text & "' and E.isauth=0 and E.trip_end_datetime is not null)"
                ElseIf ddlrtype.SelectedItem.Text.ToUpper = "CONSOLIDATED TRIP REPORT" And ddldhm.SelectedItem.Text.ToUpper = "DAILY" Then
                    oda.SelectCommand.CommandText = "select username[User Name],vehicle_no[Vehicle No.],IMEI_NO[IMEI No.],convert(nvarchar,Trip_Start_DateTime,120)[Trip Start DateTime],convert(nvarchar,Trip_end_DateTime,120)[Trip End DateTime],Trip_Start_Location[Trip Start Location],Trip_End_Location[Trip End Location],Total_Distance[Total Distance],TripType[Trip Type] from MMM_MST_ELOGBOOK e,  MMM_MST_USER u where   e.uid=u.uid and E.imei_no  in  (" & Imieno & ") and E.eid=" & Session("EID") & "  and Trip_start_DateTime >= '" & txtsdate.Text & " " & TxtStime.Text & "'   AND (Trip_end_DateTime <= '" & txtedate.Text & " " & txtetime.Text & "' or trip_end_Datetime is null or trip_end_datetime='')   order by Trip_Start_DateTime "
                ElseIf ddlrtype.SelectedItem.Text.ToUpper = "NO TRIP" And ddldhm.SelectedItem.Text.ToUpper = "DAILY" Then
                    If Imieno.ToString.Length > 17 Then
                        lblmsg.Text = "Please select only one User-Vehicle at a time"
                        con.Close()
                        Exit Sub
                    Else
                        lblmsg.Text = ""
                    End If
                    oda.SelectCommand.Parameters.Clear()
                    oda.SelectCommand.CommandType = CommandType.StoredProcedure
                    oda.SelectCommand.CommandText = "uspgetNotrip"
                    oda.SelectCommand.Parameters.AddWithValue("imieno", Imieno.ToString)
                    oda.SelectCommand.Parameters.AddWithValue("sdatetime", sdate)
                    oda.SelectCommand.Parameters.AddWithValue("edatetime", edate)
                End If
            End If
            ds.Dispose()
            Dim ds1 As New DataSet
            Try
                oda.SelectCommand.CommandTimeout = 300
            Catch exption As Exception
                lblmsg.Text = "Dear User please enter short date range "
            End Try
            Dim lo As New GPSClass()
            If ddldhm.SelectedItem.Text.ToUpper <> "DAILY" Then
                oda.Fill(dtt)
                dtt.Columns.Add("Current Location Address", Type.GetType("System.String"))
                For i As Integer = 0 To dtt.Rows.Count - 1
                    dtt.Rows(i).Item("Current Location Address") = lo.Location(dtt.Rows(i).Item("lat"), dtt.Rows(i).Item("long"))
                Next
                dtt.Columns.Remove("lat")
                dtt.Columns.Remove("long")
                ViewState("xlexport") = dtt
                ViewState("pdf") = dtt
            Else
                oda.Fill(ds1, "gpsdata")
                ViewState("xlexport") = ds1.Tables("gpsdata")
                ViewState("pdf") = ds1
            End If

            'Dim objDataColumn As New DataColumn("uri")
            'If (Session("EID") = 54) Then
            '    objDataColumn.DefaultValue = HttpContext.Current.Request.Url.Authority + HttpContext.Current.Request.ApplicationPath & "/ShowMapIndus.aspx"
            'Else
            '    objDataColumn.DefaultValue = HttpContext.Current.Request.Url.Authority + HttpContext.Current.Request.ApplicationPath & "/ShowMapGps.aspx"
            'End If

            'If ddldhm.SelectedItem.Text.ToUpper = "DAILY" Then
            '    ds1.Tables("gpsdata").Columns.Add(objDataColumn)
            'End If


            Dim dtMap As New DataTable
            oda.SelectCommand.CommandText = "Select maptype,APIKey from mmm_mst_Entity with(nolock) where Eid=" & Session("Eid")
            oda.Fill(dtMap)

            If Not IsNothing(dtMap.Rows(0).Item("maptype")) Then
                If Not dtMap.Rows(0).Item("maptype").ToString.Trim() = "" Then
                    If dtMap.Rows(0).Item("maptype").ToString.Trim().ToUpper() = "GOOGLE" Then
                        uri &= "GmapWindow.aspx"
                    Else
                        uri &= "NmapWindow.aspx"
                    End If
                Else
                    uri &= "GmapWindow.aspx"
                End If
            Else
                uri &= "GmapWindow.aspx"
            End If
            'testing remove it
            'uri &= "NMapWindow.aspx"

            If ddlrtype.SelectedItem.Text.ToUpper() = "MILEAGE REPORT" Then
                If ddldhm.SelectedItem.Text.ToUpper = "DAILY" Then
                    GVReport1.DataSource = ds1.Tables("gpsdata")
                    GVReport1.DataBind()
                    lblmsg.Text = ""
                    GVReport1.Visible = True
                    GVReport.Controls.Clear()
                    GVReport.Visible = False
                Else
                    GVReport.DataSource = dtt
                    GVReport.DataBind()
                    GVReport1.Controls.Clear()
                    GVReport1.Visible = False
                    GVReport.Visible = True
                    lblmsg.Text = ""
                End If
            Else
                GVReport.DataSource = ds1.Tables("gpsdata")
                GVReport.DataBind()
                GVReport1.Controls.Clear()
                GVReport1.Visible = False
                GVReport.Visible = True
                lblmsg.Text = ""
                '  GVReport1.Controls.Clear()
            End If
        Catch ex As Exception
        Finally
            con.Close()
            oda.Dispose()
            con.Dispose()
        End Try
    End Sub
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
        ' Show()
        If ddlrtype.SelectedItem.Text.ToUpper() = "MILEAGE REPORT" Then
            If ddldhm.SelectedItem.Text.ToUpper = "DAILY" Then
                GVReport1.AllowPaging = False
                GVReport1.DataSource = ViewState("xlexport")
                GVReport1.DataBind()
            Else
                GVReport.AllowPaging = False
                GVReport.DataSource = ViewState("xlexport")
                GVReport.DataBind()
            End If
        Else
            GVReport.AllowPaging = False
            GVReport.DataSource = ViewState("xlexport")
            GVReport.DataBind()
        End If

        Response.Clear()
        Response.Buffer = True

        Response.Write("<br/><div align=""center"" style=""border:1px solid red"" ><h3>" & "GPSDataReport" & "</h3></div> <br/>")
        Response.AddHeader("content-disposition", "attachment;filename=" & "GPSDataReport" & ".xls")
        Response.Charset = ""
        Response.ContentType = "application/vnd.ms-excel"
        Dim sw As New StringWriter()
        Dim hw As New HtmlTextWriter(sw)
        If ddlrtype.SelectedItem.Text.ToUpper() = "MILEAGE REPORT" Then
            If ddldhm.SelectedItem.Text.ToUpper = "DAILY" Then
                For i As Integer = 0 To GVReport1.Rows.Count - 1
                    'Apply text style to each Row 
                    'GVReport1.Columns(0).HeaderText = ""
                    'GVReport1.Rows(0).Visible = False
                    GVReport1.Rows(i).Attributes.Add("class", "textmode")
                Next
                GVReport1.RenderControl(hw)
            Else
                For i As Integer = 0 To GVReport.Rows.Count - 1
                    'Apply text style to each Row 
                    GVReport.Rows(i).Attributes.Add("class", "textmode")
                    'GVReport.Rows(0).Visible = False
                Next
                GVReport.RenderControl(hw)
            End If
        Else
            For i As Integer = 0 To GVReport.Rows.Count - 1
                'Apply text style to each Row 
                GVReport.Rows(i).Attributes.Add("class", "textmode")
                'GVReport.Rows(0).Visible = False
            Next
            GVReport.RenderControl(hw)
        End If
        'style to format numbers to string 
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
        If ddldhm.SelectedItem.Text = "Half Hourly" Or ddldhm.SelectedItem.Text = "One Hourly" Or ddldhm.SelectedItem.Text = "Two Hourly" Then
            txtedate.Text = txtsdate.Text
            txtedate.Enabled = False
        Else
            txtedate.Enabled = True
        End If
    End Sub

    Protected Sub ddldhm_SelectedIndexChanged(sender As Object, e As System.EventArgs) Handles ddldhm.SelectedIndexChanged
        txtedate.Enabled = True
        If ddldhm.SelectedItem.Text = "Half Hourly" Or ddldhm.SelectedItem.Text = "One Hourly" Or ddldhm.SelectedItem.Text = "Two Hourly" Then
            txtedate.Text = txtsdate.Text
            txtedate.Enabled = False
        Else
            txtedate.Enabled = True
        End If
    End Sub

    Protected Sub GVReport1_RowDataBound(sender As Object, e As GridViewRowEventArgs) Handles GVReport1.RowDataBound

        If e.Row.RowType = DataControlRowType.DataRow Then
            Dim tid As String = GVReport1.DataKeys(e.Row.RowIndex).Value.ToString()
            Dim trip As String = DataBinder.Eval(e.Row.DataItem, "TripType").ToString
            Dim dtArr = e.Row.Cells(8).Text.Split(" ")

            Dim enumval = CInt([Enum].Parse(GetType(MonthEnum), dtArr(1).ToString()))
            Dim dte As New Date(Convert.ToInt32(dtArr(2)), enumval, Convert.ToInt32(dtArr(0)))
            Dim stime As String
            Dim etime As String
            If txtsdate.Text.Trim = txtedate.Text.Trim Then
                stime = TxtStime.Text
                etime = txtetime.Text
            ElseIf dte = Convert.ToDateTime(txtsdate.Text) Then
                stime = TxtStime.Text
                etime = ""
            ElseIf dte = Convert.ToDateTime(txtedate.Text) Then
                stime = ""
                etime = txtetime.Text
            End If
            e.Row.Cells(0).Text = "<a class='detail' style='text-decoration:none;'  href='#' onclick=""OpenWindow('" & uri & "?tid=" & tid & "&type=" & trip & "&flag=3&stime=" & stime & "&etime=" & etime & "');""><img alt='Show On Map' src='images/earth_search.png'  height='16px' width='16px'/></a>"
            e.Row.Cells(0).HorizontalAlign = HorizontalAlign.Center
        End If

        If Session("EID") = 54 Then
            e.Row.Cells(1).Visible = False
        Else
        End If
    End Sub
    Public uri As String = "http://" + HttpContext.Current.Request.Url.Authority + HttpContext.Current.Request.ApplicationPath & "/"
End Class
Public Enum MonthEnum
    Jan = 1
    Feb = 2
    Mar = 3
    Apr = 4
    May = 5
    Jun = 6
    Jul = 7
    Aug = 8
    Sep = 9
    Oct = 10
    Nov = 11
    Dec = 12
End Enum
