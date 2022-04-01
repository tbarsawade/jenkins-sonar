Imports System.Net
Imports System.Net.Mail
Imports System.Threading
Imports System.IO
Imports System.Data
Imports System.Data.SqlClient
Imports Microsoft.VisualBasic
Imports System
Imports System.Collections.Generic
Imports System.Text
Imports System.Text.RegularExpressions
Imports System.Web.Hosting
Imports System.Web.UI
Imports System.Web.UI.WebControls
Imports System.Xml
Imports System.Diagnostics
Imports Microsoft.Office.Interop.Excel
Imports Microsoft.Office.Interop

Partial Class ReportScheduleSetting
    Inherits System.Web.UI.Page
    Protected Sub Add(ByVal sender As Object, ByVal e As System.EventArgs)
        lblMsgEdit.Text = ""
        btnActEdit.Text = "Save"
        clearcontrol()
        updatePanelEdit.Update()
        Me.btnEdit_ModalPopupExtender.Show()
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
    Protected Sub btnActEdit_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnActEdit.Click
        'Dim PID As Integer = 0
        'Dim TYPE As Integer = 0
        'Dim COUNT As Integer = 0
        'Dim qry As String = ""
        'Dim CC As String = ""
        ' '' check template Name 

        If ddlreportname.SelectedItem.Text.Contains("Select") = True Then
            lblMsgEdit.Text = "Please Select Report Name"
            Exit Sub
        End If
        If ddlreporttype.SelectedItem.Text.Contains("Select") = True Then
            lblMsgEdit.Text = "Please Select Report Type"
            Exit Sub
        End If
        If ddlsendtype.SelectedItem.Text.Contains("Select") = True Then
            lblMsgEdit.Text = "Please Select Mail Type"
            Exit Sub
        End If

        If txtSubject.Text = "" Then
            lblMsgEdit.Text = "Please Enter Mail Subject"
            txtSubject.Focus()
            Exit Sub
        End If

        If txtHH.Text = "" Then
            lblMsgEdit.Text = "Please Enter Hour"
            txtHH.Focus()
            Exit Sub
        End If
        If Val(txtHH.Text) > 23 Then
            lblMsgEdit.Text = "Please Enter Valid Hour"
            txtHH.Focus()
            Exit Sub
        End If
        If txtMM.Text = "" Then
            lblMsgEdit.Text = "Please Enter Minutes"
            txtMM.Focus()
            Exit Sub
        End If
        If Val(txtMM.Text) > 60 Then
            lblMsgEdit.Text = "Please Enter Valid Minutes"
            txtMM.Focus()
            Exit Sub
        End If

        If Val(txtdate.Text) > 31 Then
            lblMsgEdit.Text = "Please Enter Valid Date"
            txtdate.Focus()
            Exit Sub
        End If

        Dim selectedValues As String = String.Empty
        For Each li As ListItem In lstdept.Items
            If li.Selected = True Then
                selectedValues = selectedValues & li.Value & ","
            End If
        Next
        If selectedValues.ToString <> "" Then
            selectedValues = selectedValues.ToString.Remove(selectedValues.ToString.LastIndexOf(","), 1)
        End If
        Session("Role") = selectedValues

        'If txtName.Text = "" Then
        '    lblMsgEdit.Text = "Please Enter Template Name"
        '    txtName.Focus()
        '    Exit Sub
        'End If
        ' ''Check for Subject Line
        'If txtSubject.Text = "" Then
        '    lblMsgEdit.Text = "Please Enter a Valid Subject Line"
        '    txtSubject.Focus()
        '    Exit Sub
        'End If

        ' ''Check for Msg Body
        'If txtBody.Text = "" Then
        '    lblMsgEdit.Text = "Message Body can't be blank."
        '    txtBody.Focus()
        '    Exit Sub
        'End If

        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim con As New SqlConnection(conStr)
        Dim oda As SqlDataAdapter = New SqlDataAdapter("", con)
        'Dim dt As New DataTable

        If btnActEdit.Text = "Save" Then
            Dim cmd As New SqlCommand("UspInsertReportScheduler", con)
            cmd.CommandType = CommandType.StoredProcedure
            cmd.Parameters.Clear()
            cmd.Parameters.AddWithValue("eid", Session("EID").ToString())
            cmd.Parameters.AddWithValue("RptName", ddlreportname.SelectedItem.Text)
            cmd.Parameters.AddWithValue("sendsubject", txtSubject.Text)
            cmd.Parameters.AddWithValue("Reporttype", ddlreporttype.SelectedItem.Text)
            cmd.Parameters.AddWithValue("date", txtdate.Text)
            cmd.Parameters.AddWithValue("HH", txtHH.Text)
            cmd.Parameters.AddWithValue("MM", txtMM.Text)
            cmd.Parameters.AddWithValue("msgbody", txtbody.Text)
            cmd.Parameters.AddWithValue("mailto", txtmailto.Text)
            cmd.Parameters.AddWithValue("cc", txtcc.Text)
            cmd.Parameters.AddWithValue("bcc", txtbcc.Text)
            'cmd.Parameters.AddWithValue("sendtype", ddlsendtype.SelectedItem.Text.ToString())
            cmd.Parameters.AddWithValue("role ", Session("Role"))
            cmd.Parameters.AddWithValue("Query", Session("QRY"))
            cmd.Parameters.AddWithValue("sendtype", ddlsendtype.SelectedItem.Text.ToString())

            If con.State <> ConnectionState.Open Then
                con.Open()
            End If
            cmd.ExecuteNonQuery()
            lblMsgEdit.Text = "Scheduler Saved Successfully"
            lblMsgEdit.Text = ""
            'End If

            oda.Dispose()
        ElseIf btnActEdit.Text = "Update" Then
            ddlreportname.Enabled = False
            Dim cmd As New SqlCommand("UspUpdateReportScheduler", con)
            cmd.CommandType = CommandType.StoredProcedure
            cmd.Parameters.Clear()
            cmd.Parameters.AddWithValue("eid", Session("EID").ToString())
            'cmd.Parameters.AddWithValue("RptName", ddlreportname.SelectedItem.Text)
            cmd.Parameters.AddWithValue("sendsubject", txtSubject.Text)
            cmd.Parameters.AddWithValue("Reporttype", ddlreporttype.SelectedItem.Text)
            cmd.Parameters.AddWithValue("date", txtdate.Text)
            cmd.Parameters.AddWithValue("HH", txtHH.Text)
            cmd.Parameters.AddWithValue("MM", txtMM.Text)
            cmd.Parameters.AddWithValue("msgbody", txtbody.Text)
            cmd.Parameters.AddWithValue("mailto", txtmailto.Text)
            cmd.Parameters.AddWithValue("cc", txtcc.Text)
            cmd.Parameters.AddWithValue("bcc", txtbcc.Text)
            'cmd.Parameters.AddWithValue("sendtype", ddlsendtype.SelectedItem.Text.ToString())
            cmd.Parameters.AddWithValue("role ", Session("Role"))
            cmd.Parameters.AddWithValue("PID", ViewState("pid"))
            cmd.Parameters.AddWithValue("sendtype", ddlsendtype.SelectedItem.Text.ToString())

            If con.State <> ConnectionState.Open Then
                con.Open()
            End If
            cmd.ExecuteNonQuery()

            lblMsgEdit.Text = "Scheduler Updated Successfully"
            lblMsgEdit.Text = ""
        End If

        'Dim INS As String = da.SelectCommand.ExecuteScalar()
        'lblMsg1.Text = INS
        Me.btnEdit_ModalPopupExtender.Hide()
        getsearchresult()
        updPnlGrid.Update()
        'da.Dispose()
        con.Close()
    End Sub
    Protected Sub RefreshPanel(ByVal sender As Object, ByVal e As EventArgs)

    End Sub
    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Not IsPostBack Then
            ' session("eid") = "0"

            Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
            Dim con As New SqlConnection(conStr)
            'fill Product  
            Dim da As New SqlDataAdapter("select Reportid,ReportName FROM MMM_MST_report where eid=" & Session("EID") & "", con)
            Dim ds As New DataSet
            da.Fill(ds, "data")
            ddlreportname.DataSource = ds
            ddlreportname.DataTextField = "ReportName"
            ddlreportname.DataValueField = "Reportid"
            ddlreportname.DataBind()
            ddlreportname.Items.Insert(0, "--Please Select--")

            da.SelectCommand.CommandText = "select distinct Rolename from mmm_ref_role_user where eid=" & Session("EID") & ""
            da.Fill(ds, "data1")
            lstdept.DataSource = ds.Tables("data1")
            lstdept.DataTextField = "Rolename"
            lstdept.DataValueField = "Rolename"
            lstdept.DataBind()

            da.SelectCommand.CommandText = "SELECT MSGBODY FROM MMM_MST_TEMPLATE where eid=" & Session("EID")
            da.SelectCommand.CommandTimeout = 5000
            da.Fill(ds, "MSG")
            If ds.Tables("MSG").Rows.Count > 0 Then
                lblMsgEdit.Text = ds.Tables("MSG").Rows(0).Item(0).ToString()
            Else
                lblMsgEdit.Text = ""
            End If
            lblMsg1.Text = ""
            da.Dispose()
            con.Dispose()
            ds.Dispose()
            getsearchresult()
        End If
    End Sub

    Protected Sub EditHit(ByVal sender As Object, ByVal e As System.EventArgs)
        Try
            lblMsg1.Text = ""
            lblMsgEdit.Text = ""
            Dim EN As String = ""

            For Each li As ListItem In lstdept.Items
                li.Selected = False
            Next
            'clearcontrol()
            Dim strfield As String = ""
            Dim btnDetails As ImageButton = TryCast(sender, ImageButton)
            Dim row As GridViewRow = DirectCast(btnDetails.NamingContainer, GridViewRow)
            Dim pid As Integer = Convert.ToString(Me.gvData.DataKeys(row.RowIndex).Value)
            Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
            Dim con As New SqlConnection(conStr)
            'fill Product  
            Dim da As New SqlDataAdapter("select * FROM MMM_MST_reportscheduler WHERE TID=" & pid & "", con)
            Dim ds As New DataSet
            da.Fill(ds, "data")

            txtmailto.Text = ds.Tables("data").Rows(0).Item("EmailTo").ToString()
            txtcc.Text = ds.Tables("data").Rows(0).Item("cc").ToString()
            txtbcc.Text = ds.Tables("data").Rows(0).Item("bcc").ToString()
            ddlreportname.SelectedItem.Text = ds.Tables("data").Rows(0).Item("reportname")
            ddlreporttype.SelectedItem.Text = ds.Tables("data").Rows(0).Item("reporttype")
            ddlsendtype.SelectedItem.Text = ds.Tables("data").Rows(0).Item("sendtype")
            txtbody.Text = HttpUtility.HtmlDecode(ds.Tables("data").Rows(0).Item("MSGBODY").ToString())
            txtdate.Text = ds.Tables("data").Rows(0).Item("date").ToString()
            txtSubject.Text = ds.Tables("data").Rows(0).Item("reportsubject").ToString()
            txtHH.Text = ds.Tables("data").Rows(0).Item("HH").ToString()
            txtMM.Text = ds.Tables("data").Rows(0).Item("MM").ToString()
            Dim role As String() = Split(ds.Tables("data").Rows(0).Item("sendto").ToString(), ",")

            For Each li As ListItem In lstdept.Items
                For i As Integer = 0 To role.Length - 1
                    If li.Value.ToString = role(i).ToString Then
                        li.Selected = True
                    End If
                Next
            Next

            'lstdept.SelectedItem.Selected = ds.Tables("data").Rows(0).Item("sendto").ToString()
            ' No Value in Session just fill the Edit Form and Show two button
            btnActEdit.Text = "Update"

            'two methods.. either show data from Grid or Show data from Database.
            ViewState("pid") = pid
            Me.updatePanelEdit.Update()
            Me.btnEdit_ModalPopupExtender.Show()
            con.Dispose()
            da.Dispose()
            ds.Dispose()

        Catch ex As Exception
            Throw
        End Try
    End Sub
    Protected Sub btnSearch_Click(ByVal sender As Object, ByVal e As System.Web.UI.ImageClickEventArgs) Handles btnSearch.Click
        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim con As SqlConnection = New SqlConnection(conStr)
        Dim oda As SqlDataAdapter

        If ddlField.SelectedItem.Text = "ReportName" Then
            oda = New SqlDataAdapter("select tid,ReportName,ReportSubject,ReportType [ReportType],Sendtype [SendType]  from mmm_mst_reportscheduler where reportname like '" & txtValue.Text & "%' and EID='" & Session("eid") & "'", con)
        Else : ddlField.SelectedItem.Text = "ReportSubject"
            oda = New SqlDataAdapter("select tid,ReportName,ReportSubject,ReportType [ReportType],Sendtype [SendType]  from mmm_mst_reportscheduler where reportsubject like '" & txtValue.Text & "%' and EID='" & Session("eid") & "'", con)
        End If

        Dim ds As New DataSet()
        oda.Fill(ds, "data")

        gvData.DataSource = ds.Tables("data")
        gvData.DataBind()
        If gvData.Rows.Count = 0 Then
            lblMsg1.Visible = True
            lblMsg1.Text = "No Record Found"
        Else
            lblMsg1.Visible = False
        End If
        ds.Dispose()
        oda.Dispose()
        con.Close()
    End Sub
    Protected Sub deleteHit(ByVal sender As Object, ByVal e As System.EventArgs)
        Dim btnDetails As ImageButton = TryCast(sender, ImageButton)
        Dim row As GridViewRow = DirectCast(btnDetails.NamingContainer, GridViewRow)
        Dim pid As Integer = Convert.ToString(Me.gvData.DataKeys(row.RowIndex).Value)
        ViewState("PID") = pid

        lblMsgDelete.Text = "Do you want to delete Report Scheduler : '" & row.Cells(1).Text & "'"

        updatePanelDelete.Update()
        Me.btnDelete_ModalPopupExtender.Show()
    End Sub
    Protected Sub DeleteRecord(ByVal sender As Object, ByVal e As System.EventArgs)
        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim con As SqlConnection = New SqlConnection(conStr)
        Dim oda As SqlDataAdapter = New SqlDataAdapter("uspDeleteRptScheduler ", con)
        oda.SelectCommand.CommandType = CommandType.StoredProcedure
        oda.SelectCommand.Parameters.AddWithValue("pid", ViewState("PID").ToString)

        If con.State <> ConnectionState.Open Then
            con.Open()
        End If

        Dim tempid As Integer = oda.SelectCommand.ExecuteScalar()

        If tempid = 1 Then
            getsearchresult()
            lblMsg1.Text = "Report Scheduler deleted "
            Me.updPnlGrid.Update()
            Me.btnDelete_ModalPopupExtender.Hide()
        Else
            lblMsg1.Text = "Report Scheduler not deleted "
            Me.btnDelete_ModalPopupExtender.Hide()
        End If

        con.Close()
        oda.Dispose()
    End Sub
    Public Sub clearcontrol()
        Try
            txtbcc.Text = ""
            txtbody.Text = ""
            txtcc.Text = ""
            txtHH.Text = ""
            txtMM.Text = ""
            ddlreportname.SelectedIndex = -1
            ddlreporttype.SelectedIndex = -1
            ddlsendtype.SelectedIndex = -1
            txtSubject.Text = ""
            'DDLCC.SelectedIndex = 0
            txtbody.Text = ""
            txtmailto.Text = ""
            txtdate.Text = ""
            lblMsg1.Text = ""
            For Each li As ListItem In lstdept.Items
                li.Selected = False
            Next
            ddlreportname.Enabled = True
        Catch ex As Exception
            Throw
        End Try
    End Sub
    Public Sub getsearchresult()
        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim con As New SqlConnection(conStr)
        'fill Product  
        Try
            Dim da As SqlDataAdapter = New SqlDataAdapter("", con)
            da.SelectCommand.CommandType = CommandType.StoredProcedure
            da.SelectCommand.CommandText = "usp_GetReportScheduler"
            da.SelectCommand.Parameters.Clear()
            da.SelectCommand.Parameters.AddWithValue("@eid", Session("eid"))
            Dim ds As New DataSet
            da.Fill(ds, "data")
            gvData.DataSource = ds.Tables("data")
            gvData.DataBind()
        Catch ex As Exception
            Throw
        End Try
    End Sub
    Protected Sub gvData_RowDataBound(sender As Object, e As GridViewRowEventArgs) Handles gvData.RowDataBound
        For i As Integer = 0 To e.Row.Cells.Count - 1
            ViewState("OrigData") = e.Row.Cells(i).Text
            If e.Row.Cells(i).Text.Length >= 31 Then
                e.Row.Cells(i).Text = e.Row.Cells(i).Text.Substring(0, 31) + "..."
                e.Row.Cells(i).ToolTip = ViewState("OrigData").ToString()
                e.Row.Cells(i).Wrap = True
            End If
        Next
    End Sub

    Protected Sub ddlreportname_SelectedIndexChanged(sender As Object, e As EventArgs) Handles ddlreportname.SelectedIndexChanged
        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim con As New SqlConnection(conStr)
        'fill Product  
        Dim da As New SqlDataAdapter("", con)
        Dim str As String = ""
        If con.State <> ConnectionState.Open Then
            con.Open()
        End If
        Dim qry() As String
        da.SelectCommand.CommandText = "select qryfield FROM MMM_MST_report where eid=" & Session("EID") & " and reportname='" & ddlreportname.SelectedItem.Text & "'"
        str = da.SelectCommand.ExecuteScalar()
        str = Replace(str, "and @", "---")
        str = Replace(str, "@adate", "Convert(date,adate)")
        str = Replace(str, "$adate", "Convert(date,adate)")
        str = Replace(str, "@tid", "TID")
        Session("QRY") = str
        ' qry = Regex.Split(str, "documenttype=")

    End Sub

    Protected Sub lstdept_SelectedIndexChanged(sender As Object, e As EventArgs) Handles lstdept.SelectedIndexChanged
        Dim selectedValues As String = String.Empty
        For Each li As ListItem In lstdept.Items
            If li.Selected = True Then
                selectedValues = selectedValues & li.Value & ","
            End If
        Next
        If selectedValues.ToString <> "" Then
            selectedValues = selectedValues.ToString.Remove(selectedValues.ToString.LastIndexOf(","), 1)
        End If
        Session("Role") = selectedValues
    End Sub

    Sub ReportSchedulerMail()
        Dim FPath As String = System.Web.Hosting.HostingEnvironment.MapPath("~/import/")
        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim con As New SqlConnection(conStr)
        Dim oda As SqlDataAdapter = New SqlDataAdapter("", con)
        Dim ds As New DataSet()
        Dim eid As String = ""
        Dim alertname As String = ""
        Try
            oda.SelectCommand.CommandText = "select * from  MMM_MST_ReportScheduler with(nolock) where isActive='1' and tid >=499  order by hh,mm,ordering"
            oda.SelectCommand.CommandType = CommandType.Text
            oda.Fill(ds, "rpt")
            For d As Integer = 0 To ds.Tables("rpt").Rows.Count - 1
                Dim sendtype As String = ds.Tables("rpt").Rows(d).Item("sendtype").ToString()
                eid = ds.Tables("rpt").Rows(d).Item("eid").ToString()
                alertname = ds.Tables("rpt").Rows(d).Item("reportsubject").ToString()
                If ReportScheduler(ds.Tables("rpt").Rows(d).Item("tid")) = True Then
                    If sendtype.ToString.ToUpper = "EXCEL SHEET" Then
                        Dim MAILTO As String = ds.Tables("rpt").Rows(d).Item("emailto").ToString()
                        Dim CC As String = ds.Tables("rpt").Rows(d).Item("cc").ToString()
                        Dim Bcc As String = ds.Tables("rpt").Rows(d).Item("bcc").ToString()
                        Dim str As String = ds.Tables("rpt").Rows(d).Item("reportquery").ToString()
                        Dim role As String = ds.Tables("rpt").Rows(d).Item("sendto").ToString()
                        Dim reporttype As String = ds.Tables("rpt").Rows(d).Item("reporttype").ToString()
                        Dim dt As String = ds.Tables("rpt").Rows(d).Item("date").ToString()
                        eid = ds.Tables("rpt").Rows(d).Item("EID").ToString()

                        Dim qry As String
                        If role <> "" Then
                            role = Replace(role, ",", "','")
                            oda.SelectCommand.CommandText = " select * from mmm_mst_user where uid in (select distinct uid from mmm_ref_role_user where eid=" & eid & " and rolename in ('" & role & "') and documenttype like '%invoice%') "
                            oda.Fill(ds, "User")
                            'Dim lastschtime As String = Format(Convert.ToDateTime(ds.Tables("rpt").Rows(d).Item("lastscheduleddate").ToString), "yyyy-MM-dd")
                            'Dim curtime As String = Date.Now.ToString("yyyy-MM-dd")
                            'If lastschtime = curtime Then
                            'End If
                            oda.SelectCommand.CommandText = "select FormName,DocMapping from mmm_mst_forms where eid=" & eid & " and DocMapping is not null"
                            oda.Fill(ds, "DocMapping")
                            If ds.Tables("DocMapping").Rows.Count > 0 Then
                                oda.SelectCommand.CommandText = "select FieldMapping,documenttype from MMM_MST_FIELDS where eid=" & eid & " and dropdown like '%" & ds.Tables("DocMapping").Rows(0).Item("FormName") & "%' and documenttype in ('PES','Invoice PO','Invoice Non PO','Invoice on Hold')"
                                oda.Fill(ds, "Fieldmapping")
                            End If

                            For u As Integer = 0 To ds.Tables("User").Rows.Count - 1
                                If Len(MAILTO) < 3 Then
                                    MAILTO = ds.Tables("User").Rows(u).Item("EmailID").ToString
                                End If
                                qry = str.ToUpper
                                qry = qry.ToString.ToUpper
                                oda.SelectCommand.CommandText = "select " & ds.Tables("DocMapping").Rows(0).Item("DocMapping") & " from MMM_Ref_Role_User where eid=" & eid & " and uid=" & ds.Tables("User").Rows(u).Item("uid") & " and rolename in ('" & role & "') and documenttype like '%invoice%'"
                                Dim id As String = ""
                                If con.State <> ConnectionState.Open Then
                                    con.Open()
                                End If
                                oda.SelectCommand.CommandType = CommandType.Text
                                id = oda.SelectCommand.ExecuteScalar().ToString
                                For dm As Integer = 0 To ds.Tables("Fieldmapping").Rows.Count - 1
                                    If ds.Tables("Fieldmapping").Rows(dm).Item("documenttype").ToString.ToUpper = "INVOICE PO" Then
                                        qry = Replace(qry, "DOCUMENTTYPE='INVOICE PO'", "Documenttype='Invoice PO' and " & ds.Tables("Fieldmapping").Rows(dm).Item("FieldMapping").ToString & " in (" & id & ") ")
                                        qry = Replace(qry, "DOCUMENTTYPE IN ('INVOICE PO')", "Documenttype in ('Invoice PO') and " & ds.Tables("Fieldmapping").Rows(dm).Item("FieldMapping").ToString & " in (" & id & ") ")
                                    ElseIf ds.Tables("Fieldmapping").Rows(dm).Item("documenttype").ToString.ToUpper = "INVOICE NON PO" Then
                                        qry = Replace(qry, "DOCUMENTTYPE='INVOICE NON PO'", "Documenttype='Invoice non PO' and " & ds.Tables("Fieldmapping").Rows(dm).Item("FieldMapping").ToString & " in (" & id & ") ")
                                        qry = Replace(qry, "DOCUMENTTYPE IN ('INVOICE NON PO')", "Documenttype in ('Invoice non PO') and " & ds.Tables("Fieldmapping").Rows(dm).Item("FieldMapping").ToString & " in (" & id & ") ")
                                    ElseIf ds.Tables("Fieldmapping").Rows(dm).Item("documenttype").ToString.ToUpper = "INVOICE ON HOLD" Then
                                        qry = Replace(qry, "DOCUMENTTYPE='INVOICE ON HOLD'", "Documenttype='Invoice on Hold' and " & ds.Tables("Fieldmapping").Rows(dm).Item("FieldMapping").ToString & " in (" & id & ") ")
                                        qry = Replace(qry, "DOCUMENTTYPE IN ('INVOICE ON HOLD')", "Documenttype in ('Invoice on Hold') and " & ds.Tables("Fieldmapping").Rows(dm).Item("FieldMapping").ToString & " in (" & id & ") ")
                                    ElseIf ds.Tables("Fieldmapping").Rows(dm).Item("documenttype").ToString.ToUpper = "PES" Then
                                        qry = Replace(qry, "DOCUMENTTYPE='PES'", "Documenttype='PES' and " & ds.Tables("Fieldmapping").Rows(dm).Item("FieldMapping").ToString & " in (" & id & ") ")
                                        qry = Replace(qry, "DOCUMENTTYPE IN ('PES')", "Documenttype in ('PES') and " & ds.Tables("Fieldmapping").Rows(dm).Item("FieldMapping").ToString & " in (" & id & ") ")
                                    End If
                                Next
                                If reporttype.ToUpper = "MONTHLY" Then
                                    qry = Replace(qry.ToString.ToUpper, "EID=" & eid & "", " EID=" & eid & " and convert(date,adate)>=convert(date,DATEADD(MONTH, DATEDIFF(MONTH, 0, GETDATE())-1, 0)) and convert(date,adate)<=convert(date,DATEADD(MONTH, DATEDIFF(MONTH, -1, GETDATE())-1, -1)) ")
                                    qry = Replace(qry, "eid=" & eid & "", " EID=" & eid & " and convert(date,adate)>=convert(date,DATEADD(MONTH, DATEDIFF(MONTH, 0, GETDATE())-1, 0)) and convert(date,adate)<=convert(date,DATEADD(MONTH, DATEDIFF(MONTH, -1, GETDATE())-1, -1)) ")
                                ElseIf reporttype.ToUpper = "WEEKLY" Then
                                    qry = Replace(qry, "EID=" & eid & "", " EID=" & eid & " and convert(date,adate)>=convert(date,GETDATE()-8) and convert(date,adate)<=convert(date,GETDATE()-1) ")
                                    qry = Replace(qry, "eid=" & eid & "", " EID=" & eid & " and convert(date,adate)>=convert(date,GETDATE()-8) and convert(date,adate)<=convert(date,GETDATE()-1) ")
                                ElseIf reporttype.ToUpper = "DAILY" Then
                                    qry = Replace(qry, "EID=" & eid & "", " EID=" & eid & " and convert(date,adate)=convert(date,GETDATE()-1) ")
                                    qry = Replace(qry, "eid=" & eid & "", " EID=" & eid & " and convert(date,adate)=convert(date,GETDATE()-1) ")
                                Else
                                    'qry = Replace(qry, "eid=" & eid & "", " EID=" & eid & " and convert(date,adate)=convert(date,GETDATE()) ")
                                End If
                                oda.SelectCommand.CommandText = qry
                                oda.SelectCommand.CommandType = CommandType.Text
                                oda.SelectCommand.CommandTimeout = 1200
                                Dim Common As New System.Data.DataTable
                                oda.Fill(Common)
                                If Common.Rows.Count > 0 Then
                                    Dim CNT As Integer = 0
                                    Dim msg As String = ds.Tables("rpt").Rows(d).Item("msgbody").ToString()
                                    oda.SelectCommand.CommandText = "INSERT_MAILLOGNEW"
                                    oda.SelectCommand.CommandType = CommandType.StoredProcedure
                                    oda.SelectCommand.Parameters.Clear()
                                    oda.SelectCommand.Parameters.AddWithValue("@MAILTO", "Vishal.kumar@myndsol.com")
                                    oda.SelectCommand.Parameters.AddWithValue("@CC", "")
                                    oda.SelectCommand.Parameters.AddWithValue("@MSG", msg)
                                    oda.SelectCommand.Parameters.AddWithValue("@ALERTTYPE", "ALERT")
                                    oda.SelectCommand.Parameters.AddWithValue("@MAILEVENT", "CommonMail")
                                    oda.SelectCommand.Parameters.AddWithValue("@EID", eid)
                                    oda.SelectCommand.Parameters.AddWithValue("@RSID", ds.Tables("rpt").Rows(d).Item("tid").ToString)
                                    If con.State <> ConnectionState.Open Then
                                        con.Open()
                                    End If
                                    oda.SelectCommand.ExecuteNonQuery()


                                    Dim mailsub As String = ds.Tables("rpt").Rows(d).Item("reportsubject").ToString()
                                    Dim fname As String = ""
                                    fname = CreateCSVR(Common, FPath, ds.Tables("User").Rows(u).Item("uid"))
                                    Dim obj As New MailUtill(eid:=eid)
                                    'obj.SendMail(ToMail:="vishal.kumar@myndsol.com", Subject:=mailsub, MailBody:=msg, CC:=CC, Attachments:=FPath + fname, BCC:=Bcc)
                                    'sendMail("vishal.kumar@myndsol.com", CC, Bcc, mailsub, msg, FPath + fname)
                                    sendMailRepSchedule(MAILTO, CC, Bcc, mailsub, msg, FPath + fname)
                                    'lblMsg1.Text = "Report Scheduled...."
                                End If
                            Next
                        Else
                            qry = str
                            qry = qry.ToString.ToUpper
                            If reporttype.ToUpper = "MONTHLY" Then
                                qry = Replace(qry, "EID=" & eid & "", " EID=" & eid & " and convert(date,adate)>=convert(date,DATEADD(MONTH, DATEDIFF(MONTH, 0, GETDATE())-1, 0)) and convert(date,adate)<=convert(date,DATEADD(MONTH, DATEDIFF(MONTH, -1, GETDATE())-1, -1)) ")
                                qry = Replace(qry, "eid=" & eid & "", " EID=" & eid & " and convert(date,adate)>=convert(date,DATEADD(MONTH, DATEDIFF(MONTH, 0, GETDATE())-1, 0)) and convert(date,adate)<=convert(date,DATEADD(MONTH, DATEDIFF(MONTH, -1, GETDATE())-1, -1)) ")
                            ElseIf reporttype.ToUpper = "WEEKLY" Then
                                qry = Replace(qry, "EID=" & eid & "", " EID=" & eid & " and convert(date,adate)>=convert(date,GETDATE()-8) and convert(date,adate)<=convert(date,GETDATE()-1) ")
                                qry = Replace(qry, "eid=" & eid & "", " EID=" & eid & " and convert(date,adate)>=convert(date,GETDATE()-8) and convert(date,adate)<=convert(date,GETDATE()-1) ")
                            ElseIf reporttype.ToUpper = "DAILY" Then
                                qry = Replace(qry, "EID=" & eid & "", " EID=" & eid & " and convert(date,adate)=convert(date,GETDATE()-1) ")
                                qry = Replace(qry, "eid=" & eid & "", " EID=" & eid & " and convert(date,adate)=convert(date,GETDATE()-1) ")
                            Else
                                'qry = Replace(qry, "eid=" & eid & "", " EID=" & eid & " and convert(date,adate)=convert(date,GETDATE()) ")
                            End If
                            oda.SelectCommand.CommandText = qry
                            oda.SelectCommand.CommandType = CommandType.Text
                            oda.SelectCommand.CommandTimeout = 1200
                            Dim Common As New System.Data.DataTable
                            oda.Fill(Common)
                            If Common.Rows.Count > 0 Then
                                Dim CNT As Integer = 0
                                Dim msg As String = ds.Tables("rpt").Rows(d).Item("msgbody").ToString()
                                oda.SelectCommand.CommandText = "INSERT_MAILLOGNEW"
                                oda.SelectCommand.CommandType = CommandType.StoredProcedure
                                oda.SelectCommand.Parameters.Clear()
                                oda.SelectCommand.Parameters.AddWithValue("@MAILTO", MAILTO)
                                oda.SelectCommand.Parameters.AddWithValue("@CC", "")
                                oda.SelectCommand.Parameters.AddWithValue("@MSG", msg)
                                oda.SelectCommand.Parameters.AddWithValue("@ALERTTYPE", "ALERT")
                                oda.SelectCommand.Parameters.AddWithValue("@MAILEVENT", "CommonMail")
                                oda.SelectCommand.Parameters.AddWithValue("@EID", eid)
                                oda.SelectCommand.Parameters.AddWithValue("@RSID", ds.Tables("rpt").Rows(d).Item("tid").ToString)
                                If con.State <> ConnectionState.Open Then
                                    con.Open()
                                End If
                                oda.SelectCommand.ExecuteNonQuery()

                                Dim mailsub As String = ds.Tables("rpt").Rows(d).Item("reportsubject").ToString()
                                Dim fname As String = ""
                                fname = CreateCSVR(Common, FPath, eid)
                                'Dim obj As New MailUtill(eid:=eid)
                                'sendMail(ToMail:=MAILTO, Subject:=mailsub, MailBody:=msg, cc:=CC, Attachments:=FPath + fname, bcc:=Bcc)
                                'sendMail("vishal.kumar@myndsol.com", CC, Bcc, mailsub, msg, FPath + fname)
                                sendMailRepSchedule(MAILTO, CC, Bcc, mailsub, msg, FPath + fname)
                                'lblMsg1.Text = "Report Scheduled...."
                            End If
                        End If
                    ElseIf sendtype.ToString.ToUpper = "MAILBODY" Then
                        Dim MAILTO As String = ds.Tables("rpt").Rows(d).Item("emailto").ToString()
                        Dim CC As String = ds.Tables("rpt").Rows(d).Item("cc").ToString()
                        Dim Bcc As String = ds.Tables("rpt").Rows(d).Item("bcc").ToString()
                        Dim str As String = ds.Tables("rpt").Rows(d).Item("reportquery").ToString()
                        Dim role As String = ds.Tables("rpt").Rows(d).Item("sendto").ToString()
                        Dim reporttype As String = ds.Tables("rpt").Rows(d).Item("reporttype").ToString()
                        Dim dt As String = ds.Tables("rpt").Rows(d).Item("date").ToString()
                        eid = ds.Tables("rpt").Rows(d).Item("EID").ToString()
                        Dim msg As String = ds.Tables("rpt").Rows(d).Item("msgbody").ToString()
                        Dim qry As String
                        If role <> "" Then
                            role = Replace(role, ",", "','")
                            oda.SelectCommand.CommandText = " select * from mmm_mst_user where uid in (select distinct uid from mmm_ref_role_user where eid=" & eid & " and rolename in ('" & role & "') and documenttype like '%invoice%') "
                            oda.Fill(ds, "User")

                            'Dim lastschtime As String = Format(Convert.ToDateTime(ds.Tables("rpt").Rows(d).Item("lastscheduleddate").ToString), "yyyy-MM-dd")
                            'Dim curtime As String = Date.Now.ToString("yyyy-MM-dd")
                            'If lastschtime = curtime Then
                            '    Exit Sub
                            'End If
                            oda.SelectCommand.CommandText = "select FormName,DocMapping from mmm_mst_forms where eid=" & eid & " and DocMapping is not null"
                            oda.Fill(ds, "DocMapping")
                            If ds.Tables("DocMapping").Rows.Count > 0 Then
                                oda.SelectCommand.CommandText = "select FieldMapping,documenttype from MMM_MST_FIELDS where eid=" & eid & " and dropdown like '%" & ds.Tables("DocMapping").Rows(0).Item("FormName") & "%' and documenttype in ('PES','Invoice PO','Invoice Non PO','Invoice on Hold')"
                                oda.Fill(ds, "Fieldmapping")
                            End If

                            For u As Integer = 0 To ds.Tables("User").Rows.Count - 1
                                If Len(MAILTO) < 3 Then
                                    MAILTO = ds.Tables("User").Rows(u).Item("EmailID").ToString
                                End If
                                qry = str.ToUpper
                                qry = qry.ToString.ToUpper
                                oda.SelectCommand.CommandText = "select " & ds.Tables("DocMapping").Rows(0).Item("DocMapping") & " from MMM_Ref_Role_User where eid=" & eid & " and uid=" & ds.Tables("User").Rows(u).Item("uid") & " and rolename in ('" & role & "') and documenttype like '%invoice%'"
                                Dim id As String = ""
                                If con.State <> ConnectionState.Open Then
                                    con.Open()
                                End If
                                oda.SelectCommand.CommandType = CommandType.Text
                                id = oda.SelectCommand.ExecuteScalar().ToString
                                For dm As Integer = 0 To ds.Tables("Fieldmapping").Rows.Count - 1
                                    If ds.Tables("Fieldmapping").Rows(dm).Item("documenttype").ToString.ToUpper = "INVOICE PO" Then
                                        qry = Replace(qry, "DOCUMENTTYPE='INVOICE PO'", "Documenttype='Invoice PO' and " & ds.Tables("Fieldmapping").Rows(dm).Item("FieldMapping").ToString & " in (" & id & ") ")
                                        qry = Replace(qry, "DOCUMENTTYPE IN ('INVOICE PO')", "Documenttype in ('Invoice PO') and " & ds.Tables("Fieldmapping").Rows(dm).Item("FieldMapping").ToString & " in (" & id & ") ")
                                    ElseIf ds.Tables("Fieldmapping").Rows(dm).Item("documenttype").ToString.ToUpper = "INVOICE NON PO" Then
                                        qry = Replace(qry, "DOCUMENTTYPE='INVOICE NON PO'", "Documenttype='Invoice non PO' and " & ds.Tables("Fieldmapping").Rows(dm).Item("FieldMapping").ToString & " in (" & id & ") ")
                                        qry = Replace(qry, "DOCUMENTTYPE IN ('INVOICE NON PO')", "Documenttype in ('Invoice non PO') and " & ds.Tables("Fieldmapping").Rows(dm).Item("FieldMapping").ToString & " in (" & id & ") ")
                                    ElseIf ds.Tables("Fieldmapping").Rows(dm).Item("documenttype").ToString.ToUpper = "INVOICE ON HOLD" Then
                                        qry = Replace(qry, "DOCUMENTTYPE='INVOICE ON HOLD'", "Documenttype='Invoice on Hold' and " & ds.Tables("Fieldmapping").Rows(dm).Item("FieldMapping").ToString & " in (" & id & ") ")
                                        qry = Replace(qry, "DOCUMENTTYPE IN ('INVOICE ON HOLD')", "Documenttype in ('Invoice on Hold') and " & ds.Tables("Fieldmapping").Rows(dm).Item("FieldMapping").ToString & " in (" & id & ") ")
                                    ElseIf ds.Tables("Fieldmapping").Rows(dm).Item("documenttype").ToString.ToUpper = "PES" Then
                                        qry = Replace(qry, "DOCUMENTTYPE='PES'", "Documenttype='PES' and " & ds.Tables("Fieldmapping").Rows(dm).Item("FieldMapping").ToString & " in (" & id & ") ")
                                        qry = Replace(qry, "DOCUMENTTYPE IN ('PES')", "Documenttype in ('PES') and " & ds.Tables("Fieldmapping").Rows(dm).Item("FieldMapping").ToString & " in (" & id & ") ")
                                    End If
                                Next
                                If reporttype.ToUpper = "MONTHLY" Then
                                    qry = Replace(qry, "EID=" & eid & "", " EID=" & eid & " and convert(date,adate)>=convert(date,DATEADD(MONTH, DATEDIFF(MONTH, 0, GETDATE())-1, 0)) and convert(date,adate)<=convert(date,DATEADD(MONTH, DATEDIFF(MONTH, -1, GETDATE())-1, -1)) ")
                                    qry = Replace(qry, "eid=" & eid & "", " EID=" & eid & " and convert(date,adate)>=convert(date,DATEADD(MONTH, DATEDIFF(MONTH, 0, GETDATE())-1, 0)) and convert(date,adate)<=convert(date,DATEADD(MONTH, DATEDIFF(MONTH, -1, GETDATE())-1, -1)) ")
                                ElseIf reporttype.ToUpper = "WEEKLY" Then
                                    qry = Replace(qry, "EID=" & eid & "", " EID=" & eid & " and convert(date,adate)>=convert(date,GETDATE()-8) and convert(date,adate)<=convert(date,GETDATE()-1) ")
                                    qry = Replace(qry, "eid=" & eid & "", " EID=" & eid & " and convert(date,adate)>=convert(date,GETDATE()-8) and convert(date,adate)<=convert(date,GETDATE()-1) ")
                                ElseIf reporttype.ToUpper = "DAILY" Then
                                    qry = Replace(qry, "EID=" & eid & "", " EID=" & eid & " and convert(date,adate)=convert(date,GETDATE()-1) ")
                                    qry = Replace(qry, "eid=" & eid & "", " EID=" & eid & " and convert(date,adate)=convert(date,GETDATE()-1) ")
                                Else
                                    'qry = Replace(qry, "eid=" & eid & "", " EID=" & eid & " and convert(date,adate)=convert(date,GETDATE()) ")
                                End If
                                oda.SelectCommand.CommandText = qry
                                oda.SelectCommand.CommandType = CommandType.Text
                                oda.SelectCommand.CommandTimeout = 1200

                                Dim Common As New System.Data.DataTable
                                oda.Fill(Common)
                                If Common.Rows.Count > 0 Then
                                    Dim CNT As Integer = 0
                                    oda.SelectCommand.CommandText = "INSERT_MAILLOGNEW"
                                    oda.SelectCommand.CommandType = CommandType.StoredProcedure
                                    oda.SelectCommand.Parameters.Clear()
                                    oda.SelectCommand.Parameters.AddWithValue("@MAILTO", MAILTO)
                                    oda.SelectCommand.Parameters.AddWithValue("@CC", "")
                                    oda.SelectCommand.Parameters.AddWithValue("@MSG", msg)
                                    oda.SelectCommand.Parameters.AddWithValue("@ALERTTYPE", "ALERT")
                                    oda.SelectCommand.Parameters.AddWithValue("@MAILEVENT", "CommonMail")
                                    oda.SelectCommand.Parameters.AddWithValue("@EID", eid)
                                    oda.SelectCommand.Parameters.AddWithValue("@RSID", ds.Tables("rpt").Rows(d).Item("tid").ToString)
                                    If con.State <> ConnectionState.Open Then
                                        con.Open()
                                    End If
                                    oda.SelectCommand.ExecuteNonQuery()

                                    Dim MailTable As New StringBuilder()
                                    MailTable.Append("<table border=""1"" width=""100%"">")
                                    MailTable.Append("<tr style=""background-color:#990000"" Font-Bold=""True"">  ")

                                    For l As Integer = 0 To Common.Columns.Count - 1
                                        If Common.Columns(l).ColumnName.ToUpper = "CURRENTSTATUS" Then
                                            MailTable.Append("<td > CURRENT STATUS </td>")
                                        ElseIf Common.Columns(l).ColumnName.ToUpper = "COUNT1TO7DAYS" Then
                                            MailTable.Append("<td > COUNT 1-7 DAYS </td>")
                                        ElseIf Common.Columns(l).ColumnName.ToUpper = "INVOICEAMOUNT1TO7DAYS" Then
                                            MailTable.Append("<td > INVOICE AMOUNT 1-7 DAYS(LACS) </td>")
                                        ElseIf Common.Columns(l).ColumnName.ToUpper = "COUNT8TO14DAYS" Then
                                            MailTable.Append("<td > COUNT 8-14 DAYS</td>")
                                        ElseIf Common.Columns(l).ColumnName.ToUpper = "INVOICEAMOUNT8TO14DAYS" Then
                                            MailTable.Append("<td > INVOICE AMOUNT 8-14 DAYS(LACS) </td>")
                                        ElseIf Common.Columns(l).ColumnName.ToUpper = "COUNT15DAYS" Then
                                            MailTable.Append("<td > COUNT 15+ DAYS </td>")
                                        ElseIf Common.Columns(l).ColumnName.ToUpper = "INVOICEAMOUNT15DAYS" Then
                                            MailTable.Append("<td > INVOICE AMOUNT 15+ DAYS(LACS) </td>")
                                        ElseIf Common.Columns(l).ColumnName.ToUpper = "TOTALBARCODECOUNT" Then
                                            MailTable.Append("<td > TOTAL BAR CODE COUNT </td>")
                                        ElseIf Common.Columns(l).ColumnName.ToUpper = "TOTALINVOICEAMOUNT" Then
                                            MailTable.Append("<td > TOTAL INVOICE AMOUNT(LACS) </td>")
                                        Else
                                            MailTable.Append("<td > " & Common.Columns(l).ColumnName & " </td>")
                                        End If
                                    Next
                                    'For l As Integer = 0 To Common.Columns.Count - 1
                                    '    MailTable.Append("<td >  " & Common.Columns(l).ColumnName & " </td>")
                                    'Next
                                    '#DCDCDC
                                    For k As Integer = 0 To Common.Rows.Count - 1 ' binding the tr tab in table
                                        If (Common.Rows(k).Item(0).ToString.Contains("Total") = True) Then
                                            MailTable.Append("</tr><tr style=""background-color:#DCDCDC"">") ' for row records
                                        Else
                                            MailTable.Append("</tr><tr>") ' for row records
                                        End If
                                        For t As Integer = 0 To Common.Columns.Count - 1
                                            If IsNumeric(Common.Rows(k).Item(t).ToString()) = True Then
                                                MailTable.Append("<td align=""right""> " & Common.Rows(k).Item(t).ToString() & "  </td>")
                                            Else
                                                MailTable.Append("<td align=""left""> " & Common.Rows(k).Item(t).ToString() & "  </td>")
                                            End If
                                        Next
                                        MailTable.Append(" </tr>")
                                    Next
                                    MailTable.Append("</table>")

                                    Dim mailsub As String = ds.Tables("rpt").Rows(d).Item("reportsubject").ToString()
                                    Dim strmsgBdy As String = ds.Tables("rpt").Rows(d).Item("msgbody").ToString()
                                    If strmsgBdy.Contains("@body") Then
                                        strmsgBdy = Replace(strmsgBdy, "@body", MailTable.ToString())
                                        msg = strmsgBdy
                                    Else
                                        msg = MailTable.ToString()
                                    End If
                                    msg = strmsgBdy

                                    'sendMail1(MAILTO, CC, Bcc, mailsub, msg)
                                    ' Dim obj As New MailUtill(eid:=eid)
                                    'obj.SendMail(ToMail:=MAILTO, Subject:=mailsub, MailBody:=msg, CC:=CC, BCC:=Bcc)
                                    'sendMail("vishal.kumar@myndsol.com", CC, Bcc, mailsub, msg, "")
                                    sendMailRepSchedule(MAILTO, CC, Bcc, mailsub, msg, "")
                                    'lblMsg1.Text = "Report Scheduled...."
                                End If
                            Next
                        Else
                            qry = str
                            qry = qry.ToString.ToUpper
                            If reporttype.ToUpper = "MONTHLY" Then
                                qry = Replace(qry, "EID=" & eid & "", " EID=" & eid & " and convert(date,adate)>=convert(date,DATEADD(MONTH, DATEDIFF(MONTH, 0, GETDATE())-1, 0)) and convert(date,adate)<=convert(date,DATEADD(MONTH, DATEDIFF(MONTH, -1, GETDATE())-1, -1)) ")
                                qry = Replace(qry, "eid=" & eid & "", " EID=" & eid & " and convert(date,adate)>=convert(date,DATEADD(MONTH, DATEDIFF(MONTH, 0, GETDATE())-1, 0)) and convert(date,adate)<=convert(date,DATEADD(MONTH, DATEDIFF(MONTH, -1, GETDATE())-1, -1)) ")
                            ElseIf reporttype.ToUpper = "WEEKLY" Then
                                qry = Replace(qry, "EID=" & eid & "", " EID=" & eid & " and convert(date,adate)>=convert(date,GETDATE()-8) and convert(date,adate)<=convert(date,GETDATE()-1) ")
                                qry = Replace(qry, "eid=" & eid & "", " EID=" & eid & " and convert(date,adate)>=convert(date,GETDATE()-8) and convert(date,adate)<=convert(date,GETDATE()-1) ")
                            ElseIf reporttype.ToUpper = "DAILY" Then
                                qry = Replace(qry, "EID=" & eid & "", " EID=" & eid & " and convert(date,adate)=convert(date,GETDATE()-1) ")
                                qry = Replace(qry, "eid=" & eid & "", " EID=" & eid & " and convert(date,adate)=convert(date,GETDATE()-1) ")
                            Else
                                'qry = Replace(qry, "eid=" & eid & "", " EID=" & eid & " and convert(date,adate)=convert(date,GETDATE()) ")
                            End If
                            oda.SelectCommand.CommandText = qry
                            oda.SelectCommand.CommandType = CommandType.Text
                            oda.SelectCommand.CommandTimeout = 1200

                            Dim Common As New System.Data.DataTable
                            oda.Fill(Common)
                            If Common.Rows.Count > 0 Then
                                Dim CNT As Integer = 0
                                oda.SelectCommand.CommandText = "INSERT_MAILLOGNEW"
                                oda.SelectCommand.CommandType = CommandType.StoredProcedure
                                oda.SelectCommand.Parameters.Clear()
                                oda.SelectCommand.Parameters.AddWithValue("@MAILTO", MAILTO)
                                oda.SelectCommand.Parameters.AddWithValue("@CC", "")
                                oda.SelectCommand.Parameters.AddWithValue("@MSG", msg)
                                oda.SelectCommand.Parameters.AddWithValue("@ALERTTYPE", "ALERT")
                                oda.SelectCommand.Parameters.AddWithValue("@MAILEVENT", "CommonMail")
                                oda.SelectCommand.Parameters.AddWithValue("@EID", eid)
                                oda.SelectCommand.Parameters.AddWithValue("@RSID", ds.Tables("rpt").Rows(d).Item("tid").ToString)
                                If con.State <> ConnectionState.Open Then
                                    con.Open()
                                End If
                                oda.SelectCommand.ExecuteNonQuery()

                                Dim MailTable As New StringBuilder()
                                MailTable.Append("<table border=""1"" width=""100%"">")
                                MailTable.Append("<tr style=""background-color:#990000"" Font-Bold=""True"">")

                                For l As Integer = 0 To Common.Columns.Count - 1
                                    If Common.Columns(l).ColumnName.ToUpper = "CURRENTSTATUS" Then
                                        MailTable.Append("<td > CURRENT STATUS </td>")
                                    ElseIf Common.Columns(l).ColumnName.ToUpper = "COUNT1TO7DAYS" Then
                                        MailTable.Append("<td > COUNT 1-7 DAYS </td>")
                                    ElseIf Common.Columns(l).ColumnName.ToUpper = "INVOICEAMOUNT1TO7DAYS" Then
                                        MailTable.Append("<td > INVOICE AMOUNT 1-7 DAYS(LACS) </td>")
                                    ElseIf Common.Columns(l).ColumnName.ToUpper = "COUNT8TO14DAYS" Then
                                        MailTable.Append("<td > COUNT 8-14 DAYS</td>")
                                    ElseIf Common.Columns(l).ColumnName.ToUpper = "INVOICEAMOUNT8TO14DAYS" Then
                                        MailTable.Append("<td > INVOICE AMOUNT 8-14 DAYS(LACS) </td>")
                                    ElseIf Common.Columns(l).ColumnName.ToUpper = "COUNT15DAYS" Then
                                        MailTable.Append("<td > COUNT 15+ DAYS </td>")
                                    ElseIf Common.Columns(l).ColumnName.ToUpper = "INVOICEAMOUNT15DAYS" Then
                                        MailTable.Append("<td > INVOICE AMOUNT 15+ DAYS(LACS) </td>")
                                    ElseIf Common.Columns(l).ColumnName.ToUpper = "TOTALBARCODECOUNT" Then
                                        MailTable.Append("<td > TOTAL BAR CODE COUNT </td>")
                                    ElseIf Common.Columns(l).ColumnName.ToUpper = "TOTALINVOICEAMOUNT" Then
                                        MailTable.Append("<td > TOTAL INVOICE AMOUNT(LACS) </td>")
                                    Else
                                        MailTable.Append("<td > " & Common.Columns(l).ColumnName & " </td>")
                                    End If
                                Next

                                For k As Integer = 0 To Common.Rows.Count - 1 ' binding the tr tab in table
                                    If (Common.Rows(k).Item(0).ToString.Contains("Total") = True) Then
                                        MailTable.Append("</tr><tr style=""background-color:#DCDCDC"">") ' for row records
                                    Else
                                        MailTable.Append("</tr><tr>") ' for row records
                                    End If
                                    For t As Integer = 0 To Common.Columns.Count - 1
                                        If IsNumeric(Common.Rows(k).Item(t).ToString()) = True Then
                                            MailTable.Append("<td align=""right""> " & Common.Rows(k).Item(t).ToString() & "  </td>")
                                        Else
                                            MailTable.Append("<td align=""left""> " & Common.Rows(k).Item(t).ToString() & "  </td>")
                                        End If
                                    Next
                                    MailTable.Append(" </tr>")
                                Next
                                MailTable.Append("</table>")

                                Dim mailsub As String = ds.Tables("rpt").Rows(d).Item("reportsubject").ToString()
                                Dim strmsgBdy As String = ds.Tables("rpt").Rows(d).Item("msgbody").ToString()
                                If strmsgBdy.Contains("@body") Then
                                    strmsgBdy = Replace(strmsgBdy, "@body", MailTable.ToString())
                                    msg = strmsgBdy
                                Else
                                    msg = MailTable.ToString()
                                End If
                                msg = strmsgBdy

                                'sendMail1(MAILTO, CC, Bcc, mailsub, msg)
                                Dim obj As New MailUtill(eid:=eid)
                                'obj.SendMail(ToMail:=MAILTO, Subject:=mailsub, MailBody:=msg, CC:=CC, BCC:=Bcc)
                                'sendMail("vishal.kumar@myndsol.com", CC, Bcc, mailsub, msg, "")
                                sendMailRepSchedule(MAILTO, CC, Bcc, mailsub, msg, "")
                                'lblMsg1.Text = "Report Scheduled...."
                            End If
                        End If
                    End If
                End If
            Next
        Catch ex As Exception
            Dim sta As New StackTrace(ex, True)
            Dim fr = sta.GetFrame(sta.FrameCount - 1)
            Dim frline As String = fr.GetFileLineNumber.ToString()
            Dim methodname = fr.GetMethod.ToString()
            oda.SelectCommand.CommandText = "INSERT_ERRORLOG"
            oda.SelectCommand.CommandType = CommandType.StoredProcedure
            oda.SelectCommand.Parameters.Clear()
            oda.SelectCommand.Parameters.AddWithValue("@ERRORMSG", ex.ToString() & ": LineNo-" & frline & "MethodName" & methodname)
            oda.SelectCommand.Parameters.AddWithValue("@ALERTNAME", alertname)
            oda.SelectCommand.Parameters.AddWithValue("@EID", eid)
            If con.State <> ConnectionState.Open Then
                con.Open()
            End If
            oda.SelectCommand.ExecuteNonQuery()
        Finally
            con.Close()
            con.Dispose()
            oda.Dispose()
        End Try
    End Sub

    Public Function ReportScheduler(ByVal tid As Integer) As Boolean
        Dim b As Boolean = False
        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim con As SqlConnection = New SqlConnection(conStr)
        Dim time1 As DateTime = Now.AddMinutes(-5).ToShortTimeString()
        Dim time2 As DateTime = Now.AddMinutes(+5).ToShortTimeString()
        Dim da As New SqlDataAdapter("select HH,MM,reporttype,TID,date from MMM_MST_ReportScheduler where tid=" & tid & " ", con)
        Dim dt As New System.Data.DataTable()
        da.Fill(dt)
        Dim SchType As String = dt.Rows(0).Item("reporttype").ToString()
        If ((UCase(SchType) = "DAILY") Or (UCase(SchType) = "AS ON DATE")) Then
            Dim x As DateTime = (Convert.ToDateTime(Trim(dt.Rows(0)(0)) & ":" & Trim(dt.Rows(0)(1)) & ":" & "00").ToShortTimeString)
            If x <= time2 And x >= time1 Then
                b = True
            End If
        End If
        If UCase(SchType) = "WEEKLY" Then
            Dim dayName As String = DateTime.Now.ToString("dddd")
            Dim currentweek As String = dt.Rows(0).Item("Date").ToString()
            If currentweek = 1 Then
                currentweek = "Monday"
            ElseIf currentweek = 2 Then
                currentweek = "Tuesday"
            ElseIf currentweek = 3 Then
                currentweek = "Wednesday"
            ElseIf currentweek = 4 Then
                currentweek = "Thursday"
            ElseIf currentweek = 5 Then
                currentweek = "Friday"
            ElseIf currentweek = 6 Then
                currentweek = "Saturday"
            ElseIf currentweek = 7 Then
                currentweek = "Sunday"
            End If
            If currentweek = dayName Then
                Dim x As DateTime = (Convert.ToDateTime(Trim(dt.Rows(0)(0)) & ":" & Trim(dt.Rows(0)(1)) & ":" & "00").ToShortTimeString)
                If x <= time2 And x >= time1 Then
                    b = True
                End If
            End If
        End If
        If UCase(SchType) = "MONTHLY" Then
            Dim currentDate As DateTime = DateTime.Now
            Dim dateofMonth As Integer = currentDate.Day
            Dim dateMail As Integer = dt.Rows(0).Item("Date").ToString()
            If dateofMonth = dateMail Then
                Dim x As DateTime = (Convert.ToDateTime(Trim(dt.Rows(0)(0)) & ":" & Trim(dt.Rows(0)(1)) & ":" & "00").ToShortTimeString)
                If x <= time2 And x >= time1 Then
                    b = True
                End If
            End If
        End If
        con.Close()
        con.Dispose()
        da.Dispose()
        dt.Dispose()
        Return b
    End Function
    Private Function CreateCSVR(ByVal dt As System.Data.DataTable, ByVal path As String, ByVal uid As String) As String
        Dim fname As String = Now.Year & Now.Month & Now.Day & Now.Hour & Now.Minute & Now.Millisecond & uid & ".xls"
        'Dim fname As String = "F014Z1_" & DateTime.Now.ToString("yyyy-MM-dd") & ".CSV"
        Try
            If File.Exists(path & fname) Then
                File.Delete(path & fname)
            End If

            Dim sw As StreamWriter

            sw = New StreamWriter(path & fname, True)
            Dim hw As New HtmlTextWriter(sw)
            sw.Flush()
            Dim iColCount As Integer = dt.Columns.Count
            Dim grd As New GridView
            grd.DataSource = dt
            grd.DataBind()

            For i = 0 To grd.HeaderRow.Cells.Count - 1
                If grd.HeaderRow.Cells(i).Text.Contains("CurrentStatus").ToString Then
                    grd.HeaderRow.Cells(i).Text = "Current Status"
                ElseIf grd.HeaderRow.Cells(i).Text.Contains("Count1To7Days").ToString Then
                    grd.HeaderRow.Cells(i).Text = "Count 1-7 Days"
                ElseIf grd.HeaderRow.Cells(i).Text.Contains("InvoiceAmount1To7Days").ToString Then
                    grd.HeaderRow.Cells(i).Text = "Invoice Amount 1-7 Days(lacs)"
                ElseIf grd.HeaderRow.Cells(i).Text.Contains("Count8To14Days").ToString Then
                    grd.HeaderRow.Cells(i).Text = "Count 8-14 Days"
                ElseIf grd.HeaderRow.Cells(i).Text.Contains("InvoiceAmount8To14Days").ToString Then
                    grd.HeaderRow.Cells(i).Text = "Invoice Amount 8-14 Days(lacs)"
                ElseIf grd.HeaderRow.Cells(i).Text.Contains("Count15Days").ToString Then
                    grd.HeaderRow.Cells(i).Text = "Count 15+ Days"
                ElseIf grd.HeaderRow.Cells(i).Text.Contains("InvoiceAmount15Days").ToString Then
                    grd.HeaderRow.Cells(i).Text = "Invoice Amount 15+ Days(lacs)"
                ElseIf grd.HeaderRow.Cells(i).Text.Contains("TotalBarCodeCount").ToString Then
                    grd.HeaderRow.Cells(i).Text = "Total Bar Code Count"
                ElseIf grd.HeaderRow.Cells(i).Text.Contains("TotalInvoiceAmount").ToString Then
                    grd.HeaderRow.Cells(i).Text = "Total Invoice Amount(lacs)"
                End If
            Next

            For i As Integer = 0 To grd.Rows.Count - 1
                grd.Rows(i).Attributes.Add("class", "textmode")
                'Apply text style to each Row 
            Next

            'grd.CssClass = style
            grd.RenderControl(hw)
            Dim style As String = "<style> .textmode { mso-number-format:\""0000""; } </style>"
            sw.WriteLine(style)
            sw.Write(sw.NewLine)

            ' Now write all the rows.

            sw.Close()
        Catch ex As Exception

        End Try
        Return fname
        'sw.Close()
    End Function
    Public Sub sendMailRepSchedule(ByVal Mto As String, ByVal cc As String, ByVal bcc As String, ByVal MSubject As String, ByVal MBody As String, Optional ByVal backuppath As String = "")
        Try
            If Left(Mto, 1) = "{" Then
                Exit Sub
            End If
            Dim Email As New System.Net.Mail.MailMessage("no-reply@myndsol.com", Mto, MSubject, MBody)
            Dim mailClient As New System.Net.Mail.SmtpClient()
            Email.IsBodyHtml = True
            If cc <> "" Then
                Email.CC.Add(cc)
                ' Email.Attachments.Add(att)
            End If
            If bcc <> "" Then
                Email.Bcc.Add(bcc)
                'Email.Attachments.Add(att)
            End If
            'If att.ContentType.ToString.Contains(".csv") Then
            If backuppath <> "" Then
                Dim att As System.Net.Mail.Attachment
                att = New System.Net.Mail.Attachment(backuppath)
                Email.Attachments.Add(att)
            End If

            'End If
            Dim basicAuthenticationInfo As New System.Net.NetworkCredential("no-reply@myndsol.com", "Dn#Ms@538Ti")
            mailClient.Host = "mail.myndsol.com"
            mailClient.UseDefaultCredentials = False
            mailClient.Credentials = basicAuthenticationInfo
            Try
                mailClient.Send(Email)
            Catch ex As Exception
                Exit Sub
            End Try
        Catch ex As Exception
            Exit Sub
        End Try
    End Sub
    Protected Sub btnmail_Click(sender As Object, e As EventArgs) Handles btnmail.Click
        Try
            ReportSchedulerMail()
        Catch ex As Exception
            Throw
        End Try
    End Sub
End Class
