Imports System.Data
Imports System.Data.SqlClient
Imports System.Web.UI.Adapters.ControlAdapter
Imports iTextSharp.text
Imports System.Drawing
Imports System.IO
Partial Class ExternalPowerReport
    Inherits System.Web.UI.Page
    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Not IsPostBack Then
            Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
            Dim con As SqlConnection = New SqlConnection(conStr)
            Dim Udtype As String
            Dim Ufld As String
            Dim UVfld As String
            Dim Vdtype As String
            Dim Vfld As String
            Dim vemei As String
            Dim oda As SqlDataAdapter = New SqlDataAdapter("", con)
            Try
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
                If Session("EID") = 32 Then
                    lblveh.Text = "UserName-VehicleNo."
                    If Session("USERROLE") = "SU" Or Session("USERROLE") = "FCAGGN" Or Session("USERROLE") = "BNK" Or Session("USERROLE") = "CADMIN" Or Session("USERROLE") = "FCANHQ" Then
                        'oda.SelectCommand.CommandText = "select  distinct m." & vemei & "[IMIENO], (select username from mmm_mst_user where eid=" & Session("EID") & " and uid=d." & Ufld & ")[UserName],m." & Vfld & "[VehicleNo] from mmm_mst_Doc d inner join mmm_mst_master m on m.tid=d." & UVfld & " inner join mmm_mst_gpsdata g on g.imieno=m." & vemei & " where d.eid=" & Session("EID") & " and  d.documenttype='" & Udtype & "'  and imieno is not null order by username "
                        oda.SelectCommand.CommandText = "select  distinct m." & vemei & "[IMIENO], u.username,m." & Vfld & "[VehicleNo] from mmm_mst_Doc d inner join mmm_mst_master m on convert(nvarchar,m.tid)=d." & UVfld & "  inner join mmm_mst_user u on u.uid=d." & Ufld & " where d.eid=" & Session("EID") & " and  d.documenttype='" & Udtype & "'  and  m." & vemei & " is not null and  m." & vemei & " <>'' order by username "
                    ElseIf Session("USERROLE").ToString.ToUpper() = "USER" Then
                        oda.SelectCommand.CommandText = "select  distinct m." & vemei & "[IMIENO], u.username,m." & Vfld & "[VehicleNo] from mmm_mst_Doc d inner join mmm_mst_master m on convert(nvarchar,m.tid)=d." & UVfld & " inner join mmm_mst_gpsdata g on g.imieno=m." & vemei & " inner join mmm_mst_user u on u.uid=d." & Ufld & " where d.eid=" & Session("EID") & " and  d.documenttype='" & Udtype & "' and d." & Ufld & "= " & Session("UID").ToString() & "  and imieno is not null order by username "
                    ElseIf Session("USERROLE").ToString.ToUpper() = "VENDOR" Then
                        oda.SelectCommand.CommandText = "select distinct m." & vemei & "[IMIENO], m." & Vfld & "[VehicleNo] from  mmm_mst_user u inner join mmm_mst_master m on m.fld12=convert(nvarchar,u.extid) where m.documenttype='vehicle' and extid=" & Session("EXTID") & " and m." & vemei & " is not null and m." & vemei & "<>''"
                    Else
                        'If IsNothing(Session("SUBUID")) Then
                        '    oda.SelectCommand.CommandText = "select  distinct m." & vemei & "[IMIENO], u.username,m." & Vfld & "[VehicleNo] from mmm_mst_Doc d inner join mmm_mst_master m on m.tid=d." & UVfld & " inner join mmm_mst_gpsdata g on g.imieno=m." & vemei & " inner join mmm_mst_user u on u.uid=d." & Ufld & " where d.eid=" & Session("EID") & " and  d.documenttype='" & Udtype & "' and d." & Ufld & " in (" & Session("UID").ToString() & ")  and imieno is not null order by username "
                        'Else
                        '    oda.SelectCommand.CommandText = "select  distinct m." & vemei & "[IMIENO], u.username,m." & Vfld & "[VehicleNo] from mmm_mst_Doc d inner join mmm_mst_master m on m.tid=d." & UVfld & " inner join mmm_mst_gpsdata g on g.imieno=m." & vemei & " inner join mmm_mst_user u on u.uid=d." & Ufld & " where d.eid=" & Session("EID") & " and  d.documenttype='" & Udtype & "' and d." & Ufld & " in (" & Session("SUBUID").ToString() & ")  and imieno is not null order by username "
                        'End If
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
                    lblveh.Text = "VehicleName-VehicleNo."
                    If Session("USERROLE").ToString.ToUpper = "SU" Or Session("USERROLE").ToString.ToUpper = "CADMIN" Or Session("USERROLE").ToString.ToUpper = "CORPORATEUSER" Then
                        oda.SelectCommand.CommandText = "select fld12[IMEI],fld10[VehicleName],fld1[VehicleNo] from mmm_mst_master where documenttype='vehicle' and eid=" & Session("EID") & " and len(fld12)=15 and fld12<>''"
                    Else
                        oda.SelectCommand.CommandType = CommandType.StoredProcedure
                        ' oda.SelectCommand.CommandText = "uspGetRoleUID"
                        oda.SelectCommand.CommandText = "vehiclerightforIndus"
                        oda.SelectCommand.Parameters.Clear()
                        oda.SelectCommand.Parameters.AddWithValue("uid", Session("UID"))
                        oda.SelectCommand.Parameters.AddWithValue("eid", Session("EID"))
                        oda.SelectCommand.Parameters.AddWithValue("rolename", Session("USERROLE"))
                        oda.SelectCommand.Parameters.AddWithValue("docType", "Vehicle")
                    End If
                End If
                'oda.SelectCommand.CommandText = "select imieno,username,VehicleNo from ( select  distinct m." & vemei & "[IMIENO], (select username from mmm_mst_user where eid=" & Session("EID") & " and uid=d." & Ufld & ")[UserName],m." & Vfld & "[VehicleNo] from mmm_mst_Doc d inner join mmm_mst_master m on m.tid=d." & UVfld & " inner join mmm_mst_gpsdata g on g.imieno=m." & vemei & " where d.eid=" & Session("EID") & " and  d.documenttype='" & Udtype & "'  and imieno is not null) as table1 order by username  "
                ds.Clear()
                oda.SelectCommand.CommandTimeout = 300
                oda.Fill(ds, "vemei")
                For i As Integer = 0 To ds.Tables("vemei").Rows.Count - 1
                    If Session("USERROLE").ToString.ToUpper = "VENDOR" Then
                        UsrVeh.Items.Add(ds.Tables("vemei").Rows(i).Item(1).ToString())
                    Else
                        UsrVeh.Items.Add(ds.Tables("vemei").Rows(i).Item(1).ToString() & "/" & ds.Tables("vemei").Rows(i).Item(2).ToString())
                    End If
                    UsrVeh.Items(i).Value = ds.Tables("vemei").Rows(i).Item(0).ToString()
                Next
            Catch ex As Exception
            Finally
                con.Close()
                oda.Dispose()
                con.Dispose()
            End Try
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
    Protected Sub btnshow_Click(sender As Object, e As ImageClickEventArgs) Handles btnshow.Click
        Show()
        UpdatePanel1.Update()
    End Sub
    Protected Sub Show()
        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim con As SqlConnection = New SqlConnection(conStr)
        Dim oda As SqlDataAdapter = New SqlDataAdapter("", con)
        Dim dt As New DataTable
        'Dim dt1 As New DataTable
        Dim status As String = ""
        If CDate(Left(txtsdate.Text, 10)) > CDate(txtedate.Text) Then
            lblmsg.Text = "Start Date should not be greater than from End Date."
            Exit Sub
        End If
        If CDate(txtsdate.Text) < CDate("2014-12-19") Then
            lblmsg.Text = "Data for this report is available from 19-Dec-2014 Onward."
            Exit Sub
        End If
        Try
            Dim Imieno As String = ""
            Dim tbl As New DataTable
            tbl.Columns.Add("IMEI NO.", GetType(String))
            tbl.Columns.Add("Vehicle No.", GetType(String))
            tbl.Columns.Add("Date From", GetType(String))
            tbl.Columns.Add("Date To", GetType(String))
            '  tbl.Columns.Add("Total Time", GetType(String))
            For i As Integer = 0 To UsrVeh.Items.Count - 1
                If UsrVeh.Items(i).Selected = True Then
                    Imieno = UsrVeh.Items(i).Value
                    Dim STR As String() = Split(UsrVeh.Items(i).ToString, "/")
                    oda.SelectCommand.CommandText = "select count(*) from MMM_MST_GPSDATA  with(nolock) where   imieno='" & Imieno & "' and convert(date,ctime)>='" & txtsdate.Text.ToString & "'  and convert(date,ctime)<='" & txtedate.Text.ToString & "' and extvolt<1000"
                    dt.Clear()
                    oda.Fill(dt)
                    If dt.Rows(0).Item(0) < 1 Then
                        Continue For
                    Else
                        oda.SelectCommand.CommandText = "select extvolt,ctime,imieno from MMM_MST_GPSDATA  with(nolock) where   imieno='" & Imieno & "' and convert(date,ctime)>='" & txtsdate.Text.ToString & "'  and convert(date,ctime)<='" & txtedate.Text.ToString & "' order by ctime"
                        oda.SelectCommand.CommandType = CommandType.Text
                        oda.SelectCommand.CommandTimeout = 180
                        dt.Clear()
                        oda.Fill(dt)
                        Dim IMEI As String = ""
                        Dim Vehicle As String = ""
                        Dim frdate As String = ""
                        Dim todate As String = ""
                        Dim Totaltime As Integer = 0
                        Dim extvolt As Integer
                        Dim flag As Integer = 0
                        For j As Integer = 0 To dt.Rows.Count - 1
                            IMEI = Imieno
                            Vehicle = STR(1)
                            extvolt = dt.Rows(j).Item("extvolt")
                            If extvolt < 1000 And flag = 0 Then
                                frdate = dt.Rows(j).Item("ctime").ToString
                                flag = 1
                                Continue For
                            ElseIf extvolt > 1000 And flag = 1 Then
                                todate = dt.Rows(j).Item("ctime").ToString
                                'Totaltime = todate.Subtract(frdate).TotalDays
                                flag = 0
                            ElseIf extvolt < 1000 And flag = 1 Then
                                todate = dt.Rows(j).Item("ctime").ToString
                                'Totaltime = todate.Subtract(frdate).TotalDays
                                Continue For
                            End If
                            Dim rw As DataRow
                            rw = tbl.NewRow
                            rw(0) = IMEI
                            rw(1) = Vehicle
                            rw(2) = frdate
                            rw(3) = todate
                            ' rw(4) = Totaltime
                            'If tbl.Rows.Count > 0 Then
                            If frdate <> "" And todate <> "" Then
                                tbl.Rows.Add(rw)
                                frdate = ""
                                todate = ""
                            End If
                        Next
                    End If
                End If
            Next
            If tbl.Rows.Count > 1 Then
                GVReport.DataSource = tbl
                GVReport.DataBind()
                lblmsg.Text = ""
            Else
                lblmsg.Text = "No Records Found."
            End If
        Catch ex As Exception
        Finally
            If Not con Is Nothing Then
                oda.Dispose()
                con.Dispose()
            End If
            If Not oda Is Nothing Then
                oda.Dispose()
            End If
        End Try
    End Sub
    Protected Sub ToPdf(ByVal newDataSet As DataSet)
        Dim PDFData As New System.IO.MemoryStream()
        Dim newDocument As New iTextSharp.text.Document(PageSize.A4.Rotate(), 10, 10, 10, 10)
        Dim newPdfWriter As iTextSharp.text.pdf.PdfWriter = iTextSharp.text.pdf.PdfWriter.GetInstance(newDocument, PDFData)
        newDocument.Open()
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
            newDocument.Add(newPdfTable)
            newDocument.Add(New Phrase(Environment.NewLine))
            newDocument.Add(New Phrase(Environment.NewLine))
            newDocument.Add(New Phrase(Environment.NewLine))
            newDocument.Add(New Phrase(Environment.NewLine))
            'If Page < newDataSet.Tables.Count Then
            '    newDocument.NewPage()
            'End If
        Next
        newDocument.Add(New Phrase("Created By: " & Session("USERNAME"), FontFactory.GetFont("Tahoma", 8, iTextSharp.text.Font.BOLD, New iTextSharp.text.BaseColor(80, 80, 80))))
        newDocument.Add(New Phrase(Environment.NewLine))
        newDocument.Add(New Phrase("Printed Date: " & DateTime.Now.ToString(), FontFactory.GetFont("Tahoma", 8, iTextSharp.text.Font.BOLD, New iTextSharp.text.BaseColor(80, 80, 80))))
        newDocument.Close()
        Response.ContentType = "application/pdf"
        Response.Cache.SetCacheability(System.Web.HttpCacheability.[Public])
        Response.AppendHeader("Content-Type", "application/pdf")
        Response.AppendHeader("Content-Disposition", "attachment; filename=GpsDataReport")
        Response.OutputStream.Write(PDFData.GetBuffer(), 0, PDFData.GetBuffer().Length)
        Response.OutputStream.Flush()
        Response.OutputStream.Close()
    End Sub
    Public Overloads Overrides Sub VerifyRenderingInServerForm(ByVal control As Control)
    End Sub
    Protected Sub gvReport_PageIndexChanging(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.GridViewPageEventArgs) Handles GVReport.PageIndexChanging
        GVReport.PageIndex = e.NewPageIndex
    End Sub
    Protected Sub gvReport1_PageIndexChanging(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.GridViewPageEventArgs) Handles GVReport1.PageIndexChanging
        GVReport1.PageIndex = e.NewPageIndex
    End Sub
    Protected Sub checkuncheck(sender As Object, e As System.EventArgs) Handles CheckBox1.CheckedChanged
        If CheckBox1.Checked = True Then

            For Each chkitem As System.Web.UI.WebControls.ListItem In UsrVeh.Items
                chkitem.Selected = True
            Next
            'For Each smsreport As System.Web.UI.WebControls.ListItem In smsreports.Items
            '    smsreport.Selected = True
            'Next
            'UpdatePanel1.Update()
        Else
            For Each chkitem As System.Web.UI.WebControls.ListItem In UsrVeh.Items
                chkitem.Selected = False
            Next
            'For Each smsreport As System.Web.UI.WebControls.ListItem In smsreports.Items
            '    smsreport.Selected = False
            'Next
        End If
        UpdatePanel1.Update()
    End Sub
    Protected Sub btnExportPDF_Click(sender As Object, e As ImageClickEventArgs) Handles btnExportPDF.Click
        ToPdf(ViewState("pdf"))
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
End Class
