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
            Dim da As New SqlDataAdapter("select disname,colname from MMM_MST_SEARCH where tablename='HOLIDAY' order by DisName", con)
            Dim ds As New DataSet
            da.Fill(ds, "data")
            For i As Integer = 0 To ds.Tables("data").Rows.Count - 1
                ddlHolidayName.Items.Add(ds.Tables("data").Rows(i).Item(0))
                ddlHolidayName.Items(i).Value = ds.Tables("data").Rows(i).Item(1)
            Next
            bindGridHoli()

            da.SelectCommand.CommandText = "select locId,LocationName From MMM_MST_LOCATION where eid=" & Session("EID")
            da.Fill(ds, "lOC")
            For i As Integer = 0 To ds.Tables("LOC").Rows.Count - 1
                ddlHoliLocation.Items.Add(ds.Tables("LOC").Rows(i).Item(1))
                ddlHoliLocation.Items(i).Value = ds.Tables("LOC").Rows(i).Item(0)
            Next

            da.Dispose()
            ds.Dispose()
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
    Public Sub bindGridHoli()
        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim con As SqlConnection = New SqlConnection(conStr)
        Dim oda As SqlDataAdapter = New SqlDataAdapter("select H.HoliID,H.locID,H.holidayname,replace(CONVERT(nvarchar(11), H.holiDate,106),' ','-') [holidate] ,H.holidaydesc,H.EID,L.locID,L.locationName From MMM_MST_HOLIDAYS as H inner join MMM_MST_LOCATION as L on H.locID=L.locID  where H.EID='" & Session("EID").ToString() & "'", con)
        Dim ds As New DataSet()
        oda.Fill(ds, "data")
        gvData.DataSource = ds.Tables("data")
        gvData.DataBind()
        ds.Dispose()
        oda.Dispose()
        con.Close()
    End Sub
    
    Public Sub Add(ByVal sender As Object, ByVal e As System.EventArgs)
        txtHoliDesc.Text = ""
        txtHoliName.Text = ""
        txtHoliDate.Text = ""
        
        btnActUserSave.Text = "Save"
        Me.updatePanelEdit.Update()
        Me.btnEdit_ModalPopupExtender.Show()
    End Sub

    Protected Sub EditHit(ByVal sender As Object, ByVal e As System.EventArgs)
        Dim btnDetails As ImageButton = TryCast(sender, ImageButton)
        Dim row As GridViewRow = DirectCast(btnDetails.NamingContainer, GridViewRow)
        Dim pid As Integer = Convert.ToString(Me.gvData.DataKeys(row.RowIndex).Value)

        ' No Value in Session just fill the Edit Form and Show two button
        btnActUserSave.Text = "Update"
        'two methods.. either show data from Grid or Show data from Database.
        ViewState("pid") = pid
        txtHoliName.Text = row.Cells(1).Text
        txtHoliDesc.Text = row.Cells(4).Text
        txtHoliDate.Text = row.Cells(3).Text

        ddlHoliLocation.SelectedIndex = ddlHoliLocation.Items.IndexOf(ddlHoliLocation.Items.FindByText(row.Cells(2).Text))

        Me.updatePanelEdit.Update()
        Me.btnEdit_ModalPopupExtender.Show()
    End Sub

    Protected Sub DeleteRecord(ByVal sender As Object, ByVal e As System.EventArgs)

        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim con As SqlConnection = New SqlConnection(conStr)
        Dim oda As SqlDataAdapter = New SqlDataAdapter("uspDeleteUserInHolidays", con)
        oda.SelectCommand.CommandType = CommandType.StoredProcedure
        oda.SelectCommand.Parameters.AddWithValue("tid", Val(ViewState("Did").ToString))
        If con.State <> ConnectionState.Open Then
            con.Open()
        End If
        oda.SelectCommand.ExecuteNonQuery()
        Dim ds As New DataSet()

        oda.SelectCommand.CommandText = "select H.HoliID,H.locID,H.holidayname,H.holiDate,H.holidaydesc,H.EID,L.locID,L.locationName From MMM_MST_HOLIDAYS as H inner join MMM_MST_LOCATION as L on H.locID=L.locID  where H.EID='" & Session("EID").ToString() & "'"
        oda.SelectCommand.CommandType = CommandType.Text
        oda.Fill(ds, "data")
        gvData.DataSource = ds.Tables("data")
        gvData.DataBind()
        con.Close()
        oda.Dispose()
        con.Dispose()
        lblRecord.Text = "Holiday has been deleted successfully"

        Me.updatePanelEdit.Update()
        Me.btnDelete_ModalPopupExtender.Hide()

    End Sub

    Protected Sub DeleteHit(ByVal sender As Object, ByVal e As System.EventArgs)
        Dim btnDetails As ImageButton = TryCast(sender, ImageButton)
        Dim row As GridViewRow = DirectCast(btnDetails.NamingContainer, GridViewRow)
        Dim pid As Integer = Convert.ToString(Me.gvData.DataKeys(row.RowIndex).Value)
        ViewState("Did") = pid
        lblMsgDelete.Text = "Are you Sure Want to delete this Record? " & row.Cells(1).Text
        Me.updatePanelDelete.Update()
        Me.btnDelete_ModalPopupExtender.Show()
    End Sub

    Protected Sub btnActUserSave_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnActUserSave.Click
        If btnActUserSave.Text = "Save" Then
            Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
            Dim con As SqlConnection = New SqlConnection(conStr)
            Dim oda As SqlDataAdapter = New SqlDataAdapter("uspInsertHolidays", con)
            oda.SelectCommand.CommandType = CommandType.StoredProcedure
            oda.SelectCommand.Parameters.AddWithValue("EID", Session("EID").ToString())
            oda.SelectCommand.Parameters.AddWithValue("HoliName", txtHoliName.Text)
            oda.SelectCommand.Parameters.AddWithValue("locID", ddlHoliLocation.SelectedItem.Value())
            oda.SelectCommand.Parameters.AddWithValue("HoliDate", txtHoliDate.Text)
            oda.SelectCommand.Parameters.AddWithValue("HoliDiscription", txtHoliDesc.Text)

            If con.State <> ConnectionState.Open Then
                con.Open()
            End If
            oda.SelectCommand.ExecuteNonQuery()
            oda.SelectCommand.CommandText = "select H.HoliID,H.locID,H.holidayname,H.holiDate,H.HoliDayDesc,H.EID,L.locID,L.locationName From MMM_MST_HOLIDAYS as H inner join MMM_MST_LOCATION as L on H.locID=L.locID  where H.EID='" & Session("EID").ToString() & "'"
            oda.SelectCommand.CommandType = CommandType.Text
            Dim ds As New DataSet()
            oda.Fill(ds, "data")
            gvData.DataSource = ds.Tables("data")
            gvData.DataBind()
            lblRecord.Text = "Holiday has been created successfully"
            Me.updatePanelEdit.Update()
            Me.btnEdit_ModalPopupExtender.Hide()

            con.Close()
            oda.Dispose()

        ElseIf btnActUserSave.Text = "Update" Then
            Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
            Dim con As SqlConnection = New SqlConnection(conStr)
            Dim oda As SqlDataAdapter = New SqlDataAdapter("uspUpdateHoliday ", con)
            oda.SelectCommand.CommandType = CommandType.StoredProcedure
            oda.SelectCommand.Parameters.AddWithValue("EID", Session("EID").ToString())
            oda.SelectCommand.Parameters.AddWithValue("HoliName", txtHoliName.Text)
            oda.SelectCommand.Parameters.AddWithValue("Locid", ddlHoliLocation.SelectedItem.Value())
            oda.SelectCommand.Parameters.AddWithValue("HoliDate", txtHoliDate.Text)
            oda.SelectCommand.Parameters.AddWithValue("HoliDiscription", txtHoliDesc.Text)
            oda.SelectCommand.Parameters.AddWithValue("pid", ViewState("pid").ToString())
            If con.State <> ConnectionState.Open Then
                con.Open()
            End If
            oda.SelectCommand.ExecuteNonQuery()
            oda.SelectCommand.CommandText = "select H.HoliID,H.locID,H.holidayname,H.holiDate,H.holidaydesc,H.EID,L.locID,L.locationName From MMM_MST_HOLIDAYS as H inner join MMM_MST_LOCATION as L on H.locID=L.locID  where H.EID='" & Session("EID").ToString() & "'"
            oda.SelectCommand.CommandType = CommandType.Text
            Dim ds As New DataSet()
            oda.Fill(ds, "data")
            gvData.DataSource = ds.Tables("data")
            gvData.DataBind()
            lblRecord.Text = "Holiday has been updated successfully"
            Me.updatePanelEdit.Update()
            Me.btnEdit_ModalPopupExtender.Hide()
            con.Close()
            con.Dispose()
            oda.Dispose()
            txtHoliName.Text = ""
        End If
    End Sub
 
    Protected Sub Search(ByVal sender As Object, ByVal e As System.Web.UI.ImageClickEventArgs) Handles btnSearch.Click
        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim con As SqlConnection = New SqlConnection(conStr)
        Dim oda As SqlDataAdapter
        If ddlHolidayName.SelectedValue = "holidayname" Then
            oda = New SqlDataAdapter("select H.HoliID,H.locID,H.holidayname,H.holiDate,H.holidaydesc,H.EID,L.locID,L.locationName From MMM_MST_HOLIDAYS as H inner join MMM_MST_LOCATION as L on H.locID=L.locID  where H.holidayname like '" & txtValue.Text & "%' and H.EID='" & Session("EID") & "' ", con)
        Else
            oda = New SqlDataAdapter("select H.HoliID,H.locID,H.holidayname,H.holiDate,H.holidaydesc,H.EID,L.locID,L.locationName From MMM_MST_HOLIDAYS as H inner join MMM_MST_LOCATION as L on H.locID=L.locID  where L.Locationname like '" & txtValue.Text & "%' and H.EID='" & Session("EID") & "' ", con)
        End If

        Dim ds As New DataSet()
        oda.Fill(ds, "data")
        gvData.DataSource = ds.Tables("data")
        gvData.DataBind()
        ds.Dispose()
        oda.Dispose()
        con.Close()
    End Sub

End Class
