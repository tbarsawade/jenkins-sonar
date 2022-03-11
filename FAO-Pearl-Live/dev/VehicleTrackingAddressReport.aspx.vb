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
    Protected Sub btnshow_Click(sender As Object, e As ImageClickEventArgs) Handles btnshow.Click
        Show()
        UpdatePanel1.Update()
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
    Protected Sub Show()
        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim con As SqlConnection = New SqlConnection(conStr)
        Dim oda As SqlDataAdapter = New SqlDataAdapter("", con)
        Dim dtt As New DataTable
        Dim dt As New DataTable
        'Dim dt1 As New DataTable
        Dim status As String = ""
        If CDate(Left(txtsdate.Text, 10)) > CDate(txtedate.Text) Then
            lblmsg.Text = "Start Date should not be greater than from End Date."
            Exit Sub
        End If
        Dim DateStart As Date = CType(txtsdate.Text, Date)
        Dim DateEnd As Date = CType(txtedate.Text, Date)

        Try
            Dim Imieno As String = ""
            Dim cnt As Integer = 1
            '  tbl.Columns.Add("Total Time", GetType(String))
            Dim lo As New GPSClass
            For i As Integer = 0 To UsrVeh.Items.Count - 1
                If UsrVeh.Items(i).Selected = True Then
                    Imieno = UsrVeh.Items(i).Value
                    Dim STR As String() = Split(UsrVeh.Items(i).ToString, "/")


                    oda.SelectCommand.CommandText = " select max(convert(varchar,ctime,22))[Date Time], max(lattitude)lat,max(longitude)long ,'" & STR(1).ToString & "'[Vehicleno] ,CAST( (sum(devdist)) as decimal(10,2) )distance from MMM_MST_GPSDATA where imieno='" & UsrVeh.SelectedValue.ToString & "' and ctime between ('" & txtsdate.Text & "')  and '" & txtedate.Text & "' group by DATEADD(MINUTE, DATEDIFF(MINUTE, 0, ctime) / 15 * 15, 0),IMIENO order by IMIENO"
                    '"select max(convert(varchar,ctime,22))[Date Time],max(lattitude)lat,max(longitude)long,'" & STR(1).ToString & "'[Vehicleno] from mmm_mst_gpsdata with(nolock) where imieno='" & UsrVeh.SelectedValue.ToString & "' and DATEADD(minute,DATEDIFF(minute,0,ctime),0)>=DATEADD(minute,DATEDIFF(minute,0,'" & txtsdate.Text & "'),0) and DATEADD(minute,DATEDIFF(minute,0,ctime),0)<=DATEADD(minute,DATEDIFF(minute,0,'" & txtedate.Text & "'),0)  group by CONVERT(decimal(10,3), lattitude), CONVERT(decimal(10,3), longitude),IMIENO order by [Date Time],IMIENO"
                    dtt.Clear()
                    oda.Fill(dtt)
                    If dtt.Rows.Count > 0 Then
                        If cnt = 1 Then
                            dtt.Columns.Add("Location", Type.GetType("System.String"))
                        End If
                        For j As Integer = 0 To dtt.Rows.Count - 1
                            ' Dim ob = lo.EsriAddress(dtt.Rows(j).Item("lat"), dtt.Rows(j).Item("long"))
                            ' If ob.Success Then
                            dtt.Rows(j).Item("Location") = lo.Location(dtt.Rows(j).Item("lat"), dtt.Rows(j).Item("long"))
                            'End If
                        Next
                        dtt.Columns.Remove("lat")
                        dtt.Columns.Remove("long")
                        dt = dtt
                        cnt = 0
                    End If
                End If
            Next
            If dt.Rows.Count > 0 Then
                ViewState("pdf") = dt
                GVReport.DataSource = dt
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

    Protected Sub btnexportxl_Click(sender As Object, e As ImageClickEventArgs) Handles btnexportxl.Click
        'Show()
        GVReport.AllowPaging = False
        GVReport.DataSource = ViewState("pdf")
        GVReport.DataBind()
        Response.Clear()
        Response.Buffer = True

        Response.Write("<br/><div align=""center"" style=""border:1px solid red"" ><h3>" & "Vehicle Interval Address Report" & "</h3></div> <br/>")
        Response.AddHeader("content-disposition", "attachment;filename=VehicleReport.xls")
        Response.Charset = ""
        Response.ContentType = "application/vnd.ms-excel"
        Dim sw As New StringWriter()
        Dim hw As New HtmlTextWriter(sw)
        For i As Integer = 0 To GVReport.Rows.Count - 1
            'Apply text style to each Row 
            GVReport.Rows(i).Attributes.Add("class", "textmode")
        Next
        GVReport.RenderControl(hw)
        'style to format numbers to string 
        Dim style As String = "<style> .textmode{mso-number-format:\@;}</style>"
        Response.Write(style)
        Response.Output.Write(sw.ToString())
        Response.Flush()
        Response.End()
    End Sub
End Class
