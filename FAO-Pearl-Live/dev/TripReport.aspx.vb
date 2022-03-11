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

Partial Class TripReport
    Inherits System.Web.UI.Page
    Protected Sub Page_Load(sender As Object, e As System.EventArgs) Handles Me.Load
        If Not IsPostBack Then
            '   BindRegsNumberDDl()
            BindRegsNumberDDlFilter()
        End If
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
                    oda.SelectCommand.CommandText = "set dateformat dmy select  m." & vemei & "[IMIENO], u.username[UserName],m." & Vfld & "[VehicleNo],d." & Ufld & "[uid] from mmm_mst_Doc d inner join mmm_mst_master m on convert(nvarchar,m.tid)=d." & UVfld & " inner join mmm_mst_user u on u.uid=d." & Ufld & " where d.eid=" & Session("EID") & " and  d.documenttype='" & Udtype & "'  and d.curstatus in ('ARCHIVE','Allotted','surrender') and m.documenttype='vehicle' and len(d.fld17)=8 and len(d.fld19)=8 and m.fld2<>''and convert(varchar(7), CONVERT(date,d.fld17,3), 126)<='" & ddlYear.SelectedItem.Text & "-" & ddlMonth.SelectedValue.ToString & "' and convert(varchar(7), CONVERT(date,d.fld19,3), 126)>='" & ddlYear.SelectedItem.Text & "-" & ddlMonth.SelectedValue.ToString & "' order by u.username"
                ElseIf Session("USERROLE").ToUpper() = "USER" Then
                    oda.SelectCommand.CommandText = "select  max(m." & vemei & ")[IMIENO], max(u.username)[UserName],max(m." & Vfld & ")[VehicleNo],d." & Ufld & "[uid] from mmm_mst_Doc d inner join mmm_mst_master m on m.tid=d." & UVfld & "  inner join mmm_mst_user u on u.uid=d." & Ufld & " where d.eid=" & Session("EID") & " and  d.documenttype='" & Udtype & "' and d." & Ufld & "= " & Session("UID").ToString() & "  and m.fld2 is not null group by d." & Ufld & ",username,d.tid having d.tid=( select max(tid) from mmm_mst_doc where eid=32 and documenttype='vrf fixed_pool' and fld11=d.fld11 and curstatus in ('archive','allotted','surrender')) order by username "
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
    Public Sub bindSearchData()
        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim con As SqlConnection = New SqlConnection(conStr)
        Dim oda As SqlDataAdapter = New SqlDataAdapter("", con)
        Try
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
            If ddlVehicleRegNo.SelectedItem.Text = "--Select All--" Then
                If Session("EID") = 32 Then
                    oda.SelectCommand.CommandText = "NewconsolidatedGpsTrip"
                Else
                    oda.SelectCommand.CommandText = "NewconsolidatedGpsTripforIndus"
                End If
                oda.SelectCommand.Parameters.Clear()
                oda.SelectCommand.CommandType = CommandType.StoredProcedure
                If ViewState("vehicleNo") = "" Then
                    oda.SelectCommand.Parameters.AddWithValue("@uid", 0)
                Else
                    oda.SelectCommand.Parameters.AddWithValue("@vehNo", ViewState("vehicleNo"))
                End If
                oda.SelectCommand.Parameters.AddWithValue("@EID", Session("EID").ToString())
                oda.SelectCommand.Parameters.AddWithValue("@sdate", ddlYear.SelectedItem.Text & "-" & ddlMonth.SelectedValue & "-01 00:00")
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

                If Session("EID") = 32 Then
                    oda.SelectCommand.CommandText = "NewuspGetTripBetween"
                Else
                    oda.SelectCommand.CommandText = "NewuspGetTripVehforIndus"
                End If

                oda.SelectCommand.Parameters.Clear()
                oda.SelectCommand.CommandType = CommandType.StoredProcedure
                oda.SelectCommand.Parameters.Clear()
                If Session("USERROLE").ToString.ToUpper = "VENDOR" Or Session("EID") <> 32 Then
                    oda.SelectCommand.Parameters.AddWithValue("IMEI", ddlVehicleRegNo.SelectedValue)
                Else
                    oda.SelectCommand.Parameters.AddWithValue("uid", ddlVehicleRegNo.SelectedValue)
                End If
                oda.SelectCommand.Parameters.AddWithValue("EID", Session("EID").ToString())
                oda.SelectCommand.Parameters.AddWithValue("sdate", ddlYear.SelectedItem.Text & "-" & ddlMonth.SelectedValue & "-01 00:00")

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
                    If Session("EID") = 32 Then
                        grdTripData.Caption = "Driver Name :" & ViewState("Dname") & "    Contact No : " & ViewState("Dnum") & "   Vehicle Type : " & ViewState("Vtype") & " Vehicle No :" & ViewState("vehicleNoPdf") & "  Log Month : " & ddlMonth.SelectedItem.Text & "-" & ddlYear.SelectedItem.Text
                    End If
                End If
                con.Close()
                oda.Dispose()
            End If
            ' Dim oda As SqlDataAdapter = New SqlDataAdapter("uspGetTripBetween", con)
        Catch ex As Exception
        Finally
            con.Close()
            oda.Dispose()
        End Try
    End Sub
    Protected Sub btsSearch_Click(sender As Object, e As System.EventArgs) Handles btsSearch.Click
        Try
            bindSearchData()
        Catch ex As Exception
        Finally
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
        Try

            Dim OmMap As ImageButton = TryCast(sender, ImageButton)
            Dim row As GridViewRow = DirectCast(OmMap.NamingContainer, GridViewRow)
            Response.Redirect("ShowMap.aspx?IMIE=" & row.Cells(3).Text & "&Start=" + row.Cells(4).Text + "&End=" + row.Cells(5).Text)
        Catch ex As Exception
        Finally

        End Try
    End Sub
    Protected Sub grdTripData_RowDataBound(sender As Object, e As System.Web.UI.WebControls.GridViewRowEventArgs) Handles grdTripData.RowDataBound
        Try

            If e.Row.RowType = DataControlRowType.DataRow Or e.Row.RowType = DataControlRowType.Header Then
                e.Row.Cells(2).Visible = False
                e.Row.Cells(1).Visible = False
            End If

            If e.Row.RowType = DataControlRowType.DataRow Then
                Dim tid As String = grdTripData.DataKeys(e.Row.RowIndex).Value.ToString()
                Dim trip As String = DataBinder.Eval(e.Row.DataItem, "TripType").ToString
                e.Row.Cells(0).Text = "<a class='detail' style='text-decoration:none;'  href='#' onclick=""OpenWindow('" & uri & "?tid=" & tid & "&type=" & trip & "&flag=1');""><img alt='Show On Map' src='images/earth_search.png'  height='16px' width='16px'/></a>"
                e.Row.Cells(0).HorizontalAlign = HorizontalAlign.Center
            End If

            e.Row.Cells(0).Attributes.Add("class", "text")
            If e.Row.RowType = DataControlRowType.Header Then
                Dim HeaderGrid As GridView = DirectCast(sender, GridView)
                Dim HeaderGridRow As New GridViewRow(0, 0, DataControlRowType.Header, DataControlRowState.Insert)
                Dim HeaderCell As New TableCell()
                If ddlVehicleRegNo.SelectedItem.Text = "--Select All--" Then
                    HeaderCell.Text = ""
                    If Session("EID") = 32 Then
                        HeaderCell.ColumnSpan = 5
                    Else
                        HeaderCell.ColumnSpan = 4
                    End If
                    HeaderCell.HorizontalAlign = HorizontalAlign.Center
                    HeaderGridRow.Cells.Add(HeaderCell)
                Else
                    HeaderCell.Text = ""
                    HeaderCell.ColumnSpan = 4
                    HeaderCell.HorizontalAlign = HorizontalAlign.Center
                    HeaderGridRow.Cells.Add(HeaderCell)
                End If
                HeaderCell = New TableCell()
                HeaderCell.Text = "Location "
                HeaderCell.ColumnSpan = 2
                HeaderCell.HorizontalAlign = HorizontalAlign.Center
                HeaderGridRow.Cells.Add(HeaderCell)
                HeaderCell = New TableCell()
                HeaderCell.Text = "Meter Reading "
                HeaderCell.ColumnSpan = 2
                HeaderCell.HorizontalAlign = HorizontalAlign.Center
                HeaderGridRow.Cells.Add(HeaderCell)
                HeaderCell = New TableCell()
                HeaderCell.Text = "Total"
                HeaderCell.ColumnSpan = 1
                HeaderCell.HorizontalAlign = HorizontalAlign.Center
                HeaderGridRow.Cells.Add(HeaderCell)
                HeaderCell = New TableCell()
                HeaderCell.Text = "Date & Time"
                HeaderCell.ColumnSpan = 2
                HeaderCell.HorizontalAlign = HorizontalAlign.Center
                HeaderGridRow.Cells.Add(HeaderCell)
                HeaderCell = New TableCell()
                HeaderCell.Text = "Total"
                HeaderCell.ColumnSpan = 1
                HeaderCell.HorizontalAlign = HorizontalAlign.Center
                HeaderGridRow.Cells.Add(HeaderCell)

                grdTripData.Controls(0).Controls.AddAt(0, HeaderGridRow)
                'bindSearchData()
                'grdTripData.DataBind()
            End If
        Catch ex As Exception
        Finally

        End Try
    End Sub
    Public Overloads Overrides Sub VerifyRenderingInServerForm(ByVal control As Control)
    End Sub
    Protected Sub btnExcelExport_Click(sender As Object, e As System.Web.UI.ImageClickEventArgs) Handles btnExcelExport.Click
        Try
            grdTripData.Columns(0).HeaderText = ""
            grdTripData.Rows(0).Cells(3).Visible = False
            grdTripData.Rows(0).Cells(0).Text = ""
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
        Catch ex As Exception
        Finally

        End Try
    End Sub
    Public Sub bindRptHeader()
        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim con As SqlConnection = New SqlConnection(conStr)
        Dim oda As SqlDataAdapter = New SqlDataAdapter("", con)
        Try
            oda.SelectCommand.CommandText = "select dms.udf_split('MASTER-Vehicle TYPE-fld10',M.fld11)[Vehicle Type],M.fld16[Driver Name],M.fld17[Driver Mobile Number],M.fld2[IMEI],M.fld10[Registration No] from MMM_MST_MASTER M LEFT OUTER JOIN MMM_MST_ELOGBOOK E on M.fld10=E.vehicle_no where M.fld10='" & ViewState("vehicleNoPdf") & "' "
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

        Catch ex As Exception
        Finally
            con.Close()
            oda.Dispose()
        End Try
    End Sub
    Protected Sub ToPdf(ByVal newDataSet As DataSet)
        Try


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
        Catch ex As Exception
        Finally
        End Try
    End Sub
    Protected Sub btnPdfExport_Click(sender As Object, e As System.Web.UI.ImageClickEventArgs) Handles btnPdfExport.Click
        Try

            If ViewState("Pdf").Rows.Count > 0 Then
                grdTripData.DataBind()
                If ddlVehicleRegNo.SelectedItem.Text = "--Select All--" Then
                    ToPdf(ViewState("DataPdf"))
                Else
                    ToPdf(ViewState("Data"))
                End If
            Else
            End If
        Catch ex As Exception
        Finally
        End Try
    End Sub

    Public uri As String = "http://" + HttpContext.Current.Request.Url.Authority + HttpContext.Current.Request.ApplicationPath & "/"

    Protected Sub ddlYear_SelectedIndexChanged(sender As Object, e As System.EventArgs) Handles ddlYear.SelectedIndexChanged
        BindRegsNumberDDlFilter()
    End Sub

    Protected Sub ddlMonth_SelectedIndexChanged(sender As Object, e As System.EventArgs) Handles ddlMonth.SelectedIndexChanged

        'ddlYear.SelectedIndex = 0
        BindRegsNumberDDlFilter()
    End Sub
End Class

