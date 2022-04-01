Imports System.Data.SqlClient
Imports System.Data
Imports System.IO
Partial Class VehicleStatusIntervalReport
    Inherits System.Web.UI.Page
    Dim imeino As String = ""
    Protected Sub btnshow_Click(sender As Object, e As System.EventArgs) Handles btnshow.Click
        Show()
    End Sub
    Protected Sub Show()
        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim con As SqlConnection = New SqlConnection(conStr)
        Dim oda As SqlDataAdapter = New SqlDataAdapter("", con)
        If ViewState("imei") IsNot Nothing Then
            imeino = ViewState("imei").ToString()
            hdnimei.Value = imeino
        End If
        If ViewState("imei") <> "" Then
            Try
                oda.SelectCommand.CommandText = "select  ctime,devdist,igstatus,speed from   mmm_mst_gpsdata with (nolock) where imieno='" & imeino.ToString & "' and ctime>='" & txtfrom.Text.ToString & "'  and ctime<='" & txtTo.Text.ToString & "' order by ctime"
                Dim ds As New DataSet
                oda.SelectCommand.CommandTimeout = 180
                oda.Fill(ds, "data")
                Dim tbl As New DataTable
                tbl.Columns.Add("Ignition", GetType(String))
                tbl.Columns.Add("Status", GetType(String))
                tbl.Columns.Add("FromDate", GetType(String))
                tbl.Columns.Add("ToDate", GetType(String))
                tbl.Columns.Add("Duration", GetType(String))
                tbl.Columns.Add("Distance", GetType(String))
                tbl.Columns.Add("imgName", GetType(String))

                Dim devStatus As String = "START"
                Dim newStatus As String = "START"

                lblmsg.Text = ""
                GVReport.Controls.Clear()

                If CDate(Left(txtfrom.Text, 10)) > CDate(txtTo.Text) Then
                    lblmsg.Text = "Start Date should not be greater than from End Date."
                    Exit Sub
                End If

                If CDate(txtfrom.Text) < CDate("2014-08-01") Then
                    lblmsg.Text = "Data for this report is available from 2nd August 2014 Onward."
                    Exit Sub
                End If

                Dim FrDate As Date
                Dim ToDate As Date
                Dim dist As Decimal = 0
                Dim finFDate As Date
                If ds.Tables("data").Rows.Count > 0 Then
                    FrDate = ds.Tables("data").Rows(0).Item("ctime").ToString()
                    finFDate = FrDate.ToString()
                    ToDate = ds.Tables("data").Rows(0).Item("ctime").ToString()
                End If
                For i As Integer = 1 To ds.Tables("data").Rows.Count - 1
                    ToDate = ds.Tables("data").Rows(i).Item("ctime").ToString()
                    Select Case DateDiff(DateInterval.Second, FrDate, ToDate)
                        Case 160 To 1000
                            newStatus = "IGNITION OFF AND IDLE"
                        Case 0 To 120
                            If Val(ds.Tables("data").Rows(i).Item("speed").ToString()) = 0 And Val(ds.Tables("data").Rows(i).Item("devdist").ToString()) = 0 And Val(ds.Tables("data").Rows(i).Item("igstatus").ToString()) = 1 Then
                                newStatus = "IGNITION ON AND IDLE"
                            ElseIf Val(ds.Tables("data").Rows(i).Item("speed").ToString()) > 0 And Val(ds.Tables("data").Rows(i).Item("devdist").ToString()) > 0 And Val(ds.Tables("data").Rows(i).Item("igstatus").ToString()) = 1 Then
                                newStatus = "IGNITION ON AND MOVING"
                            ElseIf Val(ds.Tables("data").Rows(i).Item("speed").ToString()) > 0 And Val(ds.Tables("data").Rows(i).Item("devdist").ToString()) > 0 And Val(ds.Tables("data").Rows(i).Item("igstatus").ToString()) = 0 Then
                                newStatus = "IGNITION OFF AND MOVING"
                            End If
                        Case Else
                            newStatus = "IGNITION OFF AND IDLE"
                    End Select
                    If devStatus = "START" Then
                        devStatus = newStatus
                    End If

                    If devStatus = newStatus Then
                        If newStatus <> "IGNITION OFF AND IDLE" Or newStatus <> "IGNITION ON AND IDLE" Then
                            If Val(ds.Tables("data").Rows(i).Item("speed").ToString()) = 0 And Val(ds.Tables("data").Rows(i).Item("devdist").ToString()) > 0 Then
                                dist = dist + Double.Parse(ds.Tables("data").Rows(i).Item("devdist"))
                            ElseIf Val(ds.Tables("data").Rows(i).Item("speed").ToString()) > 0 And Val(ds.Tables("data").Rows(i).Item("devdist").ToString()) > 0 Then
                                dist = dist + Double.Parse(ds.Tables("data").Rows(i).Item("devdist"))
                            End If
                        End If
                    Else
                        'Insert in data table and change status of the devstatus filed to  new one
                        Dim rw As DataRow
                        rw = tbl.NewRow
                        Select Case devStatus
                            Case "IGNITION OFF AND IDLE"
                                rw(0) = "OFF"
                                rw(1) = "IDLE"
                                rw(6) = "RedGlobe.jpg"
                            Case "IGNITION ON AND IDLE"
                                rw(0) = "ON"
                                rw(1) = "IDLE"
                                rw(6) = "YellowGlobe.jpg"
                            Case "IGNITION ON AND MOVING"
                                rw(0) = "ON"
                                rw(1) = "MOVING"
                                rw(6) = "GreenGlobe.jpg"
                            Case "IGNITION OFF AND MOVING"
                                rw(0) = "OFF"
                                rw(1) = "MOVING"
                                rw(6) = "blueGlobe.jpg"
                            Case Else
                        End Select
                        rw(2) = finFDate.ToString()
                        rw(3) = ToDate.ToString()
                        rw(4) = DateDiff(DateInterval.Minute, finFDate, ToDate)
                        If devStatus = "IGNITION ON AND IDLE" Or devStatus = "IGNITION OFF AND IDLE" Then
                            rw(5) = "0.00"
                        Else
                            rw(5) = dist.ToString()
                        End If

                        tbl.Rows.Add(rw)
                        'now initilize all values
                        dist = 0.0
                        devStatus = newStatus
                        finFDate = ToDate
                    End If
                    FrDate = ToDate
                Next
                Dim rw1 As DataRow
                rw1 = tbl.NewRow
                Select Case devStatus
                    Case "IGNITION OFF AND IDLE"
                        rw1(0) = "OFF"
                        rw1(1) = "IDLE"
                        rw1(6) = "RedGlobe.jpg"
                    Case "IGNITION ON AND IDLE"
                        rw1(0) = "ON"
                        rw1(1) = "IDLE"
                        rw1(6) = "YellowGlobe.jpg"
                    Case "IGNITION ON AND MOVING"
                        rw1(0) = "ON"
                        rw1(1) = "MOVING"
                        rw1(6) = "GreenGlobe.jpg"
                    Case "IGNITION OFF AND MOVING"
                        rw1(0) = "OFF"
                        rw1(1) = "MOVING"
                        rw1(6) = "blueGlobe.jpg"
                    Case Else
                End Select
                rw1(2) = finFDate.ToString()
                rw1(3) = ToDate.ToString()
                rw1(4) = DateDiff(DateInterval.Minute, finFDate, ToDate)
                If devStatus = "IGNITION ON AND IDLE" Or devStatus = "IGNITION OFF AND IDLE" Then
                    rw1(5) = "0.00"
                Else
                    rw1(5) = dist.ToString()
                End If

                tbl.Rows.Add(rw1)
                If tbl.Rows.Count > 1 Then
                    GVReport.DataSource = tbl
                    GVReport.DataBind()
                    lblmsg.Text = ""
                Else
                    lblmsg.Text = "Data Not Found."
                End If
            Catch ex As Exception
            Finally
                con.Close()
                con.Dispose()
            End Try
        End If
    End Sub
    Public Overloads Overrides Sub VerifyRenderingInServerForm(ByVal control As Control)

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
    Protected Sub btnexportxl_Click(ByVal sender As Object, ByVal e As System.Web.UI.ImageClickEventArgs) Handles btnexportxl.Click
        Show()
        GVReport.AllowPaging = False
        GVReport.DataBind()
        GVReport.DataSource = ViewState("xlexport")

        Response.Clear()
        Response.Buffer = True

        Response.Write("<br/><div align=""center"" style=""border:1px solid red"" ><h3>" & "Vehicle Status Interval Report" & "</h3></div> <br/>")
        Response.AddHeader("content-disposition", "attachment;filename=VehicleSIReport.xls")
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
                ElseIf Session("EID") = 66 Or Session("EID") = 63 Then
                    UsrVeh.Text = "PhoneName-IMEI"
                    If Session("USERROLE").ToString.ToUpper = "SU" Or Session("USERROLE").ToString.ToUpper = "CADMIN" Or Session("USERROLE").ToString.ToUpper = "CORPORATEUSER" Then
                        oda.SelectCommand.CommandText = "select " & vemei & "[IMEI]," & Vfld & "[PhoneUserName] from mmm_mst_master with(nolock) where documenttype='" & Vdtype & "' and eid=" & Session("EID") & ""
                    Else
                        oda.SelectCommand.CommandText = "select " & vemei & "[IMEI]," & Vfld & "[PhoneUserName] from mmm_mst_master with(nolock) where documenttype='" & Vdtype & "' and eid=" & Session("EID") & ""
                    End If
                ElseIf Session("EID") = 56 Then
                    oda.SelectCommand.CommandText = "select fld2[IMEI],fld1[VehicleName],fld10[VehicleNo] from mmm_mst_master with(nolock) where documenttype='vehicle' and eid=" & Session("EID") & " and fld2<>''"
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
                    If Session("EID") = 66 Or Session("EID") = 63 Then
                        UsrVeh.Items.Add(ds.Tables("vemei").Rows(i).Item(1).ToString() & "-" & ds.Tables("vemei").Rows(i).Item(0).ToString())
                    Else
                        UsrVeh.Items.Add(ds.Tables("vemei").Rows(i).Item(1).ToString() & "-" & ds.Tables("vemei").Rows(i).Item(2).ToString())
                    End If
                    'UsrVeh.Items.Add(ds.Tables("vemei").Rows(i).Item(1).ToString() & "-" & ds.Tables("vemei").Rows(i).Item(2).ToString())
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
    Protected Sub OnMap(ByVal sender As Object, ByVal e As System.EventArgs)
        Dim OmMap As ImageButton = TryCast(sender, ImageButton)
        Dim row As GridViewRow = DirectCast(OmMap.NamingContainer, GridViewRow)
        Response.Redirect("ShowMap.aspx?IMIE=" & imeino.ToString & "&Start=" + row.Cells(3).ToString() + "&End=" + row.Cells(4).ToString())
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
    Protected Sub GVReport_RowDataBound(sender As Object, e As System.Web.UI.WebControls.GridViewRowEventArgs) Handles GVReport.RowDataBound
        If e.Row.RowType = DataControlRowType.DataRow Or e.Row.RowType = DataControlRowType.Header Then
            e.Row.Cells(7).Visible = False
        End If
    End Sub
End Class
