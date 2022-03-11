Imports System.Data
Imports System.Data.SqlClient
Imports System.Web.Script.Serialization
Imports System.Xml
Imports System.Net
Imports System.IO
Imports System.Globalization
Partial Class TripData
    Inherits System.Web.UI.Page
    Protected Sub Search(ByVal sender As Object, ByVal e As System.Web.UI.ImageClickEventArgs) Handles btnSearch.Click
        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim con As SqlConnection = New SqlConnection(conStr)
        Dim oda As SqlDataAdapter = New SqlDataAdapter("", con)
        Dim ds As New DataSet
        Try
            If Session("USERROLE").ToString.ToUpper = "SU" Then
                oda.SelectCommand.CommandText = "select vehicle_no,IMEI_NO,convert(nvarchar,Trip_Start_DateTime,120)[Trip_Start_DateTime],convert(nvarchar,Trip_end_DateTime,120)[Trip_end_DateTime],Trip_Start_Location,Trip_End_Location,Total_Distance,tid,UserName,TripType ,Islock=CASE islock  when 0 THEN 'Unlocked'WHEN 1 THEN 'Locked' END, case e.isauth when 0 then 'Pending for approval' else 'Approved' end[Status],e.isAuth from MMM_MST_ELOGBOOK e , MMM_MST_USER u where   e.uid=u.uid and e.uid=" & DropDownList1.SelectedValue & "  and Trip_start_DateTime >= '" & SDate1.Text & "  00:00:00.000'   AND (Trip_end_DateTime <= '" & SDate2.Text & "  23:59:59.000' or trip_end_Datetime is null or trip_end_datetime='')   order by Trip_Start_DateTime"
                oda.SelectCommand.CommandType = CommandType.Text
                oda.Fill(ds, "data")
                gvData.DataSource = ds.Tables("data")
                gvData.DataBind()
                For i As Integer = 0 To gvData.Rows.Count - 1
                    Dim Gridapprove As ImageButton = CType(gvData.Rows(i).FindControl("btnapprove"), ImageButton)
                    Gridapprove.Visible = False
                    Dim GridDelete As ImageButton = CType(gvData.Rows(i).FindControl("btnDelete"), ImageButton)
                    GridDelete.Visible = False
                    If ds.Tables("data").Rows(i).Item("Islock") = "Locked" Then
                        Dim cb As CheckBox = CType(gvData.Rows(i).FindControl("check"), CheckBox)
                        cb.Checked = True
                    Else
                        Dim cb As CheckBox = CType(gvData.Rows(i).FindControl("check"), CheckBox)
                        cb.Checked = False
                    End If
                Next
                If gvData.Rows.Count = 0 Then
                    lblRecord.Visible = True
                    lblRecord.Text = "No Record Found"
                Else
                    lblRecord.Text = ""
                End If
            ElseIf Session("USERROLE").ToString.ToUpper = "ADMIN" Then
                oda.SelectCommand.CommandText = "select vehicle_no,IMEI_NO,convert(nvarchar,Trip_Start_DateTime,120)[Trip_Start_DateTime],convert(nvarchar,Trip_end_DateTime,120)[Trip_end_DateTime],Trip_Start_Location,Trip_End_Location,Total_Distance,tid,UserName,TripType ,Islock=CASE islock  when 0 THEN 'Unlocked'WHEN 1 THEN 'Locked' END, case e.isauth when 0 then 'Pending for approval' else 'Approved' end[Status],e.isAuth from MMM_MST_ELOGBOOK e , MMM_MST_USER u where   e.uid=u.uid and e.uid=" & DropDownList1.SelectedValue & "  and Trip_start_DateTime >= '" & SDate1.Text & "  00:00:00.000'   AND (Trip_end_DateTime <= '" & SDate2.Text & "  23:59:59.000' or trip_end_Datetime is null or trip_end_datetime='')   order by Trip_Start_DateTime"
                oda.SelectCommand.CommandType = CommandType.Text
                oda.Fill(ds, "data")
                gvData.DataSource = ds.Tables("data")
                gvData.DataBind()
                For i As Integer = 0 To gvData.Rows.Count - 1
                    Dim GridbtnEdit As ImageButton = CType(gvData.Rows(i).FindControl("btnEdit"), ImageButton)
                    GridbtnEdit.Visible = False
                    If ds.Tables("data").Rows(i).Item("Islock") = "Locked" Then
                        Dim cb As CheckBox = CType(gvData.Rows(i).FindControl("check"), CheckBox)
                        cb.Checked = True
                    Else
                        Dim cb As CheckBox = CType(gvData.Rows(i).FindControl("check"), CheckBox)
                        cb.Checked = False
                    End If
                    Dim Gridapprove As ImageButton = CType(gvData.Rows(i).FindControl("btnapprove"), ImageButton)
                    Gridapprove.Visible = False
                    Dim GridDelete As ImageButton = CType(gvData.Rows(i).FindControl("btnDelete"), ImageButton)
                    GridDelete.Visible = False
                Next
                If gvData.Rows.Count = 0 Then
                    lblRecord.Visible = True
                    lblRecord.Text = "No Record Found"
                Else
                    lblRecord.Text = ""
                End If
            ElseIf Session("USERROLE").ToString.ToUpper <> "USER" Then
                oda.SelectCommand.CommandText = "select vehicle_no,IMEI_NO,convert(nvarchar,Trip_Start_DateTime,120)[Trip_Start_DateTime],convert(nvarchar,Trip_end_DateTime,120)[Trip_end_DateTime],Trip_Start_Location,Trip_End_Location,Total_Distance,tid,UserName,TripType ,Islock=CASE islock  when 0 THEN 'Unlocked'WHEN 1 THEN 'Locked' END, case e.isauth when 0 then 'Pending for approval' else 'Approved' end[Status],e.isAuth from MMM_MST_ELOGBOOK e , MMM_MST_USER u where   e.uid=u.uid and e.uid=" & DropDownList1.SelectedValue & "  and Trip_start_DateTime >= '" & SDate1.Text & "  00:00:00.000'   AND (Trip_end_DateTime <= '" & SDate2.Text & "  23:59:59.000' or trip_end_Datetime is null or trip_end_datetime='')   order by Trip_Start_DateTime"
                oda.SelectCommand.CommandType = CommandType.Text
                oda.Fill(ds, "data")
                gvData.DataSource = ds.Tables("data")
                gvData.DataBind()
                For i As Integer = 0 To gvData.Rows.Count - 1
                    Dim GridbtnEdit As ImageButton = CType(gvData.Rows(i).FindControl("btnEdit"), ImageButton)
                    GridbtnEdit.Visible = False
                    Dim Gridapprove As ImageButton = CType(gvData.Rows(i).FindControl("btnapprove"), ImageButton)
                    Gridapprove.Visible = False
                    Dim GridDelete As ImageButton = CType(gvData.Rows(i).FindControl("btnDelete"), ImageButton)
                    GridDelete.Visible = False
                Next
                If gvData.Rows.Count = 0 Then
                    lblRecord.Visible = True
                    lblRecord.Text = "No Record Found"
                Else
                    lblRecord.Text = ""
                End If
            Else
                oda.SelectCommand.CommandText = "select vehicle_no,IMEI_NO,convert(nvarchar,Trip_Start_DateTime,120)[Trip_Start_DateTime],convert(nvarchar,Trip_end_DateTime,120)[Trip_end_DateTime],Trip_Start_Location,Trip_End_Location,Total_Distance,tid,UserName,TripType ,Islock=CASE islock  when 0 THEN 'Unlocked'WHEN 1 THEN 'Locked' END,case e.isauth when 0 then 'Pending for approval' else 'Approved' end[Status],e.isauth from MMM_MST_ELOGBOOK e , MMM_MST_USER u where   e.uid=u.uid and e.uid=" & DropDownList1.SelectedValue & "  and Trip_start_DateTime >= '" & SDate1.Text & "  00:00:00.000'   AND (Trip_end_DateTime <= '" & SDate2.Text & "  23:59:59.000' or trip_end_Datetime is null or trip_end_datetime='')   order by Trip_Start_DateTime,Trip_end_DateTime"
                oda.SelectCommand.CommandType = CommandType.Text
                ' Dim ds As New DataSet()
                oda.Fill(ds, "data")
                gvData.DataSource = ds.Tables("data")
                gvData.DataBind()
                For i As Integer = 0 To gvData.Rows.Count - 1
                    Dim Gridapprove As ImageButton = CType(gvData.Rows(i).FindControl("btnapprove"), ImageButton)
                    Gridapprove.Visible = True
                    Dim GridDelete As ImageButton = CType(gvData.Rows(i).FindControl("btnDelete"), ImageButton)
                    GridDelete.Visible = True
                Next
                If gvData.Rows.Count = 0 Then
                    lblRecord.Visible = True
                    lblRecord.Text = "No Record Found"
                Else
                    lblRecord.Text = ""
                End If
            End If
        Catch ex As Exception
        Finally
            con.Close()
            oda.Dispose()
            con.Dispose()
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
    Protected Sub OnMap(ByVal sender As Object, ByVal e As System.EventArgs)
        Try
        Dim OmMap As ImageButton = TryCast(sender, ImageButton)
        Dim row As GridViewRow = DirectCast(OmMap.NamingContainer, GridViewRow)
            Response.Redirect("ShowMap.aspx?IMIE=" & row.Cells(3).Text & "&Start=" + row.Cells(4).Text + "&End=" + row.Cells(5).Text)
        Catch ex As Exception
        Finally
        End Try
    End Sub
    Protected Sub islock(ByVal sender As Object, ByVal e As System.EventArgs)

        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim con As SqlConnection = New SqlConnection(conStr)
        Dim oda As SqlDataAdapter = New SqlDataAdapter("", con)
        Try
            If con.State <> ConnectionState.Open Then
                con.Open()
            End If
            If ViewState("TIDS").ToString().Length > 0 Then
                oda.SelectCommand.CommandText = "update mmm_mst_elogbook set  islock=1  where tid in (" & ViewState("TIDS") & ")  "
                oda.SelectCommand.CommandType = CommandType.Text
                oda.SelectCommand.ExecuteNonQuery()
            End If
            If ViewState("tidsunlock").ToString().Length > 0 Then
                oda.SelectCommand.CommandText = "update mmm_mst_elogbook set  islock=0  where  tid in (" & ViewState("tidsunlock") & ") "
                oda.SelectCommand.CommandType = CommandType.Text
                oda.SelectCommand.ExecuteNonQuery()
            End If
            Dim tidss As String
            If ViewState("TIDS").ToString().Length > 0 And ViewState("tidsunlock").ToString().Length > 0 Then
                tidss = ViewState("TIDS") + "," & ViewState("tidsunlock")
            ElseIf ViewState("TIDS").ToString().Length > 0 Then
                tidss = ViewState("TIDS")
            ElseIf ViewState("tidsunlock").ToString().Length > 0 Then
                tidss = ViewState("tidsunlock")

            End If

            bindGridLoc()
            lockunlock_ModalPopupExtender.Hide()
            lblRecord.Visible = True
            lblRecord.Text = "Trip has  been lock/Unlock"

            'islock.Dispose() 
            'comment on 8 nov 2013
            updatePanelEdit.Update()
        Catch ex As Exception
        Finally
            con.Dispose()
            oda.Dispose()
        End Try
    End Sub
    Protected Sub LockUnlock(sender As Object, e As System.Web.UI.ImageClickEventArgs) Handles ImageButton3.Click
        Try
        
        If gvData.Rows.Count = 0 Then Exit Sub
        Dim tidslock As String = ""
        Dim tidsunlock As String = ""
        Dim cbHeader As CheckBox = CType(gvData.HeaderRow.FindControl("chkHeader"), CheckBox)
        For Each gvr As GridViewRow In gvData.Rows
            Dim cb As CheckBox = CType(gvr.FindControl("check"), CheckBox)
            If cb.Checked = True Then
                Dim row As GridViewRow = DirectCast(cb.NamingContainer, GridViewRow)
                Dim tid As Integer = Convert.ToString(Me.gvData.DataKeys(row.RowIndex).Value)
                tidslock = tidslock & tid & ","
            Else
                Dim row As GridViewRow = DirectCast(cb.NamingContainer, GridViewRow)
                Dim tid As Integer = Convert.ToString(Me.gvData.DataKeys(row.RowIndex).Value)
                tidsunlock = tidsunlock & tid & ","
            End If
        Next
        If tidsunlock.Length + tidslock.Length = 0 Then
            lblRecord.Text = "No trip for lock/Unlock"
            Exit Sub
        End If
        If tidsunlock.Length > 0 And tidslock.Length > 0 Then
            tidsunlock = Left(tidsunlock, tidsunlock.Length - 1)
            tidslock = Left(tidslock, tidslock.Length - 1)
        ElseIf tidslock.Length > 0 Then
            tidslock = Left(tidslock, tidslock.Length - 1)
        ElseIf tidsunlock.Length > 0 Then
            tidsunlock = Left(tidsunlock, tidsunlock.Length - 1)
        End If
        ViewState("TIDS") = tidslock
        ViewState("tidsunlock") = tidsunlock

        lblMsglockUnlock.Text = "Are you sure to lock/Unlock trip?"
        btnActlockunlock.Text = "Lock/Unlock"
        updatePanellockunlock.Update()
            lockunlock_ModalPopupExtender.Show()
        Catch ex As Exception
        Finally

        End Try
    End Sub
    Protected Sub EditHit(ByVal sender As Object, ByVal e As System.EventArgs)
        
        VehicleNo.Enabled = False
        IMEINO.Enabled = False
        lblRecord.Text = ""
        lblMsgEdit.Text = ""
        Label1.Text = ""
        Label2.Text = ""
        Dim btnDetails As ImageButton = TryCast(sender, ImageButton)
        Dim row As GridViewRow = DirectCast(btnDetails.NamingContainer, GridViewRow)
        Dim tid As Integer = Convert.ToString(Me.gvData.DataKeys(row.RowIndex).Value)
        btnActUserSave.Text = "Update"
        ViewState("tid") = tid
        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim con As SqlConnection = New SqlConnection(conStr)
        Dim oda As SqlDataAdapter = New SqlDataAdapter("select vehicle_no,IMEI_NO,Trip_Start_DateTime,Trip_end_DateTime,Trip_Start_Location,Trip_End_Location,Total_Distance,tid,UserName,TripType,MMM_MST_ELOGBOOK.Uid From MMM_MST_ELOGBOOK inner join MMM_MST_USER on MMM_MST_ELOGBOOK.uid=MMM_MST_USER.uid WHERE tid=" & tid & " and islock=0 ", con)
        oda.SelectCommand.CommandType = CommandType.Text
        Try
            Dim dt As New DataTable
            oda.Fill(dt)
            If dt.Rows.Count = 0 Then
                lblRecord.Text = "Your Trip is locked"
                lblRecord.Visible = True
                updatePanelEdit.Update()
                con.Close()
                Exit Sub
            Else
                lblRecord.Text = ""
            End If
            'If Session("USERROLE").ToString.ToUpper <> "USER" Then
            '    lblRecord.Text = "Only user can edit"
            '    'updatePanelEdit.Update()
            '    Exit Sub
            'End If

            Dim datetime1 As String
            Dim enddatetime As String
            ViewState("Totalkm") = dt.Rows(0).Item("Total_Distance").ToString()
            datetime1 = Format(dt.Rows(0).Item("Trip_Start_DateTime"), "yyyy-MM-dd HH:mm")
            enddatetime = Format(dt.Rows(0).Item("Trip_End_DateTime"), "yyyy-MM-dd HH:mm")
            ViewState("StartTime") = datetime1
            ViewState("EndTime") = enddatetime
            Date1.Text = datetime1.Substring(0, 10)
            ddlshh.SelectedIndex = ddlshh.Items.IndexOf(ddlshh.Items.FindByText(Format(dt.Rows(0).Item("Trip_Start_DateTime"), "HH")))
            ddlsmm.SelectedIndex = ddlsmm.Items.IndexOf(ddlsmm.Items.FindByText(Format(dt.Rows(0).Item("Trip_Start_DateTime"), "mm")))
            If Session("USERROLE").ToString.ToUpper = "USER" Or Session("USERROLE").ToString.ToUpper = "SU" Then
                If dt.Rows(0).Item("Triptype").ToString <> "" Or Not IsDBNull(dt.Rows(0).Item("Triptype")) Then
                    Triptype.SelectedIndex = Triptype.Items.IndexOf(Triptype.Items.FindByText(dt.Rows(0).Item("Triptype")))
                End If

                If dt.Rows(0).Item("Trip_end_DateTime").ToString = "" Or IsDBNull(dt.Rows(0).Item("Trip_end_DateTime")) Then
                Else
                    Dim datettime2 As String = Format(dt.Rows(0).Item("Trip_end_DateTime"), "yyyy-MM-dd HH:mm")
                    Date2.Text = datettime2.Substring(0, 10)
                    ddlehh.SelectedIndex = ddlehh.Items.IndexOf(ddlehh.Items.FindByText(Format(dt.Rows(0).Item("Trip_End_DateTime"), "HH")))
                    ddlemm.SelectedIndex = ddlemm.Items.IndexOf(ddlemm.Items.FindByText(Format(dt.Rows(0).Item("Trip_End_DateTime"), "mm")))
                End If
                If dt.Rows(0).Item("TripType").ToString().ToUpper = "MANUAL" Then
                    pnlmb.Visible = True
                    imgbtnloc.Visible = False
                    imgbtnloc1.Visible = False
                    Triptype.Enabled = False
                    StartLocation.Visible = True
                    Sloc.Visible = True
                    EndLocation.Visible = True
                    Eloc.Visible = True
                    Tdistance.Visible = True
                    todistance.Visible = True
                    IMEINO.Text = dt.Rows(0).Item("IMEI_NO").ToString()
                    VehicleNo.Text = dt.Rows(0).Item("Vehicle_No")
                    Sloc.Text = dt.Rows(0).Item("Trip_Start_Location")
                    Eloc.Text = dt.Rows(0).Item("Trip_End_Location")
                    todistance.Text = dt.Rows(0).Item("Total_Distance")
                ElseIf dt.Rows(0).Item("TripType").ToString().ToUpper = "AUTOMATIC" Then
                    IMEINO.Text = dt.Rows(0).Item("IMEI_NO").ToString()
                    pnlmb.Visible = True
                    VehicleNo.Text = dt.Rows(0).Item("Vehicle_No")
                    Triptype.Enabled = False
                    IMEINO.Visible = True
                    IMELabel.Visible = True
                    imgbtnloc.Visible = True
                    imgbtnloc1.Visible = True
                    Vehicle.Visible = True
                    VehicleNo.Visible = True
                    StartLocation.Visible = False
                    Sloc.Visible = False
                    EndLocation.Visible = False
                    Eloc.Visible = False
                    Tdistance.Visible = False
                    todistance.Visible = False
                ElseIf dt.Rows(0).Item("TripType").ToString().ToUpper = "NIL" Then
                    If Session("USERROLE").ToString.ToUpper = "SU" Or Session("USERROLE").ToString.ToUpper = "FCAGGN" Or Session("USERROLE").ToString.ToUpper = "CADMIN" Or Session("USERROLE").ToString.ToUpper <> "USER" Then
                        lblRecord.Text = "Nil trip only user can edit "
                        con.Close()
                        Exit Sub
                    ElseIf Session("USERROLE").ToString.ToUpper = "USER" Then
                        nilstartdate.Text = datetime1.Substring(0, 10)
                        Dim datettime2 As String = Format(dt.Rows(0).Item("Trip_end_DateTime"), "yyyy-MM-dd HH:mm")
                        nilenddate.Text = datettime2.Substring(0, 10)
                        niltripIMEI.Text = dt.Rows(0).Item("IMEI_NO").ToString()
                        niltripVehicle.Text = dt.Rows(0).Item("Vehicle_No").ToString
                        btbniltripsave.Text = "Update"
                        upniltrip.Update()
                        btnniltripmodelpopup.Show()
                        lblniltrip.Text = ""
                        con.Close()
                        Exit Sub
                    End If

                ElseIf dt.Rows(0).Item("TripType").ToString().ToUpper <> "AUTOMATIC" And dt.Rows(0).Item("TripType").ToString().ToUpper <> "MANUAL" Then
                    pnlmb.Visible = False
                    IMEINO.Text = dt.Rows(0).Item("IMEI_NO").ToString()
                    VehicleNo.Text = dt.Rows(0).Item("Vehicle_No")
                    Triptype.Enabled = False
                    IMEINO.Visible = True
                    IMELabel.Visible = True
                    imgbtnloc.Visible = True
                    imgbtnloc1.Visible = True
                    Vehicle.Visible = True
                    VehicleNo.Visible = True

                    StartLocation.Visible = False
                    Sloc.Visible = False
                    EndLocation.Visible = False
                    Eloc.Visible = False
                    Tdistance.Visible = False
                    todistance.Visible = False
                End If
            Else
                lblRecord.Text = "Only user can edit"
                con.Close()
                Exit Sub
            End If
        Catch ex As Exception
        Finally
            con.Close()
            oda.Dispose()
            con.Dispose()
        End Try
        Me.updatePanelEdit.Update()
        Me.btnEdit_ModalPopupExtender.Show()

    End Sub
    Protected Sub Approve(ByVal sender As Object, ByVal e As EventArgs)
        Dim btndetails As ImageButton = TryCast(sender, ImageButton)
        Dim row As GridViewRow = DirectCast(btndetails.NamingContainer, GridViewRow)
        Dim pid As Integer = Me.gvData.DataKeys(row.RowIndex).Value
        ViewState("Did") = pid
        lblRecord.Text = ""
        Dim constr As String = ConfigurationManager.ConnectionStrings("constr").ConnectionString
        Dim con As SqlConnection = New SqlConnection(constr)
        Dim oda As New SqlDataAdapter("", con)
        Try
            oda.SelectCommand.CommandText = "select * from mmm_mst_elogbook where tid=" + pid.ToString + " "
            Dim dt As New DataTable()
            oda.Fill(dt)
            Dim tredate As String = dt.Rows(0).Item("Trip_end_Datetime").ToString
            Dim isauth As String = dt.Rows(0).Item("isAuth").ToString
            Dim Ttype As String = dt.Rows(0).Item("TripType").ToString
            If isauth = "1" Or isauth = "" Then
                lblRecord.Text = "This trip  is already  approved"
                lblRecord.Visible = True
                Exit Sub
            Else
                If tredate = "" And Ttype.ToUpper = "MOBILE" Then
                    lblRecord.Text = "This trip  can not be  approved"
                    lblRecord.Visible = True
                    Exit Sub
                Else
                    If tredate <> "" And Ttype.ToUpper = "MOBILE" Or Ttype.ToUpper = "SWITCH" Then
                        lblMsgapprove.Text = "Are you Sure Want to approve this trip? " & row.Cells(0).Text
                        btnapprove.Visible = True
                        updatePanelEdit.Update()
                        updatePanel1.Update()
                        btnapprove_modelpopup.Show()
                    Else
                        lblRecord.Text = "This trip  is already  approved"
                        lblRecord.Visible = True
                        Exit Sub
                    End If
                End If
            End If
        Catch ex As Exception
        Finally
            con.Close()
            oda.Dispose()
            con.Dispose()
        End Try
    End Sub
    Protected Sub Approvetrip(ByVal sender As Object, ByVal e As EventArgs)
        'Dim btndetails As ImageButton = TryCast(sender, ImageButton)
        'Dim row As GridViewRow = DirectCast(btndetails.NamingContainer, GridViewRow)
        'Dim pid As Integer = gvData.DataKeys(row.RowIndex).Value
        Dim constr As String = ConfigurationManager.ConnectionStrings("constr").ConnectionString
        Dim con As SqlConnection = New SqlConnection(constr)
        Dim oda As New SqlDataAdapter("", con)
        Try
            oda.SelectCommand.CommandText = "update mmm_mst_elogbook set isAuth=1  where tid=" + ViewState("Did").ToString + ""
            oda.SelectCommand.CommandType = CommandType.Text
            If con.State <> ConnectionState.Open Then
                con.Open()
            End If
            oda.SelectCommand.ExecuteNonQuery()
            'oda.SelectCommand.CommandText = "select * from mmm_mst_elogbook where tid=571 "
            oda.SelectCommand.CommandText = "select  vehicle_no,IMEI_NO,convert(nvarchar,Trip_Start_DateTime,120)[Trip_Start_DateTime],convert(nvarchar,Trip_end_DateTime,120)[Trip_end_DateTime],Trip_Start_Location,Trip_End_Location,Total_Distance,tid,UserName,Triptype,Islock=CASE islock  when 0 THEN 'Unlock'WHEN 1 THEN 'Lock' END, case e.isauth when 0 then 'Pending for approval' else 'Approved' end[Status] from mmm_mst_elogbook e inner join mmm_mst_user u on e.uid=u.uid where tid=" + ViewState("Did").ToString() + " "
            oda.SelectCommand.CommandType = CommandType.Text
            Dim ds As New DataTable()
            oda.Fill(ds)
            gvData.DataSource = ds
            gvData.DataBind()
        Catch ex As Exception
        Finally
            con.Close()
            oda.Dispose()
            con.Dispose()
        End Try
        lblRecord.Visible = True
        lblRecord.Text = "Trip has been approved"
        updatePanelEdit.Update()
        ' Dim constr As SqlConnection
        btnapprove_modelpopup.Hide()
    End Sub
    Protected Sub DeleteHit(ByVal sender As Object, ByVal e As System.EventArgs)

        lblRecord.Visible = False
        Dim btnDetails As ImageButton = TryCast(sender, ImageButton)
        Dim row As GridViewRow = DirectCast(btnDetails.NamingContainer, GridViewRow)
        Dim pid As Integer = Convert.ToString(Me.gvData.DataKeys(row.RowIndex).Value)
        ViewState("Did") = pid
        Dim constr As String = ConfigurationManager.ConnectionStrings("constr").ConnectionString
        Dim con As New SqlConnection(constr)
        Dim oda As New SqlDataAdapter("", con)
        Try
            oda.SelectCommand.CommandText = "select * from  mmm_mst_elogbook where tid =" + pid.ToString + " "
            oda.SelectCommand.CommandType = CommandType.Text
            Dim dt As New DataTable
            oda.Fill(dt)
            Dim tredate As String = dt.Rows(0).Item("Trip_end_Datetime").ToString
            Dim isauth As String = dt.Rows(0).Item("isAuth").ToString
            Dim Ttype As String = dt.Rows(0).Item("TripType").ToString
            If isauth = "1" Then
                lblRecord.Text = "This trip  can not be deleted."
                lblRecord.Visible = True
                Exit Sub
            Else
                If Ttype.ToUpper = "MOBILE" Or Ttype.ToString = "" Or Ttype.ToUpper = "SWITCH" Then
                    lblMsgDelete.Text = "Are you Sure Want to delete this trip? " & row.Cells(0).Text
                    btnActDelete.Visible = True
                    '   updatePanelEdit.Update()
                    updatePanelDelete.Update()
                    Me.btnDelete_ModalPopupExtender.Show()
                Else
                    lblRecord.Text = "This trip  can not be deleted."
                    lblRecord.Visible = True
                    updatePanelEdit.Update()
                    Exit Sub
                End If
            End If

        Catch ex As Exception
        Finally
            con.Close()
            oda.Dispose()
        End Try
    End Sub
    Public Sub bindGridLoc()
        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim con As SqlConnection = New SqlConnection(conStr)
        '  Dim oda As SqlDataAdapter = New SqlDataAdapter("select vehicle_no,IMEI_NO,convert(nvarchar,Trip_Start_DateTime,120)[Trip_Start_DateTime],convert(nvarchar,Trip_end_DateTime,120)[Trip_end_DateTime],Trip_Start_Location,Trip_End_Location,Total_Distance,tid,UserName,e.Uid as uid,triptype ,Islock=CASE islock  when 0 THEN 'Unlocked'WHEN 1 THEN 'Locked' END, case e.isauth when 0 then 'Pending for approval' else 'Approved' end[Status],e.isauth from MMM_MST_ELOGBOOK e  inner join MMM_MST_USER u on e.uid=u.uid where e.Uid=" + DropDownList1.SelectedValue + " order by Trip_Start_DateTime", con)
        'start rajat bansal
        Dim oda As SqlDataAdapter = New SqlDataAdapter("select vehicle_no,IMEI_NO,convert(nvarchar,Trip_Start_DateTime,120)[Trip_Start_DateTime],convert(nvarchar,Trip_end_DateTime,120)[Trip_end_DateTime],Trip_Start_Location,Trip_End_Location,Total_Distance,tid,UserName,e.Uid as uid,triptype ,Islock=CASE islock  when 0 THEN 'Unlocked'WHEN 1 THEN 'Locked' END, case e.isauth when 0 then 'Pending for approval' else 'Approved' end[Status],e.isauth from MMM_MST_ELOGBOOK e  inner join MMM_MST_USER u on e.uid=u.uid where e.Uid=" + DropDownList1.SelectedValue + " and Trip_start_DateTime >= '" & SDate1.Text & "  00:00:00.000'   AND (Trip_end_DateTime <= '" & SDate2.Text & "  23:59:59.000' or trip_end_Datetime is null or trip_end_datetime='')   order by Trip_Start_DateTime", con)
        'end
        Dim ds As New DataSet()
        Try
            oda.Fill(ds, "data")
            gvData.DataSource = ds.Tables("data")
            gvData.DataBind()
            If Session("USERROLE").ToString.ToUpper <> "SU" And Session("USERROLE").ToString.ToUpper <> "USER" Then
                For i As Integer = 0 To gvData.Rows.Count - 1
                    Dim GridbtnEdit As ImageButton = CType(gvData.Rows(i).FindControl("btnEdit"), ImageButton)
                    GridbtnEdit.Visible = False
                Next
                For i As Integer = 0 To gvData.Rows.Count - 1
                    Dim Gridapprove As ImageButton = CType(gvData.Rows(i).FindControl("btnapprove"), ImageButton)
                    Dim GridDelete As ImageButton = CType(gvData.Rows(i).FindControl("btnDelete"), ImageButton)
                    If Session("USERROLE").ToString.ToUpper <> "USER" Then
                        Gridapprove.Visible = False
                        GridDelete.Visible = False
                    End If
                Next
            End If


            ' If Session("USERROLE").ToString.ToUpper = "SU" Then
            ''Rajat Bansal
            If Session("USERROLE").ToString.ToUpper = "SU" Or Session("USERROLE").ToString.ToUpper = "FCAGGN" Or Session("USERROLE").ToString.ToUpper = "CADMIN" Then
                For i As Integer = 0 To gvData.Rows.Count - 1
                    If gvData.Rows(i).Cells(9).Text.ToUpper = "LOCKED" Then
                        Dim cb As CheckBox = CType(gvData.Rows(i).FindControl("check"), CheckBox)
                        cb.Checked = True
                    Else
                        Dim cb As CheckBox = CType(gvData.Rows(i).FindControl("check"), CheckBox)
                        cb.Checked = False
                    End If
                Next
                For i As Integer = 0 To gvData.Rows.Count - 1
                    Dim Gridapprove As ImageButton = CType(gvData.Rows(i).FindControl("btnapprove"), ImageButton)
                    Dim GridDelete As ImageButton = CType(gvData.Rows(i).FindControl("btnDelete"), ImageButton)
                    If Session("USERROLE").ToString.ToUpper <> "USER" Then
                        Gridapprove.Visible = False
                        GridDelete.Visible = False
                    End If
                Next
            End If
            ' gvData.DataBind()
        Catch ex As Exception
        Finally
            con.Close()
            oda.Dispose()
            con.Dispose()
        End Try
    End Sub
    Public Sub checkchange(ByVal sender As Object, ByVal e As System.EventArgs)
        Try
        
        If gvData.Rows.Count = 0 Then Exit Sub
        Dim cbHeader As CheckBox = CType(gvData.HeaderRow.FindControl("chkHeader"), CheckBox)
        If cbHeader.Checked = True Then
            For Each gvr As GridViewRow In gvData.Rows
                Dim cb As CheckBox = CType(gvr.FindControl("check"), CheckBox)
                cb.Checked = True
            Next
        Else
            For Each gvr As GridViewRow In gvData.Rows
                Dim cb As CheckBox = CType(gvr.FindControl("check"), CheckBox)
                cb.Checked = False
            Next
            End If
        Catch ex As Exception
        Finally
        End Try
    End Sub
    Public Sub popupshow()
        VehicleNo.Text = ""
        IMEINO.Text = ""
        Label1.Text = ""
        Label2.Text = ""
        lblRecord.Text = ""
        lblMsgEdit.Text = ""
        Date1.Text = ""
        Date2.Text = ""
        ddlshh.SelectedIndex = -1
        ddlsmm.SelectedIndex = -1
        ddlehh.SelectedIndex = -1
        ddlemm.SelectedIndex = -1
        Me.btnEdit_ModalPopupExtender.Show()
    End Sub
    Public Sub Add()
        If DropDownList1.SelectedItem.Text.ToUpper = "SELECT" Then
            lblRecord.Text = "Must select one user"
            Exit Sub
        End If
        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim con As SqlConnection = New SqlConnection(conStr)
        Dim Udtype As String
        Dim Ufld As String
        Dim UVfld As String
        Dim Vdtype As String
        Dim Vfld As String
        Dim vemei As String
        VehicleNo.Enabled = False
        IMEINO.Enabled = False
        VehicleNo.Text = ""
        IMEINO.Text = ""
        Label1.Text = ""
        Label2.Text = ""
        lblRecord.Text = ""
        lblMsgEdit.Text = ""
        Dim oda As SqlDataAdapter = New SqlDataAdapter("", con)
        Try
            oda.SelectCommand.CommandText = "select* from mmm_mst_entity where eid=" & Session("EID") & " "
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
            'Old Code 
            'oda.SelectCommand.CommandText = "select " & UVfld & " from mmm_mst_doc where documenttype='" & Udtype & "' and " & Ufld & " = " & DropDownList1.SelectedValue & " and curstatus='Allotted'"
            'New Change 12th APR 2014
            oda.SelectCommand.CommandText = "select " & UVfld & " from mmm_mst_doc where documenttype='" & Udtype & "' and " & Ufld & " = '" & DropDownList1.SelectedValue & "'  and curstatus in ('Allotted','Surrender','ARCHIVE') and CONVERT(datetime, fld17,3)<= convert(nvarchar,convert(datetime,'" & Date1.Text & "'),21) and convert(nvarchar,convert(datetime,'" & Date1.Text & "'),21)<=CONVERT(datetime, fld19,3) and CONVERT(datetime,fld19,3)>=convert(nvarchar,convert(datetime,'" & Date1.Text & "'),21) and convert(nvarchar,convert(datetime,'" & Date1.Text & "'),21) >= CONVERT(datetime, fld17,3)"

            oda.Fill(ds, "vrfdoc")
            If ds.Tables("vrfdoc").Rows.Count > 1 Then
                lblMsgEdit.Text = "Found multiple vehicles (can not create trip)"
                con.Dispose()
                Exit Sub
            End If

            'Old Code
            'oda.SelectCommand.CommandText = "select " & Vfld & "," & vemei & " from mmm_mst_master where eid=" & Session("EID") & " and documenttype='" & Vdtype & "' and tid=(select " & UVfld & " from mmm_mst_doc where documenttype='" & Udtype & "' and " & Ufld & " = " & DropDownList1.SelectedValue & " and curstatus='Allotted') "
            'New Change 12th APR
            oda.SelectCommand.CommandText = "select " & Vfld & "," & vemei & " from mmm_mst_master where eid=" & Session("EID") & " and documenttype='" & Vdtype & "' and tid=(select " & UVfld & " from mmm_mst_doc where documenttype='" & Udtype & "' and " & Ufld & " = '" & DropDownList1.SelectedValue & "' and curstatus in ('Allotted','Surrender','ARCHIVE') and CONVERT(datetime, fld17,3)<= convert(nvarchar,convert(datetime,'" & Date1.Text & "'),21) and convert(nvarchar,convert(datetime,'" & Date1.Text & "'),21)<=CONVERT(datetime, fld19,3) and CONVERT(datetime,fld19,3)>=convert(nvarchar,convert(datetime,'" & Date1.Text & "'),21) and convert(nvarchar,convert(datetime,'" & Date1.Text & "'),21) >= CONVERT(datetime, fld17,3))"
            oda.Fill(ds, "vemei")
            If ds.Tables("vemei").Rows.Count = 0 Then
                lblMsgEdit.Text = "Dear User, Vehicle is not allocated to you. Please contact your admin."
                con.Dispose()
                Exit Sub
            End If
            ViewState("Date") = Date1.Text

            If ds.Tables("vemei").Rows.Count > 0 Then
                If ds.Tables("vemei").Rows(0).Item(1).ToString.Length <> 0 Or ds.Tables("vemei").Rows(0).Item(0).ToString.Length <> 0 Then
                    VehicleNo.Text = ds.Tables("vemei").Rows(0).Item(0).ToString
                    IMEINO.Text = ds.Tables("vemei").Rows(0).Item(1).ToString
                    Triptype.Enabled = True
                    Triptype.Visible = True
                    Triptype.SelectedIndex = Triptype.Items.IndexOf(Triptype.Items.FindByText("Automatic"))
                    IMEINO.Visible = True
                    IMELabel.Visible = True
                    imgbtnloc.Visible = True
                    imgbtnloc1.Visible = True
                    Vehicle.Visible = True
                    VehicleNo.Visible = True
                    Label1.Visible = True
                    Label2.Visible = True
                    StartLocation.Visible = False
                    Sloc.Visible = False
                    EndLocation.Visible = False
                    Eloc.Visible = False
                    Tdistance.Visible = False
                    todistance.Visible = False
                    km.Visible = False
                    pnlmb.Visible = True
                Else
                    If VehicleNo.Text = "" Or IMEINO.Text = "" Then
                        'lblMsgEdit.Text = "GPS device is not allocated to the vehicle"
                        lblRecord.Text = "GPS device is not allocated to the vehicle"
                        con.Dispose()
                        Exit Sub
                    End If
                End If
            End If
        Catch ex As Exception
        Finally
            con.Close()
            oda.Dispose()
            con.Dispose()
        End Try
        btnActUserSave.Text = "Save"
        'lblMsgEdit.Text = ""
        'ddlshh.SelectedIndex = -1
        'ddlsmm.SelectedIndex = -1
        'ddlehh.SelectedIndex = -1
        'ddlemm.SelectedIndex = -1
        'Date1.Text = ""
        'Date2.Text = ""
        Me.updatePanelEdit.Update()
        ' Date1.Text = ViewState("Date")
        ' Me.btnEdit_ModalPopupExtender.Show()
    End Sub
    Protected Sub btnActUserSave_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnActUserSave.Click
        Dim parsed As DateTime
        Dim valid As Boolean = DateTime.TryParseExact(Date1.Text, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, parsed)
        If valid = False Then
            lblMsgEdit.Text = "Enter the Correct Format"
            Date1.Focus()
            Exit Sub
        End If
        Dim validdate2 As Boolean = DateTime.TryParseExact(Date2.Text, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, parsed)
        If validdate2 = False Then
            lblMsgEdit.Text = "Enter the end date as required Format"
            Date2.Focus()
            Exit Sub
        End If
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
        ElseIf DateTime.Parse(dtst) > DateTime.Parse(crrdate) Then
            lblMsgEdit.Text = "Future date/time is not allowed in log book entry"
            Exit Sub
        ElseIf DateTime.Parse(dtet) > DateTime.Parse(crrdate) Then
            lblMsgEdit.Text = "Future date/time is not allowed in log book entry"
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
        'validation by Rajat Bansal
        If Trim(VehicleNo.Text) = "" Then
            lblMsgEdit.Text = "Update Vehicle No"
            Exit Sub

        End If
        If Trim(IMEINO.Text) = "" Then
            lblMsgEdit.Text = "Update IMEI No"
            Exit Sub
        End If

        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim con As SqlConnection = New SqlConnection(conStr)
        Dim oda As SqlDataAdapter = New SqlDataAdapter("", con)
        If con.State <> ConnectionState.Open Then
            con.Open()
        End If
        Try

            If btnActUserSave.Text <> "Update" Then
                'Rajat lock already exist 
                oda.SelectCommand.CommandText = "select count(*) from (select trip_start_datetime from mmm_mst_elogbook  where tid=( select min(tid) from mmm_msT_elogbook where islock=1 and uid=" & DropDownList1.SelectedValue & ") ) r1 ,(select trip_end_datetime from mmm_mst_elogbook  where tid=( select max(tid) from mmm_msT_elogbook where islock=1 and uid=" & DropDownList1.SelectedValue & ")) r2 where (trip_start_datetime <='" & Date1.Text & " " & ddlshh.SelectedItem.Text & ":" & ddlsmm.SelectedItem.Text & "'  and trip_end_datetime >='" & Date2.Text & " " & ddlehh.SelectedItem.Text & ":" & ddlemm.SelectedItem.Text & "')  or (trip_start_datetime >='" & Date1.Text & " " & ddlshh.SelectedItem.Text & ":" & ddlsmm.SelectedItem.Text & "'  and trip_end_datetime <='" & Date2.Text & " " & ddlehh.SelectedItem.Text & ":" & ddlemm.SelectedItem.Text & "')"
                Dim tripal As Integer = oda.SelectCommand.ExecuteScalar()
                If tripal > 0 Then
                    lblMsgEdit.Text = "Your trip is locked between this period"
                    con.Dispose()
                    Exit Sub
                End If
                'end
            End If
            If btnActUserSave.Text = "Update" Then
                'oda.SelectCommand.CommandText = "select count(*) from mmm_mst_elogbook where (vehicle_no='" & VehicleNo.Text & "' and  '" & Date1.Text & " " & ddlshh.SelectedItem.Text & ":" & ddlsmm.SelectedItem.Text & "' <= trip_end_Datetime and '" & Date1.Text & " " & ddlshh.SelectedItem.Text & ":" & ddlsmm.SelectedItem.Text & "' >= trip_start_Datetime and tid<>" & ViewState("tid") & ") or (imei_no='" & IMEINO.Text & "' and vehicle_no='" & VehicleNo.Text & "' and  '" & Date2.Text & " " & ddlehh.SelectedItem.Text & ":" & ddlemm.SelectedItem.Text & "' <= trip_end_Datetime and '" & Date2.Text & " " & ddlehh.SelectedItem.Text & ":" & ddlemm.SelectedItem.Text & "' >= trip_start_Datetime and tid<>" & ViewState("tid") & ") or (vehicle_no='" & VehicleNo.Text & "' and  '" & Date1.Text & " " & ddlshh.SelectedItem.Text & ":" & ddlsmm.SelectedItem.Text & "' <= trip_start_Datetime and  trip_end_Datetime<='" & Date2.Text & " " & ddlehh.SelectedItem.Text & ":" & ddlemm.SelectedItem.Text & "' and tid<>" & ViewState("tid") & ") "
                oda.SelectCommand.CommandText = "select count(*) from mmm_mst_elogbook where (uid=" & DropDownList1.SelectedValue & " and  '" & Date1.Text & " " & ddlshh.SelectedItem.Text & ":" & ddlsmm.SelectedItem.Text & "' <= trip_end_Datetime and '" & Date1.Text & " " & ddlshh.SelectedItem.Text & ":" & ddlsmm.SelectedItem.Text & "' >= trip_start_Datetime and tid<>" & ViewState("tid") & ") or (uid=" & DropDownList1.SelectedValue & " and  '" & Date2.Text & " " & ddlehh.SelectedItem.Text & ":" & ddlemm.SelectedItem.Text & "' <= trip_end_Datetime and '" & Date2.Text & " " & ddlehh.SelectedItem.Text & ":" & ddlemm.SelectedItem.Text & "' >= trip_start_Datetime and tid<>" & ViewState("tid") & ") or ( uid=" & DropDownList1.SelectedValue & " and  '" & Date1.Text & " " & ddlshh.SelectedItem.Text & ":" & ddlsmm.SelectedItem.Text & "' <= trip_start_Datetime and  trip_end_Datetime<='" & Date2.Text & " " & ddlehh.SelectedItem.Text & ":" & ddlemm.SelectedItem.Text & "' and tid<>" & ViewState("tid") & ") "
            Else
                ' oda.SelectCommand.CommandText = "select count(*) from mmm_mst_elogbook where (vehicle_no='" & VehicleNo.Text & "' and  '" & Date1.Text & " " & ddlshh.SelectedItem.Text & ":" & ddlsmm.SelectedItem.Text & "' <= trip_end_Datetime and '" & Date1.Text & " " & ddlshh.SelectedItem.Text & ":" & ddlsmm.SelectedItem.Text & "' >= trip_start_Datetime) or (vehicle_no='" & VehicleNo.Text & "' and  '" & Date2.Text & " " & ddlehh.SelectedItem.Text & ":" & ddlemm.SelectedItem.Text & "' <= trip_end_Datetime and '" & Date2.Text & " " & ddlehh.SelectedItem.Text & ":" & ddlemm.SelectedItem.Text & "' >= trip_start_Datetime) or (vehicle_no='" & VehicleNo.Text & "' and  '" & Date1.Text & " " & ddlshh.SelectedItem.Text & ":" & ddlsmm.SelectedItem.Text & "' <= trip_start_Datetime and  trip_end_Datetime<='" & Date2.Text & " " & ddlehh.SelectedItem.Text & ":" & ddlemm.SelectedItem.Text & "') "
                oda.SelectCommand.CommandText = "select count(*) from mmm_mst_elogbook where (uid=" & DropDownList1.SelectedValue & " and  '" & Date1.Text & " " & ddlshh.SelectedItem.Text & ":" & ddlsmm.SelectedItem.Text & "' <= trip_end_Datetime and '" & Date1.Text & " " & ddlshh.SelectedItem.Text & ":" & ddlsmm.SelectedItem.Text & "' >= trip_start_Datetime) or (uid=" & DropDownList1.SelectedValue & " and  '" & Date2.Text & " " & ddlehh.SelectedItem.Text & ":" & ddlemm.SelectedItem.Text & "' <= trip_end_Datetime and '" & Date2.Text & " " & ddlehh.SelectedItem.Text & ":" & ddlemm.SelectedItem.Text & "' >= trip_start_Datetime) or (uid=" & DropDownList1.SelectedValue & " and  '" & Date1.Text & " " & ddlshh.SelectedItem.Text & ":" & ddlsmm.SelectedItem.Text & "' <= trip_start_Datetime and  trip_end_Datetime<='" & Date2.Text & " " & ddlehh.SelectedItem.Text & ":" & ddlemm.SelectedItem.Text & "') "
            End If


            ' oda.SelectCommand.CommandText = "select count(*) from mmm_mst_elogbook where vehicle_no='" & VehicleNo.Text & "' and  '" & Date1.Text & " " & ddlshh.SelectedItem.Text & ":" & ddlsmm.SelectedItem.Text & "' <= trip_start_Datetime and  trip_end_Datetime<='" & Date2.Text & " " & ddlehh.SelectedItem.Text & ":" & ddlemm.SelectedItem.Text & "'"
            Dim cnt As Integer = oda.SelectCommand.ExecuteScalar()
            If cnt > 0 Then
                lblMsgEdit.Text = "Trip already exist at this period. "
                con.Dispose()
                Exit Sub
            End If
            Dim tedate As String = ""
            'Updated By Rajat Bansal

            ' HisTory Code By Rajat Bansal
            'comment on 06 Nov


            oda.SelectCommand.CommandText = "select UVStartDateTime , UVEndDateTime   ,CurStatus,UVDType,UVUserField,UVVehicleField,VIDType,VIVehicleField,VIImeiField from mmm_mst_entity where eid=" & Session("eid") & "   "
            If con.State <> ConnectionState.Open Then
                con.Open()
            End If
            oda.SelectCommand.CommandType = CommandType.Text
            Dim entity As New DataTable
            oda.Fill(entity)
            If entity.Rows.Count = 0 Then
                oda.Dispose()
                con.Dispose()
                Exit Sub
            End If
            ''Start Rajat Bansal
            Dim startfld As String = entity.Rows(0).Item("UVStartDateTime").ToString
            Dim endfld As String = entity.Rows(0).Item("UVEndDateTime").ToString
            Dim userfld As String = entity.Rows(0).Item("UVUserField").ToString
            ''end
            Dim dt As New DataTable
            Dim doc As New DataTable
            oda.SelectCommand.CommandText = "select * from mmm_mst_doc where documenttype='" & entity.Rows(0).Item("UVDType") & "' and   " & entity.Rows(0).Item("UVUserField") & "='" & DropDownList1.SelectedValue & "' and curstatus in ('Allotted') "
            Dim gpsdata As GPSClass = New GPSClass()
            Dim output As String
            'updated By Rajat Bansal
            If Triptype.Visible = True Then
                If Triptype.SelectedItem.Text.ToUpper() = "AUTOMATIC" Then
                    Dim elogbook As New DataTable
                    oda.SelectCommand.CommandText = "select Trip_end_datetime from mmm_mst_elogbook where tid = (select max(tid) from mmm_mst_elogbook where uid=" & DropDownList1.SelectedValue & "  and eid=" & Session("EID") & ")"
                    oda.Fill(elogbook)
                    If elogbook.Rows.Count > 0 Then
                        tedate = oda.SelectCommand.ExecuteScalar().ToString
                        If tedate = "" Then
                            lblMsgEdit.Text = "Please end previous trip before starting new trip/updating trip "
                            oda.Dispose()
                            con.Dispose()
                            Exit Sub
                        End If
                    End If
                    If dt.Rows.Count = 0 Then
                        output = gpsdata.MakeMyTrip(DropDownList1.SelectedValue, Date1.Text + " " + ddlshh.SelectedItem.Text & ":" & ddlsmm.SelectedItem.Text, Date2.Text + " " + ddlehh.SelectedItem.Text & ":" & ddlemm.SelectedItem.Text, lblRecord, gvData, btnActUserSave, Session("eid"), updatePanelEdit, btnEdit_ModalPopupExtender, ViewState("tid"), VehicleNo.Text, IMEINO.Text)
                        If output = Nothing Then
                        ElseIf output.Length > 0 Then
                            SDate1.Text = Date1.Text
                            SDate2.Text = Date2.Text
                        End If
                    Else
                        output = gpsdata.MakeMyTrip(DropDownList1.SelectedValue, Date1.Text + " " + ddlshh.SelectedItem.Text & ":" & ddlsmm.SelectedItem.Text, Date2.Text + " " + ddlehh.SelectedItem.Text & ":" & ddlemm.SelectedItem.Text, lblRecord, gvData, btnActUserSave, Session("eid"), updatePanelEdit, btnEdit_ModalPopupExtender, ViewState("tid"), dt.Rows(0).Item("VechicleNo").ToString(), dt.Rows(0).Item("IMEI").ToString())
                        If output = Nothing Then
                        ElseIf output.Length > 0 Then
                            SDate1.Text = Date1.Text
                            SDate2.Text = Date2.Text
                        End If
                    End If
                ElseIf Triptype.SelectedItem.Text.ToUpper = "MANUAL" Then
                    'If VehicalManual.Text = "" Or VehicalManual.Text = Nothing Then
                    '    lblMsgEdit.Text = "Enter the vehical no."
                    '    Exit Sub
                    ' Else
                    Dim elogbook As New DataTable
                    oda.SelectCommand.CommandText = "select Trip_end_datetime from mmm_mst_elogbook where tid = (select max(tid) from mmm_mst_elogbook where uid=" & DropDownList1.SelectedValue & "  and eid=" & Session("EID") & ")"
                    oda.Fill(elogbook)
                    If elogbook.Rows.Count > 0 Then
                        tedate = oda.SelectCommand.ExecuteScalar().ToString
                        If tedate = "" Then
                            lblMsgEdit.Text = "Please end previous trip before starting new trip/updating trip "
                            oda.Dispose()
                            con.Dispose()
                            Exit Sub
                        End If
                    End If
                    If Sloc.Text = "" Or Sloc.Text = Nothing Then
                        lblMsgEdit.Text = "Enter the start location."
                        Exit Sub
                    ElseIf Eloc.Text = "" Or Eloc.Text = Nothing Then
                        lblMsgEdit.Text = "Enter the end location."
                        Exit Sub
                    ElseIf todistance.Text = "" Or todistance.Text = Nothing Then
                        lblMsgEdit.Text = "Enter the total distance."
                        Exit Sub
                    ElseIf IsNumeric(todistance.Text) = False Then
                        lblMsgEdit.Text = "Enter the distance in numeric only."
                        Exit Sub
                    End If
                    If dt.Rows.Count = 0 Then
                        output = gpsdata.ManualTrip(DropDownList1.SelectedValue, Date1.Text + " " + ddlshh.SelectedItem.Text & ":" & ddlsmm.SelectedItem.Text, Date2.Text + " " + ddlehh.SelectedItem.Text & ":" & ddlemm.SelectedItem.Text, lblRecord, gvData, btnActUserSave, ViewState("tid"), VehicleNo.Text, Sloc.Text, Eloc.Text, todistance.Text, IMEINO.Text)
                        If output = Nothing Then
                        ElseIf output.Length > 0 Then
                            SDate1.Text = Date1.Text
                            SDate2.Text = Date2.Text
                        End If
                    Else
                        output = gpsdata.ManualTrip(DropDownList1.SelectedValue, Date1.Text + " " + ddlshh.SelectedItem.Text & ":" & ddlsmm.SelectedItem.Text, Date2.Text + " " + ddlehh.SelectedItem.Text & ":" & ddlemm.SelectedItem.Text, lblRecord, gvData, btnActUserSave, ViewState("tid"), dt.Rows(0).Item("VechicleNo").ToString(), Sloc.Text, Eloc.Text, todistance.Text, dt.Rows(0).Item("IMEI").ToString())
                        If output = Nothing Then
                        ElseIf output.Length > 0 Then
                            SDate1.Text = Date1.Text
                            SDate2.Text = Date2.Text
                        End If
                    End If
                End If
            Else
                If dt.Rows.Count = 0 Then
                    output = gpsdata.MakeMyTrip(DropDownList1.SelectedValue, Date1.Text + " " + ddlshh.SelectedItem.Text & ":" & ddlsmm.SelectedItem.Text, Date2.Text + " " + ddlehh.SelectedItem.Text & ":" & ddlemm.SelectedItem.Text, lblRecord, gvData, btnActUserSave, Session("eid"), updatePanelEdit, btnEdit_ModalPopupExtender, ViewState("tid"), VehicleNo.Text, IMEINO.Text)
                    If output = Nothing Then
                    ElseIf output.Length > 0 Then
                        SDate1.Text = Date1.Text
                        SDate2.Text = Date2.Text
                    End If
                Else
                    output = gpsdata.MakeMyTrip(DropDownList1.SelectedValue, Date1.Text + " " + ddlshh.SelectedItem.Text & ":" & ddlsmm.SelectedItem.Text, Date2.Text + " " + ddlehh.SelectedItem.Text & ":" & ddlemm.SelectedItem.Text, lblRecord, gvData, btnActUserSave, Session("eid"), updatePanelEdit, btnEdit_ModalPopupExtender, ViewState("tid"), dt.Rows(0).Item("VechicleNo").ToString(), dt.Rows(0).Item("IMEI").ToString())
                    If output = Nothing Then
                    ElseIf output.Length > 0 Then
                        SDate1.Text = Date1.Text
                        SDate2.Text = Date2.Text
                    End If

                End If
            End If

            ' TRIP EDITED MAIL TO VENDOR

            If btnActUserSave.Text = "Update" Then
                Try
                    oda.SelectCommand.CommandText = "select distinct U.uid[uid],u.username[UserName],U.fld15[ User circle],r.rolename,(select emailID from MMM_MST_USER where uid in (R.uid)  )[emailid] from MMM_MST_USER U left outer join MMM_Ref_Role_User R on U.EID=R.eid and U.fld15=R.fld4 where U.uid in (" & DropDownList1.SelectedValue & ") and r.rolename='admin' and r.fld4 like '%'+u.fld15+'%' order by emailid "
                    oda.SelectCommand.CommandType = CommandType.Text
                    Dim DTADMIN As New DataTable
                    oda.Fill(DTADMIN)
                    oda.SelectCommand.CommandText = "select fld16[DriverName] from mmm_mst_master where fld10='" & VehicleNo.Text & "' and eid=" & Session("EID") & " and documenttype='vehicle'"
                    oda.SelectCommand.CommandType = CommandType.Text
                    Dim DrvDetail As New DataTable
                    oda.Fill(DrvDetail)
                    Dim Drivername As String = DrvDetail.Rows(0).Item("DriverName").ToString()
                    Dim MailTo As String = DTADMIN.Rows(0).Item("emailid").ToString()
                    Dim UserName As String = DTADMIN.Rows(0).Item("UserName").ToString()
                    Dim MailSub As String = "Notification of trip edited by user "
                    Dim MSG As String = ""

                    ' after total km
                    oda.SelectCommand.CommandText = "select Total_Distance from MMM_MST_ELOGBOOK e , MMM_MST_USER u where   e.uid=u.uid and e.uid=" & DropDownList1.SelectedValue & "  and Trip_start_DateTime >= '" & SDate1.Text & "  00:00:00.000'   AND (Trip_end_DateTime <= '" & SDate2.Text & "  23:59:59.000' or trip_end_Datetime is null or trip_end_datetime='')and e.tid=" & ViewState("tid") & ""
                    oda.SelectCommand.CommandType = CommandType.Text
                    Dim km As New DataTable
                    oda.Fill(km)
                    Dim Akm As String = km.Rows(0).Item("Total_Distance").ToString()

                    '
                    MSG = "<p style=""color:black"">Dear Sir/Mam,<br/><br/></p>"
                    MSG = MSG & "Please note that vehicle user has edited the trip and details are as below under : " & "<br/><br/>"
                    Dim MailTable As New StringBuilder()
                    MailTable.Append("<table border=""1"" width=""100%"">")
                    MailTable.Append("<tr style=""background-color:#990000"" Font-Bold=""True""> ")  ' for header only 
                    MailTable.Append("<td >" & "Vehicle No." & "</td>")
                    MailTable.Append("<td >" & "User Name" & "</td>")
                    MailTable.Append("<td >" & "Driver Name" & "</td>")
                    MailTable.Append("<td >" & "Trip Start Date Time (before editing)" & "</td>")
                    MailTable.Append("<td >" & "Trip End Date Time (before editing)" & "</td>")
                    MailTable.Append("<td >" & "Trip Start Date Time (after editing)" & "</td>")
                    MailTable.Append("<td >" & "Trip End Date Time(after editing)" & "</td>")
                    MailTable.Append("<td >" & "Total km(before editing)" & "</td>")
                    MailTable.Append("<td >" & "Total km(after editing)" & "</td>")
                    MailTable.Append("</tr><tr>") ' for row records
                    MailTable.Append("<td>" & VehicleNo.Text & " </td>")
                    MailTable.Append("<td>" & UserName & " </td>")
                    MailTable.Append("<td>" & Drivername & " </td>")
                    MailTable.Append("<td>" & ViewState("StartTime") & " </td>")
                    MailTable.Append("<td>" & ViewState("EndTime") & " </td>")
                    MailTable.Append("<td>" & dtst & " </td>")
                    MailTable.Append("<td>" & dtet & " </td>")
                    MailTable.Append("<td>" & ViewState("Totalkm") & " </td>")
                    MailTable.Append("<td>" & Akm & " </td>")
                    MailTable.Append("</tr>")
                    MailTable.Append("</table>")

                    MSG = MSG & MailTable.ToString & "<br/><br/>"
                    MSG = MSG & "Regards" & "<br/>"
                    MSG = MSG & "R4G Vehicle Management System Team"

                    'sendMail1(MailTo, "", "", MailSub, MSG)
                    ' sendMail1(MailTo, "sanjay.kumar@hfcl.com,finance.support@hfcl.com", "sachin.madaan@myndsol.com,ravi.sharma@myndsol.com", MailSub, MSG)
                    oda.SelectCommand.CommandText = "select m.fld16[VendorEMail] from mmm_mst_master m inner join mmm_mst_master m1 on m1.fld12=m.tid where m1.fld10='" & VehicleNo.Text & "' and m.documenttype='Vendor' and m1.documenttype='Vehicle' "
                    oda.SelectCommand.CommandType = CommandType.Text
                    Dim DTVENDOR As New DataTable
                    oda.Fill(DTVENDOR)
                    ' MailTo = DTVENDOR.Rows(0).Item("VendorEMail").ToString()
                    sendMail1(DTVENDOR.Rows(0).Item("VendorEMail").ToString(), MailTo & "sanjay.kumar@hfcl.com,finance.support@hfcl.com", "bpm.mynd@gmail.com,ravi.sharma@myndsol.com", MailSub, MSG)
                    'sendMail1(MailTo, "", "", MailSub, MSG)
                Catch ex As Exception
                End Try
            End If
            ' END TRIP EDITED MAIL TO VENDOR
        Catch ex As Exception
        Finally
            con.Close()
            oda.Dispose()
            con.Dispose()
        End Try
        btnEdit_ModalPopupExtender.Hide()
        updatePanelEdit.Update()
    End Sub
    Private Sub sendMail1(ByVal Mto As String, ByVal cc As String, ByVal bcc As String, ByVal MSubject As String, ByVal MBody As String)
        Try
            If Left(Mto, 1) = "{" Then
                Exit Sub
            End If
            Dim Email As New System.Net.Mail.MailMessage("no-reply@myndsol.com", Mto, MSubject, MBody)
            Dim mailClient As New System.Net.Mail.SmtpClient()
            Email.IsBodyHtml = True
            If cc <> "" Then
                Email.CC.Add(cc)
            End If

            If bcc <> "" Then
                Email.Bcc.Add(bcc)
            End If

            Dim basicAuthenticationInfo As New System.Net.NetworkCredential("no-reply@myndsol.com", "smaca")
            mailClient.Host = "mail.myndsol.com"
            mailClient.UseDefaultCredentials = False
            mailClient.Credentials = basicAuthenticationInfo
            'mailClient.DeliveryMethod = System.Net.Mail.SmtpDeliveryMethod.PickupDirectoryFromIis
            Try
                mailClient.Send(Email)
            Catch ex As Exception
                Exit Sub
            End Try

        Catch ex As Exception
            Exit Sub
        End Try
    End Sub
    Protected Sub gvData_PageIndexChanging(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.GridViewPageEventArgs) Handles gvData.PageIndexChanging
        Try
            gvData.PageIndex = e.NewPageIndex
            bindGridLoc()
        Catch ex As Exception
        Finally

        End Try
    End Sub
    Protected Sub typechange(ByVal sender As Object, ByVal e As System.EventArgs)
        Try

        
        If Triptype.SelectedItem.Text.ToUpper() = "AUTOMATIC" Then
            IMEINO.Visible = True
            IMELabel.Visible = True
            imgbtnloc.Visible = True
            imgbtnloc1.Visible = True
            Vehicle.Visible = True
            VehicleNo.Visible = True
            Label1.Visible = True
            Label2.Visible = True
            'gvData.Visible = True
            Date1.Text = Nothing
            Date2.Text = Nothing
            'VehicalNoforManual.Visible = False
            'VehicalManual.Visible = False
            StartLocation.Visible = False
            Sloc.Visible = False
            EndLocation.Visible = False
            Eloc.Visible = False
            Tdistance.Visible = False
            todistance.Visible = False
            km.Visible = False
            'GridViewmanual.Visible = False
        End If
        If Triptype.SelectedItem.Text.ToUpper = "MANUAL" Then
            ' IMEINO.Visible = False
            ' IMELabel.Visible = False
            imgbtnloc.Visible = False
            imgbtnloc1.Visible = False
            '   Vehicle.Visible = False
            '  VehicleNo.Visible = False
            Label1.Visible = False
            Label2.Visible = False
            ' gvData.Visible = False
            'VehicalNoforManual.Visible = True
            'VehicalManual.Visible = True
            StartLocation.Visible = True
            Sloc.Visible = True
            EndLocation.Visible = True
            Eloc.Visible = True
            Tdistance.Visible = True
            todistance.Visible = True
            Date1.Text = Nothing
            Date2.Text = Nothing
            km.Visible = True
            Sloc.Text = Nothing
            Eloc.Text = Nothing
            todistance.Text = Nothing
            ' GridViewmanual.Visible = True
        End If
            updatePanelEdit.Update()
        Catch ex As Exception
        Finally

        End Try
    End Sub
    Protected Sub DeleteRecord(ByVal sender As Object, ByVal e As System.EventArgs)
        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim con As SqlConnection = New SqlConnection(conStr)
        Dim oda As SqlDataAdapter = New SqlDataAdapter("uspDeleteELOGBOOK", con)
        Try
        oda.SelectCommand.CommandType = CommandType.StoredProcedure
        oda.SelectCommand.Parameters.AddWithValue("tid", Val(ViewState("Did").ToString))
        If con.State <> ConnectionState.Open Then
            con.Open()
        End If
        oda.SelectCommand.ExecuteNonQuery()
        Dim ds As New DataSet()
        oda.SelectCommand.CommandText = "select vehicle_no,IMEI_NO,convert(nvarchar,Trip_Start_DateTime,120)[Trip_Start_DateTime],convert(nvarchar,Trip_end_DateTime,120)[Trip_end_DateTime],Trip_Start_Location,Trip_End_Location,Total_Distance,tid,UserName,TripType ,Islock=CASE islock  when 0 THEN 'Active'WHEN 1 THEN 'InActive' END, case e.isauth when 0 then 'Pending for approval' else 'Approved' end[Status],e.isauth from MMM_MST_ELOGBOOK e , MMM_MST_USER u where   e.uid=u.uid and e.uid=" & DropDownList1.SelectedValue & "  and e.isauth=0   order by Trip_Start_DateTime,Trip_end_DateTime"
        oda.SelectCommand.CommandType = CommandType.Text
        oda.Fill(ds, "data")
        gvData.DataSource = ds.Tables("data")
        gvData.DataBind()
        lblRecord.Visible = True
        lblRecord.Text = " Trip Deleted Successfully"
        Me.updatePanelEdit.Update()
            Me.btnDelete_ModalPopupExtender.Hide()
        Catch ex As Exception
        Finally
            con.Close()
            oda.Dispose()
            con.Dispose()
        End Try
    End Sub
    Protected Sub Page_Load(sender As Object, e As System.EventArgs) Handles Me.Load
        If Not IsPostBack Then
            DropDownList1.Items.Clear()
            Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
            Dim con As New SqlConnection(conStr)
            Dim da As New SqlDataAdapter("", con)
            Dim ds As New DataSet
            Try
                If Session("USERROLE").ToString.ToUpper = "SU" Or Session("USERROLE").ToString.ToUpper = "FCAGGN" Or Session("USERROLE").ToString.ToUpper = "CADMIN" Then
                    'fill Users  
                    ImageButton3.Visible = True
                    btnnil.Visible = False
                    da.SelectCommand.CommandText = "select username,uid from mmm_mst_user where eid=" & Session("EID") & " and userrole not in ('su')  order by username"
                    da.Fill(ds, "user")
                    DropDownList1.Items.Add("Select")
                    DropDownList1.Items(0).Value = 0
                    For i As Integer = 0 To ds.Tables("user").Rows.Count - 1
                        DropDownList1.Items.Add(ds.Tables("user").Rows(i).Item("username").ToString())
                        DropDownList1.Items(i + 1).Value = ds.Tables("user").Rows(i).Item("uid").ToString()
                    Next
                ElseIf Session("USERROLE").ToString.ToUpper = "USER" Then
                    ImageButton3.Visible = False
                    btnnil.Visible = True
                    btnNew.Visible = True
                    DropDownList1.Items.Add(Session("USERNAME"))
                    DropDownList1.Items(0).Value = Session("UID")
                    da.SelectCommand.CommandText = "select vehicle_no,IMEI_NO,convert(nvarchar,Trip_Start_DateTime,120)[Trip_Start_DateTime],convert(nvarchar,Trip_end_DateTime,120)[Trip_end_DateTime],Trip_Start_Location,Trip_End_Location,Total_Distance,tid,UserName,TripType ,Islock=CASE islock  when 0 THEN 'Active'WHEN 1 THEN 'InActive' END, case e.isauth when 0 then 'Pending for approval' else 'Approved' end[Status],e.isauth from MMM_MST_ELOGBOOK e , MMM_MST_USER u where   e.uid=u.uid and e.uid=" & DropDownList1.SelectedValue & "  and e.isauth=0   order by Trip_Start_DateTime,Trip_end_DateTime"
                    da.SelectCommand.CommandType = CommandType.Text
                    ' Dim ds As New DataSet()
                    da.Fill(ds, "data")
                    gvData.DataSource = ds.Tables("data")
                    gvData.DataBind()
                    If gvData.Rows.Count = 0 Then
                        lblRecord.Visible = True
                        lblRecord.Text = "No Record Found"
                    Else
                        lblRecord.Text = ""
                    End If
                Else
                    ImageButton3.Visible = True
                    btnnil.Visible = False
                    btnNew.Visible = False
                    da.SelectCommand.CommandText = "select count(*) from mmm_ref_role_user where uid=" & Session("UID") & ""
                    If con.State <> ConnectionState.Open Then
                        con.Open()
                    End If
                    Dim cnt As Integer = da.SelectCommand.ExecuteScalar()
                    If cnt > 0 Then
                        da.SelectCommand.CommandText = "select username,uid from mmm_mst_user where eid=" & Session("EID") & " and userrole not in ('su') and fld15 in (select * from inputstring((select top 1 fld4 from mmm_ref_role_user where UID=" & Session("UID") & ")))  order by username "
                        da.Fill(ds, "user")
                        DropDownList1.Items.Add("Select")
                        DropDownList1.Items(0).Value = 0
                        For i As Integer = 0 To ds.Tables("user").Rows.Count - 1
                            DropDownList1.Items.Add(ds.Tables("user").Rows(i).Item("username").ToString())
                            DropDownList1.Items(i + 1).Value = ds.Tables("user").Rows(i).Item("uid").ToString()
                        Next
                    Else
                        ImageButton3.Visible = False
                        DropDownList1.Items.Add(Session("USERNAME"))
                        DropDownList1.Items(0).Value = Session("UID")
                    End If
                End If
                Dim str As String = ""
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
                'bindGridLoc()
            Catch ex As Exception
            Finally
                con.Close()
                da.Dispose()
                con.Dispose()
            End Try
        End If
    End Sub
    Protected Sub imgbtnloc_Click1(sender As Object, e As ImageClickEventArgs) Handles imgbtnloc.Click
        Dim dtst As String
        Dim crrdate As String = Date.Now.ToString("yyyy-MM-dd HH:mm")
        dtst = Date1.Text & " " & ddlshh.SelectedItem.Text & ":" & ddlsmm.SelectedItem.Text
        If Date1.Text = "" Then
            lblMsgEdit.Text = "Please Select Start Date"
            Exit Sub
        ElseIf DateTime.Parse(dtst) > DateTime.Parse(crrdate) Then
            lblMsgEdit.Text = "Future date/time is not allowed in log book entry"
            Exit Sub
        Else
            Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
            Dim con As SqlConnection = New SqlConnection(conStr)
            Dim oda1 As SqlDataAdapter = New SqlDataAdapter("select top 1 lattitude,longitude,ctime,speed,distancetravel from MMM_MST_GPSDATA where tid in (select max(tid) from MMM_MST_GPSDATA where IMIENO='" & IMEINO.Text & "'  and   DATEADD(minute,DATEDIFF(minute,0,ctime),0)  >= '" + Date1.Text + " " + ddlshh.SelectedItem.Text & ":" & ddlsmm.SelectedItem.Text + "' group by lattitude,longitude) order by DATEADD(minute,DATEDIFF(minute,0,ctime),0) ", con)
            Dim dtlatlong As DataTable = New DataTable()
            oda1.Fill(dtlatlong)
            Try
                If dtlatlong.Rows.Count > 0 Then
                    Dim gpsdata As GPSClass = New GPSClass()
                    Label1.Text = gpsdata.Location(dtlatlong.Rows(0).ItemArray(0).ToString(), dtlatlong.Rows(0).ItemArray(1).ToString())
                    Me.updatePanelEdit.Update()
                Else
                    Label1.Text = "To be further updated while Vehicle is not running "
                End If
            Catch ex As Exception
            Finally
                con.Close()
                oda1.Dispose()
                con.Dispose()
            End Try
        End If
    End Sub
    Protected Sub imgbtnloc1_Click1(sender As Object, e As ImageClickEventArgs) Handles imgbtnloc1.Click
        Dim dtet As String
        Dim crrdate As String = Date.Now.ToString("yyyy-MM-dd HH:mm")
        dtet = Date2.Text & " " & ddlehh.SelectedItem.Text & ":" & ddlemm.SelectedItem.Text
        If Date2.Text = "" Then
            lblMsgEdit.Text = "Please Select End Date"
            Exit Sub
        ElseIf DateTime.Parse(dtet) > DateTime.Parse(crrdate) Then
            lblMsgEdit.Text = "Future date/time is not allowed in log book entry"
            Exit Sub
        Else
            Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
            Dim con As SqlConnection = New SqlConnection(conStr)
            Dim oda1 As SqlDataAdapter = New SqlDataAdapter("select  top 1 lattitude,longitude,ctime,speed,distancetravel from MMM_MST_GPSDATA where tid in (select max(tid) from MMM_MST_GPSDATA where IMIENO='" & IMEINO.Text & "'  and  convert(nvarchar(16),cTime,121)  >= convert(nvarchar,'" + Date2.Text + " " + ddlehh.SelectedItem.Text & ":" & ddlemm.SelectedItem.Text + "',120) group by lattitude,longitude) order by ctime ", con)
            Dim dtlatlong As DataTable = New DataTable()
            oda1.Fill(dtlatlong)
            Try
                If dtlatlong.Rows.Count > 0 Then
                    Dim gpsdata As GPSClass = New GPSClass()
                    Label2.Text = gpsdata.Location(dtlatlong.Rows(0).ItemArray(0).ToString(), dtlatlong.Rows(0).ItemArray(1).ToString())
                    Me.updatePanelEdit.Update()
                Else
                    Label2.Text = "To be further updated while Vehicle is not running "
                End If
            Catch ex As Exception
            Finally
                con.Close()
                oda1.Dispose()
                con.Dispose()
            End Try
        End If
    End Sub
    Public Sub NilShow()
        niltripVehicle.Text = ""
        niltripIMEI.Text = ""
        Label1.Text = ""
        Label2.Text = ""
        lblRecord.Text = ""
        lblniltrip.Text = ""
        btnniltripmodelpopup.Show()
    End Sub
    Public Sub NilTrip()
        If DropDownList1.SelectedItem.Text.ToUpper = "SELECT" Then
            lblRecord.Text = "Must select one user"
            Exit Sub
        End If
        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim con As SqlConnection = New SqlConnection(conStr)
        Dim Udtype As String
        Dim Ufld As String
        Dim UVfld As String
        Dim Vdtype As String
        Dim Vfld As String
        Dim vemei As String
        VehicleNo.Enabled = False
        IMEINO.Enabled = False
        niltripVehicle.Text = ""
        niltripIMEI.Text = ""
        Label1.Text = ""
        Label2.Text = ""
        lblniltrip.Text = ""
        lblRecord.Text = ""
        Dim oda As SqlDataAdapter = New SqlDataAdapter("", con)
        Try
            oda.SelectCommand.CommandText = "select* from mmm_mst_entity where eid=" & Session("EID") & " "
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
            ' ViewState("Ndate") = nilstartdate.Text
            'Old Code
            'oda.SelectCommand.CommandText = "select " & UVfld & " from mmm_mst_doc where documenttype='" & Udtype & "' and " & Ufld & " = " & DropDownList1.SelectedValue & " and curstatus='Allotted'"
            oda.SelectCommand.CommandText = "select " & UVfld & " from mmm_mst_doc where documenttype='" & Udtype & "' and " & Ufld & " = " & DropDownList1.SelectedValue & "  and curstatus in ('Allotted','Surrender','ARCHIVE') and CONVERT(datetime, fld17,3)<= convert(nvarchar,convert(datetime,'" & nilstartdate.Text & "'),21) and convert(nvarchar,convert(datetime,'" & nilstartdate.Text & "'),21)<=CONVERT(datetime, fld19,3) and CONVERT(datetime,fld19,3)>=convert(nvarchar,convert(datetime,'" & nilstartdate.Text & "'),21) and convert(nvarchar,convert(datetime,'" & nilstartdate.Text & "'),21) >= CONVERT(datetime, fld17,3)"
            oda.Fill(ds, "vrfdoc")
            If ds.Tables("vrfdoc").Rows.Count > 1 Then
                lblniltrip.Text = "Found multiple vehicles (can not create trip)"
                con.Dispose()
                Exit Sub
            End If

            'Old Code
            'oda.SelectCommand.CommandText = "select " & Vfld & "," & vemei & " from mmm_mst_master where eid=" & Session("EID") & " and documenttype='" & Vdtype & "' and tid=(select " & UVfld & " from mmm_mst_doc where documenttype='" & Udtype & "' and " & Ufld & " = " & DropDownList1.SelectedValue & " and curstatus='Allotted') "
            'New Change 12th APR 2014
            oda.SelectCommand.CommandText = "select " & Vfld & "," & vemei & " from mmm_mst_master where eid=" & Session("EID") & " and documenttype='" & Vdtype & "' and tid=(select " & UVfld & " from mmm_mst_doc where documenttype='" & Udtype & "' and " & Ufld & " = " & DropDownList1.SelectedValue & " and curstatus in ('Allotted','Surrender','ARCHIVE') and CONVERT(datetime, fld17,3)<= convert(nvarchar,convert(datetime,'" & nilstartdate.Text & "'),21) and convert(nvarchar,convert(datetime,'" & nilstartdate.Text & "'),21)<=CONVERT(datetime, fld19,3) and CONVERT(datetime,fld19,3)>=convert(nvarchar,convert(datetime,'" & nilstartdate.Text & "'),21) and convert(nvarchar,convert(datetime,'" & nilstartdate.Text & "'),21) >= CONVERT(datetime, fld17,3))"
            oda.Fill(ds, "vemei")
            If ds.Tables("vemei").Rows.Count = 0 Then
                lblniltrip.Text = "Dear User, Vehicle is not allocated to you. Please contact your admin."
                con.Dispose()
                Exit Sub
            End If

            If ds.Tables("vemei").Rows.Count > 0 Then
                If ds.Tables("vemei").Rows(0).Item(1).ToString.Length <> 0 Or ds.Tables("vemei").Rows(0).Item(0).ToString.Length <> 0 Then
                    niltripVehicle.Text = ds.Tables("vemei").Rows(0).Item(0).ToString
                    niltripIMEI.Text = ds.Tables("vemei").Rows(0).Item(1).ToString
                Else
                    If niltripVehicle.Text = "" Or niltripIMEI.Text = "" Then
                        'lblMsgEdit.Text = "GPS device is not allocated to the vehicle"
                        lblniltrip.Text = "GPS device is not allocated to the vehicle"
                        con.Dispose()
                        Exit Sub
                    End If
                End If
            End If
            btbniltripsave.Text = "Save"
            'lblniltrip.Text = ""
            'nilstartdate.Text = ""
            'nilenddate.Text = ""
            updatePanelEdit.Update()
            upniltrip.Update()
            ' oda.SelectCommand.CommandText = "insert   into mmm_mst_elogbook (trip_start_datetime,Trip_end_datetime) values(" + +"," + +")"
        Catch ex As Exception
        Finally
            con.Dispose()
            oda.Dispose()
        End Try
    End Sub
    Protected Sub niltripsave(sender As Object, e As System.EventArgs)
        lblRecord.Visible = True
        Dim constr As String = ConfigurationManager.ConnectionStrings("constr").ConnectionString
        Dim con As New SqlConnection(constr)
        Dim oda As New SqlDataAdapter("", con)
        Dim ds As New DataSet()

        'Dim parsed As DateTime
        'Dim valid As Boolean = DateTime.TryParseExact(Date1.Text, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, parsed)
        'If valid = False Then
        '    lblMsgEdit.Text = "Enter the Correct Format"
        '    Date1.Focus()
        '    Exit Sub
        'End If
        Try
            Dim parsed As DateTime
            Dim valid As Boolean = DateTime.TryParseExact(nilstartdate.Text, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, parsed)
            If valid = False Then
                lblniltrip.Text = "Enter the correct formate of start date"
                Exit Sub
            End If
            Dim validate As DateTime

            Dim valid2 As Boolean = DateTime.TryParseExact(nilenddate.Text, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, validate)
            If valid2 = False Then
                lblniltrip.Text = "Enter the correct formate of end date"
                Exit Sub
            End If
            If CDate(nilstartdate.Text + " 00:00") > CDate(nilenddate.Text + " 23:59") Then
                lblniltrip.Text = "Start should be less than end date"
                Exit Sub
            End If
            If Trim(niltripVehicle.Text) = "" Then
                lblniltrip.Text = "Update Vehicle No"
                Exit Sub

            End If
            If Trim(niltripIMEI.Text) = "" Then
                lblniltrip.Text = "Update IMEI No"
                Exit Sub

            End If
            If con.State <> ConnectionState.Open Then
                con.Open()
            End If
            If btnActUserSave.Text <> "Update" Then
                'Rajat lock already exist 
                oda.SelectCommand.CommandText = "select count(*) from (select trip_start_datetime from mmm_mst_elogbook  where tid=( select min(tid) from mmm_msT_elogbook where islock=1 and uid=1053) ) r1 ,(select trip_end_datetime from mmm_mst_elogbook  where tid=( select max(tid) from mmm_msT_elogbook where islock=1 and uid=1053)) r2 where (trip_start_datetime <='" & nilstartdate.Text & " 00:00'  and trip_end_datetime >='" & nilenddate.Text & " 23:59')  or (trip_start_datetime >='" & nilstartdate.Text & " 00:00'  and trip_end_datetime <='" & nilenddate.Text & " 23:59')"
                Dim tripal As Integer = oda.SelectCommand.ExecuteScalar()
                If tripal > 0 Then
                    lblniltrip.Text = "Your trip is locked between this period"
                    'con.Dispose()
                    Exit Sub
                End If
                'end
            End If



            If btbniltripsave.Text = "Update" Then
                'oda.SelectCommand.CommandText = "select count(*) from mmm_mst_elogbook where (vehicle_no='" & VehicleNo.Text & "' and  '" & Date1.Text & " " & ddlshh.SelectedItem.Text & ":" & ddlsmm.SelectedItem.Text & "' <= trip_end_Datetime and '" & Date1.Text & " " & ddlshh.SelectedItem.Text & ":" & ddlsmm.SelectedItem.Text & "' >= trip_start_Datetime and tid<>" & ViewState("tid") & ") or (imei_no='" & IMEINO.Text & "' and vehicle_no='" & VehicleNo.Text & "' and  '" & Date2.Text & " " & ddlehh.SelectedItem.Text & ":" & ddlemm.SelectedItem.Text & "' <= trip_end_Datetime and '" & Date2.Text & " " & ddlehh.SelectedItem.Text & ":" & ddlemm.SelectedItem.Text & "' >= trip_start_Datetime and tid<>" & ViewState("tid") & ") or (vehicle_no='" & VehicleNo.Text & "' and  '" & Date1.Text & " " & ddlshh.SelectedItem.Text & ":" & ddlsmm.SelectedItem.Text & "' <= trip_start_Datetime and  trip_end_Datetime<='" & Date2.Text & " " & ddlehh.SelectedItem.Text & ":" & ddlemm.SelectedItem.Text & "' and tid<>" & ViewState("tid") & ") "
                oda.SelectCommand.CommandText = "select count(*) from mmm_mst_elogbook where (uid=" & DropDownList1.SelectedValue & " and  '" & nilstartdate.Text & " 00:00' <= trip_end_Datetime and '" & nilstartdate.Text & " 23:59' >= trip_start_Datetime and tid<>" & ViewState("tid") & ") or (imei_no='" & niltripIMEI.Text & "' and uid='" & DropDownList1.SelectedValue & " and  '" & nilenddate.Text & " 23:59' <= trip_end_Datetime and '" & nilenddate.Text & " 23:59' >= trip_start_Datetime and tid<>" & ViewState("tid") & ") or (uid=" & DropDownList1.SelectedValue & " and  '" & nilstartdate.Text & " 00:00' <= trip_start_Datetime and  trip_end_Datetime<='" & nilenddate.Text & " 23:59' and tid<>" & ViewState("tid") & ") "
            Else
                'oda.SelectCommand.CommandText = "select count(*) from mmm_mst_elogbook where (vehicle_no='" & VehicleNo.Text & "' and  '" & Date1.Text & " " & ddlshh.SelectedItem.Text & ":" & ddlsmm.SelectedItem.Text & "' <= trip_end_Datetime and '" & Date1.Text & " " & ddlshh.SelectedItem.Text & ":" & ddlsmm.SelectedItem.Text & "' >= trip_start_Datetime) or (vehicle_no='" & VehicleNo.Text & "' and  '" & Date2.Text & " " & ddlehh.SelectedItem.Text & ":" & ddlemm.SelectedItem.Text & "' <= trip_end_Datetime and '" & Date2.Text & " " & ddlehh.SelectedItem.Text & ":" & ddlemm.SelectedItem.Text & "' >= trip_start_Datetime) or (vehicle_no='" & VehicleNo.Text & "' and  '" & Date1.Text & " " & ddlshh.SelectedItem.Text & ":" & ddlsmm.SelectedItem.Text & "' <= trip_start_Datetime and  trip_end_Datetime<='" & Date2.Text & " " & ddlehh.SelectedItem.Text & ":" & ddlemm.SelectedItem.Text & "') "
                oda.SelectCommand.CommandText = "select count(*) from mmm_mst_elogbook where (uid=" & DropDownList1.SelectedValue & " and  '" & nilstartdate.Text & " 00:00' <= trip_end_Datetime and '" & nilstartdate.Text & " 23:59' >= trip_start_Datetime) or (uid=" & DropDownList1.SelectedValue & " and  '" & nilenddate.Text & " 23:59' <= trip_end_Datetime and '" & nilenddate.Text & " 23:59' >= trip_start_Datetime) or (uid=" & DropDownList1.SelectedValue & " and  '" & nilstartdate.Text & " 00:00' <= trip_start_Datetime and  trip_end_Datetime<='" & nilenddate.Text & " 23:59') "
            End If
            ' oda.SelectCommand.CommandText = "select count(*) from mmm_mst_elogbook where vehicle_no='" & VehicleNo.Text & "' and  '" & Date1.Text & " " & ddlshh.SelectedItem.Text & ":" & ddlsmm.SelectedItem.Text & "' <= trip_start_Datetime and  trip_end_Datetime<='" & Date2.Text & " " & ddlehh.SelectedItem.Text & ":" & ddlemm.SelectedItem.Text & "'"
            Dim cnt As Integer = oda.SelectCommand.ExecuteScalar()

            If cnt > 0 Then
                lblniltrip.Text = "Trip already exist at this period. "
                con.Dispose()
                Exit Sub
            End If
            Dim tedate As String = ""
            'Updated By Rajat Bansal

            ' HisTory Code By Rajat Bansal
            oda.SelectCommand.CommandText = "select UVStartDateTime , UVEndDateTime   ,CurStatus,UVDType,UVUserField,UVVehicleField,VIDType,VIVehicleField,VIImeiField from mmm_mst_entity where eid=" & Session("eid") & "   "
            If con.State <> ConnectionState.Open Then
                con.Open()
            End If
            oda.SelectCommand.CommandType = CommandType.Text
            Dim entity As New DataTable
            oda.Fill(entity)
            If entity.Rows.Count = 0 Then
                oda.Dispose()
                con.Dispose()
                Exit Sub
            End If
            ''Start Rajat Bansal
            Dim startfld As String = entity.Rows(0).Item("UVStartDateTime").ToString
            Dim endfld As String = entity.Rows(0).Item("UVEndDateTime").ToString
            Dim userfld As String = entity.Rows(0).Item("UVUserField").ToString
            ''end
            Dim dt As New DataTable
            Dim doc As New DataTable
            oda.SelectCommand.CommandText = "select * from mmm_mst_doc where documenttype='" & entity.Rows(0).Item("UVDType") & "' and   " & entity.Rows(0).Item("UVUserField") & "='" & DropDownList1.SelectedValue & "' and curstatus in ('Allotted') "
            ''Start Rajat Bansal
            'oda.SelectCommand.CommandText = "select  * from mmm_mst_doc where documenttype='" & entity.Rows(0).Item("UVDType") & "' and convert(date," + startfld + ",5) <='" + nilstartdate.Text + "' and convert(date, " + endfld + ",5) >='" + nilenddate.Text + "' and " + userfld + "=" + DropDownList1.SelectedValue + " and curstatus in ('Allotted','surrender','Archive')"
            'Dim picvrf As New DataTable()
            'oda.Fill(picvrf)
            'If picvrf.Rows.Count = 0 Then
            '    lblRecord.Text = "Please enter correct date for trip"
            '    updatePanelEdit.Update()
            '    btnniltripmodelpopup.Hide()
            '    Exit Sub
            'Else
            '    lblMsgEdit.Text = ""
            'End If
            ''end 

            Dim elogbook As New DataTable
            oda.SelectCommand.CommandText = "select Trip_end_datetime from mmm_mst_elogbook where tid = (select max(tid) from mmm_mst_elogbook where vehicle_no='" & VehicleNo.Text & "' and uid=" + DropDownList1.SelectedValue + " and eid=" & Session("EID") & ")"
            oda.Fill(elogbook)
            If elogbook.Rows.Count > 0 Then
                tedate = oda.SelectCommand.ExecuteScalar().ToString
                If tedate = "" Then
                    lblniltrip.Text = "Please end previous trip before starting new trip/updating trip "
                    oda.Dispose()
                    con.Dispose()
                    Exit Sub
                End If
            End If
            If btbniltripsave.Text = "Save" Then
                oda.SelectCommand.CommandText = "insert   into mmm_mst_elogbook (vehicle_no,IMEI_NO,uid,trip_start_datetime,Trip_end_datetime,Triptype,islock,eid) values('" + niltripVehicle.Text + "','" + niltripIMEI.Text + "'," & DropDownList1.SelectedValue & ",'" + nilstartdate.Text + " 00:00','" + nilenddate.Text + " 23:59','Nil',0," + Session("eid").ToString + ")"
                oda.SelectCommand.CommandType = CommandType.Text
                SDate1.Text = nilstartdate.Text
                SDate2.Text = nilenddate.Text
                Dim str As String = oda.SelectCommand.ExecuteNonQuery().ToString
                lblRecord.Text = "Nil trip has been created successfully"
                oda.SelectCommand.CommandText = "select vehicle_no,IMEI_NO,convert(nvarchar,Trip_Start_DateTime,120)[Trip_Start_DateTime],convert(nvarchar,Trip_end_DateTime,120)[Trip_end_DateTime],Trip_Start_Location,Trip_End_Location,Total_Distance,tid,UserName,TripType ,Islock=CASE islock  when 0 THEN 'Unlocked'WHEN 1 THEN 'Locked' END, case e.isauth when 0 then 'Pending for approval' else 'Approved' end[Status],e.isAuth from MMM_MST_ELOGBOOK e , MMM_MST_USER u where   e.uid=u.uid and e.uid=" & DropDownList1.SelectedValue & "  and e.tid= (select max(tid) from mmm_mst_elogbook where uid=" & DropDownList1.SelectedValue & ")"
                oda.SelectCommand.CommandType = CommandType.Text

            ElseIf btbniltripsave.Text = "Update" Then
                oda.SelectCommand.CommandText = "Update  mmm_mst_elogbook set trip_start_datetime='" + nilstartdate.Text + " 00:00',Trip_end_datetime='" + nilenddate.Text + " 23:59' where tid=" + ViewState("tid").ToString + " "
                oda.SelectCommand.CommandType = CommandType.Text
                lblRecord.Text = "Nil trip has been updated successfully"
                oda.SelectCommand.ExecuteNonQuery()
                oda.SelectCommand.CommandText = "select vehicle_no,IMEI_NO,convert(nvarchar,Trip_Start_DateTime,120)[Trip_Start_DateTime],convert(nvarchar,Trip_end_DateTime,120)[Trip_end_DateTime],Trip_Start_Location,Trip_End_Location,Total_Distance,tid,UserName,TripType ,Islock=CASE islock  when 0 THEN 'Unlocked'WHEN 1 THEN 'Locked' END, case e.isauth when 0 then 'Pending for approval' else 'Approved' end[Status],e.isAuth from MMM_MST_ELOGBOOK e , MMM_MST_USER u where   e.uid=u.uid and e.uid=" & DropDownList1.SelectedValue & "  and e.tid= " & ViewState("tid").ToString & ""
                oda.SelectCommand.CommandType = CommandType.Text
            End If

            oda.Fill(ds, "data")
            gvData.DataSource = ds.Tables("data")
            gvData.DataBind()
            upniltrip.Update()
            updatePanelEdit.Update()
            ' oda.SelectCommand.CommandText = "insert   into mmm_mst_elogbook (trip_start_datetime,Trip_end_datetime) values(" + +"," + +")"
            btnniltripmodelpopup.Hide()

        Catch ex As Exception
        Finally
            con.Close()
            oda.Dispose()
            con.Dispose()
        End Try
    End Sub
    Protected Sub gvData_RowDataBound(sender As Object, e As System.Web.UI.WebControls.GridViewRowEventArgs) Handles gvData.RowDataBound
        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim con As SqlConnection = New SqlConnection(conStr)
        Dim oda As SqlDataAdapter = New SqlDataAdapter("", con)
        Dim ds As New DataSet
        'If btbniltripsave.Text = "Save" Then
        '    oda.SelectCommand.CommandText = "select vehicle_no,IMEI_NO,convert(nvarchar,Trip_Start_DateTime,120)[Trip_Start_DateTime],convert(nvarchar,Trip_end_DateTime,120)[Trip_end_DateTime],Trip_Start_Location,Trip_End_Location,Total_Distance,tid,UserName,TripType ,Islock=CASE islock  when 0 THEN 'Unlocked'WHEN 1 THEN 'Locked' END, case e.isauth when 0 then 'Pending for approval' else 'Approved' end[Status],e.isAuth from MMM_MST_ELOGBOOK e , MMM_MST_USER u where   e.uid=u.uid and e.uid=" & DropDownList1.SelectedValue & "  and e.tid= (select max(tid) from mmm_mst_elogbook where uid=" & DropDownList1.SelectedValue & ")"
        '    oda.SelectCommand.CommandType = CommandType.Text

        'ElseIf btbniltripsave.Text = "Update" Then
        '    oda.SelectCommand.CommandText = "select vehicle_no,IMEI_NO,convert(nvarchar,Trip_Start_DateTime,120)[Trip_Start_DateTime],convert(nvarchar,Trip_end_DateTime,120)[Trip_end_DateTime],Trip_Start_Location,Trip_End_Location,Total_Distance,tid,UserName,TripType ,Islock=CASE islock  when 0 THEN 'Unlocked'WHEN 1 THEN 'Locked' END, case e.isauth when 0 then 'Pending for approval' else 'Approved' end[Status],e.isAuth from MMM_MST_ELOGBOOK e , MMM_MST_USER u where   e.uid=u.uid and e.uid=" & DropDownList1.SelectedValue & "  and e.tid= " & ViewState("tid").ToString & ""
        '    oda.SelectCommand.CommandType = CommandType.Text

        'Else
        Try
            oda.SelectCommand.CommandText = "select vehicle_no,IMEI_NO,convert(nvarchar,Trip_Start_DateTime,120)[Trip_Start_DateTime],convert(nvarchar,Trip_end_DateTime,120)[Trip_end_DateTime],Trip_Start_Location,Trip_End_Location,Total_Distance,tid,UserName,TripType ,Islock=CASE islock  when 0 THEN 'Active'WHEN 1 THEN 'InActive' END,e.isauth from MMM_MST_ELOGBOOK e , MMM_MST_USER u where   e.uid=u.uid and e.uid=" & DropDownList1.SelectedValue & "  and Trip_start_DateTime >= '" & SDate1.Text & "  00:00:00.000'   AND (Trip_end_DateTime <= '" & SDate2.Text & "  23:59:59.000' or trip_end_Datetime is null or trip_end_datetime='')   order by Trip_Start_DateTime,Trip_end_DateTime"
            oda.SelectCommand.CommandType = CommandType.Text

            'End If
            ' Dim ds As New DataSet()
            oda.Fill(ds, "data")
            'Dim isauth As Integer
            If Session("USERROLE").ToString.ToUpper = "SU" Or Session("USERROLE").ToString.ToUpper = "ADMIN" Then
                If e.Row.RowType = DataControlRowType.DataRow Then
                    e.Row.Cells(11).Visible = True
                End If
                If e.Row.RowType = DataControlRowType.Header Then
                    e.Row.Cells(11).Visible = True
                End If
            Else
                If e.Row.RowType = DataControlRowType.DataRow Then
                    e.Row.Cells(11).Visible = False
                End If
                If e.Row.RowType = DataControlRowType.Header Then
                    e.Row.Cells(11).Visible = False
                End If
            End If
        Catch ex As Exception
        Finally
            con.Close()
            oda.Dispose()
            con.Dispose()
        End Try
    End Sub
End Class
