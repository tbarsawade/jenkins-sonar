Imports System.Data
Imports System.Data.SqlClient
Imports System.Web.UI.DataVisualization.Charting.SeriesChartType
Imports System.Web.UI.DataVisualization.Charting
Imports System.IO
Imports System.Web.UI.Adapters.ControlAdapter
Imports iTextSharp.text
Imports System.Drawing
Imports System.Net.Mail
Partial Class ReportScheduler
    Inherits System.Web.UI.Page
    Dim stradd As String = ""
    Dim actualval As String = ""
    Dim strOr As String = ""
    Private FPath As String = System.Web.HttpContext.Current.Server.MapPath("~/export/")

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
    Protected Sub Page_Load(sender As Object, e As System.EventArgs) Handles Me.Load
        If Not IsPostBack Then
            BinddataGrid()
            GeofenceDoctype()
            gvpending.Columns(7).Visible = False
            'gvpending.Columns(11).Visible = False
            'gvpending.Columns(12).Visible = False
            'lblDocmType.Visible = False
            'lblMailField.Visible = False
            'ddlDoctype.Visible = False
            'ddlMailField.Visible = False
            'ddlGeoDocType.Visible = True
            'ddlIMEINo.Visible = True
            'ddlMobileFields.Visible = True
            'ddlSmsField.Visible = True
            pnlsms.Visible = True
        End If

    End Sub
    Protected currentPageNumberp As Integer = 1
    Protected Sub gvPending_PageIndexChanging(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.GridViewPageEventArgs) Handles gvpending.PageIndexChanging
        Try
            gvpending.PageIndex = e.NewPageIndex
            currentPageNumberp = e.NewPageIndex + 1
            BinddataGrid()
            UpdatePanel1.Update()
        Catch ex As Exception
        End Try
    End Sub
    Public Sub BindDocTypeDdl()
        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim con As New SqlConnection(conStr)
        'fill Product  
        Dim da As New SqlDataAdapter("select distinct fd.documenttype[documenttype] from mmm_mst_fields fd inner join mmm_mst_forms f on f.formname=fd.documenttype  where fd.ismail=1 and formtype='master' and  fd.eid=" & Session("EID") & "  ", con)
        Dim ds As New DataTable()
        da.Fill(ds)
        If con.State <> ConnectionState.Open Then
            con.Open()
        End If
        'ddlDoctype.DataSource = ds
        'ddlDoctype.DataValueField = "documenttype"
        'ddlDoctype.DataTextField = "documenttype"
        'ddlDoctype.DataBind()
        btnSc_ModalPopupExtender.Show()
        con.Close()
        con.Dispose()

    End Sub
    Public Sub BinddataGrid()
        'fill Product  
        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim con As New SqlConnection(conStr)
        Dim da As New SqlDataAdapter("SELECT tid,reportname,reportsubject,reporttype,date,HH,MM,msgbody,ordering from MMM_MST_ReportScheduler WHERE EID = " & Session("EID").ToString() & "", con)
        da.SelectCommand.CommandType = CommandType.Text
        Dim ds As New DataSet()
        da.Fill(ds, "data")
        gvPending.DataSource = ds.Tables("data")
        gvPending.DataBind()
        da.Dispose()
        con.Close()
        con.Dispose()
        

    End Sub

    Public Sub BinddataSendToDDl()
        'Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        'Dim con As New SqlConnection(conStr)
        ''fill Product  
        'Dim da As New SqlDataAdapter("select distinct userrole from mmm_mst_user where eid=" & Session("EID") & "  ", con)
        'Dim ds As New DataTable()
        'da.Fill(ds)
        'If con.State <> ConnectionState.Open Then
        '    con.Open()
        'End If
        'ddlSendTo.DataSource = ds
        'ddlSendTo.DataValueField = "userrole"
        'ddlSendTo.DataTextField = "userrole"
        'ddlSendTo.DataBind()
        'ddlSendTo.Items.Add("OTHERS")


        'con.Close()
        'con.Dispose()
    End Sub

    Protected Sub btnReportSch_Click(sender As Object, e As System.Web.UI.ImageClickEventArgs) Handles btnReportSch.Click
        clearfields()
        'lblDocmType.Visible = False
        'lblMailField.Visible = False
        'ddlDoctype.Visible = False
        'ddlMailField.Visible = False
        ' txtToDate.Text = DateTime.Now.ToString("dd/MM/yy")
        'lblweek.Text = "Weekly(1-7),Monthly(1-31) OR @d"
        lblForm.Text = ""
        btnSave.Text = "Save"
        lblForm.Text = ""
        txtBody.Visible = True
        'lblFrdate.Text = "Date format should be (dd/mm/yy)"
        btnSc_ModalPopupExtender.Show()
        'BinddataSendToDDl()


    End Sub

    Protected Sub EditHit(ByVal sender As Object, ByVal e As System.EventArgs)
        txtBody.Enabled = False
        btnSave.Text = "Update"
        Dim btnDetails As ImageButton = TryCast(sender, ImageButton)
        Dim row As GridViewRow = DirectCast(btnDetails.NamingContainer, GridViewRow)
        Dim pid As Integer = Convert.ToString(Me.gvpending.DataKeys(row.RowIndex).Value)
        ViewState("pid") = pid
        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim con As New SqlConnection(conStr)
        Dim da As New SqlDataAdapter("", con)
        da.SelectCommand.CommandType = CommandType.Text
        da.SelectCommand.CommandText = "SELECT msgbody from MMM_MST_ReportScheduler WHERE tid = " & pid & ""
        Dim ds As New DataSet()
        da.Fill(ds, "msg")
        Dim msg As String = ds.Tables("msg").Rows(0).Item(0).ToString
        txtRptname.Text = Trim(row.Cells(1).Text)
        ' ddlRptname.SelectedIndex = ddlRptname.Items.IndexOf(ddlRptname.Items.FindByText(row.Cells(1).Text))
        txtSendSub.Text = Trim(row.Cells(2).Text)
        ddlStype.SelectedIndex = ddlStype.Items.IndexOf(ddlStype.Items.FindByText(row.Cells(3).Text))
        txtDay.Text = Trim(row.Cells(4).Text)
        ddlHour.SelectedIndex = ddlHour.Items.IndexOf(ddlHour.Items.FindByText(row.Cells(5).Text))
        txtminut.Text = Trim(row.Cells(6).Text)
        'txtFrDate.Text = row.Cells(8).Text
        'txtToDate.Text = row.Cells(9).Text
        txtOrder.Text = Trim(row.Cells(8).Text)
        'Dim str As String = Trim(row.Cells(7).Text.ToString())
        txtBody.Text = Trim(HttpUtility.HtmlDecode(msg))
        'txtBody.Text = Trim(row.Cells(7).Text)
        'If Val(row.Cells(11).Text) = 1 Then
        '    ChkPrevMnth.Checked = True
        'Else
        '    ChkPrevMnth.Checked = False
        'End If
        'ddlSendtype.SelectedIndex = ddlSendtype.Items.IndexOf(ddlSendtype.Items.FindByText(row.Cells(12).Text))
        gvpending.Columns(7).Visible = False
        'gvpending.Columns(11).Visible = False
        'gvpending.Columns(12).Visible = False
        Me.btnSc_ModalPopupExtender.Show()
    End Sub

    Protected Sub ddlStype_SelectedIndexChanged(sender As Object, e As System.EventArgs) Handles ddlStype.SelectedIndexChanged

        If ddlStype.SelectedItem.Text = "Daily" Then
            txtDay.Visible = False
        Else
            txtDay.Visible = True
        End If

        If ddlStype.SelectedItem.Text = "Monthly" Then
            lblweek.Visible = True
            'lblweek.Text = "Enter Date(eg. 1 to 30)"
        Else
            lblweek.Visible = False
        End If

        If ddlStype.SelectedItem.Text = "Weekly" Then
            lblweek.Visible = True
            'lblweek.Text = "Enter 1 to 7(eg. 1=Monday)"
        Else
            lblweek.Visible = False
        End If


    End Sub



    Protected Sub btnSave_Click(sender As Object, e As System.EventArgs) Handles btnSave.Click
        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim con As New SqlConnection(conStr)
        Dim oda As SqlDataAdapter = New SqlDataAdapter("", con)
        Dim dt As New DataTable
        lblForm.Text = ""
        If btnSave.Text = "Save" Then
            
            Dim cmd As New SqlCommand("UspInsertRptSchDetail", con)
            cmd.CommandType = CommandType.StoredProcedure
            cmd.Parameters.Clear()
            cmd.Parameters.AddWithValue("eid", Session("EID").ToString())
            cmd.Parameters.AddWithValue("RptName", txtRptname.Text)
            cmd.Parameters.AddWithValue("sendsubject", txtSendSub.Text)
            cmd.Parameters.AddWithValue("Reporttype", ddlStype.SelectedItem.Text)
            ' cmd.Parameters.AddWithValue("sendto ", ddlSendTo.SelectedValue.ToString())
            cmd.Parameters.AddWithValue("date", txtDay.Text)
            cmd.Parameters.AddWithValue("HH", ddlHour.SelectedItem.Text)
            cmd.Parameters.AddWithValue("MM", txtminut.Text)

            If ddlAlertType.SelectedItem.Text.ToString.ToUpper = "GPS SMS ALERT" Then
                cmd.Parameters.AddWithValue("AlertType", ddlAlertType.SelectedValue.ToString)
                cmd.Parameters.AddWithValue("GeoDocType", ddlGeoDocType.SelectedValue.ToString)
                cmd.Parameters.AddWithValue("MobileFields", ddlMobileFields.SelectedValue.ToString)
                cmd.Parameters.AddWithValue("SmsField", ddlSmsField.SelectedValue.ToString)
                cmd.Parameters.AddWithValue("IMEINo", ddlIMEINo.SelectedValue.ToString)
            Else
                cmd.Parameters.AddWithValue("AlertType", "")
                cmd.Parameters.AddWithValue("GeoDocType", "")
                cmd.Parameters.AddWithValue("MobileFields", "")
                cmd.Parameters.AddWithValue("SmsField", "")
                cmd.Parameters.AddWithValue("IMEINo", "")
            End If

           

            'If ChkPrevMnth.Checked = True Then
            '    cmd.Parameters.AddWithValue("fromdate", prevfrom)
            '    cmd.Parameters.AddWithValue("todate", prevto)
            'Else
            '    cmd.Parameters.AddWithValue("fromdate", txtFrDate.Text)
            '    cmd.Parameters.AddWithValue("todate", txtToDate.Text)
            'End If

            'If ddlSendTo.SelectedItem.Text.ToUpper = "OTHERS" Then
            '    cmd.Parameters.AddWithValue("maildoc", ddlDoctype.SelectedItem.Text)
            '    cmd.Parameters.AddWithValue("mailfield", ddlMailField.SelectedValue.ToString())
            'Else
            '    cmd.Parameters.AddWithValue("maildoc", "")
            '    cmd.Parameters.AddWithValue("mailfield", "")
            'End If

            'cmd.Parameters.AddWithValue("reportqry", reportquery)
            ' cmd.Parameters.AddWithValue("chkprev", chkPrev)
            'cmd.Parameters.AddWithValue("sendtype", ddlSendtype.SelectedItem.Text.ToString())
            cmd.Parameters.AddWithValue("msgbody", txtBody.Text)
            cmd.Parameters.AddWithValue("order", txtOrder.Text)

            If con.State <> ConnectionState.Open Then
                con.Open()
            End If
            cmd.ExecuteNonQuery()
            lblForm.Text = "Scheduler Saved Successfully"
            lblweek.Text = ""
            'End If

            oda.Dispose()
            clearfields()
            btnSc_ModalPopupExtender.Hide()
        ElseIf btnSave.Text = "Update" Then
            Dim cmd As New SqlCommand("UspUpdateRptSchDetail", con)
            cmd.CommandType = CommandType.StoredProcedure
            cmd.Parameters.Clear()
            cmd.Parameters.AddWithValue("eid", Session("EID").ToString())
            cmd.Parameters.AddWithValue("RptName", txtRptname.Text)
            cmd.Parameters.AddWithValue("sendsubject", txtSendSub.Text)
            cmd.Parameters.AddWithValue("Reporttype", ddlStype.SelectedItem.Text)
            ' cmd.Parameters.AddWithValue("sendto ", ddlSendTo.SelectedValue.ToString())
            cmd.Parameters.AddWithValue("date", txtDay.Text)
            cmd.Parameters.AddWithValue("HH", ddlHour.SelectedItem.Text)
            cmd.Parameters.AddWithValue("MM", txtminut.Text)

            If ddlAlertType.SelectedItem.Text.ToString.ToUpper = "GPS SMS ALERT" Then
                cmd.Parameters.AddWithValue("AlertType", ddlAlertType.SelectedValue.ToString)
                cmd.Parameters.AddWithValue("GeoDocType", ddlGeoDocType.SelectedValue.ToString)
                cmd.Parameters.AddWithValue("MobileFields", ddlMobileFields.SelectedValue.ToString)
                cmd.Parameters.AddWithValue("SmsField", ddlSmsField.SelectedValue.ToString)
                cmd.Parameters.AddWithValue("IMEINo", ddlIMEINo.SelectedValue.ToString)
            Else
                cmd.Parameters.AddWithValue("AlertType", "")
                cmd.Parameters.AddWithValue("GeoDocType", "")
                cmd.Parameters.AddWithValue("MobileFields", "")
                cmd.Parameters.AddWithValue("SmsField", "")
                cmd.Parameters.AddWithValue("IMEINo", "")
            End If
            'If ChkPrevMnth.Checked = True Then
            '    cmd.Parameters.AddWithValue("fromdate", prevfrom)
            '    cmd.Parameters.AddWithValue("todate", prevto)
            'Else
            '    cmd.Parameters.AddWithValue("fromdate", txtFrDate.Text)
            '    cmd.Parameters.AddWithValue("todate", txtToDate.Text)
            'End If
            'cmd.Parameters.AddWithValue("reportqry", reportquery)
            'cmd.Parameters.AddWithValue("chkprev", chkPrev)
            'cmd.Parameters.AddWithValue("sendtype", ddlSendtype.SelectedItem.Text.ToString())
            'If ddlSendTo.SelectedItem.Text.ToUpper = "OTHERS" Then
            '    cmd.Parameters.AddWithValue("maildoc", ddlDoctype.SelectedItem.Text)
            '    cmd.Parameters.AddWithValue("mailfield", ddlMailField.SelectedValue.ToString())
            'Else
            '    cmd.Parameters.AddWithValue("maildoc", "")
            '    cmd.Parameters.AddWithValue("mailfield", "")
            'End If
            'cmd.Parameters.AddWithValue("msgbody", txtBody.Text)
            cmd.Parameters.AddWithValue("order", txtOrder.Text)
            cmd.Parameters.AddWithValue("pid", ViewState("pid"))
            If con.State <> ConnectionState.Open Then
                con.Open()
            End If
            cmd.ExecuteNonQuery()

            lblForm.Text = "Scheduler Updated Successfully"
            lblweek.Text = ""
            clearfields()
        End If

        BinddataGrid()
        'SCHEDULER()
    End Sub

    Public Function ChangeFormat(ByVal dtm As DateTime, ByVal format As String) As String
        Return dtm.ToString(format)
    End Function
    Public Sub GeofenceDoctype()
        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim con As SqlConnection = New SqlConnection(conStr)
        Dim oda As SqlDataAdapter = New SqlDataAdapter("", con)
        Try
            oda.SelectCommand.CommandType = CommandType.Text
            oda.SelectCommand.CommandText = "select formname from MMM_MST_Forms where formtype='MASTER' and eid=" & Session("EID") & " and isactive=1"
            Dim ds As New DataTable()
            oda.Fill(ds)
            ddlGeoDocType.DataSource = ds
            ddlGeoDocType.DataValueField = "formname"
            ddlGeoDocType.DataTextField = "formname"
            ddlGeoDocType.DataBind()
        Catch ex As Exception

        Finally
            oda.Dispose()
            con.Close()
            con.Dispose()
        End Try
    End Sub

    Protected Sub DeleteHit(ByVal sender As Object, ByVal e As System.EventArgs)
        Dim btnDetails As ImageButton = TryCast(sender, ImageButton)
        Dim row As GridViewRow = DirectCast(btnDetails.NamingContainer, GridViewRow)
        Dim pid As Integer = Convert.ToString(Me.gvpending.DataKeys(row.RowIndex).Value)
        ViewState("Did") = pid
        lblMsgDelete.Text = "Are you Sure Want to delete this Record? " & row.Cells(1).Text
        Me.updatePanelDelete.Update()
        Me.btnDelete_ModalPopupExtender.Show()
    End Sub

    Protected Sub DeleteRecord(ByVal sender As Object, ByVal e As System.EventArgs)
        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim con As SqlConnection = New SqlConnection(conStr)
        Dim oda As SqlDataAdapter = New SqlDataAdapter("uspDeleteReportScheduler", con)
        oda.SelectCommand.CommandType = CommandType.StoredProcedure
        oda.SelectCommand.Parameters.AddWithValue("tid", Val(ViewState("Did").ToString))
        If con.State <> ConnectionState.Open Then
            con.Open()
        End If
        oda.SelectCommand.ExecuteNonQuery()
        Dim ds As New DataSet()
        lblForm.Text = "Rerport Scheduler has been deleted successfully"
        Me.btnDelete_ModalPopupExtender.Hide()
        BinddataGrid()
        con.Close()
    End Sub

    Public Sub clearfields()
        txtDay.Text = ""
        txtRptname.Text = ""
        txtminut.Text = ""
        ' txtMobile.Text = ""
        txtOrder.Text = "0"
        'txtFrDate.Text = ""
        'txtToDate.Text = ""
        txtSendSub.Text = ""
        'txtRptQry.Text = ""
        txtBody.Text = ""
        'ChkPrevMnth.Checked = False
        'ChkPrevMnth.Controls.Clear()
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
    
    
    
    Public Sub BinddataMail()
        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim con As New SqlConnection(conStr)
        'fill Product  
        Dim da As New SqlDataAdapter("select displayname,fieldmapping from mmm_mst_fields where documenttype='vendor' and ismail=1 and eid=" & Session("EID") & "  ", con)
        Dim ds As New DataTable()
        da.Fill(ds)
        If con.State <> ConnectionState.Open Then
            con.Open()
        End If
     
        con.Close()
        con.Dispose()
    End Sub

    Protected Sub ddlGeoDocType_SelectedIndexChanged(sender As Object, e As System.EventArgs) Handles ddlGeoDocType.SelectedIndexChanged
        Me.btnSc_ModalPopupExtender.Show()
        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim con As New SqlConnection(conStr)
        'fill Product  
        Dim da As New SqlDataAdapter("select displayname,fieldmapping from mmm_mst_fields where documenttype='" & ddlGeoDocType.SelectedItem.Text & "' and isactive=1 and eid=" & Session("EID") & "  ", con)
        Dim ds As New DataTable()
        da.Fill(ds)
        If con.State <> ConnectionState.Open Then
            con.Open()
        End If
        ddlMobileFields.DataSource = ds
        ddlMobileFields.DataValueField = "fieldmapping"
        ddlMobileFields.DataTextField = "displayname"
        ddlMobileFields.DataBind()

        ddlSmsField.DataSource = ds
        ddlSmsField.DataValueField = "fieldmapping"
        ddlSmsField.DataTextField = "displayname"
        ddlSmsField.DataBind()

        ddlIMEINo.DataSource = ds
        ddlIMEINo.DataValueField = "fieldmapping"
        ddlIMEINo.DataTextField = "displayname"
        ddlIMEINo.DataBind()
        da.Dispose()
        con.Close()
        con.Dispose()
    End Sub

    Protected Sub ddlAlertType_SelectedIndexChanged(sender As Object, e As System.EventArgs) Handles ddlAlertType.SelectedIndexChanged

        Me.btnSc_ModalPopupExtender.Show()

        If ddlAlertType.SelectedItem.Text.ToString.ToUpper <> "GPS SMS ALERT" Then
            pnlsms.Visible = False
            'ddlGeoDocType.Visible = False
            'ddlIMEINo.Visible = False
            'ddlMobileFields.Visible = False
            'ddlSmsField.Visible = False
        Else
            pnlsms.Visible = True
            'ddlGeoDocType.Visible = True
            'ddlIMEINo.Visible = True
            'ddlMobileFields.Visible = True
            'ddlSmsField.Visible = True
        End If
    End Sub
End Class
