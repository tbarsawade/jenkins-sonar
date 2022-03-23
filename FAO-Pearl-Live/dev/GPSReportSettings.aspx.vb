Imports System.Data.SqlClient
Imports System.Data

Partial Class GPSReportSettings
    Inherits System.Web.UI.Page

    Dim ConStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
    Dim con As SqlConnection = New SqlConnection(ConStr)

    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        If Not IsPostBack Then
            FillVehicleDDL()

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
    Function ValidateData() As Boolean
        Try
            ' Dim cmd As New SqlCommand()
            Dim isvalid As Boolean = True

            'Other settings Validate()
            If (ddlReport.SelectedValue = "0") Then
                isvalid = False
                lblError.Text = "Please select Report Type"
                Return isvalid
            ElseIf (ddlReport.SelectedValue <> "MileageReport") Then
                If (HttpContext.Current.Session("EID").ToString() = "32" Or HttpContext.Current.Session("EID").ToString() = "56") Then
                    If (ddlDocumenttype.SelectedValue = "0") Then
                        isvalid = False
                        lblError.Text = "Please select a Document"
                        Return isvalid
                    Else
                        If (ddlDocField.SelectedValue = "0") Then
                            isvalid = False
                            lblError.Text = "Please select a Field for Vehicle"
                            Return isvalid
                        End If
                    End If
                End If
               
                If (chkUserName.Checked = True) Then
                    If (ddlDocFieldUser.SelectedValue = "0") Then
                        isvalid = False
                        lblError.Text = "Please select a Field for User"
                        Return isvalid
                    End If
                End If

                If (ddlDocForVehPermmision.SelectedValue <> "0") Then
                    If (ddlVehFieldForPermission.SelectedValue = "0") Then
                        isvalid = False
                        lblError.Text = "Please select a (Vehicle) Field used for Vehicle Permission"
                        Return isvalid
                    End If

                End If

            End If

            If (ddlVehicle.SelectedValue = "0") Then
                isvalid = False
                lblError.Text = "Please select Vehicle Document"
                Return isvalid
            ElseIf txtVehIMEIFld.Text = "" Or txtVehIMEIFld.Text = "0" Or lblVehFldMaping.Text = "" Then
                isvalid = False
                lblError.Text = "Invalid Vehicle Document!"
                Return isvalid
            Else : lblError.Text = ""
            End If

            ' Grid Settings Validate()

            If (chkLVehicle.SelectedIndex = -1) Then
                isvalid = False
                lblError.Text = "Please select atleast one Vehicle Field"
                Return isvalid
            End If
            If (chkLGPSCol.SelectedIndex = -1) Then
                isvalid = False
                lblError.Text = "Please select atleast one GPS Field"
                Return isvalid
            End If



        Catch ex As Exception

        End Try

        Return IsValid

    End Function

    Protected Sub ddlReport_SelectedIndexChanged(sender As Object, e As EventArgs) Handles ddlReport.SelectedIndexChanged
        Try
            Clearcontrols()
            If (ddlReport.SelectedValue <> "0") Then
                lblMsg.Text = ""
                lblError.Text = ""
                If (ddlReport.SelectedValue = "VehiclePosition" Or ddlReport.SelectedValue = "VehicleMap") Then
                    trUserName.Visible = True
                    trDocType.Visible = True
                    trDocFields.Visible = True
                    trVehPermission.Visible = True
                    FillDocDDL()
                Else
                    trUserName.Visible = False
                    trDocType.Visible = False
                    trDocFields.Visible = False
                    trVehPermission.Visible = False
                End If

                divDetail.Visible = True
                Dim da As New SqlDataAdapter("", ConStr)
                Dim cmd As New SqlCommand()
                Dim dt As New DataTable
                da.SelectCommand.CommandText = " SELECT * from [MMM_MST_GpsReportSettings] where  ReportType ='" + ddlReport.SelectedValue + "' and eid =" & HttpContext.Current.Session("EID")
                da.SelectCommand.CommandType = CommandType.Text
                da.Fill(dt)

                If (dt.Rows.Count > 0) Then
                    AssignValuesToControls(dt)
                    btnSaveSettings.Text = "UPDATE"
                Else
                    gvGPS.DataSource = Nothing
                    gvGPS.DataBind()
                    btnSaveSettings.Text = "SAVE"
                End If

            Else

            End If

        Catch ex As Exception

        End Try

    End Sub

    Protected Sub chkLGPSCol_DataBinding(sender As Object, e As EventArgs) Handles chkLGPSCol.DataBinding

        Try

            For Each item As ListItem In chkLGPSCol.Items

                If (item.Value = "IMIENO") Then

                    If (item.Selected = False) Then
                        lblGridError.Text = "IMEI is a  Compulsory Fields !"
                        item.Selected = True
                    End If

                End If

            Next

        Catch ex As Exception

        End Try

    End Sub

    Sub FillVehicleDDL()

        Try

            Dim da As New SqlDataAdapter("", ConStr)
            Dim dt As New DataTable
            da.SelectCommand.CommandText = "select distinct m.Documenttype from mmm_mst_master m  where  m.documenttype is not null and m.Eid=" & Session("Eid")
            da.Fill(dt)
            If dt.Rows.Count > 0 Then

                ddlVehicle.DataSource = dt
                ddlVehicle.DataTextField = "Documenttype"
                ddlVehicle.DataValueField = "Documenttype"
                ddlVehicle.DataBind()
                ddlVehicle.Items.Insert(0, New ListItem("--Select--", "0"))
                ddlDocForVehPermmision.DataSource = dt
                ddlDocForVehPermmision.DataTextField = "Documenttype"
                ddlDocForVehPermmision.DataValueField = "Documenttype"
                ddlDocForVehPermmision.DataBind()
                ddlDocForVehPermmision.Items.Insert(0, New ListItem("--Select--", "0"))

            End If

        Catch ex As Exception

        End Try

    End Sub

    Sub InsertSettings(Optional ByVal tid As Integer = 0)

        Dim cmd = New SqlCommand("", con)
        Dim dt = New DataTable()

        cmd.CommandType = CommandType.StoredProcedure
        cmd.CommandText = "InsertUpdateGPSRptSettings"
        cmd.Parameters.AddWithValue("@Eid", HttpContext.Current.Session("EID"))
        cmd.Parameters.AddWithValue("@DocType", GPSRptSettings.DocType)
        cmd.Parameters.AddWithValue("@DocField", GPSRptSettings.DocField)
        cmd.Parameters.AddWithValue("@DispName", GPSRptSettings.DispName)
        cmd.Parameters.AddWithValue("@ReportType", GPSRptSettings.ReportType)
        cmd.Parameters.AddWithValue("@VType", GPSRptSettings.VType)
        cmd.Parameters.AddWithValue("@CreationDate", GPSRptSettings.Creation)
        cmd.Parameters.AddWithValue("@IsActive", GPSRptSettings.IsActive)
        cmd.Parameters.AddWithValue("@SettingType", GPSRptSettings.SettingType)
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

    Protected Sub ddlVehicle_SelectedIndexChanged(sender As Object, e As EventArgs) Handles ddlVehicle.SelectedIndexChanged

        If (ddlVehicle.SelectedValue <> "0") Then
            Try

                txtVehIMEIFld.Text = ""
                lblVehFldMaping.Text = ""
                Dim da As New SqlDataAdapter("", ConStr)
                Dim cmd As New SqlCommand()
                Dim dt As New DataTable
                If (ddlVehicle.SelectedValue <> "0") Then

                    cmd.Connection = con
                    cmd.CommandText = "Select DisplayName+':'+FieldMapping from mmm_mst_Fields with(nolock) where documenttype ='" & ddlVehicle.SelectedValue & "' and DisplayName like '%IMEI%' and Eid=" & Session("Eid")
                    If (con.State = ConnectionState.Closed) Then
                        con.Open()
                    End If
                    Dim IsIMEI = cmd.ExecuteScalar()
                    con.Close()
                    If (IsIMEI <> Nothing) Then

                        If (IsIMEI <> "") Then
                            txtVehIMEIFld.Text = IsIMEI.ToString().Split(":")(0)
                            lblVehFldMaping.Text = IsIMEI.ToString().Split(":")(1)
                            da.SelectCommand.CommandText = "Select FieldId, Displayname ,FieldMapping from mmm_mst_Fields with(nolock) where documenttype ='" & ddlVehicle.SelectedValue & "'   and Eid=" & Session("Eid")
                            da.Fill(dt)
                            If dt.Rows.Count > 0 Then
                                chkLVehicle.DataSource = dt
                                chkLVehicle.DataTextField = "Displayname"
                                chkLVehicle.DataValueField = "FieldId"
                                chkLVehicle.DataBind()
                                ddlVehFieldForPermission.DataSource = dt
                                ddlVehFieldForPermission.DataTextField = "Displayname"
                                ddlVehFieldForPermission.DataValueField = "FieldMapping"
                                ddlVehFieldForPermission.DataBind()
                                ddlVehFieldForPermission.Items.Insert(0, New ListItem("Select", "0"))

                            End If
                            lblError.Text = ""
                        End If
                    Else : lblError.Text = "Invalid Document Selection for 'Vehicle Document'"
                    End If
                End If

                lblMsg.Text = ""
            Catch ex As Exception

            End Try
        End If

    End Sub

    Protected Sub chkLGPSCol_SelectedIndexChanged(sender As Object, e As EventArgs) Handles chkLGPSCol.SelectedIndexChanged
        Try

            For Each item As ListItem In chkLGPSCol.Items

                If (item.Value = "IMIENO") Then

                    If (item.Selected = False) Then
                        lblGridError.Text = "IMEI is Compulsory Field !"
                        item.Selected = True
                    End If

                End If
                If (item.Value = "speed") Then
                    If (ddlReport.SelectedValue = "MileageReport") Then
                        If (item.Selected = True) Then
                            trSpeed.Visible = True
                        Else
                            trSpeed.Visible = False
                        End If
                    Else : trSpeed.Visible = False
                    End If
                End If

            Next

        Catch ex As Exception

        End Try
    End Sub

    Protected Sub btnAddGPSFields_Click(sender As Object, e As ImageClickEventArgs) Handles btnAddGPSFields.Click
        Try

            If (chkLGPSCol.SelectedIndex > -1) Then

                Dim flds = GetChkboxListValues(chkLGPSCol)

                For i As Integer = 0 To flds.Split(",").Length - 1
                    Dim TID = 0
                    If (flds.Split(",")(i) = "speed") Then
                        Dim da As New SqlDataAdapter("", ConStr)
                        Dim cmd As New SqlCommand()
                        Dim dt As New DataTable
                        cmd.CommandText = " SELECT TID from [MMM_MST_GpsReportSettings] where  ReportType ='" + ddlReport.SelectedValue + "' and DocField like '%speed%' and eid =" & HttpContext.Current.Session("EID")
                        cmd.CommandType = CommandType.Text
                        cmd.Connection = con
                        con.Open()
                        TID = Convert.ToInt32(cmd.ExecuteScalar())
                        con.Close()
                        If (ddlReport.SelectedValue = "MileageReport") Then
                            GPSRptSettings.DocField = rbLSpeed.SelectedValue + "(" + flds.Split(",")(i) + ")" + ":" + flds.Split(",")(i)
                        Else : GPSRptSettings.DocField = flds.Split(",")(i) + ":" + flds.Split(",")(i)
                        End If


                    ElseIf (flds.Split(",")(i) = "DevDist") Then
                        If (ddlReport.SelectedValue = "MileageReport") Then
                            GPSRptSettings.DocField = "Sum(" + flds.Split(",")(i) + ")" + ":" + flds.Split(",")(i)
                        Else
                            GPSRptSettings.DocField = flds.Split(",")(i) + ":" + flds.Split(",")(i)
                        End If
                    Else : GPSRptSettings.DocField = "g." + flds.Split(",")(i) + ":g." + flds.Split(",")(i)

                    End If

                    GPSRptSettings.DispName = flds.Split(",")(i)
                    GPSRptSettings.eid = Convert.ToInt32(Session("EID"))
                    GPSRptSettings.IsActive = False
                    GPSRptSettings.DocType = "mmm_mst_gpsdata"
                    GPSRptSettings.ReportType = ddlReport.SelectedValue
                    GPSRptSettings.VType = "g." + flds.Split(",")(i)
                    GPSRptSettings.SettingType = "Grid"

                    InsertSettings(TID)

                Next
            Else : lblGridError.Text = "Please select atleast one Vehicle field to add!"
            End If

            BindGrid()

        Catch ex As Exception

        End Try

    End Sub

    Sub BindGrid()

        Try
            gvGPS.DataSource = Nothing
            gvGPS.DataBind()
            Dim da As New SqlDataAdapter("", ConStr)
            Dim dt As New DataTable

            da.SelectCommand.CommandText = "select Tid,Eid,DocType,DocField,DispName,SequenceNo,IsActive, row_number() over (order by SequenceNo)SNO " &
                "  from [MMM_MST_GpsReportSettings] where eid =" & Session("EID") & " and ReportType ='" + ddlReport.SelectedValue + "' and SettingType='Grid' "

            da.Fill(dt)

            If dt.Rows.Count > 0 Then

                gvGPS.DataSource = dt
                gvGPS.DataBind()

            End If

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
                    da.SelectCommand.CommandText = "Select DisplayName,FieldMapping,dropdown,fieldtype,dropdowntype from mmm_mst_fields where FieldId = " & flds.Split(",")(i)
                    da.Fill(dt)

                    If (dt.Rows.Count > 0) Then
                        If (dt.Rows(0)("dropdown").ToString() <> "") Then

                            If (dt.Rows(0)("dropdowntype").ToString() = "MASTER VALUED") Then
                                GPSRptSettings.DocField = " dms.udf_split('" & dt.Rows(0)("dropdown").ToString() & "',Vehicle." & dt.Rows(0)("FieldMapping") & ") " + ":" + dt.Rows(0)("DisplayName")

                            ElseIf (dt.Rows(0)("dropdowntype").ToString() = "FIX VALUED") Then
                                GPSRptSettings.DocField = "Vehicle." & dt.Rows(0)("FieldMapping").ToString() + ":" + dt.Rows(0)("DisplayName")
                            End If

                        Else : GPSRptSettings.DocField = "Vehicle." & dt.Rows(0)("FieldMapping").ToString() + ":" + dt.Rows(0)("DisplayName")

                        End If

                        GPSRptSettings.DispName = dt.Rows(0)("DisplayName").ToString()

                    End If

                    GPSRptSettings.eid = Convert.ToInt32(Session("EID"))
                    GPSRptSettings.IsActive = False
                    GPSRptSettings.DocType = "mmm_mst_master"
                    GPSRptSettings.ReportType = ddlReport.SelectedValue
                    GPSRptSettings.VType = "Vehicle." & dt.Rows(0)("FieldMapping").ToString()
                    GPSRptSettings.SettingType = "Grid"
                    InsertSettings()

                Next
            Else : lblGridError.Text = "Please select atleast one Vehicle field to add!"
            End If

            BindGrid()

        Catch ex As Exception

        End Try

    End Sub

    Function GetChkboxListValues(ByVal chklist As CheckBoxList) As String
        Dim values As String = ""
        Try

            For i As Integer = 0 To chklist.Items.Count - 1
                If (chklist.Items(i).Selected = True) Then
                    values &= chklist.Items(i).Value.ToString() & ","
                End If
            Next
            values = values.Remove(values.LastIndexOf(","))

        Catch ex As Exception

        End Try

        Return values

    End Function

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

    Protected Sub gvGPS_RowCommand(sender As Object, e As GridViewCommandEventArgs) Handles gvGPS.RowCommand
        Try

            Dim tid = e.CommandArgument
            Dim rowindex = DirectCast(DirectCast(e.CommandSource, ImageButton).Parent.Parent, GridViewRow).RowIndex

            If (e.CommandName = "UP") Then
                If (rowindex > 0) Then

                    Dim prevtid = gvGPS.DataKeys(rowindex - 1).Values("TID")
                    Dim cmd = New SqlCommand("", con)
                    cmd.CommandType = CommandType.Text
                    cmd.CommandText = "update mmm_mst_gpsreportsettings set sequenceNo =sequenceNo -1 where sequenceNo >1 and tid =" & tid &
                        " update mmm_mst_gpsreportsettings set sequenceNo =sequenceNo + 1 where tid =" & prevtid
                    con.Open()
                    cmd.ExecuteNonQuery()
                    con.Close()

                End If

            ElseIf (e.CommandName = "DOWN") Then
                If (rowindex < gvGPS.Rows.Count - 1) Then

                    Dim nexttid = gvGPS.DataKeys(rowindex + 1).Values("TID")
                    Dim cmd = New SqlCommand("", con)
                    cmd.CommandType = CommandType.Text
                    cmd.CommandText = "update mmm_mst_gpsreportsettings set sequenceNo =sequenceNo + 1 where tid =" & tid &
                        " update mmm_mst_gpsreportsettings set sequenceNo =sequenceNo - 1 where sequenceNo > 1 and tid =" & nexttid
                    con.Open()
                    cmd.ExecuteNonQuery()
                    con.Close()

                End If
            ElseIf (e.CommandName = "DELETE") Then

                Dim Sequence = gvGPS.DataKeys(rowindex).Values("SequenceNo")
                Dim cmd = New SqlCommand("", con)
                cmd.CommandType = CommandType.Text
                cmd.CommandText = "delete from mmm_mst_gpsreportsettings where tid =" & tid &
                    " update mmm_mst_gpsreportsettings set SequenceNo = sequenceNo -1 where eid =" & HttpContext.Current.Session("EID") & " and ReportType ='" + ddlReport.SelectedValue + "' and Vtype='Grid'  and sequenceNo >" & Sequence
                con.Open()
                cmd.ExecuteNonQuery()
                con.Close()

            End If

            BindGrid()

        Catch ex As Exception

        End Try
    End Sub

    Protected Sub btnSaveSettings_Click(sender As Object, e As EventArgs) Handles btnSaveSettings.Click
        Try
            If (ValidateData()) Then

                Dim da As New SqlDataAdapter("", ConStr)
                Dim cmd As New SqlCommand()
                Dim dt As New DataTable
                da.SelectCommand.CommandText = " SELECT * from [MMM_MST_GpsReportSettings] where  ReportType ='" + ddlReport.SelectedValue + "' and eid =" & HttpContext.Current.Session("EID")
                da.SelectCommand.CommandType = CommandType.Text
                da.Fill(dt)

                If (ddlReport.SelectedValue <> "") Then

                    If (ddlReport.SelectedValue <> "MileageReport") Then
                        If (ddlDocumenttype.SelectedValue <> "0") Then

                            Dim dtDoc = dt.Select("DispName ='DocumentType'")
                            Dim TID = 0

                            If (dtDoc.Length > 0) Then
                                TID = dtDoc(0).Item("TID").ToString()
                            End If

                            GPSRptSettings.DispName = "DocumentType"
                            GPSRptSettings.DocField = ddlDocField.SelectedValue
                            GPSRptSettings.DocType = "mmm_mst_doc"
                            GPSRptSettings.IsActive = True
                            GPSRptSettings.ReportType = ddlReport.SelectedValue
                            GPSRptSettings.VType = ddlDocumenttype.SelectedValue
                            GPSRptSettings.SettingType = "DocumentType"
                            InsertSettings(TID)
                        End If
                    End If
                    ' For UserName 
                    If (chkUserName.Checked = True) Then
                        Dim dtUserNm = dt.Select("DispName ='User Name'")
                        Dim TID = 0
                        If (dtUserNm.Length > 0) Then
                            TID = dtUserNm(0).Item("TID").ToString()
                        End If
                        GPSRptSettings.DispName = "User Name"
                        GPSRptSettings.DocField = "(select isnull(UserName,'') from  mmm_mst_user where uid = doc." & ddlDocFieldUser.SelectedValue & ")"
                        GPSRptSettings.DocType = "User"
                        GPSRptSettings.IsActive = True
                        GPSRptSettings.ReportType = ddlReport.SelectedValue
                        GPSRptSettings.VType = "Grid"
                        GPSRptSettings.SettingType = "Grid"
                        InsertSettings(TID)
                    End If

                    If (ddlDocForVehPermmision.SelectedValue <> "0") Then
                        Dim dtVehPermission = dt.Select("SettingType='VehiclePermission'")
                        Dim TID = 0
                        If (dtVehPermission.Length > 0) Then
                            TID = dtVehPermission(0).Item("TID").ToString()
                        End If
                        GPSRptSettings.DispName = "VehiclePermission"
                        GPSRptSettings.DocField = ddlVehFieldForPermission.SelectedValue
                        GPSRptSettings.DocType = "mmm_mst_master"
                        GPSRptSettings.IsActive = True
                        GPSRptSettings.ReportType = ddlReport.SelectedValue
                        GPSRptSettings.VType = ddlDocForVehPermmision.SelectedValue
                        GPSRptSettings.SettingType = "VehiclePermission"
                        InsertSettings(TID)

                    End If


                    If (ddlVehicle.SelectedValue <> "0") Then
                        Dim dtVehicle = dt.Select("DispName ='Vehicle'")
                        Dim TID = 0
                        If (dtVehicle.Length > 0) Then
                            TID = dtVehicle(0).Item("TID").ToString()
                        End If
                        GPSRptSettings.DispName = "Vehicle"
                        GPSRptSettings.DocField = lblVehFldMaping.Text
                        GPSRptSettings.DocType = "mmm_mst_master"
                        GPSRptSettings.IsActive = True
                        GPSRptSettings.ReportType = ddlReport.SelectedValue
                        GPSRptSettings.VType = ddlVehicle.SelectedValue
                        GPSRptSettings.SettingType = "Vehicle"
                        InsertSettings(TID)
                    End If


                    If (btnSaveSettings.Text = "UPDATE") Then
                        If (chkLGPSCol.Items.FindByValue("speed").Selected = True) Then
                            Dim dtspeed = dt.Select("DocField like '%speed%' and SettingType='Grid'")
                            Dim TID = 0
                            Dim olddocfield = ""
                            If (dtspeed.Length > 0) Then
                                TID = dtspeed(0).Item("TID")
                                olddocfield = dtspeed(0).Item("DocField").ToString()
                            End If


                            GPSRptSettings.DispName = dtspeed(0).Item("DispName")
                            If (ddlReport.SelectedValue = "MileageReport") Then
                                GPSRptSettings.DocField = rbLSpeed.SelectedValue + olddocfield.Substring(3)
                            Else
                                GPSRptSettings.DocField = "speed:speed"
                            End If
                            GPSRptSettings.DocType = "mmm_mst_gpsdata"
                            GPSRptSettings.IsActive = True
                            GPSRptSettings.ReportType = ddlReport.SelectedValue
                            GPSRptSettings.VType = "g.speed"
                            GPSRptSettings.SettingType = "Grid"
                            InsertSettings(TID)

                        End If
                    End If

                    savegridfieldswithDisplaytext(gvGPS, "txtDispName")
                    If (btnSaveSettings.Text = "SAVE") Then
                        lblMsg.Text = "Records Saved"
                        btnSaveSettings.Text = "UPDATE"
                    Else : lblMsg.Text = "Records Updated"
                    End If
                End If

            End If
        Catch ex As Exception

        End Try

    End Sub

    Sub savegridfieldswithDisplaytext(gridview As GridView, txtid As String)
        Try
            For Each row As GridViewRow In gridview.Rows
                Dim cmd As New SqlCommand()
                cmd.Connection = con
                Dim text = DirectCast(gridview.Rows(row.RowIndex).FindControl(txtid), System.Web.UI.WebControls.TextBox).Text
                cmd.CommandText = " update mmm_mst_GpsReportSettings set Isactive = 1, DispName ='" & text & "' where tid =" & gridview.DataKeys(row.RowIndex).Values("TID")
                con.Open()
                cmd.ExecuteNonQuery()
                con.Close()
            Next
        Catch ex As Exception

        End Try
    End Sub

    Sub AssignValuesToControls(ByVal dt As DataTable)
        Try

            BindGrid()

            Dim dtVehicle = dt.Select("SettingType='Vehicle'")
            If (dtVehicle.Length > 0) Then
                ddlVehicle.SelectedValue = ddlVehicle.Items.FindByText(dtVehicle(0).Item("VType").ToString()).Value
                txtVehIMEIFld.Text = dtVehicle(0).Item("DispName").ToString()
                lblVehFldMaping.Text = dtVehicle(0).Item("DocField").ToString()
                ddlVehicle_SelectedIndexChanged(Nothing, Nothing)
            End If

            Dim dtGpsrows = dt.Select("SettingType='Grid' and DocType='mmm_mst_gpsdata'")
            If (dtGpsrows.Length > 0) Then
                For i As Integer = 0 To dtGpsrows.Length - 1
                    If (dtGpsrows(i).Item("DocField").ToString().Contains("g.")) Then
                        chkLGPSCol.Items.FindByValue((dtGpsrows(i).Item("DocField").ToString().Split(":")(1)).Substring(2)).Selected = True
                    Else : chkLGPSCol.Items.FindByValue((dtGpsrows(i).Item("DocField").ToString().Split(":")(1))).Selected = True

                    End If
                Next
            End If

            chkLGPSCol_SelectedIndexChanged(Nothing, Nothing)
            If (ddlReport.SelectedValue = "MileageReport") Then
                Dim dtspeed = dt.Select("DocField like '%speed%'")
                If (dtspeed.Length > 0) Then
                    rbLSpeed.Items.FindByValue(dtspeed(0).Item("DocField").ToString().Substring(0, 3)).Selected = True
                End If
            End If
            Dim dtVehiclerows = dt.Select("SettingType='Grid' and DocType='mmm_mst_master'")
            If (dtVehiclerows.Length > 0) Then
                For i As Integer = 0 To dtVehiclerows.Length - 1
                    chkLVehicle.Items.FindByText(dtVehiclerows(i).Item("DocField").ToString().Split(":")(1)).Selected = True
                Next
            End If

            Dim dtdocument = dt.Select("SettingType='DocumentType'")
            If (dtdocument.Length > 0) Then
                ddlDocumenttype.SelectedValue = ddlDocumenttype.Items.FindByText(dtdocument(0).Item("VType").ToString()).Value
                ddlDocumenttype_SelectedIndexChanged(Nothing, Nothing)
                ddlDocField.SelectedValue = ddlDocField.Items.FindByValue(dtdocument(0).Item("DocField").ToString()).Value
                Dim dtuser = dt.Select("SettingType='Grid' and DocType='User'")
                If (dtuser.Length > 0) Then
                    chkUserName.Checked = True
                    Try
                        Dim docuserfld = dtuser(0).Item("DocField").ToString()
                        Dim userfld2 = docuserfld.Substring(docuserfld.IndexOf("=")).TrimEnd(")").Split(".")(1)
                        ddlDocFieldUser.SelectedValue = ddlDocFieldUser.Items.FindByValue(userfld2).Value
                    Catch ex As Exception
                        ddlDocFieldUser.SelectedValue = "0"
                    End Try
                End If
            End If
        Catch ex As Exception

        End Try

    End Sub
   
    Protected Sub gvGPS_RowDeleting(sender As Object, e As GridViewDeleteEventArgs) Handles gvGPS.RowDeleting

    End Sub

    Sub FillDocDDL()

        Try

            Dim da As New SqlDataAdapter("", ConStr)
            Dim dt As New DataTable
            da.SelectCommand.CommandText = "select Distinct DocumentType from mmm_mst_doc where eid =" & Session("Eid") & " and DocumentType is not null order by DocumentType "
            da.Fill(dt)
            If dt.Rows.Count > 0 Then

                ddlDocumenttype.DataSource = dt
                ddlDocumenttype.DataTextField = "DocumentType"
                ddlDocumenttype.DataValueField = "DocumentType"
                ddlDocumenttype.DataBind()
                ddlDocumenttype.Items.Insert(0, New ListItem("--Select--", "0"))

            End If

        Catch ex As Exception

        End Try
    End Sub

    Protected Sub ddlDocumenttype_SelectedIndexChanged(sender As Object, e As EventArgs) Handles ddlDocumenttype.SelectedIndexChanged
        Try

            Dim da As New SqlDataAdapter("", ConStr)
            Dim dt As New DataTable
            da.SelectCommand.CommandText = "select Displayname , FieldMapping from mmm_mst_fields where Documenttype ='" & ddlDocumenttype.SelectedValue & "' and eid =" & Session("EID") & ""
            da.Fill(dt)
            If dt.Rows.Count > 0 Then

                ddlDocField.DataSource = dt
                ddlDocField.DataTextField = "Displayname"
                ddlDocField.DataValueField = "FieldMapping"
                ddlDocField.DataBind()
                ddlDocField.Items.Insert(0, New ListItem("--Select--", "0"))
                ddlDocFieldUser.DataSource = dt
                ddlDocFieldUser.DataTextField = "Displayname"
                ddlDocFieldUser.DataValueField = "FieldMapping"
                ddlDocFieldUser.DataBind()
                ddlDocFieldUser.Items.Insert(0, New ListItem("--Select--", "0"))


            End If

        Catch ex As Exception

        End Try
    End Sub



    Sub Clearcontrols()
        ddlVehicle.SelectedValue = "0"
        txtVehIMEIFld.Text = ""
        trSpeed.Visible = False
        trUserName.Visible = False
        trDocType.Visible = False
        trDocFields.Visible = False
        trVehPermission.Visible = False
        divDetail.Visible = False
        chkLGPSCol.ClearSelection()
        chkLVehicle.ClearSelection()


    End Sub
End Class
