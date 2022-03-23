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
    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        lblRecord.Text = ""
        If Not IsPostBack Then

            ' BindRegsNumberDDl()
            BindRegsNumberDDlFilter()
        End If
        For i As Integer = 0 To 23
            If i < 10 Then
                'str = i
                ddlshh.Items.Add("0" & i)
                ddlehh.Items.Add("0" & i)
            Else
                ddlshh.Items.Add(i)
                ddlehh.Items.Add(i)
            End If
        Next
        For j As Integer = 0 To 59
            If j < 10 Then
                'str = j
                ddlsmm.Items.Add("0" & j)
                ddlemm.Items.Add("0" & j)
            Else
                ddlsmm.Items.Add(j)
                ddlemm.Items.Add(j)
            End If
        Next
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
 Public Sub BindRegsNumberDDl()
        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim con As SqlConnection = New SqlConnection(conStr)
        Try
            Dim Udtype As String
            Dim Ufld As String
            Dim UVfld As String
            Dim Vdtype As String
            Dim Vfld As String
            Dim vemei As String = ""
            Dim oda As SqlDataAdapter = New SqlDataAdapter("", con)
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
            Dim UserQry As String = String.Empty
            If Session("EID") = 32 Then
                If Session("USERROLE") = "SU" Or Session("USERROLE") = "FCAGGN" Or Session("USERROLE") = "BNK" Or Session("USERROLE") = "CADMIN" Or Session("USERROLE") = "FCANHQ" Then
                    oda.SelectCommand.CommandText = "set dateformat dmy select  max(m." & vemei & ")[IMIENO], max(u.username)[UserName],max(m." & Vfld & ")[VehicleNo],d." & Ufld & "[uid] from mmm_mst_Doc d inner join mmm_mst_master m on convert(nvarchar,m.tid)=d." & UVfld & " inner join mmm_mst_user u on u.uid=d." & Ufld & " where d.eid=" & Session("EID") & " and  d.documenttype='" & Udtype & "'  and m." & vemei & " is not null and m." & vemei & "<>''  group by d." & Ufld & ",u.username ,d.tid having d.tid=( select max(tid) from mmm_mst_doc where eid=32 and documenttype='vrf fixed_pool' and fld11=d.fld11 and curstatus in ('archive','allotted','surrender')) order by username "
                ElseIf Session("USERROLE").ToUpper() = "USER" Then
                    oda.SelectCommand.CommandText = "select  max(m." & vemei & ")[IMIENO], max(u.username)[UserName],max(m." & Vfld & ")[VehicleNo],d." & Ufld & "[uid] from mmm_mst_Doc d inner join mmm_mst_master m on m.tid=d." & UVfld & "  inner join mmm_mst_user u on u.uid=d." & Ufld & " where d.eid=" & Session("EID") & " and  d.documenttype='" & Udtype & "' and d." & Ufld & "= " & Session("UID").ToString() & "  and m.fld2 is not null group by d." & Ufld & ",username,d.tid having d.tid=( select max(tid) from mmm_mst_doc where eid=32 and documenttype='vrf fixed_pool' and fld11=d.fld11 and curstatus in ('archive','allotted','surrender')) order by username "
                ElseIf Session("USERROLE").ToString.ToUpper() = "VENDOR" Then
                    oda.SelectCommand.CommandText = "select distinct m." & vemei & "[IMIENO], m." & Vfld & "[VehicleNo] from  mmm_mst_user u inner join mmm_mst_master m on m.fld12=convert(nvarchar,u.extid) where m.documenttype='vehicle' and extid=" & Session("EXTID") & " and m." & vemei & " is not null and m." & vemei & "<>''"
                Else
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
                oda.SelectCommand.CommandText = "select fld12[IMEI],fld10[VehicleName],fld1[VehicleNo] from mmm_mst_master where documenttype='vehicle' and eid=" & Session("EID") & " and len(fld12)=15"
            End If
            ds.Clear()
            oda.SelectCommand.CommandTimeout = 180
            oda.Fill(ds, "vemei")
            con.Dispose()
            Dim ii As Integer = 0
            If Session("USERROLE").ToUpper() <> "USER" Then
                ddlVehicleRegNo.Items.Insert(0, "--Select All--")
            End If
            For i As Integer = 0 To ds.Tables("vemei").Rows.Count - 1
                If ds.Tables("vemei").Rows(i).Item(1).ToString().ToString() = "" Then
                Else
                    If Session("USERROLE").ToString.ToUpper() = "USER" Then
                        ddlVehicleRegNo.Items.Add(ds.Tables("vemei").Rows(i).Item(1).ToString() & "/" & ds.Tables("vemei").Rows(i).Item(2).ToString())
                        'ViewState("vehicleNoPdf") = ds.Tables("vemei").Rows(i).Item(2).ToString()
                        ddlVehicleRegNo.Items(i).Value = ds.Tables("vemei").Rows(i).Item(3).ToString()
                        ViewState("vehicleNo") = ViewState("vehicleNo") & ds.Tables("vemei").Rows(i).Item(2).ToString() & ","
                    ElseIf Session("USERROLE").ToString.ToUpper = "VENDOR" Then
                        ddlVehicleRegNo.Items.Add(ds.Tables("vemei").Rows(i).Item(1).ToString())
                        'ViewState("vehicleNoPdf") = ds.Tables("vemei").Rows(i).Item(2).ToString()
                        ddlVehicleRegNo.Items(i + 1).Value = ds.Tables("vemei").Rows(i).Item(0).ToString()
                        ViewState("vehicleNo") = ViewState("vehicleNo") & ds.Tables("vemei").Rows(i).Item(1).ToString() & ","
                    Else
                        If Session("EID") = 32 Then
                            ddlVehicleRegNo.Items.Add(ds.Tables("vemei").Rows(i).Item(1).ToString() & "/" & ds.Tables("vemei").Rows(i).Item(2).ToString())
                            'ViewState("vehicleNoPdf") = ddlVehicleRegNo.SelectedItem.Text()
                            ddlVehicleRegNo.Items(i + 1).Value = ds.Tables("vemei").Rows(i).Item(3).ToString()
                            ViewState("vehicleNo") = ViewState("vehicleNo") & ds.Tables("vemei").Rows(i).Item(2).ToString() & ","
                        Else
                            ddlVehicleRegNo.Items.Add(ds.Tables("vemei").Rows(i).Item(1).ToString() & "/" & ds.Tables("vemei").Rows(i).Item(2).ToString())
                            'ViewState("vehicleNoPdf") = ddlVehicleRegNo.SelectedItem.Text()
                            ddlVehicleRegNo.Items(i + 1).Value = ds.Tables("vemei").Rows(i).Item(0).ToString()
                            ViewState("vehicleNo") = ViewState("vehicleNo") & ds.Tables("vemei").Rows(i).Item(2).ToString() & ","
                        End If
                    End If
                End If
            Next
            If ds.Tables("vemei").Rows.Count > 0 Then
                If ViewState("vehicleNo").ToString.Length > 0 Then
                    ViewState("vehicleNo") = Left(ViewState("vehicleNo"), ViewState("vehicleNo").ToString.Length - 1)
                End If
            End If
        Catch ex As Exception
        Finally

        End Try

    End Sub
    Public Sub BindRegsNumberDDlFilter()
        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim con As SqlConnection = New SqlConnection(conStr)
        Try
            Dim Udtype As String
            Dim Ufld As String
            Dim UVfld As String
            Dim Vdtype As String
            Dim Vfld As String
            Dim vemei As String = ""
            Dim oda As SqlDataAdapter = New SqlDataAdapter("", con)
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
            Dim UserQry As String = String.Empty
            If Session("EID") = 32 Then
                If Session("USERROLE") = "SU" Or Session("USERROLE") = "FCAGGN" Or Session("USERROLE") = "BNK" Or Session("USERROLE") = "CADMIN" Or Session("USERROLE") = "FCANHQ" Then
                    oda.SelectCommand.CommandText = "set dateformat dmy select  m." & vemei & "[IMIENO], u.username[UserName],m." & Vfld & "[VehicleNo],d." & Ufld & "[uid] from mmm_mst_Doc d inner join mmm_mst_master m on convert(nvarchar,m.tid)=d." & UVfld & " inner join mmm_mst_user u on convert(nvarchar,u.uid)=d." & Ufld & " where d.eid=" & Session("EID") & " and  d.documenttype='" & Udtype & "'  and d.curstatus in ('ARCHIVE','Allotted','surrender') and m.documenttype='vehicle' and len(d.fld17)=8 and len(d.fld19)=8 and m.fld2<>''and convert(varchar(7), CONVERT(date,d.fld17,3), 126)<='" & ddlYear.SelectedItem.Text & "-" & ddlMonth.SelectedValue.ToString & "' and convert(varchar(7), CONVERT(date,d.fld19,3), 126)>='" & ddlYear.SelectedItem.Text & "-" & ddlMonth.SelectedValue.ToString & "' order by u.username"
                ElseIf Session("USERROLE").ToUpper() = "USER" Then
                    oda.SelectCommand.CommandText = "select  max(m." & vemei & ")[IMIENO], max(u.username)[UserName],max(m." & Vfld & ")[VehicleNo],d." & Ufld & "[uid] from mmm_mst_Doc d inner join mmm_mst_master m on m.tid=d." & UVfld & "  inner join mmm_mst_user u on convert(nvarchar,u.uid)=d." & Ufld & " where d.eid=" & Session("EID") & " and  d.documenttype='" & Udtype & "' and d." & Ufld & "= " & Session("UID").ToString() & "  and m.fld2 is not null group by d." & Ufld & ",username,d.tid having d.tid=( select max(tid) from mmm_mst_doc where eid=32 and documenttype='vrf fixed_pool' and fld11=d.fld11 and curstatus in ('archive','allotted','surrender')) order by username "
                ElseIf Session("USERROLE").ToString.ToUpper() = "VENDOR" Then
                    oda.SelectCommand.CommandText = "select distinct m." & vemei & "[IMIENO], m." & Vfld & "[VehicleNo] from  mmm_mst_user u inner join mmm_mst_master m on m.fld12=convert(nvarchar,u.extid) where m.documenttype='vehicle' and extid=" & Session("EXTID") & " and m." & vemei & " is not null and m." & vemei & "<>''"
                Else
                    oda.SelectCommand.CommandType = CommandType.StoredProcedure
                    ' oda.SelectCommand.CommandText = "uspGetRoleUID"
                    'oda.SelectCommand.CommandText = "uspGetRoleUIDWithSUID"
                    oda.SelectCommand.CommandText = "uspGetRoleUIDWithVRFPeriod"
                    oda.SelectCommand.Parameters.Clear()
                    oda.SelectCommand.Parameters.AddWithValue("uid", Session("UID"))
                    oda.SelectCommand.Parameters.AddWithValue("eid", Session("EID"))
                    If IsNothing(Session("SUBUID")) Then
                        oda.SelectCommand.Parameters.AddWithValue("SUID", Session("UID"))
                    Else
                        oda.SelectCommand.Parameters.AddWithValue("SUID", Session("SUBUID"))
                    End If
                    oda.SelectCommand.Parameters.AddWithValue("role", Session("USERROLE"))
                    oda.SelectCommand.Parameters.AddWithValue("mnth", ddlMonth.SelectedValue.ToString)
                    oda.SelectCommand.Parameters.AddWithValue("year", ddlYear.SelectedItem.Text)
                End If
            Else
                oda.SelectCommand.CommandText = "select fld12[IMEI],fld10[VehicleName],fld1[VehicleNo] from mmm_mst_master where documenttype='vehicle' and eid=" & Session("EID") & " and len(fld12)=15"
            End If
            ds.Clear()
            oda.SelectCommand.CommandTimeout = 180
            oda.Fill(ds, "vemei")
            ddlVehicleRegNo.Items.Clear()
            con.Dispose()
            Dim ii As Integer = 0
            If Session("USERROLE").ToUpper() <> "USER" Then
                ddlVehicleRegNo.Items.Remove("--Select All--")
                ddlVehicleRegNo.Items.Insert(0, "--Select All--")
            End If
            For i As Integer = 0 To ds.Tables("vemei").Rows.Count - 1
                If ds.Tables("vemei").Rows(i).Item(1).ToString().ToString() = "" Then
                Else
                    If Session("USERROLE").ToString.ToUpper() = "USER" Then
                        ddlVehicleRegNo.Items.Add(ds.Tables("vemei").Rows(i).Item(1).ToString() & "/" & ds.Tables("vemei").Rows(i).Item(2).ToString())
                        'ViewState("vehicleNoPdf") = ds.Tables("vemei").Rows(i).Item(2).ToString()
                        ddlVehicleRegNo.Items(i).Value = ds.Tables("vemei").Rows(i).Item(3).ToString()
                        ViewState("vehicleNo") = ViewState("vehicleNo") & ds.Tables("vemei").Rows(i).Item(2).ToString() & ","
                    ElseIf Session("USERROLE").ToString.ToUpper = "VENDOR" Then
                        ddlVehicleRegNo.Items.Add(ds.Tables("vemei").Rows(i).Item(1).ToString())
                        'ViewState("vehicleNoPdf") = ds.Tables("vemei").Rows(i).Item(2).ToString()
                        ddlVehicleRegNo.Items(i + 1).Value = ds.Tables("vemei").Rows(i).Item(0).ToString()
                        ViewState("vehicleNo") = ViewState("vehicleNo") & ds.Tables("vemei").Rows(i).Item(1).ToString() & ","
                    Else
                        If Session("EID") = 32 Then
                            ddlVehicleRegNo.Items.Add(ds.Tables("vemei").Rows(i).Item(1).ToString() & "/" & ds.Tables("vemei").Rows(i).Item(2).ToString())
                            'ViewState("vehicleNoPdf") = ddlVehicleRegNo.SelectedItem.Text()
                            ddlVehicleRegNo.Items(i + 1).Value = ds.Tables("vemei").Rows(i).Item(3).ToString()
                            ViewState("vehicleNo") = ViewState("vehicleNo") & ds.Tables("vemei").Rows(i).Item(2).ToString() & ","
                        Else
                            ddlVehicleRegNo.Items.Add(ds.Tables("vemei").Rows(i).Item(1).ToString() & "/" & ds.Tables("vemei").Rows(i).Item(2).ToString())
                            'ViewState("vehicleNoPdf") = ddlVehicleRegNo.SelectedItem.Text()
                            ddlVehicleRegNo.Items(i + 1).Value = ds.Tables("vemei").Rows(i).Item(0).ToString()
                            ViewState("vehicleNo") = ViewState("vehicleNo") & ds.Tables("vemei").Rows(i).Item(2).ToString() & ","
                        End If
                    End If
                End If
            Next
            If ds.Tables("vemei").Rows.Count > 0 Then
                If ViewState("vehicleNo").ToString.Length > 0 Then
                    ViewState("vehicleNo") = Left(ViewState("vehicleNo"), ViewState("vehicleNo").ToString.Length - 1)
                End If
            End If
        Catch ex As Exception
        Finally
        End Try
    End Sub
    

    Public Sub popupshow()
        VehicleNo.Text = ""
        IMEINO.Text = ""
        lblRecord.Text = ""
        lblMsgEdit.Text = ""
        Date1.Text = ""
        Date2.Text = ""
        ddlshh.SelectedIndex = -1
        ddlsmm.SelectedIndex = -1
        ddlehh.SelectedIndex = -1
        ddlemm.SelectedIndex = -1
    End Sub

    



    Public Sub bindSearchData()
        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim con As SqlConnection = New SqlConnection(conStr)
        If ddlVehicleRegNo.SelectedItem.Text = "--Select All--" Then
            Dim oda As SqlDataAdapter = New SqlDataAdapter("NewUploaderconsolidatedGpsTrip", con)
            oda.SelectCommand.Parameters.Clear()
            oda.SelectCommand.CommandType = CommandType.StoredProcedure
            If ViewState("vehicleNo") = "" Then
                oda.SelectCommand.Parameters.AddWithValue("@uid", 0)
            Else
                oda.SelectCommand.Parameters.AddWithValue("@vehNo", ViewState("vehicleNo"))
            End If
            oda.SelectCommand.Parameters.AddWithValue("@EID", Session("EID").ToString())
            'oda.SelectCommand.Parameters.AddWithValue("@sdate", ddlYear.SelectedItem.Text & "-" & ddlMonth.SelectedValue & "-01 00:00")
            oda.SelectCommand.Parameters.AddWithValue("@month", ddlMonth.SelectedValue.ToString)
            oda.SelectCommand.Parameters.AddWithValue("@year", ddlYear.SelectedItem.Text)
            Dim ds As New DataSet()
            oda.Fill(ds, "data")
            Dim usrqry As String = String.Empty
            grdTripData.DataSource = ds.Tables("data")
            ViewState("Data") = ds.Tables("data")
            ViewState("DataPdf") = ds
            ViewState("Pdf") = ds.Tables("data")
            grdTripData.DataBind()
            grdTripData.Caption = ""
            con.Close()
            oda.Dispose()
        Else
            Dim str As String() = Split(ddlVehicleRegNo.SelectedItem.Text, "/")
            If Session("USERROLE").ToString.ToUpper = "VENDOR" Then
                ViewState("vehicleNoPdf") = str(0)
            Else
                ViewState("vehicleNoPdf") = str(1)
            End If
            Dim oda As SqlDataAdapter = New SqlDataAdapter("NewUploaderuspGetTripBetweenTest20", con)
            oda.SelectCommand.Parameters.Clear()
            oda.SelectCommand.CommandType = CommandType.StoredProcedure
            oda.SelectCommand.Parameters.Clear()
            If Session("USERROLE").ToString.ToUpper = "VENDOR" Then
                oda.SelectCommand.Parameters.AddWithValue("IMEI", ddlVehicleRegNo.SelectedValue)
            Else
                oda.SelectCommand.Parameters.AddWithValue("uid", ddlVehicleRegNo.SelectedValue)
            End If
            oda.SelectCommand.Parameters.AddWithValue("EID", Session("EID").ToString())
            'oda.SelectCommand.Parameters.AddWithValue("sdate", ddlYear.SelectedItem.Text & "-" & ddlMonth.SelectedValue & "-01 00:00")
            oda.SelectCommand.Parameters.AddWithValue("@month", ddlMonth.SelectedValue.ToString)
            oda.SelectCommand.Parameters.AddWithValue("@year", ddlYear.SelectedItem.Text)
            Dim ds As New DataSet()
            oda.Fill(ds, "data")
            Dim usrqry As String = String.Empty
            grdTripData.DataSource = ds.Tables("data")
            ViewState("Data") = ds
            ViewState("Pdf") = ds.Tables("data")
            grdTripData.DataBind()
            bindRptHeader()
            If ViewState("Dname") = "" Then
                ViewState("Dname") = "N/A"
            End If
            If ViewState("Dnum") = "" Then
                ViewState("Dnum") = "N/A"
            End If
            If ViewState("Vtype") = "" Then
                ViewState("Vtype") = "N/A"
            End If
            If ddlVehicleRegNo.SelectedItem.Text = "--Select All--" Then
                grdTripData.Caption = ""
            Else
                grdTripData.Caption = "Driver Name :" & ViewState("Dname") & "    Contact No : " & ViewState("Dnum") & "   Vehicle Type : " & ViewState("Vtype") & " Vehicle No :" & ViewState("vehicleNoPdf") & "  Log Month : " & ddlMonth.SelectedItem.Text & "-" & ddlYear.SelectedItem.Text
            End If
            If ddlVehicleRegNo.SelectedItem.Text = "" Then
                grdTripData.Caption = ""
                grdTripData.Caption = "Please Enter Valid Folder"

            Else
                grdTripData.Caption = "Driver Name :" & ViewState("Dname") & "    Contact No : " & ViewState("Dnum") & "   Vehicle Type : " & ViewState("Vtype") & " Vehicle No :" & ViewState("vehicleNoPdf") & "  Log Month : " & ddlMonth.SelectedItem.Text & "-" & ddlYear.SelectedItem.Text
            End If
            con.Close()
            oda.Dispose()
        End If
        ' Dim oda As SqlDataAdapter = New SqlDataAdapter("uspGetTripBetween", con)
    End Sub
    Protected Sub btsSearch_Click(sender As Object, e As System.EventArgs) Handles btsSearch.Click
        bindSearchData()
    End Sub


    Protected Sub EditHit(ByVal sender As Object, ByVal e As System.EventArgs)

        popupshow()
        VehicleNo.Enabled = False
        IMEINO.Enabled = False
        lblRecord.Text = ""
        lblMsgEdit.Text = ""
        Dim btnDetails As ImageButton = TryCast(sender, ImageButton)
        Dim row As GridViewRow = DirectCast(btnDetails.NamingContainer, GridViewRow)
        Dim tid As Integer = Convert.ToString(Me.grdTripData.DataKeys(row.RowIndex).Value)
        btnActUserSave.Text = "Update"
        ViewState("pid") = tid
        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim con As SqlConnection = New SqlConnection(conStr)
        Dim oda As New SqlDataAdapter("", con)
        Try
            If Session("USERROLE").ToString.ToUpper = "SU" Then
                oda.SelectCommand.CommandText = "select tid,vehicle_no,IMEI_NO,Trip_Start_DateTime,Trip_End_DateTime from mmm_mst_newelogbook WHERE tid=" & tid & ""
                oda.SelectCommand.CommandType = CommandType.Text
                Dim dt As New DataTable
                oda.Fill(dt)
                Dim datetime1 As String
                Dim enddatetime As String
                datetime1 = Format(dt.Rows(0).Item("Trip_Start_DateTime"), "yyyy-MM-dd HH:mm")
                enddatetime = Format(dt.Rows(0).Item("Trip_End_DateTime"), "yyyy-MM-dd HH:mm")
                ViewState("StartTime") = datetime1
                ViewState("EndTime") = enddatetime
                Date1.Text = datetime1.Substring(0, 10)
                ddlshh.SelectedIndex = ddlshh.Items.IndexOf(ddlshh.Items.FindByText(Format(dt.Rows(0).Item("Trip_Start_DateTime"), "HH")))
                ddlsmm.SelectedIndex = ddlsmm.Items.IndexOf(ddlsmm.Items.FindByText(Format(dt.Rows(0).Item("Trip_Start_DateTime"), "mm")))
                Dim datettime2 As String = Format(dt.Rows(0).Item("Trip_end_DateTime"), "yyyy-MM-dd HH:mm")
                Date2.Text = datettime2.Substring(0, 10)
                ddlehh.SelectedIndex = ddlehh.Items.IndexOf(ddlehh.Items.FindByText(Format(dt.Rows(0).Item("Trip_End_DateTime"), "HH")))
                ddlemm.SelectedIndex = ddlemm.Items.IndexOf(ddlemm.Items.FindByText(Format(dt.Rows(0).Item("Trip_End_DateTime"), "mm")))
                IMEINO.Text = dt.Rows(0).Item("IMEI_NO").ToString()
                VehicleNo.Text = dt.Rows(0).Item("Vehicle_No")
                updatePanelEdit.Update()
                Me.btnEdit_ModalPopupExtender.Show()
            Else
                lblRecord.Text = "Only Super User can edit/delete trip"
                Exit Sub
            End If
        Catch ex As Exception
        Finally
            con.Close()
            con.Dispose()
            oda.Dispose()
        End Try

    End Sub

    Protected Sub btnActUserSave_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnActUserSave.Click
        Dim dtst As String
        Dim dtet As String
        Dim crrdate As String = Date.Now.ToString("yyyy-MM-dd HH:mm")
        dtst = Date1.Text & " " & ddlshh.SelectedItem.Text & ":" & ddlsmm.SelectedItem.Text
        dtet = Date2.Text & " " & ddlehh.SelectedItem.Text & ":" & ddlemm.SelectedItem.Text
        If Date1.Text = "" Then
            lblMsgEdit.Text = "Please Select Start Date"
            Exit Sub
        ElseIf Date2.Text = "" Then
            lblMsgEdit.Text = "Please Select End Date"
            Exit Sub
        Else
            If CDate(Date1.Text) > CDate(Date2.Text) Then
                lblMsgEdit.Text = "Start Date should be less than from End Date "
                Exit Sub
            End If
            If CDate(Date1.Text) = CDate(Date2.Text) Then
                If ddlshh.SelectedItem.Text & ":" & ddlsmm.SelectedItem.Text > ddlehh.SelectedItem.Text & ":" & ddlemm.SelectedItem.Text Then
                    lblMsgEdit.Text = "Start Time should be less than from End Time "
                    Exit Sub
                End If
            End If
        End If
        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim con As SqlConnection = New SqlConnection(conStr)
        Dim oda As SqlDataAdapter = New SqlDataAdapter("", con)
        If con.State <> ConnectionState.Open Then
            con.Open()
        End If
        Try
            If btnActUserSave.Text = "Update" Then
                Dim startloc As String = ""
                Dim endloc As String = ""
                Dim devdist As String = ""
                Dim gpsdata As GPSClass = New GPSClass()
                oda.SelectCommand.CommandText = "select top 1 lattitude,longitude,ctime,speed,distancetravel from MMM_MST_GPSDATA with(nolock) where tid in (select max(tid) from MMM_MST_GPSDATA where IMIENO='" & IMEINO.Text & "'  and   DATEADD(minute,DATEDIFF(minute,0,ctime),0)  >= '" + dtst + "' group by lattitude,longitude) order by DATEADD(minute,DATEDIFF(minute,0,ctime),0) "
                Dim dtlatlong As New DataSet
                oda.SelectCommand.CommandTimeout = 300
                oda.Fill(dtlatlong, "StartLoc")
                If dtlatlong.Tables("StartLoc").Rows.Count > 0 Then
                    startloc = gpsdata.Location(dtlatlong.Tables("StartLoc").Rows(0).ItemArray(0).ToString(), dtlatlong.Tables("StartLoc").Rows(0).ItemArray(1).ToString())
                End If

                oda.SelectCommand.CommandText = "select top 1 lattitude,longitude,ctime,speed,distancetravel from MMM_MST_GPSDATA with(nolock) where tid in (select max(tid) from MMM_MST_GPSDATA where IMIENO='" & IMEINO.Text & "'  and   DATEADD(minute,DATEDIFF(minute,0,ctime),0)  >= '" + dtet + "' group by lattitude,longitude) order by DATEADD(minute,DATEDIFF(minute,0,ctime),0) "
                oda.SelectCommand.CommandTimeout = 300
                oda.Fill(dtlatlong, "EndLoc")

                If dtlatlong.Tables("StartLoc").Rows.Count > 0 Then
                    endloc = gpsdata.Location(dtlatlong.Tables("EndLoc").Rows(0).ItemArray(0).ToString(), dtlatlong.Tables("EndLoc").Rows(0).ItemArray(1).ToString())
                End If
                If CDate(Date1.Text.ToString) > CDate("2014-03-31".ToString) Then
                    oda.SelectCommand.CommandText = "select isnull(sum(devdist),0)[TotalDist] from mmm_mst_gpsdata with (nolock) where imieno='356307042785393' and CTime >= '" + dtst + "' AND CTime <= '" + dtet + "' and ((devdist>0.03 and speed=0) or (devdist>0 and speed>0 )) group by imieno"
                Else
                    oda.SelectCommand.CommandText = "select sum(DevDist)[TotalDist]  from MMM_MST_GPSDATA with (nolock) where devdist <> 0 and   IMIENO='" & IMEINO.Text & "'  and   convert(nvarchar(16),cTime,121) >= convert(nvarchar,'" + dtst + "',120) AND  convert(nvarchar(16),cTime,121) <= convert(nvarchar,'" + dtet + "',121)"
                End If
                oda.SelectCommand.CommandTimeout = 300
                oda.Fill(dtlatlong, "Devdist")
                If dtlatlong.Tables("Devdist").Rows.Count > 0 Then
                    devdist = dtlatlong.Tables("Devdist").Rows(0).Item("TotalDist").ToString()
                End If

                Dim cmd As New SqlCommand("UspUpdateElogUploadReportTestT20", con)
                cmd.CommandType = CommandType.StoredProcedure
                cmd.Parameters.Clear()
                cmd.Parameters.AddWithValue("eid", Session("EID").ToString())
                cmd.Parameters.AddWithValue("TripStartDateTime", dtst)
                cmd.Parameters.AddWithValue("TripEndDateTime", dtet)
                cmd.Parameters.AddWithValue("TripStartLocation", startloc)
                cmd.Parameters.AddWithValue("TripEndLocation", endloc)
                cmd.Parameters.AddWithValue("TotalDistance", devdist)
                cmd.Parameters.AddWithValue("StatrLat", dtlatlong.Tables("StartLoc").Rows(0).ItemArray(0).ToString())
                cmd.Parameters.AddWithValue("StartLong", dtlatlong.Tables("StartLoc").Rows(0).ItemArray(1).ToString())
                cmd.Parameters.AddWithValue("EndLat", (dtlatlong.Tables("EndLoc").Rows(0).ItemArray(0).ToString()))
                cmd.Parameters.AddWithValue("EndLong", (dtlatlong.Tables("EndLoc").Rows(0).ItemArray(1).ToString()))
                cmd.Parameters.AddWithValue("pid", ViewState("pid"))
                cmd.ExecuteNonQuery()
                lblRecord.Text = "Trip Updated Successfully"
                bindSearchData()
            End If
        Catch ex As Exception
        Finally
            con.Close()
            oda.Dispose()
            con.Dispose()
        End Try
        btnEdit_ModalPopupExtender.Hide()
        updatePanelEdit.Update()
        grdTripData.DataBind()
    End Sub

    Protected Sub DeleteHit(ByVal sender As Object, ByVal e As System.EventArgs)
        lblRecord.Visible = False
        Dim btnDetails As ImageButton = TryCast(sender, ImageButton)
        Dim row As GridViewRow = DirectCast(btnDetails.NamingContainer, GridViewRow)
        Dim pid As Integer = Convert.ToString(Me.grdTripData.DataKeys(row.RowIndex).Value)
        ViewState("Did") = pid
        
        Try
            If Session("USERROLE").ToString.ToUpper = "SU" Then
                lblMsgDelete.Text = "Are you Sure Want to delete this trip? "
                updatePanelDelete.Update()
                Me.btnDelete_ModalPopupExtender.Show()
            Else
                lblRecord.Text = "Only  Super User can edit/delete trip"
                lblRecord.Visible = True
            End If

        Catch ex As Exception
        Finally
           
        End Try
      
    End Sub

    Protected Sub DeleteRecord(ByVal sender As Object, ByVal e As System.EventArgs)
        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim con As SqlConnection = New SqlConnection(conStr)
        Try
            Dim oda As SqlDataAdapter = New SqlDataAdapter("UspDeleteElogUploadReport", con)
            oda.SelectCommand.CommandType = CommandType.StoredProcedure
            oda.SelectCommand.Parameters.AddWithValue("eid", Session("EID").ToString())
            oda.SelectCommand.Parameters.AddWithValue("tid", Val(ViewState("Did").ToString))
            If con.State <> ConnectionState.Open Then
                con.Open()
            End If
            oda.SelectCommand.ExecuteNonQuery()
            Dim ds As New DataSet()
            lblRecord.Text = "Trip has been deleted successfully"
            Me.btnDelete_ModalPopupExtender.Hide()
            bindSearchData()
        Catch ex As Exception
        Finally
            con.Close()
            con.Dispose()
        End Try
    End Sub

    Protected currentPageNumber As Integer = 1
    Protected currentPageNumberu As Integer = 1
    Protected currentPageNumberp As Integer = 1
    Protected Sub grdTripData_PageIndexChanging(sender As Object, e As System.Web.UI.WebControls.GridViewPageEventArgs) Handles grdTripData.PageIndexChanging
        Try
            grdTripData.PageIndex = e.NewPageIndex
            currentPageNumberp = e.NewPageIndex + 1
            'grdTripData.DataBind()
            bindSearchData()
            'updPnlSearch.Update()
        Catch ex As Exception
        End Try
    End Sub
    Protected Sub OnMap(ByVal sender As Object, ByVal e As System.EventArgs)
        Dim OmMap As ImageButton = TryCast(sender, ImageButton)
        Dim row As GridViewRow = DirectCast(OmMap.NamingContainer, GridViewRow)
        Response.Redirect("ShowMap1.aspx?IMIE=" & row.Cells(3).Text & "&Start=" + row.Cells(4).Text + "&End=" + row.Cells(5).Text)
    End Sub
    Protected Sub grdTripData_RowDataBound(sender As Object, e As System.Web.UI.WebControls.GridViewRowEventArgs) Handles grdTripData.RowDataBound
        grdTripData.Columns(1).Visible = False
        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim con As SqlConnection = New SqlConnection(conStr)
        Dim oda As New SqlDataAdapter("", con)
        Dim ds As New DataSet
        Try
            oda.SelectCommand.CommandText = "select roles from mmm_mst_menu with(nolock) where eid=' " & Session("EID") & "' and pagelink='ReportELogbookupload.aspx'"
            oda.Fill(ds, "Roles")
            If ds.Tables("Roles").Columns.Count > 0 Then
                Dim userrole As String = ds.Tables("Roles").Rows(0).Item("roles").ToString
                userrole = userrole.Replace("{", "")
                userrole = userrole.Replace("}", "")
                Dim role As String() = userrole.Split(",")
                Dim ur As String()
                For i As Integer = 0 To role.Count - 1
                    ur = role(i).Split(":")
                    If ur(0) = Session("USERROLE") And (ur(1) = "4" Or ur(1) = "5" Or ur(1) = "6" Or ur(1) = "7" Or ur(1) = "12" Or ur(1) = "13" Or ur(1) = "14" Or ur(1) = "15") Then
                        grdTripData.Columns(1).Visible = True
                    Else
                        Continue For
                    End If
                Next
            End If
        Catch ex As Exception
        Finally
            con.Close()
            con.Dispose()
            oda.Dispose()
        End Try
        


        'If Session("USERROLE").ToString.ToUpper <> "SU" Then
        '    grdTripData.Columns(1).Visible = False

        'End If
        'e.Row.Cells(0).Attributes.Add("class", "text")
        'If e.Row.RowType = DataControlRowType.Header Then
        '    Dim HeaderGrid As GridView = DirectCast(sender, GridView)
        '    Dim HeaderGridRow As New GridViewRow(0, 0, DataControlRowType.Header, DataControlRowState.Insert)
        '    Dim HeaderCell As New TableCell()
        '    If ddlVehicleRegNo.SelectedItem.Text = "--Select All--" Then
        '        HeaderCell.Text = ""
        '        HeaderCell.ColumnSpan = 6
        '        HeaderCell.HorizontalAlign = HorizontalAlign.Center
        '        HeaderGridRow.Cells.Add(HeaderCell)
        '    Else
        '        HeaderCell.Text = ""
        '        HeaderCell.ColumnSpan = 5
        '        HeaderCell.HorizontalAlign = HorizontalAlign.Center
        '        HeaderGridRow.Cells.Add(HeaderCell)
        '    End If
        '    HeaderCell = New TableCell()
        '    HeaderCell.Text = "Location "
        '    HeaderCell.ColumnSpan = 2
        '    HeaderCell.HorizontalAlign = HorizontalAlign.Center
        '    HeaderGridRow.Cells.Add(HeaderCell)
        '    HeaderCell = New TableCell()
        '    HeaderCell.Text = "Meter Reading "
        '    HeaderCell.ColumnSpan = 2
        '    HeaderCell.HorizontalAlign = HorizontalAlign.Center
        '    HeaderGridRow.Cells.Add(HeaderCell)
        '    HeaderCell = New TableCell()
        '    HeaderCell.Text = "Total"
        '    HeaderCell.ColumnSpan = 1
        '    HeaderCell.HorizontalAlign = HorizontalAlign.Center
        '    HeaderGridRow.Cells.Add(HeaderCell)
        '    HeaderCell = New TableCell()
        '    HeaderCell.Text = "Date & Time"
        '    HeaderCell.ColumnSpan = 2
        '    HeaderCell.HorizontalAlign = HorizontalAlign.Center
        '    HeaderGridRow.Cells.Add(HeaderCell)
        '    HeaderCell = New TableCell()
        '    HeaderCell.Text = "Total"
        '    HeaderCell.ColumnSpan = 1
        '    HeaderCell.HorizontalAlign = HorizontalAlign.Center
        '    HeaderGridRow.Cells.Add(HeaderCell)
        '    grdTripData.Controls(0).Controls.AddAt(0, HeaderGridRow)
        '    'bindSearchData()
        '    'grdTripData.DataBind()
        '  End If
    End Sub
    Public Overloads Overrides Sub VerifyRenderingInServerForm(ByVal control As Control)
    End Sub
    Protected Sub btnExcelExport_Click(sender As Object, e As System.Web.UI.ImageClickEventArgs) Handles btnExcelExport.Click
        grdTripData.Columns(0).HeaderText = ""
        grdTripData.Rows(0).Cells(4).Visible = False
        grdTripData.Rows(0).Cells(1).Text = ""
        grdTripData.ShowHeader = True
        grdTripData.RowHeaderColumn = True
        grdTripData.AllowPaging = False
        grdTripData.DataSource = ViewState("Data")
        grdTripData.DataBind()
        Response.Clear()
        Response.Buffer = True
        Response.Write("<br/><div align=""center"" style=""border:1px solid red"" ><h3>VEHICLE LOG BOOK (Electronic)</h3></div> <br/>")
        Response.AddHeader("content-disposition", "attachment;filename=Trip Report.xls")
        Response.Charset = ""
        Response.ContentType = "application/vnd.ms-excel"
        Dim sw As New StringWriter()
        Dim hw As New HtmlTextWriter(sw)
        For i As Integer = 0 To grdTripData.Rows.Count - 1
            'Apply text style to each Row 

            grdTripData.Rows(i).Attributes.Add("class", "textmode")
        Next
        grdTripData.RenderControl(hw)
        'style to format numbers to string 
        Dim style As String = "<style> .textmode{mso-number-format:\@;}</style>"
        Response.Write(style)
        Response.Output.Write(sw.ToString())
        Response.Flush()
        Response.End()
    End Sub
    Public Sub bindRptHeader()
        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim con As SqlConnection = New SqlConnection(conStr)
        Dim oda As SqlDataAdapter = New SqlDataAdapter("", con)
        oda.SelectCommand.CommandText = "select dms.udf_split('MASTER-Vehicle TYPE-fld10',M.fld11)[Vehicle Type],M.fld16[Driver Name],M.fld17[Driver Mobile Number],M.fld2[IMEI],M.fld10[Registration No] from MMM_MST_MASTER M LEFT OUTER JOIN MMM_MST_NewELOGBOOK E on M.fld10=E.vehicle_no where M.fld10='" & ViewState("vehicleNoPdf") & "' and m.eid=" & Session("eid") & " "
        Dim ds As New DataSet()
        If con.State <> ConnectionState.Open Then
            con.Open()
        End If
        oda.Fill(ds, "data")
        If ds.Tables("data").Rows.Count > 0 Then
            If ddlVehicleRegNo.SelectedItem.Text = "--Select All--" Then

                ViewState("Dnum") = ""
                ViewState("Dname") = ""
                ViewState("Vtype") = ""
            Else
                ViewState("Dnum") = ds.Tables("data").Rows(0).Item("Driver Mobile Number").ToString()
                ViewState("Dname") = ds.Tables("data").Rows(0).Item("Driver Name").ToString()
                ViewState("Vtype") = ds.Tables("data").Rows(0).Item("Vehicle Type").ToString()
            End If
        Else
            Exit Sub
        End If
        con.Close()
        oda.Dispose()
    End Sub
    Protected Sub ToPdf(ByVal newDataSet As DataSet)
        Dim PDFData As New System.IO.MemoryStream()
        Dim newDocument As New iTextSharp.text.Document(PageSize.A4.Rotate(), 10, 10, 10, 10)
        Dim newPdfWriter As iTextSharp.text.pdf.PdfWriter = iTextSharp.text.pdf.PdfWriter.GetInstance(newDocument, PDFData)
        newDocument.Open()
        '  newDataSet.Tables("data").Remove("tid")
        newDataSet.Tables("data").Columns.RemoveAt(0)
        newDataSet.Tables("data").Columns.RemoveAt(0)
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
            newDocument.Add(New Phrase("VEHICLE LOG BOOK (Electronic) ", FontFactory.GetFont("Tahoma", 15, iTextSharp.text.Font.BOLD, New iTextSharp.text.BaseColor(0, 80, 0))))
            newDocument.Add(New Phrase(Environment.NewLine))
            newDocument.Add(New Phrase(Environment.NewLine))
            If ddlVehicleRegNo.SelectedItem.Text = "--Select All--" Then
            Else
                newDocument.Add(New Phrase("Driver Name :" & ViewState("Dname") & "    Contact No : " & ViewState("Dnum") & "   Vehicle Type : " & ViewState("Vtype") & "   Vehicle No :" & ViewState("vehicleNoPdf") & "  Log for Month of : " & ddlMonth.SelectedItem.Text & "-" & ddlYear.SelectedItem.Text, FontFactory.GetFont("Tahoma", 12, iTextSharp.text.Font.BOLD, New iTextSharp.text.BaseColor(80, 0, 0))))
            End If
            newDocument.Add(New Phrase(Environment.NewLine))
            newDocument.Add(newPdfTable)
            newDocument.Add(New Phrase(Environment.NewLine))
            newDocument.Add(New Phrase(Environment.NewLine))
            newDocument.Add(New Phrase("Created By: " & Session("USERNAME"), FontFactory.GetFont("Tahoma", 8, iTextSharp.text.Font.BOLD, New iTextSharp.text.BaseColor(80, 80, 80))))
            newDocument.Add(New Phrase(Environment.NewLine))
            newDocument.Add(New Phrase("Printed Date: " & DateTime.Now.ToString(), FontFactory.GetFont("Tahoma", 8, iTextSharp.text.Font.BOLD, New iTextSharp.text.BaseColor(80, 80, 80))))
            newDocument.Add(New Phrase(Environment.NewLine))
            newDocument.Add(New Phrase(Environment.NewLine))
            '  newDocument.Add(New Phrase("Company Address: " & ViewState("pagefooter"), FontFactory.GetFont("Tahoma", 10, iTextSharp.text.Font.BOLD, New iTextSharp.text.BaseColor(21, 66, 157))))
            If Page < newDataSet.Tables.Count Then
                newDocument.NewPage()
            End If
        Next
        newDocument.Close()
        Response.ContentType = "application/pdf"
        Response.Cache.SetCacheability(System.Web.HttpCacheability.[Public])
        Response.AppendHeader("Content-Type", "application/pdf")
        Response.AppendHeader("Content-Disposition", "attachment; filename=VEHICLE LOG BOOK (Electronic)")
        Response.OutputStream.Write(PDFData.GetBuffer(), 0, PDFData.GetBuffer().Length)
        Response.OutputStream.Flush()
        Response.OutputStream.Close()
    End Sub
    Protected Sub btnPdfExport_Click(sender As Object, e As System.Web.UI.ImageClickEventArgs) Handles btnPdfExport.Click
        If ViewState("Pdf").Rows.Count > 0 Then
            grdTripData.DataBind()
            If ddlVehicleRegNo.SelectedItem.Text = "--Select All--" Then
                ToPdf(ViewState("DataPdf"))
            Else
                ToPdf(ViewState("Data"))
            End If
        Else
        End If
    End Sub
    Protected Sub ddlYear_SelectedIndexChanged(sender As Object, e As System.EventArgs) Handles ddlYear.SelectedIndexChanged
        BindRegsNumberDDlFilter()
    End Sub

    Protected Sub ddlMonth_SelectedIndexChanged(sender As Object, e As System.EventArgs) Handles ddlMonth.SelectedIndexChanged

        'ddlYear.SelectedIndex = 0
        BindRegsNumberDDlFilter()
    End Sub
End Class
