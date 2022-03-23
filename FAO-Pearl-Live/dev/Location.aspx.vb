
Imports System.Data
Imports System.Data.SqlClient

Partial Class Location
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Not IsPostBack Then
            btnEdit_ModalPopupExtender.Hide()

            Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
            Dim con As New SqlConnection(conStr)
            'fill Product  
            Dim da As New SqlDataAdapter("select disname,colname from MMM_MST_SEARCH where tablename='LOCATION' order by DisName", con)
            Dim dtLoc As New DataTable
            da.Fill(dtLoc)
            For i As Integer = 0 To dtLoc.Rows.Count - 1
                ddlLocationName.Items.Add(dtLoc.Rows(i).Item(0))
                ddlLocationName.Items(i).Value = dtLoc.Rows(i).Item(1)
            Next

            da.SelectCommand.CommandText = "select ZoneId,UPPER(ZoneName + ' (' + zoneoff + ')') [ZoneName] From MMM_MST_TIMEZONE"
            da.SelectCommand.CommandType = CommandType.Text
            Dim dtZone As New DataTable
            da.Fill(dtZone)

            For i As Integer = 0 To dtZone.Rows.Count - 1
                ddlTimeZone.Items.Add(dtZone.Rows(i).Item(1))
                ddlTimeZone.Items(i).Value = dtZone.Rows(i).Item(0)
            Next

            da.SelectCommand.CommandText = "select distinct country from MMM_MST_location "
            da.SelectCommand.CommandType = CommandType.Text
            Dim ds As New DataSet
            da.Fill(ds)
            ddlCountry.DataSource = ds
            ddlCountry.DataTextField = "country"
            ddlCountry.DataBind()
            ddlCountry.Items.Insert(0, "Please Select")
            ds.Dispose()
            con.Dispose()


            bindGridLoc()
            da.Dispose()
            dtLoc.Dispose()
            dtZone.Dispose()
            con.Close()
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
    Public Sub bindGridLoc()
        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim con As SqlConnection = New SqlConnection(conStr)
        Dim oda As SqlDataAdapter = New SqlDataAdapter("select loc.locid, case when isdefault='1' then loc.locationName + ' (DEFAULT)' else loc.locationName end [locationName], loc.timeFormat,loc.ZoneID,time.ZoneID, UPPER(time.ZoneName + ' (' + time.zoneoff + ')') [ZoneName]   From MMM_MST_LOCATION as loc inner join MMM_MST_TIMEZONE as time on loc.ZoneID=time.ZoneID ", con)
        Dim ds As New DataSet()
        oda.Fill(ds, "data")
        gvData.DataSource = ds.Tables("data")
        gvData.DataBind()
        ds.Dispose()
        oda.Dispose()
        con.Close()
    End Sub
    
    Public Sub Add(ByVal sender As Object, ByVal e As System.EventArgs)
        txtLocName.Text = ""
        lblRecord.Visible = False
        'chkDefLoc.Checked = False

        ddlCountry.Items.Clear()
        ddlState.Items.Clear()
        updatePanelEdit.Update()

        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim con As New SqlConnection(conStr)
        Dim da As New SqlDataAdapter("", con)
        da.SelectCommand.CommandText = "select distinct country from MMM_MST_location "
        da.SelectCommand.CommandType = CommandType.Text
        Dim ds As New DataSet
        da.Fill(ds)
        ddlCountry.DataSource = ds
        ddlCountry.DataTextField = "country"
        ddlCountry.DataBind()
        ddlCountry.Items.Insert(0, "Please Select")
        ds.Dispose()
        con.Dispose()

        btnActUserSave.Text = "Save"
        Me.updatePanelEdit.Update()
        Me.btnEdit_ModalPopupExtender.Show()
        '  ds.Dispose()
        '  oda.Dispose()
        '  con.Close()
    End Sub

    Protected Sub EditHit(ByVal sender As Object, ByVal e As System.EventArgs)
        lblRecord.Visible = False
        Dim btnDetails As ImageButton = TryCast(sender, ImageButton)
        Dim row As GridViewRow = DirectCast(btnDetails.NamingContainer, GridViewRow)
        Dim pid As Integer = Convert.ToString(Me.gvData.DataKeys(row.RowIndex).Value)

        ' No Value in Session just fill the Edit Form and Show two button
        btnActUserSave.Text = "Update"
        'two methods.. either show data from Grid or Show data from Database.
        ViewState("pid") = pid
        txtLocName.Text = row.Cells(1).Text

        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim con As SqlConnection = New SqlConnection(conStr)
        Dim oda As SqlDataAdapter = New SqlDataAdapter("select * From MMM_MST_LOCATION WHERE locid=" & pid & " ", con)
        oda.SelectCommand.CommandType = CommandType.Text
        Dim dt As New DataTable
        oda.Fill(dt)

        Dim ZonID As Integer
        Dim isDef As String = ""
        If dt.Rows.Count > 0 Then
            ZonID = dt.Rows(0).Item("zoneid")
            isDef = dt.Rows(0).Item("isdefault").ToString
        End If

        'If isDef = "1" Then
        '    chkDefLoc.Checked = True
        'Else
        '    chkDefLoc.Checked = False
        'End If

        ddlState.Items.Clear()
        updatePanelEdit.Update()

        oda.SelectCommand.CommandText = "select distinct sid , locationstate from MMM_MST_LOCATION where country='" & dt.Rows(0).Item("country").ToString() & "' order by locationstate"
        oda.SelectCommand.CommandType = CommandType.Text
        Dim ds As New DataTable
        oda.Fill(ds)
        ddlState.DataSource = ds
        ddlState.DataTextField = "locationstate"
        ddlState.DataValueField = "sid"
        ddlState.DataBind()


        ddlTimeZone.SelectedIndex = ddlTimeZone.Items.IndexOf(ddlTimeZone.Items.FindByValue(ZonID))
        ddlCountry.SelectedIndex = ddlCountry.Items.IndexOf(ddlCountry.Items.FindByText(dt.Rows(0).Item("country").ToString()))
        ddlState.SelectedIndex = ddlState.Items.IndexOf(ddlState.Items.FindByText(dt.Rows(0).Item("Locationstate").ToString()))

        Me.updatePanelEdit.Update()
        Me.btnEdit_ModalPopupExtender.Show()

        oda.Dispose()
        con.Close()
        dt.Dispose()
    End Sub

    Protected Sub DeleteRecord(ByVal sender As Object, ByVal e As System.EventArgs)
        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim con As SqlConnection = New SqlConnection(conStr)
        Dim oda As SqlDataAdapter = New SqlDataAdapter("uspDeleteUserInLocation", con)
        oda.SelectCommand.CommandType = CommandType.StoredProcedure
        oda.SelectCommand.Parameters.AddWithValue("tid", Val(ViewState("Did").ToString))
        If con.State <> ConnectionState.Open Then
            con.Open()
        End If
        oda.SelectCommand.ExecuteNonQuery()
        Dim ds As New DataSet()

        oda.SelectCommand.CommandText = "select loc.locid,loc.locationName,loc.timeFormat,loc.ZoneID,time.ZoneID,time.ZoneName From MMM_MST_LOCATION as loc inner join MMM_MST_TIMEZONE as time on loc.ZoneID=time.ZoneID  where loc.EID='" & Session("EID").ToString() & "'"
        oda.SelectCommand.CommandType = CommandType.Text
        oda.Fill(ds, "data")
        gvData.DataSource = ds.Tables("data")
        gvData.DataBind()


        lblRecord.Visible = True
        lblRecord.Text = " Location Deleted Successfully"
        con.Close()
        oda.Dispose()
        con.Dispose()
        Me.updatePanelEdit.Update()
        Me.btnDelete_ModalPopupExtender.Hide()

    End Sub

    Protected Sub DeleteHit(ByVal sender As Object, ByVal e As System.EventArgs)
        lblRecord.Visible = False
        Dim btnDetails As ImageButton = TryCast(sender, ImageButton)
        Dim row As GridViewRow = DirectCast(btnDetails.NamingContainer, GridViewRow)
        Dim pid As Integer = Convert.ToString(Me.gvData.DataKeys(row.RowIndex).Value)
        ViewState("Did") = pid

        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim con As SqlConnection = New SqlConnection(conStr)
        Dim oda As SqlDataAdapter = New SqlDataAdapter("uspDeleteChkLocation", con)
        oda.SelectCommand.CommandType = CommandType.StoredProcedure
        oda.SelectCommand.Parameters.AddWithValue("Pid", Val(ViewState("Did").ToString()))
        oda.SelectCommand.Parameters.AddWithValue("EID", Session("EID").ToString())
        Dim ds As New DataSet()
        If con.State <> ConnectionState.Open Then
            con.Open()
        End If
        Dim res As String = oda.SelectCommand.ExecuteScalar()
        If res = "Yes" Then

            lblMsgDelete.Text = "Are you Sure Want to delete this Record? " & row.Cells(1).Text
            btnActDelete.Visible = True

            
        Else
            lblMsgDelete.Text = " This location cant be deleted Because User Exist this Location "
            btnActDelete.Visible = False
        End If
        Me.updatePanelDelete.Update()
        Me.btnDelete_ModalPopupExtender.Show()



        oda.Dispose()
        con.Close()
        ds.Dispose()

    End Sub

    Protected Sub btnActUserSave_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnActUserSave.Click

        If ddlCountry.SelectedItem.Text = "Please Select" Then
            lblMsgEdit.Text = "Please select State "
            Exit Sub
        End If

        If ddlState.SelectedItem.Text = "Please Select" Then
            lblMsgEdit.Text = "Please select State "
            Exit Sub
        End If


        If btnActUserSave.Text = "Save" Then

            'Dim chkstrin As String = ""
            'If chkDefLoc.Checked = True Then
            '    chkstrin = "1"
            'Else
            '    chkstrin = "0"
            'End If

            Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
            Dim con As SqlConnection = New SqlConnection(conStr)
            Dim oda As SqlDataAdapter = New SqlDataAdapter("uspInsertLocationNew1", con)
            oda.SelectCommand.CommandType = CommandType.StoredProcedure
            'oda.SelectCommand.Parameters.AddWithValue("EID", Session("EID").ToString())
            oda.SelectCommand.Parameters.AddWithValue("locationName", txtLocName.Text)
            oda.SelectCommand.Parameters.AddWithValue("timeformate", ddlTimeFormate.SelectedItem.Value)
            oda.SelectCommand.Parameters.AddWithValue("ZoneId", ddlTimeZone.SelectedItem.Value())
            'oda.SelectCommand.Parameters.AddWithValue("defLoc", chkstrin)
            oda.SelectCommand.Parameters.AddWithValue("country", ddlCountry.SelectedItem.Text)
            oda.SelectCommand.Parameters.AddWithValue("state", ddlState.SelectedItem.Text)
            oda.SelectCommand.Parameters.AddWithValue("Sid", ddlState.SelectedItem.Value)


            If con.State <> ConnectionState.Open Then
                con.Open()
            End If
            oda.SelectCommand.ExecuteNonQuery()
            oda.SelectCommand.CommandText = "select loc.locid,loc.LocationName,loc.timeFormat,loc.ZoneID,time.ZoneID,time.ZoneName From MMM_MST_Location as loc inner join MMM_MST_TIMEZONE as time on loc.ZoneID=time.ZoneID  "
            oda.SelectCommand.CommandType = CommandType.Text
            Dim ds As New DataSet()
            oda.Fill(ds, "data")
            gvData.DataSource = ds.Tables("data")
            gvData.DataBind()


            lblRecord.Visible = True
            lblRecord.Text = " Location Created successfully "
            Me.updatePanelEdit.Update()
            Me.btnEdit_ModalPopupExtender.Hide()

            con.Close()
            oda.Dispose()

        ElseIf btnActUserSave.Text = "Update" Then

            Dim chkstrin As String = ""
            'If chkDefLoc.Checked = True Then
            '    chkstrin = "1"
            'Else
            '    chkstrin = "0"
            'End If
            Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
            Dim con As SqlConnection = New SqlConnection(conStr)
            Dim oda As SqlDataAdapter = New SqlDataAdapter("uspUpdateLocationNew1", con)
            oda.SelectCommand.CommandType = CommandType.StoredProcedure
            'oda.SelectCommand.Parameters.AddWithValue("EID", Session("EID").ToString())
            oda.SelectCommand.Parameters.AddWithValue("locationName", txtLocName.Text)
            oda.SelectCommand.Parameters.AddWithValue("timeformate", ddlTimeFormate.SelectedItem.Value)
            oda.SelectCommand.Parameters.AddWithValue("Zoneid", ddlTimeZone.SelectedItem.Value())
            oda.SelectCommand.Parameters.AddWithValue("pid", ViewState("pid").ToString())
            'oda.SelectCommand.Parameters.AddWithValue("isdefLoc", chkstrin)
            oda.SelectCommand.Parameters.AddWithValue("country", ddlCountry.SelectedItem.Text)
            oda.SelectCommand.Parameters.AddWithValue("state", ddlState.SelectedItem.Text)
            oda.SelectCommand.Parameters.AddWithValue("Sid", ddlState.SelectedItem.Value)

            If con.State <> ConnectionState.Open Then
                con.Open()
            End If
            oda.SelectCommand.ExecuteNonQuery()
            oda.SelectCommand.CommandText = "select loc.locid,loc.locationName,loc.timeFormat,loc.ZoneID,time.ZoneID,time.ZoneName From MMM_MST_LOCATION as loc inner join MMM_MST_TIMEZONE as time on loc.ZoneID=time.ZoneID  "
            oda.SelectCommand.CommandType = CommandType.Text
            Dim ds As New DataSet()
            oda.Fill(ds, "data")
            gvData.DataSource = ds.Tables("data")
            gvData.DataBind()
            lblRecord.Visible = True
            lblRecord.Text = "Location Updated Successfully"
            Me.updatePanelEdit.Update()
            Me.btnEdit_ModalPopupExtender.Hide()
            con.Close()
            con.Dispose()
            oda.Dispose()
            txtLocName.Text = ""

        End If

        'ddlState.Items.Clear()
        'updatePanelEdit.Update()

    End Sub
 

    Protected Sub gvData_PageIndexChanging(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.GridViewPageEventArgs) Handles gvData.PageIndexChanging
        gvData.PageIndex = e.NewPageIndex
        bindGridLoc()
    End Sub

    Protected Sub Search(ByVal sender As Object, ByVal e As System.Web.UI.ImageClickEventArgs) Handles btnSearch.Click

        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim con As SqlConnection = New SqlConnection(conStr)
        Dim oda As SqlDataAdapter
        If ddlLocationName.SelectedValue = "LocationName" Then
            oda = New SqlDataAdapter("select loc.locid,loc.locationName,loc.timeFormat,loc.ZoneID,time.ZoneID,time.ZoneName From MMM_MST_LOCATION as loc inner join MMM_MST_TIMEZONE as time on loc.ZoneID=time.ZoneID  where loc.locationName like '" & txtValue.Text & "%'  ", con)
        Else
            oda = New SqlDataAdapter("select loc.locid,loc.locationName,loc.timeFormat,loc.ZoneID,time.ZoneID,time.ZoneName From MMM_MST_LOCATION as loc inner join MMM_MST_TIMEZONE as time on loc.ZoneID=time.ZoneID  where time.ZoneName like '" & txtValue.Text & "%' ", con)
        End If
        Dim ds As New DataSet()
        oda.Fill(ds, "data")

        gvData.DataSource = ds.Tables("data")
        gvData.DataBind()
        If gvData.Rows.Count = 0 Then
            lblRecord.Visible = True
            lblRecord.Text = "No Record Found"
        Else
            lblRecord.Visible = False
        End If
        ds.Dispose()
        oda.Dispose()
        con.Close()


    End Sub

    Protected Sub ddlCountry_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles ddlCountry.SelectedIndexChanged

        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim con As SqlConnection = New SqlConnection(conStr)
        Dim da As SqlDataAdapter = New SqlDataAdapter("select Distinct sid, locationstate from MMM_MST_LOCATION where country='" & ddlCountry.SelectedItem.Text & "' order by locationstate", con)
        Dim ds As New DataSet
        da.Fill(ds)
        ddlState.DataSource = ds
        ddlState.DataTextField = "locationstate"
        ddlState.DataValueField = "sid"
        ddlState.DataBind()
        ddlState.Items.Insert(0, "Please Select")
        con.Close()
        da.Dispose()


    End Sub
End Class
