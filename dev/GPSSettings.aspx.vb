Imports System.Data
Imports System.Data.SqlClient
Imports System.Configuration
Imports System.Globalization
Partial Class GPSSettings
    Inherits System.Web.UI.Page
    Protected Sub Page_Load(sender As Object, e As System.EventArgs) Handles Me.Load
        If Not IsPostBack Then
            Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
            Dim con As SqlConnection = New SqlConnection(conStr)
            Dim oda As SqlDataAdapter = New SqlDataAdapter("", con)

            oda.SelectCommand.CommandText = "select * from  MMM_MST_FORMS where FormSource='MENU DRIVEN'  and Eid=" & Session("EID") & "  order by FormName"
            Dim ds As New DataSet()
            oda.Fill(ds, "data")


            ddlDocumentType.Items.Clear()
            ddlVDtype.Items.Clear()

            For j As Integer = 0 To ds.Tables("data").Rows.Count - 1
                ddlDocumentType.Items.Add(ds.Tables("data").Rows(j).Item("FormName").ToString().ToUpper())
                ddlDocumentType.Items(j).Value = ds.Tables("data").Rows(j).Item("FormName").ToString().ToUpper()
                ddlVDtype.Items.Add(ds.Tables("data").Rows(j).Item("FormName").ToString.ToUpper())
                ddlVDtype.Items(j).Value = ds.Tables("data").Rows(j).Item("FormName").ToString.ToUpper()
            Next
            oda.SelectCommand.CommandText = "select * from  mmm_mst_entity  where  Eid=" + Session("EID").ToString() + ""
            oda.Fill(ds, "mmm_mst_entity")
            If ds.Tables(1).Rows(0).Item("UVDType") <> "" Or ds.Tables(1).Rows(0).Item("VIDtype") <> "" Then
                ddlDocumentType.SelectedIndex = ddlDocumentType.Items.IndexOf(ddlDocumentType.Items.FindByText(ds.Tables(1).Rows(0).Item("UVDType").ToString().ToUpper()))
                ddlVDtype.SelectedIndex = ddlVDtype.Items.IndexOf(ddlVDtype.Items.FindByText(ds.Tables(1).Rows(0).Item("VIDtype").ToString.ToUpper()))
            End If

            oda.SelectCommand.CommandText = "select DISTINCT displayname,fieldmapping FROM MMM_MST_FIELDS WHERE DOCUMENTTYPE='" & ds.Tables(1).Rows(0).Item("UVDType") & "' AND EID= " & Session("EID") & " order by displayname"
            oda.Fill(ds, "MMM_MST_FIELDS")
            StartDate.Items.Clear()
            EndDate.Items.Clear()
            For j As Integer = 0 To ds.Tables("MMM_MST_FIELDS").Rows.Count - 1
                StartDate.Items.Add(ds.Tables("MMM_MST_FIELDS").Rows(j).Item(0).ToString.ToUpper())
                StartDate.Items(j).Value = ds.Tables("MMM_MST_FIELDS").Rows(j).Item(1).ToString.ToUpper()
            Next
            For j As Integer = 0 To ds.Tables("MMM_MST_FIELDS").Rows.Count - 1
                EndDate.Items.Add(ds.Tables("MMM_MST_FIELDS").Rows(j).Item(0).ToString.ToUpper())
                EndDate.Items(j).Value = ds.Tables("MMM_MST_FIELDS").Rows(j).Item(1).ToString.ToUpper()
            Next
            If ds.Tables("mmm_mst_entity").Rows(0).Item("UVStartDateTime").ToString <> "" Or ds.Tables("mmm_mst_entity").Rows(0).Item("UVEndDateTime").ToString <> "" Then
                StartDate.SelectedIndex = StartDate.Items.IndexOf(StartDate.Items.FindByValue(ds.Tables("mmm_mst_entity").Rows(0).Item("UVStartDateTime").ToString.ToUpper))
                EndDate.SelectedIndex = EndDate.Items.IndexOf(EndDate.Items.FindByValue(ds.Tables("mmm_mst_entity").Rows(0).Item("UVEndDateTime").ToString.ToUpper))
            End If

            ddVechicleField.Items.Clear()
            For j As Integer = 0 To ds.Tables("MMM_MST_FIELDS").Rows.Count - 1
                ddVechicleField.Items.Add(ds.Tables("MMM_MST_FIELDS").Rows(j).Item(0).ToString.ToUpper())
                ddVechicleField.Items(j).Value = ds.Tables("MMM_MST_FIELDS").Rows(j).Item(1).ToString.ToUpper()
            Next
            ddUserField.Items.Clear()
            For i As Integer = 0 To ds.Tables("MMM_MST_FIELDS").Rows.Count - 1
                ddUserField.Items.Add(ds.Tables("MMM_MST_FIELDS").Rows(i).Item(0).ToString.ToUpper())
                ddUserField.Items(i).Value = ds.Tables("MMM_MST_FIELDS").Rows(i).Item(1).ToString.ToUpper()
            Next


            If ds.Tables(1).Rows(0).Item("VIVehicleField").ToString.Length > 0 Or ds.Tables(1).Rows(0).Item("UVUserField").ToString.Length > 0 Then
                ddVechicleField.SelectedIndex = ddVechicleField.Items.IndexOf(ddVechicleField.Items.FindByValue(ds.Tables(1).Rows(0).Item("UVVehicleField").ToString.ToUpper()))
                ddUserField.SelectedIndex = ddUserField.Items.IndexOf(ddUserField.Items.FindByValue(ds.Tables(1).Rows(0).Item("UVUserField").ToString.ToUpper()))
            End If




            oda.SelectCommand.CommandText = "select DISTINCT displayname,fieldmapping FROM MMM_MST_FIELDS WHERE DOCUMENTTYPE='" & ds.Tables(1).Rows(0).Item("VIDtype") & "' AND EID= " & Session("EID") & " order by displayname"

            oda.Fill(ds, "VehicleMapping")
            ddVIMEIfield.Items.Clear()

            For j As Integer = 0 To ds.Tables("VehicleMapping").Rows.Count - 1
                ddVIMEIfield.Items.Add(ds.Tables("VehicleMapping").Rows(j).Item(0).ToString.ToUpper())
                ddVIMEIfield.Items(j).Value = ds.Tables("VehicleMapping").Rows(j).Item(1).ToString.ToUpper()
            Next
            ddVehMapping.Items.Clear()
            For i As Integer = 0 To ds.Tables("VehicleMapping").Rows.Count - 1
                ddVehMapping.Items.Add(ds.Tables("VehicleMapping").Rows(i).Item(0).ToString.ToUpper())
                ddVehMapping.Items(i).Value = ds.Tables("VehicleMapping").Rows(i).Item(1).ToString.ToUpper()
            Next

            If ds.Tables(1).Rows(0).Item("VIVehicleField").ToString.Length > 0 Or ds.Tables(1).Rows(0).Item("VIImeiField").ToString.Length > 0 Then
                ddVehMapping.SelectedIndex = ddVehMapping.Items.IndexOf(ddVehMapping.Items.FindByValue(ds.Tables(1).Rows(0).Item("VIVehicleField").ToString.ToUpper()))
                ddVIMEIfield.SelectedIndex = ddVIMEIfield.Items.IndexOf(ddVIMEIfield.Items.FindByValue(ds.Tables(1).Rows(0).Item("VIImeiField").ToString.ToUpper()))
            End If


            'If ds.Tables(1).Rows(0).Item("mapType").ToString.Length > 0 Then
            '    MapName.SelectedIndex = ddUserField.Items.IndexOf(ddUserField.Items.FindByValue(ds.Tables("mmm_mst_entity").Rows(0).Item("mapType")))
            'End If
            'Key.Text = ds.Tables("mmm_mst_entity").Rows(0).Item("APIKey").ToString

            ddlDocumentTypec.Items.Clear()
            ddRatecarddoc.Items.Clear()
            ddlCustomerDoc.Items.Clear()
            'Dim viewform As New StringBuilder()
            'viewform.
            For j As Integer = 0 To ds.Tables("data").Rows.Count - 1
                Dim view As String = String.Empty
                view = ds.Tables("data").Rows(j).Item("formname").ToString.ToUpper
                view = view.Replace(" ", "_")
                view = "V" + Session("eid").ToString + view + ""
                ddlDocumentTypec.Items.Add(ds.Tables("data").Rows(j).Item("FormName").ToString().ToUpper())

                ddlDocumentTypec.Items(j).Value = view
                ddRatecarddoc.Items.Add(ds.Tables("data").Rows(j).Item("formname").ToString.ToUpper)
                ddRatecarddoc.Items(j).Value = view
                ddlCustomerDoc.Items.Add(ds.Tables("data").Rows(j).Item("formname").ToString.ToUpper)
                ddlCustomerDoc.Items(j).Value = view

            Next


            oda.SelectCommand.CommandText = "select * FROM mmm_mst_gpssetting WHERE  EID= " & Session("EID") & ""
            Dim dt As New DataTable
            oda.Fill(dt)
            If dt.Rows.Count > 0 Then
                ddlDocumentTypec.SelectedIndex = ddlDocumentTypec.Items.IndexOf(ddlDocumentTypec.Items.FindByValue(dt.Rows(0).Item("Cab_Vehicle_doc").ToString.ToUpper))
                ddRatecarddoc.SelectedIndex = ddRatecarddoc.Items.IndexOf(ddRatecarddoc.Items.FindByValue(dt.Rows(0).Item("Cab_rate_card_doc").ToString.ToUpper))
                ddlCustomerDoc.SelectedIndex = ddlCustomerDoc.Items.IndexOf(ddlCustomerDoc.Items.FindByValue(dt.Rows(0).Item("Cab_Customer_doc").ToString.ToUpper))

                If dt.Rows(0).Item("Cab_Vehicle_doc").ToString.Length > 0 Then
                    Dim docname As String = dt.Rows(0).Item("Cab_Vehicle_doc").ToString
                    docname = docname.Replace("V" + Session("eid").ToString + "", "")
                    docname = docname.Replace("_", " ")
                    '  oda.SelectCommand.CommandText = "select DISTINCT displayname,fieldmapping FROM MMM_MST_FIELDS WHERE DOCUMENTTYPE= '" & dt.Rows(0).Item("Cab_Vehicle_doc").ToString & "' AND EID= " & Session("EID") & ""
                    oda.SelectCommand.CommandText = "select DISTINCT displayname,fieldmapping FROM MMM_MST_FIELDS WHERE DOCUMENTTYPE= '" & docname & "' AND EID= " & Session("EID") & ""
                    oda.Fill(ds, "datafieldname")
                    ddstatus.Items.Clear()
                    ddowner.Items.Clear()
                    For j As Integer = 0 To ds.Tables("datafieldname").Rows.Count - 1
                        ddstatus.Items.Add(ds.Tables("datafieldname").Rows(j).Item("displayname").ToString().ToUpper())
                        ddstatus.Items(j).Value = ds.Tables("datafieldname").Rows(j).Item("displayname").ToString.ToUpper
                        ddowner.Items.Add(ds.Tables("datafieldname").Rows(j).Item("displayname").ToString.ToUpper)
                        ddVehicleType.Items.Add(ds.Tables("datafieldname").Rows(j).Item("displayname").ToString.ToUpper)
                    Next
                    If dt.Rows(0).Item("cab_vehicle_status").ToString.Length > 0 Then

                        ddstatus.SelectedIndex = ddstatus.Items.IndexOf(ddstatus.Items.FindByText(dt.Rows(0).Item("cab_vehicle_status").ToString.ToUpper))
                        ddowner.SelectedIndex = ddowner.Items.IndexOf(ddowner.Items.FindByText(dt.Rows(0).Item("cab_owner").ToString.ToUpper))
                        ddVehicleType.SelectedIndex = ddVehicleType.Items.IndexOf(ddVehicleType.Items.FindByText(dt.Rows(0).Item("vehicle_type").ToString.ToUpper))
                    End If

                End If

            End If
            
            ' Cab_Vehicle_doc
            FilldropdownCal()
            BindGeofenceDropDowns()
            oda.Dispose()
            ds.Dispose()
            con.Dispose()
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
    Protected Sub FilldropDown(ByVal sender As Object, ByVal e As System.EventArgs) Handles doctypecal.SelectedIndexChanged
        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim con As New SqlConnection(conStr)
        Dim da As New SqlDataAdapter("", con)
        Dim ds As New DataSet

        da.SelectCommand.CommandText = "select DISTINCT displayname,fieldmapping FROM MMM_MST_FIELDS WHERE datatype='New Datetime' and DOCUMENTTYPE='" & doctypecal.SelectedItem.Text & "' AND EID= " & Session("EID") & ""

        da.Fill(ds, "data")
        ddVechicleField.Items.Clear()

        For j As Integer = 0 To ds.Tables("data").Rows.Count - 1
            ddVechicleField.Items.Add(ds.Tables("data").Rows(j).Item(0))
            ddVechicleField.Items(j).Value = ds.Tables("data").Rows(j).Item(1)
        Next
        ddUserField.Items.Clear()
        For i As Integer = 0 To ds.Tables("data").Rows.Count - 1
            ddUserField.Items.Add(ds.Tables("data").Rows(i).Item(0))
            ddUserField.Items(i).Value = ds.Tables("data").Rows(i).Item(1)
        Next
        StartDate.Items.Clear()
        For i As Integer = 0 To ds.Tables("data").Rows.Count - 1
            StartDate.Items.Add(ds.Tables("data").Rows(i).Item(0))
            StartDate.Items(i).Value = ds.Tables("data").Rows(i).Item(1)
        Next

        EndDate.Items.Clear()
        For i As Integer = 0 To ds.Tables("data").Rows.Count - 1
            EndDate.Items.Add(ds.Tables("data").Rows(i).Item(0))
            EndDate.Items(i).Value = ds.Tables("data").Rows(i).Item(1)
        Next

        ds.Dispose()
        da.Dispose()
        con.Dispose()
    End Sub
    Protected Sub FilldropDownVehicle(ByVal sender As Object, ByVal e As System.EventArgs)
        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim con As New SqlConnection(conStr)
        Dim da As New SqlDataAdapter("", con)
        Dim ds As New DataSet

        da.SelectCommand.CommandText = "select DISTINCT displayname,fieldmapping FROM MMM_MST_FIELDS WHERE DOCUMENTTYPE='" & ddlVDtype.SelectedItem.Text & "' AND EID= " & Session("EID") & ""

        da.Fill(ds, "data")
        ddVIMEIfield.Items.Clear()

        For j As Integer = 0 To ds.Tables("data").Rows.Count - 1
            ddVIMEIfield.Items.Add(ds.Tables("data").Rows(j).Item(0))
            ddVIMEIfield.Items(j).Value = ds.Tables("data").Rows(j).Item(1)
        Next
        ddVehMapping.Items.Clear()
        For i As Integer = 0 To ds.Tables("data").Rows.Count - 1
            ddVehMapping.Items.Add(ds.Tables("data").Rows(i).Item(0))
            ddVehMapping.Items(i).Value = ds.Tables("data").Rows(i).Item(1)
        Next


        da.Dispose()
        con.Dispose()
    End Sub

    Protected Sub btnUname_Click(sender As Object, e As System.EventArgs) Handles btnUname.Click

        ' Dim parsed As DateTime
        'If StartDate.Text.Length > 0 And EndDate.Text.Length > 0 Then
        '    Dim valid As Boolean = DateTime.TryParseExact(StartDate.Text, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, parsed)
        '    If valid = False Then
        '        savedvehiclemapping.Text = "Enter the Correct Start date Format"
        '        StartDate.Focus()
        '        Exit Sub
        '    End If
        '    Dim validdate2 As Boolean = DateTime.TryParseExact(EndDate.Text, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, parsed)
        '    If validdate2 = False Then
        '        savedvehiclemapping.Text = "Enter the Correct end date Format"
        '        EndDate.Focus()
        '        Exit Sub
        '    End If
        'ElseIf StartDate.Text.Length = 0 And EndDate.Text.Length = 0 Then

        'Else

        '    savedvehiclemapping.Text = "either  blank or fill both(start date and end date)"
        '    Exit Sub
        'End If

        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim con As New SqlConnection(conStr)
        Dim oda As SqlDataAdapter = New SqlDataAdapter("uspUpdateUserVehicleMapping", con)
        oda.SelectCommand.CommandType = CommandType.StoredProcedure
        oda.SelectCommand.Parameters.AddWithValue("eid", Session("Eid"))
        oda.SelectCommand.Parameters.AddWithValue("UVDType", ddlDocumentType.SelectedItem.Text)
        oda.SelectCommand.Parameters.AddWithValue("UVUserField", ddUserField.SelectedItem.Value)
        oda.SelectCommand.Parameters.AddWithValue("UVVehicleField", ddVechicleField.SelectedItem.Value)
        oda.SelectCommand.Parameters.AddWithValue("UVStartDateTime", StartDate.SelectedValue)
        oda.SelectCommand.Parameters.AddWithValue("UVEndDateTime", EndDate.SelectedValue)
        If con.State <> ConnectionState.Open Then
            con.Open()
        End If
        oda.SelectCommand.ExecuteNonQuery()
        uservehiclemap.Text = "User Vehicle Mapping has been saved"

        oda.Dispose()
        con.Dispose()
    End Sub
    Protected Sub btnImei_Click(sender As Object, e As System.EventArgs) Handles btnImei.Click

        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim con As New SqlConnection(conStr)
        Dim oda As SqlDataAdapter = New SqlDataAdapter("uspUpdateVehicleIMEIMapping", con)
        oda.SelectCommand.CommandType = CommandType.StoredProcedure
        oda.SelectCommand.Parameters.AddWithValue("eid", Session("Eid"))
        oda.SelectCommand.Parameters.AddWithValue("VIDType", ddlVDtype.SelectedItem.Text)
        oda.SelectCommand.Parameters.AddWithValue("VIVehicleField", ddVehMapping.SelectedItem.Value)
        oda.SelectCommand.Parameters.AddWithValue("VIImeiField", ddVIMEIfield.SelectedItem.Value)
        '    oda.SelectCommand.Parameters.AddWithValue("VIDriverNamefield", ddDriverNamefield.SelectedItem.Value)
        '  oda.SelectCommand.Parameters.AddWithValue("VIDrivermnofield", drivermnofield.SelectedItem.Value)
        If con.State <> ConnectionState.Open Then
            con.Open()
        End If
        oda.SelectCommand.ExecuteNonQuery()
        savemappingdocumenttype.Text = "Vehicle IMEI Mapping has been saved"


        oda.Dispose()
        con.Dispose()
    End Sub

    Protected Sub btnUname_Clickc(sender As Object, e As System.EventArgs) Handles btnUnamec.Click
        Dim constr As String = ConfigurationManager.ConnectionStrings("constr").ConnectionString
        Dim con As SqlConnection = New SqlConnection(constr)
        Dim oda As SqlDataAdapter = New SqlDataAdapter("", con)
        oda.SelectCommand.CommandText = "update mmm_mst_gpssetting set Cab_Vehicle_doc='" & ddlDocumentTypec.SelectedItem.Value & "',cab_vehicle_status='" & ddstatus.SelectedItem.Value & "',cab_owner='" & ddowner.SelectedItem.Value & "',Cab_rate_card_doc='" & ddRatecarddoc.SelectedItem.Value & "',Cab_Customer_doc='" & ddlCustomerDoc.SelectedItem.Value & "',Vehicle_Type='" + ddVehicleType.SelectedItem.Value + "'  where eid=" & Session("eid").ToString & "  "
        If con.State <> ConnectionState.Open Then
            con.Open()
        End If
        oda.SelectCommand.ExecuteNonQuery()
        savedvehiclemapping.Text = "GPS  Mapping has been saved"
        '   UpdatePanel1.Update()
        con.Close()
        oda.Dispose()

    End Sub

    Protected Sub FilldropDownc(ByVal sender As Object, ByVal e As System.EventArgs)
        Dim constr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim con As SqlConnection = New SqlConnection(constr)
        Dim oda As SqlDataAdapter = New SqlDataAdapter("", con)
        oda.SelectCommand.CommandText = "select DISTINCT displayname,fieldmapping FROM MMM_MST_FIELDS WHERE DOCUMENTTYPE= '" & ddlDocumentTypec.SelectedItem.Text & "' AND EID= " & Session("EID") & " order by displayname"
        Dim ds As New DataSet()
        oda.Fill(ds, "data")
        ddstatus.Items.Clear()
        For r As Integer = 0 To ds.Tables("data").Rows.Count - 1
            ddstatus.Items.Add(ds.Tables("data").Rows(r).Item("displayname").ToString.ToUpper)
            ddstatus.Items(r).Value = ds.Tables("data").Rows(r).Item("displayname").ToString.ToUpper
            'ddstatus.Items.Add(ds.Tables("data").Rows(r).Item("").ToString.ToUpper)

        Next
        oda.SelectCommand.CommandText = "select * from  MMM_MST_FORMS where FormSource='MENU DRIVEN'  and Eid=" & Session("EID") & " order by FormName"
        oda.Fill(ds, "docname")
        ddRatecarddoc.Items.Clear()
        For r As Integer = 0 To ds.Tables("docname").Rows.Count - 1
            ddRatecarddoc.Items.Add(ds.Tables("docname").Rows(r).Item("FormName").ToString.ToUpper)
            ddRatecarddoc.Items(r).Value = ds.Tables("docname").Rows(r).Item("FormName").ToString.ToUpper
            'ddstatus.Items.Add(ds.Tables("data").Rows(r).Item("").ToString.ToUpper)

        Next

        ddlCustomerDoc.Items.Clear()
        For r As Integer = 0 To ds.Tables("docname").Rows.Count - 1
            ddlCustomerDoc.Items.Add(ds.Tables("docname").Rows(r).Item("FormName").ToString.ToUpper)
            ddlCustomerDoc.Items(r).Value = ds.Tables("docname").Rows(r).Item("FormName").ToString.ToUpper
            'ddstatus.Items.Add(ds.Tables("data").Rows(r).Item("").ToString.ToUpper)

        Next

        'oda.SelectCommand.CommandText = "select DISTINCT displayname,fieldmapping FROM MMM_MST_FIELDS WHERE DOCUMENTTYPE='" & ddlDocumentTypec.SelectedItem.Text & "' AND EID= " & Session("EID") & ""

        ' oda.Fill(ds, "data")
        ddowner.Items.Clear()
        For r As Integer = 0 To ds.Tables("data").Rows.Count - 1
            ddowner.Items.Add(ds.Tables("data").Rows(r).Item("displayname").ToString.ToUpper)
            ddowner.Items(r).Value = ds.Tables("data").Rows(r).Item("displayname").ToString.ToUpper
            'ddstatus.Items.Add(ds.Tables("data").Rows(r).Item("").ToString.ToUpper)

        Next


        ddVehicleType.Items.Clear()
        For r As Integer = 0 To ds.Tables("data").Rows.Count - 1
            ddVehicleType.Items.Add(ds.Tables("data").Rows(r).Item("displayname").ToString.ToUpper)
            ddVehicleType.Items(r).Value = ds.Tables("data").Rows(r).Item("displayname").ToString.ToUpper
            'ddstatus.Items.Add(ds.Tables("data").Rows(r).Item("").ToString.ToUpper)
        Next


        ' UpdatePanel1.Update()

    End Sub

    Protected Sub APISave_Click(sender As Object, e As System.EventArgs) Handles APISave.Click
        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim con As New SqlConnection(conStr)
        Try
            Dim qry = "Update MMM_mst_Entity Set DefaultPage='" & ddlHomeMap.SelectedValue & "', MapType='" & ddlHomeMap.SelectedItem.Text & "' "
            qry &= " where Eid=" & Session("Eid") & ""

            Dim cmd As New SqlCommand(qry, con)
            ' Dim oda As SqlDataAdapter = New SqlDataAdapter("uspUpdateMapAPI", con)
            If con.State <> ConnectionState.Open Then
                con.Open()
            End If
            Dim rowAff As Integer = cmd.ExecuteNonQuery()
            'Dim rowAff As Integer = 1
            If rowAff > 0 Then
                Label1.Text = "Data saved."
                Label1.ForeColor = Drawing.Color.Green
            Else
                Label1.Text = "Try again.."
                Label1.ForeColor = Drawing.Color.Red
            End If
        Catch ex As Exception
            Label1.Text = "Try again.."
            Label1.ForeColor = Drawing.Color.Red
        Finally
            con.Close()
            con.Dispose()
        End Try

    End Sub

    Protected Sub doctypecal_SelectedIndexChanged(sender As Object, e As EventArgs) Handles doctypecal.SelectedIndexChanged
        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim con As New SqlConnection(conStr)
        Dim da As New SqlDataAdapter("", con)
        Dim ds As New DataSet

        da.SelectCommand.CommandText = "select DISTINCT displayname,fieldmapping FROM MMM_MST_FIELDS WHERE (datatype='Datetime' or datatype='New Datetime') AND EID= " & Session("EID") & " and documenttype='" & doctypecal.SelectedItem.Text.ToString() & "'"
        da.Fill(ds, "date")
        strtdatecal.Items.Clear()

        strtdatecal.DataSource = ds.Tables("date")
        strtdatecal.DataTextField = "displayname"
        strtdatecal.DataValueField = "fieldmapping"
        strtdatecal.DataBind()
        strtdatecal.Items.Insert(0, "Select")
        strtdatecal.Items(0).Value = 0


        'strtdatecal.Items.Add("Select")
        'For i As Integer = 0 To ds.Tables("date").Rows.Count - 1
        '    strtdatecal.Items.Add(ds.Tables("date").Rows(i).Item(0))
        '    strtdatecal.Items(i).Value = ds.Tables("date").Rows(i).Item(1)
        'Next

        enddatecal.Items.Clear()
        enddatecal.DataSource = ds.Tables("date")
        enddatecal.DataTextField = "displayname"
        enddatecal.DataValueField = "fieldmapping"
        enddatecal.DataBind()
        enddatecal.Items.Insert(0, "Select")
        enddatecal.Items(0).Value = 0

        'enddatecal.Items.Add("Select")
        'For i As Integer = 0 To ds.Tables("date").Rows.Count - 1
        '    enddatecal.Items.Add(ds.Tables("date").Rows(i).Item(0))
        '    enddatecal.Items(i).Value = ds.Tables("date").Rows(i).Item(1)
        'Next

        da.SelectCommand.CommandText = "select DISTINCT displayname,fieldmapping FROM MMM_MST_FIELDS WHERE fieldtype='drop down' and dropdown like '%user%' and documenttype='" & doctypecal.SelectedItem.Text.ToString() & "' AND EID= " & Session("EID") & ""
        da.Fill(ds, "data")
        usrfieldcal.Items.Clear()
        'usrfieldcal.Items.Add("Select")

        usrfieldcal.DataSource = ds.Tables("data")
        usrfieldcal.DataValueField = "fieldmapping"
        usrfieldcal.DataTextField = "displayname"
        usrfieldcal.DataBind()
        usrfieldcal.Items.Insert(0, "Select")
        'For i As Integer = 0 To ds.Tables("data").Rows.Count - 1
        '    usrfieldcal.Items.Add(ds.Tables("data").Rows(i).Item(0))
        '    usrfieldcal.Items(i).Value = ds.Tables("data").Rows(i).Item(1)
        'Next
        ds.Dispose()
        da.Dispose()
        con.Dispose()
    End Sub

    Public Sub Editrecord()
        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim con As New SqlConnection(conStr)
        If usrfieldcal.SelectedItem.Text.ToString() = "Select" Then
            calndr.Text = "This Document Type Doesn't Have User Field"
            Exit Sub
        End If
        Dim odaa As SqlDataAdapter = New SqlDataAdapter("Select doctype from mmm_mst_doc_cal where Eid=" & Session("EID") & " ", con)
        Dim dt As New DataTable
        odaa.Fill(dt)
        For i As Integer = 0 To dt.Rows.Count - 1
            If doctypecal.SelectedItem.Text.ToString() = dt.Rows(i).Item("doctype").ToString() Then
                calndr.Text = "Calender Entry for Selected document already Exists,To Change any field, First delete exiting and Create New"
                Exit Sub
            End If
        Next
       

        If btncal.Text = "Save" Then
            Dim oda As SqlDataAdapter = New SqlDataAdapter("uspInsertUserCalender", con)
            oda.SelectCommand.CommandType = CommandType.StoredProcedure
            oda.SelectCommand.Parameters.AddWithValue("eid", Session("Eid"))
            oda.SelectCommand.Parameters.AddWithValue("doctype", doctypecal.SelectedItem.Text)
            oda.SelectCommand.Parameters.AddWithValue("usrfield", usrfieldcal.SelectedValue)
            oda.SelectCommand.Parameters.AddWithValue("startdate", strtdatecal.SelectedValue)
            oda.SelectCommand.Parameters.AddWithValue("enddate", enddatecal.SelectedValue)
            oda.SelectCommand.Parameters.AddWithValue("Nameusrfield", usrfieldcal.SelectedItem.Text)
            oda.SelectCommand.Parameters.AddWithValue("Namestartdate", strtdatecal.SelectedItem.Text)
            oda.SelectCommand.Parameters.AddWithValue("Nameenddate", enddatecal.SelectedItem.Text)
            If con.State <> ConnectionState.Open Then
                con.Open()
            End If
            oda.SelectCommand.ExecuteNonQuery()
            calndr.Text = "Calender Mapping has been  saved"

            'ElseIf btncal.Text = "Update" Then
            '    Dim oda As SqlDataAdapter = New SqlDataAdapter("uspUpdateUserCalender", con)
            '    oda.SelectCommand.CommandType = CommandType.StoredProcedure
            '    oda.SelectCommand.Parameters.AddWithValue("eid", Session("Eid"))
            '    oda.SelectCommand.Parameters.AddWithValue("doctype", doctypecal.SelectedItem.Text)
            '    oda.SelectCommand.Parameters.AddWithValue("usrfield", usrfieldcal.SelectedItem.Value)
            '    oda.SelectCommand.Parameters.AddWithValue("startdate", strtdatecal.SelectedValue)
            '    oda.SelectCommand.Parameters.AddWithValue("enddate", enddatecal.SelectedValue)
            '    If con.State <> ConnectionState.Open Then
            '        con.Open()
            '    End If
            '    oda.SelectCommand.ExecuteNonQuery()
            '    calndr.Text = "Calender Mapping has been Updated"
        End If
        bindgridrecord()
        Reset()
        con.Dispose()
    End Sub

    'Protected Sub EditHit(ByVal sender As Object, ByVal e As System.EventArgs)
    '    btncal.Text = "Update"
    '    Dim btnDetails As ImageButton = TryCast(sender, ImageButton)
    '    Dim row As GridViewRow = DirectCast(btnDetails.NamingContainer, GridViewRow)
    '    Dim Tid As Integer = Convert.ToString(Me.gvData.DataKeys(row.RowIndex).Value)
    '    Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
    '    Dim con As New SqlConnection(conStr)
    '    Dim da As SqlDataAdapter = New SqlDataAdapter("", con)
    '    Dim ds As New DataSet
    '    FilldropdownCal()
    '    da.SelectCommand.CommandText = "SELECT * FROM MMM_MST_DOC_CAl WHERE Tid = " & Tid & " AND EID = " & Session("EID").ToString() & ""
    '    da.Fill(ds, "Cal")
    '    doctypecal.SelectedIndex = doctypecal.Items.IndexOf(doctypecal.Items.FindByText(ds.Tables("Cal").Rows(0).Item("doctype").ToString.ToUpper()))
    '    da.SelectCommand.CommandText = "SELECT * displayname,fieldmapping from mmm_mst_fields WHERE EID = " & Session("EID").ToString() & " AND (datatype='Datetime' or datatype='New Datetime') and fieldmapping in ('" & ds.Tables("Cal").Rows(0).Item("startdate").ToString() & "','" & ds.Tables("Cal").Rows(0).Item("enddate").ToString() & "') "
    '    da.Fill(ds, "Strtdate")
    '    strtdatecal.SelectedIndex = strtdatecal.Items.IndexOf(strtdatecal.Items.FindByText(ds.Tables("Cal").Rows(0).Item("startdate").ToString.ToUpper()))
    '    enddatecal.SelectedIndex = enddatecal.Items.IndexOf(enddatecal.Items.FindByText(ds.Tables("Cal").Rows(0).Item("enddate").ToString.ToUpper()))
    '    da.SelectCommand.CommandText = "SELECT * displayname,fieldmapping from mmm_mst_fields WHERE EID = " & Session("EID").ToString() & " AND fieldmapping in ('" & ds.Tables("Cal").Rows(0).Item("usrfield").ToString.ToUpper() & "') and dropdown like '%user%'"
    '    da.Fill(ds, "userfld")
    '    usrfieldcal.SelectedIndex = usrfieldcal.Items.IndexOf(usrfieldcal.Items.FindByText(ds.Tables("Cal").Rows(0).Item("usrfield").ToString.ToUpper()))
    '    btncal.Text = "UPDATE"
    '    con.Dispose()
    'End Sub

    Public Sub FilldropdownCal()
        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim con As New SqlConnection(conStr)
        Dim da As SqlDataAdapter = New SqlDataAdapter("", con)
        Dim ds As New DataSet
        da.SelectCommand.CommandText = "select FormId,FormName from  MMM_MST_FORMS where FormSource='MENU DRIVEN'  and Eid=" & Session("EID") & " and isActive=1 order by FormName"
        da.Fill(ds, "data")

        doctypecal.DataSource = ds.Tables("data")
        doctypecal.DataTextField = "FormName"
        doctypecal.DataValueField = "FormID"
        doctypecal.DataBind()
        doctypecal.Items.Insert(0, "Select")
        doctypecal.Items(0).Value = 0


        'doctypecal.Items.Add("Select")
        'For j As Integer = 0 To ds.Tables("data").Rows.Count - 1
        '    doctypecal.Items.Add(ds.Tables("data").Rows(j).Item("FormName").ToString.ToUpper())
        '    doctypecal.Items(j).Value = ds.Tables("data").Rows(j).Item("FormName").ToString.ToUpper()
        'Next

        'da.SelectCommand.CommandText = "select DISTINCT displayname,fieldmapping FROM MMM_MST_FIELDS WHERE (datatype='Datetime' or datatype='New Datetime') AND EID= " & Session("EID") & " order by displayname"
        'da.Fill(ds, "MMM_MST_FIELDS_CAL")
        'usrfieldcal.Items.Add("Select")
        'For j As Integer = 0 To ds.Tables("MMM_MST_FIELDS_CAL").Rows.Count - 1
        '    usrfieldcal.Items.Add(ds.Tables("MMM_MST_FIELDS_CAL").Rows(j).Item(0).ToString.ToUpper())
        '    usrfieldcal.Items(j).Value = ds.Tables("MMM_MST_FIELDS_CAL").Rows(j).Item(1).ToString.ToUpper()
        'Next

        'strtdatecal.Items.Add("Select")
        'For j As Integer = 0 To ds.Tables("MMM_MST_FIELDS_CAL").Rows.Count - 1
        '    strtdatecal.Items.Add(ds.Tables("MMM_MST_FIELDS_CAL").Rows(j).Item(0).ToString.ToUpper())
        '    strtdatecal.Items(j).Value = ds.Tables("MMM_MST_FIELDS_CAL").Rows(j).Item(1).ToString.ToUpper()
        'Next

        'enddatecal.Items.Add("Select")
        'For j As Integer = 0 To ds.Tables("MMM_MST_FIELDS_CAL").Rows.Count - 1
        '    enddatecal.Items.Add(ds.Tables("MMM_MST_FIELDS_CAL").Rows(j).Item(0).ToString.ToUpper())
        '    enddatecal.Items(j).Value = ds.Tables("MMM_MST_FIELDS_CAL").Rows(j).Item(1).ToString.ToUpper()
        'Next
        bindgridrecord()
    End Sub

    Public Sub bindgridrecord()
        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim con As New SqlConnection(conStr)
        Dim da As SqlDataAdapter = New SqlDataAdapter("select * from  MMM_MST_DOC_CAl Where EID ='" & Session("EID") & "'", con)
        Dim dt As New DataTable
        da.Fill(dt)
        gvData.DataSource = dt
        gvData.DataBind()
    End Sub

    Public Sub DeleteHit(ByVal sender As Object, ByVal e As System.EventArgs)
        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim con As New SqlConnection(conStr)
        Dim btnDetails As ImageButton = TryCast(sender, ImageButton)
        Dim row As GridViewRow = DirectCast(btnDetails.NamingContainer, GridViewRow)
        Dim Tid As Integer = Convert.ToString(Me.gvData.DataKeys(row.RowIndex).Value)
        Dim da As SqlDataAdapter = New SqlDataAdapter("Delete from  MMM_MST_DOC_CAl Where tid=" & Tid & " ", con)
        Dim dt As New DataTable
        da.Fill(dt)
        bindgridrecord()
    End Sub

    Public Sub Reset()
        doctypecal.SelectedIndex = 0
        usrfieldcal.SelectedIndex = 0
        strtdatecal.SelectedIndex = 0
        enddatecal.SelectedIndex = 0
        calndr.Text = ""
    End Sub


    Public Sub BindGeofenceDropDowns()
        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim con As New SqlConnection(conStr)
        Dim da As SqlDataAdapter = New SqlDataAdapter("", con)

        Try

        Dim ds As New DataSet
        da.SelectCommand.CommandText = "select FormId,FormName from  MMM_MST_FORMS where Eid=" & Session("EID") & " and isActive=1 order by FormName"
            da.Fill(ds, "Forms")
            ddlGeoDocument.Items.Clear()
            ddlGeoDocument.DataSource = ds.Tables("Forms")
        ddlGeoDocument.DataTextField = "FormName"
        ddlGeoDocument.DataValueField = "FormID"
        ddlGeoDocument.DataBind()
        ddlGeoDocument.Items.Insert(0, "Select")
        ddlGeoDocument.Items(0).Value = 0

            ddlVehicleDoc.Items.Clear()
            ddlVehicleDoc.DataSource = ds.Tables("Forms")
        ddlVehicleDoc.DataTextField = "FormName"
        ddlVehicleDoc.DataValueField = "FormID"
        ddlVehicleDoc.DataBind()
        ddlVehicleDoc.Items.Insert(0, "Select")
            ddlVehicleDoc.Items(0).Value = 0

            da.SelectCommand.CommandText = "Select FieldMapping[Value], DisplayName[Text] from mmm_mst_fields where Eid=" & Session("EID") & " and Documenttype='User'"
            da.Fill(ds, "UserMappings")

            ddlUserMapGeo.Items.Clear()
            ddlUserMapGeo.DataSource = ds.Tables("UserMappings")
            ddlUserMapGeo.DataTextField = "Text"
            ddlUserMapGeo.DataValueField = "Value"
            ddlUserMapGeo.DataBind()
            ddlUserMapGeo.Items.Insert(0, "Select")

            ddlGeoDocName.Items.Insert(0, "Select")
            ddlVehicleNo.Items.Insert(0, "Select")
            ddlVehicleName.Items.Insert(0, "Select")
            ddlVehicleIMEI.Items.Insert(0, "Select")
            ddlVehicleMapGeofence.Items.Insert(0, "Select")
            ddlGeofence.Items.Insert(0, "Select")

        Catch ex As Exception
        Finally
            con.Dispose()
            da.Dispose()
        End Try

    End Sub
    
    Protected Sub ddlGeoDocument_SelectedIndexChanged(sender As Object, e As EventArgs) Handles ddlGeoDocument.SelectedIndexChanged
        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim con As New SqlConnection(conStr)
        Dim da As SqlDataAdapter = New SqlDataAdapter("", con)
        Try
            Dim ds As New DataSet
            da.SelectCommand.CommandText = "Select FieldMapping[Value], DisplayName[Text] from mmm_mst_fields where Eid=" & Session("Eid") & " and Documenttype='" & ddlGeoDocument.SelectedItem.Text & "'"
            da.Fill(ds, "data")
            ddlGeoDocName.Items.Clear()
            ddlGeoDocName.DataSource = ds.Tables("data")
            ddlGeoDocName.DataTextField = "Text"
            ddlGeoDocName.DataValueField = "Value"
            ddlGeoDocName.DataBind()
            ddlGeoDocName.Items.Insert(0, "Select")

            ddlGeofence.Items.Clear()
            ddlGeofence.DataSource = ds.Tables("data")
            ddlGeofence.DataTextField = "Text"
            ddlGeofence.DataValueField = "Value"
            ddlGeofence.DataBind()
            ddlGeofence.Items.Insert(0, "Select")

        Catch ex As Exception
        Finally
            con.Dispose()
            da.Dispose()
        End Try
    End Sub

    Protected Sub ddlGeoDocType_SelectedIndexChanged(sender As Object, e As EventArgs) Handles ddlGeoDocType.SelectedIndexChanged
        Try
            If ddlGeoDocType.SelectedValue = "Others" Then
                trGeoField.Visible = True
            Else
                trGeoField.Visible = False
            End If
        Catch ex As Exception

        End Try
    End Sub

    Protected Sub ddlVehicleDoc_SelectedIndexChanged(sender As Object, e As EventArgs) Handles ddlVehicleDoc.SelectedIndexChanged
        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim con As New SqlConnection(conStr)
        Dim da As SqlDataAdapter = New SqlDataAdapter("", con)
        Try
            Dim ds As New DataSet
            da.SelectCommand.CommandText = "Select FieldMapping[Value], DisplayName[Text] from mmm_mst_fields where Eid=" & Session("Eid") & " and Documenttype='" & ddlVehicleDoc.SelectedItem.Text & "'"
            da.Fill(ds, "data")

            ddlVehicleNo.Items.Clear()
            ddlVehicleNo.DataSource = ds.Tables("data")
            ddlVehicleNo.DataTextField = "Text"
            ddlVehicleNo.DataValueField = "Value"
            ddlVehicleNo.DataBind()

            ddlVehicleName.Items.Clear()
            ddlVehicleName.DataSource = ds.Tables("data")
            ddlVehicleName.DataTextField = "Text"
            ddlVehicleName.DataValueField = "Value"
            ddlVehicleName.DataBind()

            ddlVehicleIMEI.Items.Clear()
            ddlVehicleIMEI.DataSource = ds.Tables("data")
            ddlVehicleIMEI.DataTextField = "Text"
            ddlVehicleIMEI.DataValueField = "Value"
            ddlVehicleIMEI.DataBind()

            ddlVehicleMapGeofence.Items.Clear()
            ddlVehicleMapGeofence.DataSource = ds.Tables("data")
            ddlVehicleMapGeofence.DataTextField = "Text"
            ddlVehicleMapGeofence.DataValueField = "Value"
            ddlVehicleMapGeofence.DataBind()

        Catch ex As Exception
            con.Dispose()
            da.Dispose()
        End Try
    End Sub

    Protected Sub btnSaveGeofenceAlertSettings_Click(sender As Object, e As EventArgs) Handles btnSaveGeofenceAlertSettings.Click
        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim con As New SqlConnection(conStr)
        divMsg.InnerHtml = "<div class='green'>Processing data. Please wait...</div>"
        divMsg.Visible = True
        Try
            Dim valid As Boolean = ValidateGeofenceAlertSettings()
            If valid Then
                If Not con.State = ConnectionState.Open Then
                    con.Open()
                End If

                Dim qry As String = "Insert into mmm_mst_GeofenceAlertSettings(Eid,GeofenceDocument,GeofenceType,GeofenceField,GeofenceDocName,VehicleDocument,VehicleName,VehicleNo,VehicleIMEI,VehicleMapGeofence,UserMapGeofenceDoc)"
                qry &= " Values(" & Session("Eid") & ", '" & ddlGeoDocument.SelectedItem.Text & "', '" & ddlGeoDocType.SelectedItem.Text & "',"
                If ddlGeoDocType.SelectedItem.Text = "Others" Then
                    qry &= "'" & ddlGeofence.SelectedValue & "'"
                Else
                    qry &= "null, "
                End If
                qry &= "'" & ddlGeoDocName.SelectedValue & "',"
                qry &= "'" & ddlVehicleDoc.SelectedItem.Text & "',"
                qry &= "'" & ddlVehicleName.SelectedValue & "',"
                qry &= "'" & ddlVehicleNo.SelectedValue & "',"
                qry &= "'" & ddlVehicleIMEI.SelectedValue & "',"
                qry &= "'" & ddlVehicleMapGeofence.SelectedValue & "',"
                qry &= "'" & ddlUserMapGeo.SelectedValue & "')"

                Dim cmd As New SqlCommand(qry, con)
                Dim rowAff As Integer = cmd.ExecuteNonQuery()
                If rowAff > 0 Then
                    Dim str As String = "<div class='green'>"
                    str &= "Data saved successfully."
                    str &= "</div>"
                    divMsg.Visible = True
                    divMsg.InnerHtml = str
                Else
                    Dim str As String = "<div class='red'>"
                    str &= "Server error. Please contact your system administrator."
                    str &= "</div>"
                    divMsg.Visible = True
                    divMsg.InnerHtml = str
                End If

            End If
        Catch ex As Exception

        End Try
    End Sub


    Private Function ValidateGeofenceAlertSettings() As Boolean
        Dim ErrorMsg As String = ""
        If ddlGeoDocument.SelectedValue.ToUpper() = "0" Or ddlGeoDocument.SelectedValue.ToUpper() = "SELECT" Then
            ErrorMsg &= "Geofence Document is required.<br/>"
        End If
        If ddlGeoDocName.SelectedValue = "0" Or ddlGeoDocName.SelectedValue.ToUpper() = "SELECT" Then
            ErrorMsg &= "Geofence Document name is required.<br/>"
        End If
        If ddlGeoDocType.SelectedValue.ToUpper() = "SELECT" Or ddlGeoDocType.SelectedValue.ToUpper() = "0" Then
            ErrorMsg &= "Geofence Document type is required.<br/>"
        Else
            If ddlGeofence.SelectedValue = "0" Or ddlGeofence.SelectedValue.ToUpper() = "SELECT" Then
                ErrorMsg &= "Geofence field is required.<br/>"
            End If
        End If
        If ddlUserMapGeo.SelectedValue = "0" Or ddlUserMapGeo.SelectedValue.ToUpper() = "SELECT" Then
            ErrorMsg &= "User map geofence document name is required.<br/>"
        End If
        If ddlVehicleDoc.SelectedValue = "0" Or ddlVehicleDoc.SelectedValue.ToUpper() = "SELECT" Then
            ErrorMsg &= "Vehicle document name is required.<br/>"
        End If
        If ddlVehicleNo.SelectedValue = "0" Or ddlVehicleNo.SelectedValue.ToUpper() = "SELECT" Then
            ErrorMsg &= "Vehicle no. is required.<br/>"
        End If
        If ddlVehicleName.SelectedValue = "0" Or ddlVehicleName.SelectedValue.ToUpper() = "SELECT" Then
            ErrorMsg &= "Vehicle name is required.<br/>"
        End If
        If ddlVehicleIMEI.SelectedValue = "0" Or ddlVehicleIMEI.SelectedValue.ToUpper() = "SELECT" Then
            ErrorMsg &= "Vehicle IMEI no. is required.<br/>"
        End If

        If Not ErrorMsg = "" Then
            Dim str As String = "<div class='red'>"
            str &= ErrorMsg
            str &= "</div>"
            divMsg.Visible = True
            divMsg.InnerHtml = str
            Return False
        Else
            Return True
        End If
        Return False
    End Function

End Class
