Imports System.Data.SqlClient
Imports System.Data
Imports System.Windows.Forms

Partial Class Elogbook_Settings
    Inherits System.Web.UI.Page
    Dim ConStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
    Dim con As SqlConnection = New SqlConnection(ConStr)
    Public Event ItemCheck As ItemCheckEventHandler
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
    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        Try
            If Not IsPostBack Then
                FillDocDDLS()
                FillElogbookDDL()
                Dim da As New SqlDataAdapter("", ConStr)
                da.SelectCommand.CommandText = "delete from mmm_mst_elogbookSettings where isactive=0 ; SELECT * from mmm_mst_elogbookSettings where isactive =1 and  eid =" & HttpContext.Current.Session("EID")
                da.SelectCommand.CommandType = CommandType.Text
                Dim dt As New DataTable()
                da.Fill(dt)
                If (dt.Rows.Count > 0) Then

                    AssignVlauestoControls(dt)
                    BindElogbookGrid()
                    BindElogbookDetGrid()

                Else

                    Dim cmd As New SqlCommand()
                    cmd.Connection = con
                    cmd.CommandText = "delete from mmm_mst_elogbookSettings where eid = " & HttpContext.Current.Session("EID")
                    con.Open()
                    cmd.ExecuteNonQuery()
                    con.Close()
                    gvElogbookflds.DataSource = Nothing
                    gvElogbookflds.DataBind()
                    gvElogbookDetFlds.DataSource = Nothing
                    gvElogbookDetFlds.DataBind()
                    ddlVehicleDoc_SelectedIndexChanged(Nothing, Nothing)
                    ddlSiteDoc_SelectedIndexChanged(Nothing, Nothing)
                    btnSaveSettings.Text = "SAVE"

                End If

            End If
            lblMsg.Text = ""
        Catch ex As Exception

        End Try

    End Sub

    Protected Sub ddlVehicleDoc_SelectedIndexChanged(sender As Object, e As EventArgs) Handles ddlVehicleDoc.SelectedIndexChanged
        Try

            txtVehIMEIFld.Text = ""
            lblVehFldMaping.Text = ""
            Dim da As New SqlDataAdapter("", ConStr)
            Dim cmd As New SqlCommand()
            Dim dt As New DataTable
            If (ddlVehicleDoc.SelectedValue <> "0") Then

                cmd.Connection = con
                cmd.CommandText = "Select DisplayName+':'+FieldMapping from mmm_mst_Fields with(nolock) where documenttype ='" & ddlVehicleDoc.SelectedValue & "' and DisplayName like '%IMEI%' and Eid=" & Session("Eid")
                If (con.State = ConnectionState.Closed) Then
                    con.Open()
                End If
                Dim IsIMEI = cmd.ExecuteScalar()
                con.Close()
                If (IsIMEI <> Nothing) Then
                    If (IsIMEI <> "") Then
                        txtVehIMEIFld.Text = IsIMEI.ToString().Split(":")(0)
                        lblVehFldMaping.Text = IsIMEI.ToString().Split(":")(1)
                        da.SelectCommand.CommandText = "Select FieldId, Displayname  from mmm_mst_Fields with(nolock) where documenttype ='" & ddlVehicleDoc.SelectedValue & "'   and Eid=" & Session("Eid")
                        da.Fill(dt)
                        If dt.Rows.Count > 0 Then
                            chkLVehicle.DataSource = dt
                            chkLVehicle.DataTextField = "Displayname"
                            chkLVehicle.DataValueField = "FieldId"
                            chkLVehicle.DataBind()
                        End If
                    End If
                Else : lblGridErr.Text = "Invalid Document Selection for 'Vehicle Document'"
                End If
            End If
        Catch ex As Exception

        End Try

    End Sub

    Protected Sub ddlSiteDoc_SelectedIndexChanged(sender As Object, e As EventArgs) Handles ddlSiteDoc.SelectedIndexChanged
        Try

            Dim da As New SqlDataAdapter("", ConStr)
            Dim dt As New DataTable
            da.SelectCommand.CommandText = "Select FieldId, Displayname  from mmm_mst_Fields with(nolock) where documenttype ='" & ddlSiteDoc.SelectedValue & "' and Eid=" & Session("Eid")
            da.Fill(dt)

            If dt.Rows.Count > 0 Then
                chkLSite.DataSource = dt
                chkLSite.DataTextField = "Displayname"
                chkLSite.DataValueField = "FieldId"
                chkLSite.DataBind()
            End If

        Catch ex As Exception

        End Try
    End Sub

    Sub FillElogbookDDL()
        Try
            Dim da As New SqlDataAdapter("", ConStr)
            Dim dt As New DataTable
            da.SelectCommand.CommandText = "SELECT COLUMN_NAME [ColName] FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'mmm_mst_elogbook'"
            da.Fill(dt)
            If dt.Rows.Count > 0 Then
                chkLELogbook.DataSource = dt
                chkLELogbookDet.DataSource = dt
                chkLELogbook.DataTextField = "ColName"
                chkLELogbook.DataValueField = "ColName"
                chkLELogbookDet.DataTextField = "ColName"
                chkLELogbookDet.DataValueField = "ColName"
                chkLELogbook.DataBind()
                chkLELogbookDet.DataBind()
                chkLELogbook.Items.FindByValue("vehicle_no").Selected = True
                chkLELogbook.Items.FindByValue("IMEI_NO").Selected = True
                chkLELogbook.Items.FindByValue("Trip_Start_DateTime").Selected = True
                chkLELogbook.Items.FindByValue("Trip_end_DateTime").Selected = True
                chkLELogbookDet.Items.FindByValue("vehicle_no").Selected = True
                chkLELogbookDet.Items.FindByValue("IMEI_NO").Selected = True
                chkLELogbookDet.Items.FindByValue("Trip_Start_DateTime").Selected = True
                chkLELogbookDet.Items.FindByValue("Trip_end_DateTime").Selected = True

            End If
        Catch ex As Exception

        End Try
    End Sub

    Sub FillDocDDLS()

        Try
            Dim da As New SqlDataAdapter("", ConStr)
            Dim dt As New DataTable
            da.SelectCommand.CommandText = "select distinct m.Documenttype from mmm_mst_master m inner join" &
              " mmm_mst_fields f on m.eid =f.eid and m.documenttype =f.documenttype where f.displayname like '%IMEI%' and m.documenttype is not null and m.Eid=" & Session("Eid")
            da.Fill(dt)
            If dt.Rows.Count > 0 Then

                ddlVehicleDoc.DataSource = dt
                ddlVehicleDoc.DataTextField = "Documenttype"
                ddlVehicleDoc.DataValueField = "Documenttype"
                ddlVehicleDoc.DataBind()
                ddlVehicleDoc.Items.Insert(0, New ListItem("--Select--", "0"))

            End If

            da.SelectCommand.CommandText = "select distinct m.Documenttype from mmm_mst_master m where m.documenttype is not null and m.Eid=" & Session("Eid")
            da.Fill(dt)
            If dt.Rows.Count > 0 Then
                ddlSiteDoc.DataSource = dt
                ddlSiteDoc.DataTextField = "Documenttype"
                ddlSiteDoc.DataValueField = "Documenttype"
                ddlSiteDoc.DataBind()
                ddlSiteDoc.Items.Insert(0, New ListItem("--Select--", "0"))

            End If

        Catch ex As Exception

        End Try


    End Sub

    Function ValidateData() As Boolean
        Try
            ' Dim cmd As New SqlCommand()
            Dim isvalid As Boolean = True
            'Chart Validate()
            If (chkVChartVis.Checked = True) Then
                If (txtVChartNm.Text = "") Then
                    isvalid = False
                    lblChartErr.Text = "Please enter Display text for Vehicle Chart"
                    Return isvalid
                End If
            ElseIf (chkSChartVis.Checked = True) Then
                If (txtSChartNm.Text = "") Then
                    isvalid = False
                    lblChartErr.Text = "Please enter Display text for Site Chart"
                    Return isvalid
                End If
            ElseIf (chkKChartVis.Checked = True) Then
                If (txtKChartNm.Text = "") Then
                    isvalid = False
                    lblChartErr.Text = "Please enter Display text for Kms Chart"
                    Return isvalid
                End If
            Else : lblChartErr.Text = ""
            End If


            'Grid Validate()
            If (chkLELogbook.SelectedValue = "") Then
                isvalid = False
                lblGridErr.Text = "Please select atleast one field from ELogbook"
                Return isvalid

            End If
            If (ddlVehicleDoc.SelectedValue = "0") Then
                isvalid = False
                lblGridErr.Text = "Please select Vehicle Document"
                Return isvalid
            ElseIf txtVehIMEIFld.Text = "" Or txtVehIMEIFld.Text = "0" Or lblVehFldMaping.Text = "" Then
                isvalid = False
                lblGridErr.Text = "Invalid Vehicle Document!"
                Return isvalid

            ElseIf (rbOnOff.SelectedIndex = -1) Then
                isvalid = False
                lblGridErr.Text = "Please specify Detail Section's ON/OFF"
                Return isvalid
            Else : lblGridErr.Text = ""
            End If

            'Detail Grid Validate()
            If (rbOnOff.SelectedValue = "1") Then


                If (chkLELogbookDet.SelectedValue = "") Then
                    isvalid = False
                    lblDetailErr.Text = "Please select atleast one field from ELogbook"
                    Return isvalid
                Else
                    lblDetailErr.Text = ""
                End If
                If (ddlSiteDoc.SelectedValue <> "0") Then
                    If (rbSiteOnOff.SelectedIndex = -1) Then
                        lblDetailErr.Text = "Please specify Site Type (From-To) On/Off."
                    End If
                End If

            End If
        Catch ex As Exception

        End Try

        Return IsValid

    End Function

    Protected Sub btnSaveSettings_Click(sender As Object, e As EventArgs) Handles btnSaveSettings.Click
        Try
            If (ValidateData()) Then
                Dim cmd As New SqlCommand()
                ' Activate ChartSettings 
                Dim tid = 0
                If (chkVChartVis.Checked = True) Then
                    tid = 0
                    If (btnSaveSettings.Text = "UPDATE") Then
                        cmd.Connection = con
                        cmd.CommandText = "select tid from mmm_mst_elogbooksettings where eid =" & Session("EID") & " and SettingType='Chart' and SettingName ='VehicleChart'"
                        con.Open()
                        tid = Convert.ToInt16(cmd.ExecuteScalar())
                        con.Close()
                    End If
                    esettings.DisplayText = txtVChartNm.Text
                    esettings.Field = "1"
                    esettings.IsActive = True
                    esettings.Sequence = 0
                    esettings.SettingName = "VehicleChart"
                    esettings.SettingType = "Chart"
                    esettings.Value = "1"
                    InsertSettings(tid)
                Else
                    cmd.Connection = con
                    cmd.CommandText = "Delete from mmm_mst_elogbooksettings where eid =" & Session("EID") & " and SettingType='Chart' and SettingName ='SiteChart'"
                    con.Open()
                    cmd.ExecuteNonQuery()
                    con.Close()
                End If
                If (chkSChartVis.Checked = True) Then
                    tid = 0
                    If (btnSaveSettings.Text = "UPDATE") Then
                        cmd.Connection = con
                        cmd.CommandText = "select tid from mmm_mst_elogbooksettings where eid =" & Session("EID") & " and SettingType='Chart' and SettingName ='SiteChart'"
                        con.Open()
                        tid = Convert.ToInt16(cmd.ExecuteScalar())
                        con.Close()
                    End If
                    esettings.DisplayText = txtSChartNm.Text
                    esettings.Field = "1"
                    esettings.IsActive = True
                    esettings.Sequence = 0
                    esettings.SettingName = "SiteChart"
                    esettings.SettingType = "Chart"
                    esettings.Value = "1"
                    InsertSettings(tid)
                Else
                    cmd.Connection = con
                    cmd.CommandText = "Delete from mmm_mst_elogbooksettings where eid =" & Session("EID") & " and SettingType='Chart' and SettingName ='SiteChart'"
                    con.Open()
                    cmd.ExecuteNonQuery()
                    con.Close()

                End If
                If (chkKChartVis.Checked = True) Then
                    tid = 0
                    If (btnSaveSettings.Text = "UPDATE") Then
                        cmd.Connection = con
                        cmd.CommandText = "select tid from mmm_mst_elogbooksettings where eid =" & Session("EID") & " and SettingType='Chart' and SettingName ='KmsChart'"
                        con.Open()
                        tid = Convert.ToInt16(cmd.ExecuteScalar())
                        con.Close()
                    End If
                    esettings.DisplayText = txtKChartNm.Text
                    esettings.Field = "1"
                    esettings.IsActive = True
                    esettings.Sequence = 0
                    esettings.SettingName = "KmsChart"
                    esettings.SettingType = "Chart"
                    esettings.Value = "1"
                    InsertSettings(tid)
                Else
                    cmd.Connection = con
                    cmd.CommandText = "Delete from mmm_mst_elogbooksettings where eid =" & Session("EID") & " and SettingType='Chart' and SettingName ='SiteChart'"
                    con.Open()
                    cmd.ExecuteNonQuery()
                    con.Close()
                End If

                'Activate MainGrid Settings 
                savegridfieldswithDisplaytext(gvElogbookflds, "txtlogbookdis")
                'Save VehicleDoc 
                If (ddlVehicleDoc.SelectedIndex > 0) Then
                    tid = 0
                    If (btnSaveSettings.Text = "UPDATE") Then
                        cmd.Connection = con
                        cmd.CommandText = "select tid from mmm_mst_elogbooksettings where eid =" & Session("EID") & " and SettingType='MainGrid' and SettingName ='VehicleDoc'"
                        con.Open()
                        tid = Convert.ToInt16(cmd.ExecuteScalar())
                        con.Close()
                    End If
                    esettings.DisplayText = ddlVehicleDoc.SelectedValue
                    esettings.Field = ddlVehicleDoc.SelectedValue
                    esettings.IsActive = True
                    esettings.Sequence = 0
                    esettings.SettingName = "VehicleDoc"
                    esettings.SettingType = "MainGrid"
                    esettings.Value = ddlVehicleDoc.SelectedValue
                    InsertSettings(tid)
                    tid = 0
                    If (btnSaveSettings.Text = "UPDATE") Then
                        cmd.Connection = con
                        cmd.CommandText = "select tid from mmm_mst_elogbooksettings where eid =" & Session("EID") & " and SettingType='MainGrid' and SettingName ='VehicleIMEIFld'"
                        con.Open()
                        tid = Convert.ToInt16(cmd.ExecuteScalar())
                        con.Close()
                    End If
                    esettings.DisplayText = txtVehIMEIFld.Text
                    esettings.Field = lblVehFldMaping.Text
                    esettings.IsActive = True
                    esettings.Sequence = 0
                    esettings.SettingName = "VehicleIMEIFld"
                    esettings.SettingType = "MainGrid"
                    esettings.Value = lblVehFldMaping.Text
                    InsertSettings(tid)

                End If

                If (rbOnOff.SelectedValue = "1") Then
                    tid = 0
                    If (btnSaveSettings.Text = "UPDATE") Then
                        cmd.Connection = con
                        cmd.CommandText = "select tid from mmm_mst_elogbooksettings where eid =" & Session("EID") & " and SettingType='MainGrid' and SettingName ='DetailSectionOn'"
                        con.Open()
                        tid = Convert.ToInt16(cmd.ExecuteScalar())
                        con.Close()

                    End If
                    esettings.DisplayText = "On/Off"
                    esettings.Field = "On/Off"
                    esettings.IsActive = True
                    esettings.Sequence = 0
                    esettings.SettingName = "DetailSectionOn"
                    esettings.SettingType = "MainGrid"
                    esettings.Value = rbOnOff.SelectedValue.ToString()
                    InsertSettings(tid)

                    savegridfieldswithDisplaytext(gvElogbookDetFlds, "txtlogbookdetdis")

                    If (ddlSiteDoc.SelectedIndex > 0) Then
                        tid = 0
                        If (btnSaveSettings.Text = "UPDATE") Then
                            cmd.Connection = con
                            cmd.CommandText = "select tid from mmm_mst_elogbooksettings where eid =" & Session("EID") & " and SettingType='Details' and SettingName ='SiteDoc'"
                            con.Open()
                            tid = Convert.ToInt16(cmd.ExecuteScalar())
                            con.Close()
                        End If
                        esettings.DisplayText = ddlSiteDoc.SelectedValue
                        esettings.Field = ddlSiteDoc.SelectedValue
                        esettings.IsActive = True
                        esettings.Sequence = 0
                        esettings.SettingName = "SiteDoc"
                        esettings.SettingType = "Details"
                        esettings.Value = ddlSiteDoc.SelectedValue
                        InsertSettings(tid)
                        'For Site From-To On/Off
                        tid = 0
                        If (btnSaveSettings.Text = "UPDATE") Then
                            cmd.Connection = con
                            cmd.CommandText = "select tid from mmm_mst_elogbooksettings where eid =" & Session("EID") & " and SettingType='Details' and SettingName ='SiteFromToOn'"
                            con.Open()
                            tid = Convert.ToInt16(cmd.ExecuteScalar())
                            con.Close()
                        End If
                        esettings.DisplayText = "On/Off"
                        esettings.Field = "On/Off"
                        esettings.IsActive = True
                        esettings.Sequence = 0
                        esettings.SettingName = "SiteFromToOn"
                        esettings.SettingType = "Details"
                        esettings.Value = rbSiteOnOff.SelectedValue
                        InsertSettings(tid)
                    End If

                ElseIf (rbOnOff.SelectedValue = "0") Then
                    cmd.Connection = con
                    cmd.CommandText = "delete from mmm_mst_elogbooksettings where eid =" & Session("EID") & " and SettingType='Details'"
                    con.Open()
                    tid = Convert.ToInt16(cmd.ExecuteScalar())
                    con.Close()
                    tid = 0
                    If (btnSaveSettings.Text = "UPDATE") Then
                        cmd.Connection = con
                        cmd.CommandText = "select tid from mmm_mst_elogbooksettings where eid =" & Session("EID") & " and SettingType='MainGrid' and SettingName ='DetailSectionOn'"
                        con.Open()
                        tid = Convert.ToInt16(cmd.ExecuteScalar())
                        con.Close()

                    End If
                    esettings.DisplayText = "On/Off"
                    esettings.Field = "On/Off"
                    esettings.IsActive = True
                    esettings.Sequence = 0
                    esettings.SettingName = "DetailSectionOn"
                    esettings.SettingType = "MainGrid"
                    esettings.Value = rbOnOff.SelectedValue.ToString()
                    InsertSettings(tid)
                End If
                If (btnSaveSettings.Text = "SAVE") Then
                    lblMsg.Text = "Records Saved"
                    btnSaveSettings.Text = "UPDATE"
                Else : lblMsg.Text = "Records Updated"
                End If


            End If

        Catch ex As Exception
        End Try

    End Sub

    Protected Sub rbOnOff_SelectedIndexChanged(sender As Object, e As EventArgs) Handles rbOnOff.SelectedIndexChanged
        Try

            If (rbOnOff.SelectedValue = "1") Then
                divDetail.Visible = True
            Else : divDetail.Visible = False

                ddlSiteDoc.SelectedValue = "0"
                gvElogbookDetFlds.DataSource = Nothing
                gvElogbookDetFlds.DataBind()
                ddlVehicleDoc_SelectedIndexChanged(Nothing, Nothing)
                ddlSiteDoc_SelectedIndexChanged(Nothing, Nothing)
            End If

        Catch ex As Exception

        End Try
    End Sub

    Function GetChkboxListValues(ByVal chklist As CheckBoxList) As String
        Dim values As String = ""
        Try

            For i As Integer = 0 To chklist.Items.Count - 1
                If (chklist.Items(i).Selected = True) Then
                    values &= chklist.Items(i).Text + ":" + chklist.Items(i).Value.ToString() & ","
                End If
            Next
            values = values.Remove(values.LastIndexOf(","))
        Catch ex As Exception

        End Try

        Return values


    End Function

    Sub AssignVlauestoControls(ByVal dt As DataTable)
        Try

            'For Chart Settings 
            Dim dtchart = dt.Select("SettingType='Chart'")
            For i As Integer = 0 To dtchart.Length - 1
                If (dtchart(i).Item("SettingName") = "VehicleChart") Then
                    If (dtchart(i).Item("Value").ToString() = "1") Then
                        chkVChartVis.Checked = True
                        txtVChartNm.Text = dtchart(i).Item("DisplayText")
                    End If
                ElseIf (dtchart(i).Item("SettingName") = "SiteChart") Then
                    If (dtchart(i).Item("Value").ToString() = "1") Then
                        chkSChartVis.Checked = True
                        txtSChartNm.Text = dtchart(i).Item("DisplayText")
                    End If
                ElseIf (dtchart(i).Item("SettingName") = "KmsChart") Then
                    If (dtchart(i).Item("Value").ToString() = "1") Then
                        chkKChartVis.Checked = True
                        txtKChartNm.Text = dtchart(i).Item("DisplayText")
                    End If
                End If
            Next

            'For Grid Settings 
            Dim dtmaingrid = dt.Select("SettingType='MainGrid'")
            For i As Integer = 0 To dtmaingrid.Length - 1
                If (dtmaingrid(i).Item("SettingName") = "VehicleDoc") Then
                    ddlVehicleDoc.SelectedValue = dtmaingrid(i).Item("Value")
                    ddlVehicleDoc_SelectedIndexChanged(Nothing, Nothing)
                ElseIf (dtmaingrid(i).Item("SettingName") = "DetailSectionOn") Then
                    If (dtmaingrid(i).Item("Value").ToString() = "1") Then
                        rbOnOff.SelectedValue = "1"
                    Else : rbOnOff.SelectedValue = "0"
                    End If
                ElseIf (dtmaingrid(i).Item("SettingName") = "ElogbookFlds") Then
                    chkLELogbook.Items.FindByValue(dtmaingrid(i).Item("Field").ToString().Split(":")(1)).Selected = True
                End If
            Next

            If (rbOnOff.SelectedValue = "1") Then
                divDetail.Visible = True
            End If
            'For Detail Section
            Dim sitedoc = dt.Select("SettingType='Details' AND SettingName='SiteDoc'")

            If (sitedoc.Length > 0) Then
                ddlSiteDoc.SelectedValue = ddlSiteDoc.Items.FindByValue(sitedoc(0).Item("Field").ToString()).Value
                ddlSiteDoc_SelectedIndexChanged(Nothing, Nothing)
            End If

            Dim dtDetails = dt.Select("SettingType='Details'")

            For i As Integer = 0 To dtDetails.Length - 1
                If (dtDetails(i).Item("SettingName") = "ElogbookFlds") Then
                    chkLELogbookDet.Items.FindByValue(dtDetails(i).Item("Field").ToString().Split(":")(1)).Selected = True
                ElseIf (dtDetails(i).Item("SettingName") = "VehicleFlds") Then
                    chkLVehicle.Items.FindByValue(dtDetails(i).Item("Field").ToString().Split(":")(1)).Selected = True
                ElseIf (dtDetails(i).Item("SettingName") = "SiteFlds") Then
                    chkLSite.Items.FindByValue(dtDetails(i).Item("Field").ToString().Split(":")(1)).Selected = True
                ElseIf (dtDetails(i).Item("SettingName") = "SiteFromToOn") Then

                    If (dtDetails(i).Item("Value").ToString() = "1") Then
                        rbSiteOnOff.SelectedValue = "1"
                    Else : rbSiteOnOff.SelectedValue = "0"
                    End If
                End If
            Next

            btnSaveSettings.Text = "UPDATE"

        Catch ex As Exception

        End Try

    End Sub

    Protected Sub btnAddElogbookFlds_Click(sender As Object, e As ImageClickEventArgs) Handles btnAddElogbookFlds.Click
        Try

            If (chkLELogbook.SelectedIndex > -1) Then

                Dim flds = GetChkboxListValues(chkLELogbook)
                For i As Integer = 0 To flds.Split(",").Length - 1

                    esettings.eid = Convert.ToInt32(Session("EID"))
                    esettings.Field = flds.Split(",")(i).ToString()
                    esettings.DisplayText = flds.Split(",")(i).ToString().Split(":")(0)
                    esettings.IsActive = False
                    esettings.Sequence = i + 1
                    esettings.SettingName = "ElogbookFlds"
                    esettings.SettingType = "MainGrid"
                    If (flds.Split(",")(i).ToString().Split(":")(1) = "Total_Distance") Then
                        esettings.Value = "Sum(convert(decimal(18,2),e." + flds.Split(",")(i).ToString().Split(":")(1) + "))"
                    ElseIf (flds.Split(",")(i).ToString().Split(":")(1) = "Trip_Start_DateTime" Or flds.Split(",")(i).ToString().Split(":")(1) = "Trip_end_DateTime") Then
                        esettings.Value = "Convert(varchar,e." + flds.Split(",")(i).ToString().Split(":")(1) + ", 103)"
                    Else : esettings.Value = " e." & flds.Split(",")(i).ToString().Split(":")(1)
                    End If

                    InsertSettings()

                Next

            Else : lblGridErr.Text = "Please select atleast one field to add!"

            End If

            BindElogbookGrid()

        Catch ex As Exception

        End Try

    End Sub

    Protected Sub gvElogbookflds_RowCommand(sender As Object, e As GridViewCommandEventArgs) Handles gvElogbookflds.RowCommand
        Try

            Dim tid = e.CommandArgument
            Dim rowindex = DirectCast(DirectCast(e.CommandSource, ImageButton).Parent.Parent, GridViewRow).RowIndex

            If (e.CommandName = "UP") Then
                If (rowindex > 0) Then
                    Dim prevtid = gvElogbookflds.DataKeys(rowindex - 1).Values("Tid")
                    Dim cmd = New SqlCommand("", con)
                    cmd.CommandType = CommandType.Text
                    cmd.CommandText = "update mmm_mst_ElogbookSettings set sequence =sequence -1 where tid =" & tid &
                        "update mmm_mst_ElogbookSettings set sequence =sequence + 1 where tid =" & prevtid
                    con.Open()
                    cmd.ExecuteNonQuery()
                    con.Close()

                End If

            ElseIf (e.CommandName = "DOWN") Then
                If (rowindex < gvElogbookflds.Rows.Count - 1) Then
                    Dim nexttid = gvElogbookflds.DataKeys(rowindex + 1).Values("Tid")
                    Dim cmd = New SqlCommand("", con)
                    cmd.CommandType = CommandType.Text
                    cmd.CommandText = "update mmm_mst_ElogbookSettings set sequence =sequence + 1 where tid =" & tid &
                        "update mmm_mst_ElogbookSettings set sequence =sequence - 1 where tid =" & nexttid
                    con.Open()
                    cmd.ExecuteNonQuery()
                    con.Close()

                End If

            ElseIf (e.CommandName = "DELETE") Then
                Dim Sequence = gvElogbookflds.DataKeys(rowindex).Values("Sequence")
                Dim cmd = New SqlCommand("", con)
                cmd.CommandType = CommandType.Text
                cmd.CommandText = "delete from mmm_mst_ElogbookSettings where tid =" & tid &
                    " update mmm_mst_ElogbookSettings set Sequence = sequence -1 where eid =" & HttpContext.Current.Session("EID") & " and settingType ='MainGrid' and settingName='Elogbookflds' and sequence >" & Sequence
                con.Open()
                cmd.ExecuteNonQuery()
                con.Close()

            End If

            BindElogbookGrid()

        Catch ex As Exception

        End Try


    End Sub

    Sub BindElogbookGrid()

        Try

            gvElogbookflds.DataSource = Nothing
            gvElogbookflds.DataBind()
            Dim da As New SqlDataAdapter("", ConStr)
            Dim dt As New DataTable

            da.SelectCommand.CommandText = "select Tid,Eid,SettingType,SettingName,Field,Value,Sequence,IsActive, row_number() over (order by Sequence)SNO,substring(Field,1,charindex(':',Field,0)-1) [FieldNm] , " &
                "   (case when DisplayText ='vehicle_no' or DisplayText ='Vehicle No' then 'False' when DisplayText ='IMEI NO'  or DisplayText ='IMEI_NO' or DisplayText ='Trip_Start_DateTime' or DisplayText ='Trip_end_DateTime'  then 'False' else 'True' End) [Enable] ,(case when DisplayText ='vehicle_No' then 'Vehicle No' when DisplayText ='IMEI_NO' then 'IMEI NO' else DisplayText End) [DisplayText] from [mmm_mst_elogbookSettings] where eid =" & Session("EID") & " and settingType ='MainGrid' and settingName='Elogbookflds'"

            da.Fill(dt)

            If dt.Rows.Count > 0 Then

                gvElogbookflds.DataSource = dt
                gvElogbookflds.DataBind()

            End If
        Catch ex As Exception

        End Try


    End Sub

    Sub BindElogbookDetGrid()
        Try

            Dim da As New SqlDataAdapter("", ConStr)
            Dim dt As New DataTable
            da.SelectCommand.CommandText = "select Tid,Eid,SettingType,SettingName,Field,Value,Sequence,IsActive, row_number() over (order by Sequence)SNO,substring(Field,1,charindex(':',Field,0)-1) [FieldNm] , " &
                "   (case when DisplayText ='vehicle_no' or DisplayText='Vehicle No' then 'False' when DisplayText ='IMEI_NO'  or DisplayText ='IMEI NO' or DisplayText ='Trip_Start_DateTime' or DisplayText ='Trip_end_DateTime' then 'False' else 'True' End) [Enable] ,(case when DisplayText ='vehicle_No' then 'Vehicle No' when DisplayText ='IMEI_NO' then 'IMEI NO' else DisplayText End) [DisplayText] from  [mmm_mst_elogbookSettings] where eid =" & Session("EID") & " and settingType ='Details' and settingname in ('ElogbookFlds','VehicleFlds','SiteFlds')"
            da.Fill(dt)
            If dt.Rows.Count > 0 Then
                gvElogbookDetFlds.DataSource = dt
                gvElogbookDetFlds.DataBind()
            Else : gvElogbookDetFlds.DataSource = Nothing
                gvElogbookDetFlds.DataBind()
            End If

        Catch ex As Exception

        End Try


    End Sub

    'Sub BindVehicleGrid()
    '    Try

    '        Dim da As New SqlDataAdapter("", ConStr)
    '        Dim dt As New DataTable

    '        da.SelectCommand.CommandText = " select es.*,f.displayname as [FieldNm] ,row_number() over (order by Sequence)SNO from [mmm_mst_elogbookSettings] es " &
    '                        " inner join mmm_mst_fields f on es.field= f.fieldid where es.eid =" & HttpContext.Current.Session("EID") & " and settingType ='Details' and settingName='VehicleFlds'"
    '        da.Fill(dt)

    '        If dt.Rows.Count > 0 Then
    '            gvVehicleFlds.DataSource = dt
    '            gvVehicleFlds.DataBind()
    '        Else : gvVehicleFlds.DataSource = Nothing
    '            gvVehicleFlds.DataBind()

    '        End If

    '    Catch ex As Exception

    '    End Try


    'End Sub

    'Sub BindSiteGrid(Optional ByVal aftersave As Boolean = False)
    '    Try

    '        Dim da As New SqlDataAdapter("", ConStr)
    '        Dim dt As New DataTable

    '            da.SelectCommand.CommandText = " select es.*,f.displayname as [FieldNm] ,row_number() over (order by Sequence)SNO from [mmm_mst_elogbookSettings] es " &
    '                            " inner join mmm_mst_fields f on es.field= f.fieldid where es.eid =" & HttpContext.Current.Session("EID") & " and settingType ='Details' and settingName='SiteFlds'"
    '        da.Fill(dt)
    '        If dt.Rows.Count > 0 Then
    '            gvSiteflds.DataSource = dt
    '            gvSiteflds.DataBind()
    '        Else : gvSiteflds.DataSource = Nothing
    '            gvSiteflds.DataBind()

    '        End If

    '    Catch ex As Exception

    '    End Try


    'End Sub

    Sub InsertSettings(Optional ByVal tid As Integer = 0)
        Dim cmd = New SqlCommand("", con)
        Dim dt = New DataTable()

        cmd.CommandType = CommandType.StoredProcedure
        cmd.CommandText = "InsertUpdateElogbookSettings"
        cmd.Parameters.AddWithValue("@Eid", HttpContext.Current.Session("EID"))
        cmd.Parameters.AddWithValue("@SettingType", esettings.SettingType)
        cmd.Parameters.AddWithValue("@SettingName", esettings.SettingName)
        cmd.Parameters.AddWithValue("@Field", esettings.Field)
        cmd.Parameters.AddWithValue("@Value", esettings.Value)
        cmd.Parameters.AddWithValue("@DisplayText", esettings.DisplayText)
        cmd.Parameters.AddWithValue("@Sequence", esettings.Sequence)
        cmd.Parameters.AddWithValue("@IsActive", esettings.IsActive)
        cmd.Parameters.AddWithValue("@Tid", tid)

        Try
            con.Open()
            cmd.ExecuteNonQuery()
            con.Close()
        Catch ex As Exception

        Finally
            con.Close()
        End Try

    End Sub

    Class esettings

        Public Shared eid As Integer = 0
        Public Shared SettingType As String = ""
        Public Shared SettingName As String = ""
        Public Shared Field As String = ""
        Public Shared Value As String = ""
        Public Shared DisplayText As String = ""
        Public Shared Sequence As Integer = 0
        Public Shared IsActive As Boolean = True


    End Class

    Protected Sub btnAddElogbookdetflds_Click(sender As Object, e As ImageClickEventArgs) Handles btnAddElogbookdetflds.Click
        Try

            If (chkLELogbookDet.SelectedIndex > -1) Then

                Dim flds = GetChkboxListValues(chkLELogbookDet)

                For i As Integer = 0 To flds.Split(",").Length - 1

                    esettings.eid = Convert.ToInt32(Session("EID"))
                    esettings.Field = flds.Split(",")(i).ToString()
                    esettings.DisplayText = flds.Split(",")(i).ToString().Split(":")(0)
                    esettings.IsActive = False
                    esettings.Sequence = i + 1
                    esettings.SettingName = "ElogbookFlds"
                    esettings.SettingType = "Details"
                    esettings.Value = " e." & flds.Split(",")(i).ToString().Split(":")(1)
                    InsertSettings()

                Next
            Else : lblDetailErr.Text = "Please select atleast one ELogbook field to add!"
            End If

            BindElogbookDetGrid()
        Catch ex As Exception

        End Try

    End Sub

    Protected Sub gvElogbookDetFlds_RowCommand(sender As Object, e As GridViewCommandEventArgs) Handles gvElogbookDetFlds.RowCommand

        Try

            Dim tid = e.CommandArgument
            Dim rowindex = DirectCast(DirectCast(e.CommandSource, ImageButton).Parent.Parent, GridViewRow).RowIndex

            If (e.CommandName = "UP") Then
                If (rowindex > 0) Then
                    Dim prevtid = gvElogbookDetFlds.DataKeys(rowindex - 1).Values("Tid")
                    Dim cmd = New SqlCommand("", con)
                    cmd.CommandType = CommandType.Text
                    cmd.CommandText = "update mmm_mst_ElogbookSettings set sequence =sequence -1 where sequence >1 and tid =" & tid &
                        "update mmm_mst_ElogbookSettings set sequence =sequence + 1 where tid =" & prevtid
                    con.Open()
                    cmd.ExecuteNonQuery()
                    con.Close()

                End If

            ElseIf (e.CommandName = "DOWN") Then
                If (rowindex < gvElogbookDetFlds.Rows.Count - 1) Then
                    Dim nexttid = gvElogbookDetFlds.DataKeys(rowindex + 1).Values("Tid")
                    Dim cmd = New SqlCommand("", con)
                    cmd.CommandType = CommandType.Text
                    cmd.CommandText = "update mmm_mst_ElogbookSettings set sequence =sequence + 1 where tid =" & tid &
                        "update mmm_mst_ElogbookSettings set sequence =sequence - 1 where sequence > 1 and tid =" & nexttid
                    con.Open()
                    cmd.ExecuteNonQuery()
                    con.Close()

                End If
            ElseIf (e.CommandName = "DELETE") Then
                Dim Sequence = gvElogbookDetFlds.DataKeys(rowindex).Values("Sequence")
                Dim cmd = New SqlCommand("", con)
                cmd.CommandType = CommandType.Text
                cmd.CommandText = "delete from mmm_mst_ElogbookSettings where tid =" & tid &
                    " update mmm_mst_ElogbookSettings set Sequence = sequence -1 where eid =" & HttpContext.Current.Session("EID") & " and settingType ='Details' and settingName='Elogbookflds'  and sequence >" & Sequence
                con.Open()
                cmd.ExecuteNonQuery()
                con.Close()

            End If

            BindElogbookDetGrid()

        Catch ex As Exception

        End Try


    End Sub

    Protected Sub btnAddVehicleFlds_Click(sender As Object, e As ImageClickEventArgs) Handles btnAddVehicleFlds.Click
        Try

            If (chkLVehicle.SelectedIndex > -1) Then

                Dim flds = GetChkboxListValues(chkLVehicle)

                For i As Integer = 0 To flds.Split(",").Length - 1

                    Dim da As New SqlDataAdapter("", ConStr)
                    Dim dt As New DataTable
                    da.SelectCommand.CommandText = "Select DisplayName,FieldMapping,dropdown,fieldtype,dropdowntype from mmm_mst_fields where FieldId = " & flds.Split(",")(i).Split(":")(1)
                    da.Fill(dt)

                    If (dt.Rows.Count > 0) Then
                        If (dt.Rows(0)("dropdown").ToString() <> "") Then

                            If (dt.Rows(0)("dropdowntype").ToString() = "MASTER VALUED") Then
                                esettings.Value = " dms.udf_split('" & dt.Rows(0)("dropdown").ToString() & "',Vehicle." & dt.Rows(0)("FieldMapping") & ") "
                            ElseIf (dt.Rows(0)("dropdowntype").ToString() = "FIX VALUED") Then
                                esettings.Value = "Vehicle." & dt.Rows(0)("FieldMapping").ToString()
                            End If

                        Else : esettings.Value = "Vehicle." & dt.Rows(0)("FieldMapping").ToString()

                        End If

                        esettings.DisplayText = dt.Rows(0)("DisplayName").ToString()

                    End If


                    esettings.eid = Convert.ToInt32(Session("EID"))
                    esettings.Field = flds.Split(",")(i).ToString()
                    esettings.IsActive = False
                    esettings.Sequence = i + 1
                    esettings.SettingName = "VehicleFlds"
                    esettings.SettingType = "Details"

                    InsertSettings()

                Next
            Else : lblDetailErr.Text = "Please select atleast one Vehicle field to add!"
            End If

            BindElogbookDetGrid()


        Catch ex As Exception

        End Try





    End Sub

    'Protected Sub gvVehicleFlds_RowCommand(sender As Object, e As GridViewCommandEventArgs) Handles gvVehicleFlds.RowCommand
    '    Try

    '        Dim tid = e.CommandArgument
    '        Dim rowindex = DirectCast(DirectCast(e.CommandSource, ImageButton).Parent.Parent, GridViewRow).RowIndex

    '        If (e.CommandName = "UP") Then
    '            If (rowindex > 0) Then
    '                Dim prevtid = gvVehicleFlds.DataKeys(rowindex - 1).Values("Tid")
    '                Dim cmd = New SqlCommand("", con)
    '                cmd.CommandType = CommandType.Text
    '                cmd.CommandText = "update mmm_mst_ElogbookSettings set sequence =sequence -1 where tid =" & tid &
    '                    "update mmm_mst_ElogbookSettings set sequence =sequence + 1 where tid =" & prevtid
    '                con.Open()
    '                cmd.ExecuteNonQuery()
    '                con.Close()

    '            End If

    '        ElseIf (e.CommandName = "DOWN") Then
    '            If (rowindex < gvVehicleFlds.Rows.Count - 1) Then
    '                Dim nexttid = gvVehicleFlds.DataKeys(rowindex + 1).Values("Tid")
    '                Dim cmd = New SqlCommand("", con)
    '                cmd.CommandType = CommandType.Text
    '                cmd.CommandText = "update mmm_mst_ElogbookSettings set sequence =sequence + 1 where tid =" & tid &
    '                    "update mmm_mst_ElogbookSettings set sequence =sequence - 1 where tid =" & nexttid
    '                con.Open()
    '                cmd.ExecuteNonQuery()
    '                con.Close()

    '            End If
    '        ElseIf (e.CommandName = "DELETE") Then
    '            Dim Sequence = gvVehicleFlds.DataKeys(rowindex + 1).Values("Sequence")
    '            Dim cmd = New SqlCommand("", con)
    '            cmd.CommandType = CommandType.Text
    '            cmd.CommandText = "delete from mmm_mst_ElogbookSettings where tid =" & tid &
    '                " update mmm_mst_ElogbookSettings set Sequence = sequence -1 where eid =" & HttpContext.Current.Session("EID") & " and settingType ='Details' and settingName='Elogbookflds' and sequence >" & Sequence
    '            con.Open()
    '            cmd.ExecuteNonQuery()
    '            con.Close()

    '        End If

    '        BindVehicleGrid()
    '    Catch ex As Exception

    '    End Try
    'End Sub

    Protected Sub btnAddSiteflds_Click(sender As Object, e As ImageClickEventArgs) Handles btnAddSiteflds.Click

        Try

            If (chkLSite.SelectedIndex > -1) Then

                Dim flds = GetChkboxListValues(chkLSite)

                For i As Integer = 0 To flds.Split(",").Length - 1

                    Dim da As New SqlDataAdapter("", ConStr)
                    Dim dt As New DataTable
                    da.SelectCommand.CommandText = "Select DisplayName,FieldMapping,dropdown,fieldtype,dropdowntype from mmm_mst_fields where FieldId = " & flds.Split(",")(i).Split(":")(1)
                    da.Fill(dt)

                    If (dt.Rows.Count > 0) Then
                        If (dt.Rows(0)("dropdown").ToString() <> "") Then

                            If (dt.Rows(0)("dropdowntype").ToString() = "MASTER VALUED") Then
                                esettings.Value = " dms.udf_split('" & dt.Rows(0)("dropdown").ToString() & "',Site." & dt.Rows(0)("FieldMapping") & ") "
                            ElseIf (dt.Rows(0)("dropdowntype").ToString() = "FIX VALUED") Then
                                esettings.Value = "Site." & dt.Rows(0)("FieldMapping").ToString()
                            End If

                        Else : esettings.Value = "Site." & dt.Rows(0)("FieldMapping").ToString()

                        End If

                        esettings.DisplayText = dt.Rows(0)("DisplayName").ToString()

                    End If
                    esettings.eid = Convert.ToInt32(Session("EID"))
                    esettings.Field = flds.Split(",")(i).ToString()
                    esettings.IsActive = False
                    esettings.Sequence = i + 1
                    esettings.SettingName = "SiteFlds"
                    esettings.SettingType = "Details"
                    InsertSettings()

                Next
            Else : lblDetailErr.Text = "Please select atleast one Site field to add!"
            End If

            BindElogbookDetGrid()

        Catch ex As Exception

        End Try

    End Sub

    'Protected Sub gvSiteflds_RowCommand(sender As Object, e As GridViewCommandEventArgs) Handles gvSiteflds.RowCommand
    '    Try

    '        Dim tid = e.CommandArgument
    '        Dim rowindex = DirectCast(DirectCast(e.CommandSource, ImageButton).Parent.Parent, GridViewRow).RowIndex

    '        If (e.CommandName = "UP") Then
    '            If (rowindex > 0) Then
    '                Dim prevtid = gvSiteflds.DataKeys(rowindex - 1).Values("Tid")
    '                Dim cmd = New SqlCommand("", con)
    '                cmd.CommandType = CommandType.Text
    '                cmd.CommandText = "update mmm_mst_ElogbookSettings set sequence =sequence -1 where tid =" & tid &
    '                    "update mmm_mst_ElogbookSettings set sequence =sequence + 1 where tid =" & prevtid
    '                con.Open()
    '                cmd.ExecuteNonQuery()
    '                con.Close()

    '            End If

    '        ElseIf (e.CommandName = "DOWN") Then
    '            If (rowindex < gvSiteflds.Rows.Count - 1) Then
    '                Dim nexttid = gvSiteflds.DataKeys(rowindex + 1).Values("Tid")
    '                Dim cmd = New SqlCommand("", con)
    '                cmd.CommandType = CommandType.Text
    '                cmd.CommandText = "update mmm_mst_ElogbookSettings set sequence =sequence + 1 where tid =" & tid &
    '                    "update mmm_mst_ElogbookSettings set sequence =sequence - 1 where tid =" & nexttid
    '                con.Open()
    '                cmd.ExecuteNonQuery()
    '                con.Close()

    '            End If
    '        ElseIf (e.CommandName = "DELETE") Then
    '            Dim Sequence = gvSiteflds.DataKeys(rowindex + 1).Values("Sequence")
    '            Dim cmd = New SqlCommand("", con)
    '            cmd.CommandType = CommandType.Text
    '            cmd.CommandText = "delete from mmm_mst_ElogbookSettings where tid =" & tid &
    '                " update mmm_mst_ElogbookSettings set Sequence = sequence -1 where eid =" & HttpContext.Current.Session("EID") & " and settingType ='Details' and settingName='Elogbookflds' and sequence >" & Sequence
    '            con.Open()
    '            cmd.ExecuteNonQuery()
    '            con.Close()

    '        End If

    '        BindSiteGrid()
    '    Catch ex As Exception

    '    End Try
    'End Sub

    Sub savegridfieldswithDisplaytext(gridview As GridView, txtid As String)
        Try
            For Each row As GridViewRow In gridview.Rows
                Dim cmd As New SqlCommand()
                cmd.Connection = con
                Dim text = DirectCast(gridview.Rows(row.RowIndex).FindControl(txtid), System.Web.UI.WebControls.TextBox).Text
                cmd.CommandText = " update mmm_mst_elogbooksettings set Isactive = 1, displaytext ='" & text & "' where tid =" & gridview.DataKeys(row.RowIndex).Values("Tid")
                con.Open()
                cmd.ExecuteNonQuery()
                con.Close()
            Next
        Catch ex As Exception

        End Try
    End Sub

    Protected Sub chkLELogbook_SelectedIndexChanged(sender As Object, e As EventArgs) Handles chkLELogbook.SelectedIndexChanged
        Try

        For Each item As ListItem In chkLELogbook.Items

                If (item.Value = "vehicle_no" Or item.Value = "IMEI_NO" Or item.Value = "Trip_Start_DateTime" Or item.Value = "Trip_end_DateTime") Then

                    If (item.Selected = False) Then
                        lblGridErr.Text = "Vehicle_no,Trip_Start_DateTime,Trip_end_DateTime and IMEI are Compulsory Fields !"
                        item.Selected = True
                    End If

                End If

            Next

        Catch ex As Exception

        End Try
    End Sub
   
    Protected Sub chkLELogbookDet_SelectedIndexChanged(sender As Object, e As EventArgs) Handles chkLELogbookDet.SelectedIndexChanged
        Try
            For Each item As ListItem In chkLELogbookDet.Items

                If (item.Value = "vehicle_no" Or item.Value = "IMEI_NO" Or item.Value = "Trip_Start_DateTime" Or item.Value = "Trip_end_DateTime") Then
                    If (item.Selected = False) Then
                        lblDetailErr.Text = "Vehicle_no,Trip_Start_DateTime,Trip_end_DateTime and IMEI are Compulsory Fields !"
                        item.Selected = True
                    End If

                End If

            Next
        Catch ex As Exception

        End Try
    End Sub

    Public Sub New()

    End Sub

    Protected Sub gvElogbookflds_RowDeleting(sender As Object, e As GridViewDeleteEventArgs) Handles gvElogbookflds.RowDeleting

    End Sub

    Protected Sub gvElogbookDetFlds_RowDeleting(sender As Object, e As GridViewDeleteEventArgs) Handles gvElogbookDetFlds.RowDeleting

    End Sub

End Class

