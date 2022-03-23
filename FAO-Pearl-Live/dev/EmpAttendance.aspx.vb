Imports System.Data
Imports System.Data.SqlClient
Imports System.Web.UI.DataVisualization.Charting.SeriesChartType
Imports System.Web.UI.DataVisualization.Charting
Imports System.IO
Imports System.Web.UI.Adapters.ControlAdapter
Imports iTextSharp.text
Imports System.Drawing
Imports System.Web.Services
Imports Ionic.Zip
Imports Microsoft.Office.Interop
Imports System.Web.Hosting
Imports System.Globalization
Imports iTextSharp.text.pdf

Partial Class EmpAttendance
    Inherits System.Web.UI.Page

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

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Not IsPostBack Then
            ddlLocation.Visible = False
            '  lblLoc.Visible = False
            chkLoc.Visible = False
            ' div1.Visible = False
        End If
    End Sub
    Public Sub BindGrid()
        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim con As New SqlConnection(conStr)
        Dim oda As SqlDataAdapter = New SqlDataAdapter("", con)
        oda.SelectCommand.CommandType = CommandType.Text
        Dim qry As String = ""
        Try

            If ddlLocation.SelectedValue.ToString = "26 to 25" Then
                If ddlPeriod.SelectedItem.Text = "First" Then
                    If ddlMonth.SelectedItem.Text = "JAN" Or ddlMonth.SelectedItem.Text = "MAR" Or ddlMonth.SelectedItem.Text = "MAY" Or ddlMonth.SelectedItem.Text = "JUL" Or ddlMonth.SelectedItem.Text = "AUG" Or ddlMonth.SelectedItem.Text = "OCT" Or ddlMonth.SelectedItem.Text = "DEC" Then
                        qry &= "select distinct m.fld1[Employee Name],m.fld4[Employee Code],dms.udf_split('MASTER-Position Master-fld1',m.FLD7)[Designation],"
                        qry &= "m.fld18[Payroll Code],'Attd.'[ ],'P'[D26],'P'[D27],'P'[D28],'P'[D29],'P'[D30],'P'[D31],'P'[D1],'P'[D2],'P'[D3],'P'[D4],'P'[D5],'P'[D6],'P'[D7],'P'[D8],'P'[D9],'P'[D10], "
                        qry &= "''[CL],''[PL],''[NightCon],''[Parking],''[Outstation Charges],''[Mobile Charges],''[Other Remb.],''[Fule Ded.] from mmm_mst_master m with(nolock) inner join mmm_mst_master d with(nolock) on m.fld19=d.tid "
                        qry &= "where m.eid=103  "
                        qry &= "and m.documenttype='Employee Master' and d.eid=103 and d.fld1='" & ddlLocation.SelectedItem.Text & "' and d.documenttype='job code master' "
                        qry &= " union all  select distinct m.fld1[Employee Name],m.fld4[Employee Code],dms.udf_split('MASTER-Position Master-fld1',m.FLD7)[Designation],"
                        qry &= "m.fld18[Payroll Code],'OT'[Type],''[D26],''[D27],''[D28],''[D29],''[D30],''[D31],''[D1],''[D2],''[D3],''[D4],''[D5],''[D6],''[D7],''[D8],''[D9],''[D10],"
                        qry &= "''[CL],''[PL],''[NightCon],''[Parking],''[Outstation Charges],''[Mobile Charges],''[Other Remb.],''[Fule Ded.] from mmm_mst_master m with(nolock) inner join mmm_mst_master d with(nolock) on m.fld19=d.tid "
                        qry &= "where m.eid=103 "
                        qry &= "and m.documenttype='Employee Master' and d.eid=103 and d.fld1='" & ddlLocation.SelectedItem.Text & "' and d.documenttype='job code master' order by m.fld1 DESC "

                    ElseIf ddlMonth.SelectedItem.Text = "APR" Or ddlMonth.SelectedItem.Text = "JUN" Or ddlMonth.SelectedItem.Text = "SEP" Or ddlMonth.SelectedItem.Text = "NOV" Then
                        qry &= "select distinct m.fld1[Employee Name],m.fld4[Employee Code],dms.udf_split('MASTER-Position Master-fld1',m.FLD7)[Designation],"
                        qry &= "m.fld18[Payroll Code],'Attd.'[ ],'P'[D26],'P'[D27],'P'[D28],'P'[D29],'P'[D30],'P'[D1],'P'[D2],'P'[D3],'P'[D4],'P'[D5],'P'[D6],'P'[D7],'P'[D8],'P'[D9],'P'[D10], "
                        qry &= "''[CL],''[PL],''[NightCon],''[Parking],''[Outstation Charges],''[Mobile Charges],''[Other Remb.],''[Fule Ded.] from mmm_mst_master m with(nolock) inner join mmm_mst_master d with(nolock) on m.fld19=d.tid "
                        qry &= "where m.eid=103  "
                        qry &= "and m.documenttype='Employee Master' and d.eid=103 and d.fld1='" & ddlLocation.SelectedItem.Text & "' and d.documenttype='job code master' "
                        qry &= " union all  select distinct m.fld1[Employee Name],m.fld4[Employee Code],dms.udf_split('MASTER-Position Master-fld1',m.FLD7)[Designation],"
                        qry &= "m.fld18[Payroll Code],'OT'[Type],''[D26],''[D27],''[D28],''[D29],''[D30],''[D1],''[D2],''[D3],''[D4],''[D5],''[D6],''[D7],''[D8],''[D9],''[D10],"
                        qry &= "''[CL],''[PL],''[NightCon],''[Parking],''[Outstation Charges],''[Mobile Charges],''[Other Remb.],''[Fule Ded.] from mmm_mst_master m with(nolock) inner join mmm_mst_master d with(nolock) on m.fld19=d.tid "
                        qry &= "where m.eid=103 "
                        qry &= "and m.documenttype='Employee Master' and d.eid=103 and d.fld1='" & ddlLocation.SelectedItem.Text & "' and d.documenttype='job code master' order by m.fld1 DESC "

                    ElseIf ddlMonth.SelectedItem.Text = "FEB" Then
                        qry &= "select distinct m.fld1[Employee Name],m.fld4[Employee Code],dms.udf_split('MASTER-Position Master-fld1',m.FLD7)[Designation],"
                        qry &= "m.fld18[Payroll Code],'Attd.'[ ],'P'[D26],'P'[D27],'P'[D28],'P'[D29],'P'[D1],'P'[D2],'P'[D3],'P'[D4],'P'[D5],'P'[D6],'P'[D7],'P'[D8],'P'[D9],'P'[D10], "
                        qry &= "''[CL],''[PL],''[NightCon],''[Parking],''[Outstation Charges],''[Mobile Charges],''[Other Remb.],''[Fule Ded.] from mmm_mst_master m with(nolock) inner join mmm_mst_master d with(nolock) on m.fld19=d.tid "
                        qry &= "where m.eid=103  "
                        qry &= "and m.documenttype='Employee Master' and d.eid=103 and d.fld1='" & ddlLocation.SelectedItem.Text & "' and d.documenttype='job code master' "
                        qry &= " union all  select distinct m.fld1[Employee Name],m.fld4[Employee Code],dms.udf_split('MASTER-Position Master-fld1',m.FLD7)[Designation],"
                        qry &= "m.fld18[Payroll Code],'OT'[Type],''[D26],''[D27],''[D28],''[D29],''[D1],''[D2],''[D3],''[D4],''[D5],''[D6],''[D7],''[D8],''[D9],''[D10],"
                        qry &= "''[CL],''[PL],''[NightCon],''[Parking],''[Outstation Charges],''[Mobile Charges],''[Other Remb.],''[Fule Ded.] from mmm_mst_master m with(nolock) inner join mmm_mst_master d with(nolock) on m.fld19=d.tid "
                        qry &= "where m.eid=103 "
                        qry &= "and m.documenttype='Employee Master' and d.eid=103 and d.fld1='" & ddlLocation.SelectedItem.Text & "' and d.documenttype='job code master' order by m.fld1 DESC "


                    End If
                ElseIf ddlPeriod.SelectedItem.Text = "Second" Then

                    qry &= " select distinct m.fld1[Employee Name],m.fld4[Employee Code],dms.udf_split('MASTER-Position Master-fld1',m.FLD7)[Designation],"
                    qry &= "m.fld18[Payroll Code],'OT'[ ],''[D11],''[D12],''[D13],''[D14],''[D15],''[D16],''[D17],''[D18],''[D19],''[D20],''[D21],''[D22],''[D23],''[D24],''[D25], "
                    qry &= "''[CL],''[PL],''[NightCon],''[Parking],''[Outstation Charges],''[Mobile Charges],''[Other Remb.],''[Fule Ded.] from mmm_mst_master m with(nolock) inner join mmm_mst_master d with(nolock) on m.fld19=d.tid "
                    qry &= "where m.eid=103 "
                    qry &= "and m.documenttype='Employee Master' and d.eid=103 and d.fld1='" & ddlLocation.SelectedItem.Text & "' and d.documenttype='job code master' "
                    qry &= "union all  select distinct m.fld1[Employee Name],m.fld4[Employee Code],dms.udf_split('MASTER-Position Master-fld1',m.FLD7)[Designation],"
                    qry &= "m.fld18[Payroll Code],'Attd.','P'[D11],'P'[D12],'P'[D13],'P'[D14],'P'[D15],'P'[D16],'P'[D17],'P'[D18],'P'[D19],'P'[D20],'P'[D21],'P'[D22],'P'[D23],'P'[D24],'P'[D25], "
                    qry &= "''[CL],''[PL],''[NightCon],''[Parking],''[Outstation Charges],''[Mobile Charges],''[Other Remb.],''[Fule Ded.] from mmm_mst_master m with(nolock) inner join mmm_mst_master d with(nolock) on m.fld19=d.tid "
                    qry &= "where m.eid=103  "
                    qry &= "and m.documenttype='Employee Master' and d.eid=103 and d.fld1='" & ddlLocation.SelectedItem.Text & "' and d.documenttype='job code master' order by m.fld1 DESC"

                End If

            ElseIf ddlLocation.SelectedValue.ToString = "1 to 31" Then
                If ddlPeriod.SelectedItem.Text = "First" Then

                    qry &= "select distinct m.fld1[Employee Name],m.fld4[Employee Code],dms.udf_split('MASTER-Position Master-fld1',m.FLD7)[Designation],"
                    qry &= "m.fld18[Payroll Code],'Attd.'[ ],'P'[D1],'P'[D2],'P'[D3],'P'[D4],'P'[D5],'P'[D6],'P'[D7],'P'[D8],'P'[D9],'P'[D10],'P'[D11],'P'[D12],'P'[D13],'P'[D14],'P'[D15], "
                    qry &= "''[CL],''[PL],''[NightCon],''[Parking],''[Outstation Charges],''[Mobile Charges],''[Other Remb.],''[Fule Ded.] from mmm_mst_master m with(nolock) inner join mmm_mst_master d with(nolock) on m.fld19=d.tid "
                    qry &= "where m.eid=103  "
                    qry &= "and m.documenttype='Employee Master' and d.eid=103 and d.fld1='" & ddlLocation.SelectedItem.Text & "' and d.documenttype='job code master' "

                    qry &= "union all select distinct m.fld1[Employee Name],m.fld4[Employee Code],dms.udf_split('MASTER-Position Master-fld1',m.FLD7)[Designation],"
                    qry &= "m.fld18[Payroll Code],'OT',''[D1],''[D2],''[D3],''[D4],''[D5],''[D6],''[D7],''[D8],''[D9],''[D10],''[D11],''[D12],''[D13],''[D14],''[D15], "
                    qry &= "''[CL],''[PL],''[NightCon],''[Parking],''[Outstation Charges],''[Mobile Charges],''[Other Remb.],''[Fule Ded.] from mmm_mst_master m with(nolock) inner join mmm_mst_master d with(nolock) on m.fld19=d.tid "
                    qry &= "where m.eid=103 "
                    qry &= "and m.documenttype='Employee Master' and d.eid=103 and d.fld1='" & ddlLocation.SelectedItem.Text & "' and d.documenttype='job code master' order by m.fld1 DESC "


                ElseIf ddlPeriod.SelectedItem.Text = "Second" Then

                    If ddlMonth.SelectedItem.Text = "JAN" Or ddlMonth.SelectedItem.Text = "MAR" Or ddlMonth.SelectedItem.Text = "MAY" Or ddlMonth.SelectedItem.Text = "JUL" Or ddlMonth.SelectedItem.Text = "AUG" Or ddlMonth.SelectedItem.Text = "OCT" Or ddlMonth.SelectedItem.Text = "DEC" Then
                        qry &= " select distinct m.fld1[Employee Name],m.fld4[Employee Code],dms.udf_split('MASTER-Position Master-fld1',m.FLD7)[Designation],"
                        qry &= "m.fld18[Payroll Code],'Attd.'[ ],'P'[D16],'P'[D17],'P'[D18],'P'[D19],'P'[D20],'P'[D21],'P'[D22],'P'[D23],'P'[D24],'P'[D25],'P'[D26],'P'[D27],'P'[D28],'P'[D29],'P'[D30],'P'[D31], "
                        qry &= "''[CL],''[PL],''[NightCon],''[Parking],''[Outstation Charges],''[Mobile Charges],''[Other Remb.],''[Fule Ded.] from mmm_mst_master m with(nolock) inner join mmm_mst_master d with(nolock) on m.fld19=d.tid "
                        qry &= "where m.eid=103  "
                        qry &= "and m.documenttype='Employee Master' and d.eid=103 and d.fld1='" & ddlLocation.SelectedItem.Text & "' and d.documenttype='job code master' "

                        qry &= " union all select distinct m.fld1[Employee Name],m.fld4[Employee Code],dms.udf_split('MASTER-Position Master-fld1',m.FLD7)[Designation],"
                        qry &= "m.fld18[Payroll Code],'OT',''[D16],''[D17],''[D18],''[D19],''[D20],''[D21],''[D22],''[D23],''[D24],''[D25],''[D26],''[D27],''[D28],''[D29],''[D30],''[D31], "
                        qry &= "''[CL],''[PL],''[NightCon],''[Parking],''[Outstation Charges],''[Mobile Charges],''[Other Remb.],''[Fule Ded.] from mmm_mst_master m with(nolock) inner join mmm_mst_master d with(nolock) on m.fld19=d.tid "
                        qry &= "where m.eid=103 "
                        qry &= "and m.documenttype='Employee Master' and d.eid=103 and d.fld1='" & ddlLocation.SelectedItem.Text & "' and d.documenttype='job code master' order by m.fld1 DESC"

                    ElseIf ddlMonth.SelectedItem.Text = "APR" Or ddlMonth.SelectedItem.Text = "JUN" Or ddlMonth.SelectedItem.Text = "SEP" Or ddlMonth.SelectedItem.Text = "NOV" Then
                        qry &= " select distinct m.fld1[Employee Name],m.fld4[Employee Code],dms.udf_split('MASTER-Position Master-fld1',m.FLD7)[Designation],"
                        qry &= "m.fld18[Payroll Code],'Attd.'[ ],'P'[D16],'P'[D17],'P'[D18],'P'[D19],'P'[D20],'P'[D21],'P'[D22],'P'[D23],'P'[D24],'P'[D25],'P'[D26],'P'[D27],'P'[D28],'P'[D29],'P'[D30], "
                        qry &= "''[CL],''[PL],''[NightCon],''[Parking],''[Outstation Charges],''[Mobile Charges],''[Other Remb.],''[Fule Ded.] from mmm_mst_master m with(nolock) inner join mmm_mst_master d with(nolock) on m.fld19=d.tid "
                        qry &= "where m.eid=103  "
                        qry &= "and m.documenttype='Employee Master' and d.eid=103 and d.fld1='" & ddlLocation.SelectedItem.Text & "' and d.documenttype='job code master' "

                        qry &= " union all select distinct m.fld1[Employee Name],m.fld4[Employee Code],dms.udf_split('MASTER-Position Master-fld1',m.FLD7)[Designation],"
                        qry &= "m.fld18[Payroll Code],'OT',''[D16],''[D17],''[D18],''[D19],''[D20],''[D21],''[D22],''[D23],''[D24],''[D25],''[D26],''[D27],''[D28],''[D29],''[D30], "
                        qry &= "''[CL],''[PL],''[NightCon],''[Parking],''[Outstation Charges],''[Mobile Charges],''[Other Remb.],''[Fule Ded.] from mmm_mst_master m with(nolock) inner join mmm_mst_master d with(nolock) on m.fld19=d.tid "
                        qry &= "where m.eid=103 "
                        qry &= "and m.documenttype='Employee Master' and d.eid=103 and d.fld1='" & ddlLocation.SelectedItem.Text & "' and d.documenttype='job code master' order by m.fld1 DESC"
                    ElseIf ddlMonth.SelectedItem.Text = "FEB" Then
                        qry &= " select distinct m.fld1[Employee Name],m.fld4[Employee Code],dms.udf_split('MASTER-Position Master-fld1',m.FLD7)[Designation],"
                        qry &= "m.fld18[Payroll Code],'Attd.'[ ],'P'[D16],'P'[D17],'P'[D18],'P'[D19],'P'[D20],'P'[D21],'P'[D22],'P'[D23],'P'[D24],'P'[D25],'P'[D26],'P'[D27],'P'[D28],'P'[D29], "
                        qry &= "''[CL],''[PL],''[NightCon],''[Parking],''[Outstation Charges],''[Mobile Charges],''[Other Remb.],''[Fule Ded.] from mmm_mst_master m with(nolock) inner join mmm_mst_master d with(nolock) on m.fld19=d.tid "
                        qry &= "where m.eid=103  "
                        qry &= "and m.documenttype='Employee Master' and d.eid=103 and d.fld1='" & ddlLocation.SelectedItem.Text & "' and d.documenttype='job code master' "

                        qry &= " union all select distinct m.fld1[Employee Name],m.fld4[Employee Code],dms.udf_split('MASTER-Position Master-fld1',m.FLD7)[Designation],"
                        qry &= "m.fld18[Payroll Code],'OT',''[D16],''[D17],''[D18],''[D19],''[D20],''[D21],''[D22],''[D23],''[D24],''[D25],''[D26],''[D27],''[D28],''[D29], "
                        qry &= "''[CL],''[PL],''[NightCon],''[Parking],''[Outstation Charges],''[Mobile Charges],''[Other Remb.],''[Fule Ded.] from mmm_mst_master m with(nolock) inner join mmm_mst_master d with(nolock) on m.fld19=d.tid "
                        qry &= "where m.eid=103 "
                        qry &= "and m.documenttype='Employee Master' and d.eid=103 and d.fld1='" & ddlLocation.SelectedItem.Text & "' and d.documenttype='job code master' order by m.fld1 DESC"

                    End If

                    
                End If

            End If

            oda.SelectCommand.CommandText = qry
            Dim ds As New DataSet
            oda.Fill(ds, "data")
            gvPending.DataSource = ds.Tables("data")
            gvPending.DataBind()
            ViewState("xlexport") = ds.Tables("data")
            ViewState("PDF") = ds
            Dim qrySvr As String = ""
            qrySvr = "select top 1 fld1[Svisor] from mmm_mst_master where eid='" & Session("EID") & "' and documenttype='Supervisor Master' and fld2='" & ddlLocation.SelectedValue.ToString & "'"
            oda.SelectCommand.CommandText = qrySvr
            Dim ds1 As New DataSet
            oda.Fill(ds1, "Svr")
            ViewState("Svr") = ds1.Tables("Svr").Rows(0).Item(0).ToString

        Catch ex As Exception

        Finally
            If Not con Is Nothing Then
                con.Close()
                oda.Dispose()
                con.Dispose()
            End If
            If Not oda Is Nothing Then
                oda.Dispose()
            End If
        End Try
    End Sub
    
    Protected Sub btnSearch_Click(sender As Object, ByVal e As System.EventArgs) Handles btnSearch.Click
        If ddlRtype.SelectedItem.Text = "Attendance" Then
            BindGrid()
        ElseIf ddlRtype.SelectedItem.Text = "Send on Email" Then

        ElseIf ddlRtype.SelectedItem.Text = "" Then

        End If

    End Sub

    Protected Sub gvPending_DataBound(sender As Object, e As System.EventArgs) Handles gvPending.DataBound
        For rowIndex As Integer = gvPending.Rows.Count - 2 To 0 Step -1
            Dim gviewRow As GridViewRow = gvPending.Rows(rowIndex)


            Dim gviewPreviousRow As GridViewRow = gvPending.Rows(rowIndex + 1)
            If ddlLocation.SelectedValue.ToString = "26 to 25" Then
                If ddlPeriod.SelectedItem.Text = "First" Then
                    If ddlMonth.SelectedItem.Text = "JAN" Or ddlMonth.SelectedItem.Text = "MAR" Or ddlMonth.SelectedItem.Text = "MAY" Or ddlMonth.SelectedItem.Text = "JUL" Or ddlMonth.SelectedItem.Text = "AUG" Or ddlMonth.SelectedItem.Text = "OCT" Or ddlMonth.SelectedItem.Text = "DEC" Then
                        For cellCount As Integer = 0 To gviewRow.Cells.Count - 26
                            If gviewRow.Cells(0).Text = gviewPreviousRow.Cells(0).Text And gviewRow.Cells(1).Text = gviewPreviousRow.Cells(1).Text And gviewRow.Cells(2).Text = gviewPreviousRow.Cells(2).Text Then
                                If gviewPreviousRow.Cells(cellCount).RowSpan < 2 Then
                                    gviewRow.Cells(cellCount).RowSpan = 2
                                Else
                                    gviewRow.Cells(cellCount).RowSpan = gviewPreviousRow.Cells(cellCount).RowSpan + 1
                                End If
                                gviewPreviousRow.Cells(cellCount).Visible = False
                            End If
                            ' End If
                        Next
                    ElseIf ddlMonth.SelectedItem.Text = "APR" Or ddlMonth.SelectedItem.Text = "JUN" Or ddlMonth.SelectedItem.Text = "SEP" Or ddlMonth.SelectedItem.Text = "NOV" Then

                        For cellCount As Integer = 0 To gviewRow.Cells.Count - 25
                            If gviewRow.Cells(0).Text = gviewPreviousRow.Cells(0).Text And gviewRow.Cells(1).Text = gviewPreviousRow.Cells(1).Text And gviewRow.Cells(2).Text = gviewPreviousRow.Cells(2).Text Then
                                If gviewPreviousRow.Cells(cellCount).RowSpan < 2 Then
                                    gviewRow.Cells(cellCount).RowSpan = 2
                                Else
                                    gviewRow.Cells(cellCount).RowSpan = gviewPreviousRow.Cells(cellCount).RowSpan + 1
                                End If
                                gviewPreviousRow.Cells(cellCount).Visible = False
                            End If
                            ' End If
                        Next
                    ElseIf ddlMonth.SelectedItem.Text = "FEB" Then
                        For cellCount As Integer = 0 To gviewRow.Cells.Count - 24
                            If gviewRow.Cells(0).Text = gviewPreviousRow.Cells(0).Text And gviewRow.Cells(1).Text = gviewPreviousRow.Cells(1).Text And gviewRow.Cells(2).Text = gviewPreviousRow.Cells(2).Text Then
                                If gviewPreviousRow.Cells(cellCount).RowSpan < 2 Then
                                    gviewRow.Cells(cellCount).RowSpan = 2
                                Else
                                    gviewRow.Cells(cellCount).RowSpan = gviewPreviousRow.Cells(cellCount).RowSpan + 1
                                End If
                                gviewPreviousRow.Cells(cellCount).Visible = False
                            End If
                            ' End If
                        Next

                    End If
                    
                ElseIf ddlPeriod.SelectedItem.Text = "Second" Then
                    For cellCount As Integer = 0 To gviewRow.Cells.Count - 25
                        If gviewRow.Cells(0).Text = gviewPreviousRow.Cells(0).Text And gviewRow.Cells(1).Text = gviewPreviousRow.Cells(1).Text And gviewRow.Cells(2).Text = gviewPreviousRow.Cells(2).Text Then
                            If gviewPreviousRow.Cells(cellCount).RowSpan < 2 Then
                                gviewRow.Cells(cellCount).RowSpan = 2
                            Else
                                gviewRow.Cells(cellCount).RowSpan = gviewPreviousRow.Cells(cellCount).RowSpan + 1
                            End If
                            gviewPreviousRow.Cells(cellCount).Visible = False
                        End If
                        ' End If
                    Next
                End If

            End If
            If ddlLocation.SelectedValue.ToString = "1 to 31" Then
                If ddlPeriod.SelectedItem.Text = "First" Then
                    For cellCount As Integer = 0 To gviewRow.Cells.Count - 25
                        If gviewRow.Cells(0).Text = gviewPreviousRow.Cells(0).Text And gviewRow.Cells(1).Text = gviewPreviousRow.Cells(1).Text And gviewRow.Cells(2).Text = gviewPreviousRow.Cells(2).Text Then
                            If gviewPreviousRow.Cells(cellCount).RowSpan < 2 Then
                                gviewRow.Cells(cellCount).RowSpan = 2
                            Else
                                gviewRow.Cells(cellCount).RowSpan = gviewPreviousRow.Cells(cellCount).RowSpan + 1
                            End If
                            gviewPreviousRow.Cells(cellCount).Visible = False
                        End If
                        ' End If
                    Next
                ElseIf ddlPeriod.SelectedItem.Text = "Second" Then
                    For cellCount As Integer = 0 To gviewRow.Cells.Count - 26
                        If gviewRow.Cells(0).Text = gviewPreviousRow.Cells(0).Text And gviewRow.Cells(1).Text = gviewPreviousRow.Cells(1).Text And gviewRow.Cells(2).Text = gviewPreviousRow.Cells(2).Text Then
                            If gviewPreviousRow.Cells(cellCount).RowSpan < 2 Then
                                gviewRow.Cells(cellCount).RowSpan = 2
                            Else
                                gviewRow.Cells(cellCount).RowSpan = gviewPreviousRow.Cells(cellCount).RowSpan + 1
                            End If
                            gviewPreviousRow.Cells(cellCount).Visible = False
                        End If
                        ' End If
                    Next
                End If

            End If
           
        Next
    End Sub

    Protected Sub gvPending_PageIndexChanging(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.GridViewPageEventArgs) Handles gvPending.PageIndexChanging
        Try
            gvPending.PageIndex = e.NewPageIndex
            currentPageNumberp = e.NewPageIndex + 1
            BindGrid()
        Catch ex As Exception
        End Try
    End Sub
    Private Const ASCENDING As String = " ASC"
    Private Const DESCENDING As String = " DESC"
    Public Property GridViewSortDirection As SortDirection
        Get
            If Val(ViewState("sortDirection")) = Val(DBNull.Value.ToString) Then
                ViewState("sortDirection") = SortDirection.Ascending
            End If
            Return CType(ViewState("sortDirection"), SortDirection)
        End Get
        Set(ByVal value As SortDirection)
            ViewState("sortDirection") = value
        End Set
    End Property
    Protected currentPageNumber As Integer = 1
    Protected currentPageNumberu As Integer = 1
    Protected currentPageNumberp As Integer = 1
    Protected Sub ChangePagep(ByVal sender As Object, ByVal e As CommandEventArgs)
        Select Case e.CommandName
            Case "Previous"
                currentPageNumberp = Int32.Parse(ViewState("cpnp")) - 1
                If gvPending.PageIndex > 0 Then
                    gvPending.PageIndex = gvPending.PageIndex - 1
                End If
                Exit Select
            Case "Next"
                currentPageNumberp = Int32.Parse(ViewState("cpnp")) + 1
                gvPending.PageIndex = gvPending.PageIndex + 1
                Exit Select
        End Select
    End Sub
    Private Sub SortGridView(ByVal sortExpression As String, ByVal direction As String)
        'You can cache the DataTable for improving performance
        Dim dt As DataTable = CType(ViewState("pending"), DataTable)
        dt.DefaultView.Sort = sortExpression + direction
        Dim dt1 As DataTable = dt.DefaultView.ToTable()
        gvPending.DataSource = ViewState("pending")
        gvPending.DataBind()

        'BindGrid()
        'AdvSearch()
        'ViewState("data") = dt1
    End Sub
   
    Protected Sub gvPending_Sorting(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.GridViewSortEventArgs) Handles gvPending.Sorting
        Dim sortExpression As String = e.SortExpression
        If (GridViewSortDirection = SortDirection.Ascending) Then
            GridViewSortDirection = SortDirection.Descending
            SortGridView(sortExpression, DESCENDING)
        Else
            GridViewSortDirection = SortDirection.Ascending
            SortGridView(sortExpression, ASCENDING)
        End If
    End Sub
    Protected Sub btnexportxl_Click(ByVal sender As Object, ByVal e As System.Web.UI.ImageClickEventArgs) Handles btnexportxl.Click

        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim con As New SqlConnection(conStr)
        Dim oda As SqlDataAdapter = New SqlDataAdapter("", con)
        Dim ds As New DataSet
        oda.SelectCommand.CommandType = CommandType.Text
        Dim Jcode As String = ""
        Dim location As String = ""
        Dim loccode As String = ""
        Dim site As String = ""
        oda.SelectCommand.CommandText = " select top 1 fld1[JobCode],dms.udf_split('MASTER-Location Master-fld1',fld4)[Location],fld2[site] from MMM_MST_master where documenttype='Job Code Master'  and eid=" & Session("EID") & " and fld1='" & ddlLocation.SelectedItem.ToString & "'"
        oda.Fill(ds, "Jcode")
        If ds.Tables("Jcode").Rows.Count > 0 Then
            Jcode = ds.Tables("Jcode").Rows(0).Item("JobCode").ToString
            location = ds.Tables("Jcode").Rows(0).Item("Location").ToString
            site = ds.Tables("Jcode").Rows(0).Item("site").ToString
        End If
        oda.SelectCommand.CommandText = "select top 1 fld2 from MMM_MST_master where documenttype='Location Master' and eid=" & Session("EID") & " and fld1='" & location & "' order by tid desc"
        oda.Fill(ds, "LocCode")
        If ds.Tables("LocCode").Rows.Count > 0 Then
            loccode = ds.Tables("locCode").Rows(0).Item(0).ToString
        End If

        Response.Clear()
        Response.Buffer = True
        ' Dim location As String = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(ddlLocation.SelectedItem.Text.ToLower)
        Dim Period As String = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(ddlMonth.SelectedItem.Text.ToLower & "-" & ddlYear.SelectedItem.Text.ToLower & "-" & ddlPeriod.SelectedItem.Text.ToLower)
        'Dim Svr As String = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(ViewState("Svr").ToString.ToLower)
        Response.Write("<br/><br/><div align=""left"" style=""border:1px solid black"" ><h3>For the month of: " & Period & "<br/>Location:" & location & "<br/>Location Code:" & loccode & "<br/>Job Code:" & Jcode & "<br/>Site Name:" & site & "<br/></h3></div>")
        Response.AddHeader("content-disposition", "attachment;filename=AttendanceRota.xls")
        Response.Charset = ""
        Response.ContentType = "application/vnd.ms-excel"
        Using sw As New StringWriter()
            Dim hw As New HtmlTextWriter(sw)

            'To Export all pages
            gvPending.AllowPaging = False
            Me.BindGrid()

            gvPending.HeaderRow.BackColor = Color.White
            For Each cell As TableCell In gvPending.HeaderRow.Cells
                cell.BackColor = gvPending.HeaderStyle.BackColor
            Next
            For Each row As GridViewRow In gvPending.Rows
                row.BackColor = Color.White
                For Each cell As TableCell In row.Cells
                    If row.RowIndex Mod 2 = 0 Then
                        cell.BackColor = gvPending.AlternatingRowStyle.BackColor

                    Else
                        cell.BackColor = gvPending.RowStyle.BackColor
                    End If
                    cell.CssClass = "textmode"

                Next
            Next

            gvPending.RenderControl(hw)
            'style to format numbers to string
            Dim style As String = "<style> .textmode { } </style>"
            Response.Write(style)
            Response.Output.Write(sw.ToString())
            Response.Flush()
            Response.[End]()
        End Using

        oda.Dispose()
        con.Close()

    End Sub

    Protected Sub ToPdf(ByVal newDataSet As DataSet)
        Dim PDFData As New System.IO.MemoryStream()
        Dim newDocument As New iTextSharp.text.Document(PageSize.A4.Rotate(), 10, 10, 10, 10)
        Dim newPdfWriter As iTextSharp.text.pdf.PdfWriter = iTextSharp.text.pdf.PdfWriter.GetInstance(newDocument, PDFData)
        newDocument.Open()
        For Page As Integer = 0 To newDataSet.Tables.Count - 1
            Dim totalColumns As Integer = newDataSet.Tables(Page).Columns.Count
            Dim newPdfTable As New iTextSharp.text.pdf.PdfPTable(totalColumns)
            newPdfTable.DefaultCell.Padding = 1

            newPdfTable.DefaultCell.HorizontalAlignment = iTextSharp.text.Element.ALIGN_CENTER
            newPdfTable.DefaultCell.VerticalAlignment = iTextSharp.text.Element.ALIGN_CENTER
            newPdfTable.HeaderRows = 1
            newPdfTable.DefaultCell.BorderWidth = 0.2
            newPdfTable.DefaultCell.BorderColor = New iTextSharp.text.BaseColor(0, 0, 0)
            newPdfTable.DefaultCell.BackgroundColor = New iTextSharp.text.BaseColor(255, 255, 255)
            'newPdfTable.DefaultCell.Column.FilledWidth = 20
            For i As Integer = 0 To totalColumns - 1
                newPdfTable.AddCell(New Phrase(newDataSet.Tables(Page).Columns(i).ColumnName, FontFactory.GetFont("Tahoma", 6, iTextSharp.text.Font.BOLD, New iTextSharp.text.BaseColor(0, 0, 0))))
                 newPdfTable.WidthPercentage = 1020

            Next
            For Each record As DataRow In newDataSet.Tables(Page).Rows
                For i As Integer = 0 To totalColumns - 1
                    newPdfTable.DefaultCell.BorderColor = New iTextSharp.text.BaseColor(0, 0, 0)
                    newPdfTable.AddCell(New Phrase(record(i).ToString, FontFactory.GetFont("Tahoma", 6, iTextSharp.text.Font.NORMAL, New iTextSharp.text.BaseColor(0, 0, 0))))
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
        newDocument.Add(New Phrase("Location:" & ddlLocation.SelectedItem.Text, FontFactory.GetFont("Tahoma", 8, iTextSharp.text.Font.BOLD, New iTextSharp.text.BaseColor(80, 80, 80))))
        newDocument.Add(New Phrase("  Supervisor:" & ViewState("Svr"), FontFactory.GetFont("Tahoma", 8, iTextSharp.text.Font.BOLD, New iTextSharp.text.BaseColor(80, 80, 80))))
        newDocument.Add(New Phrase("  Month:" & ddlMonth.SelectedItem.Text & "   Year:" & ddlYear.SelectedItem.Text & "  Period" & ddlPeriod.SelectedItem.Text, FontFactory.GetFont("Tahoma", 8, iTextSharp.text.Font.BOLD, New iTextSharp.text.BaseColor(80, 80, 80))))
        newDocument.Close()
        Response.ContentType = "application/pdf"
        Response.Cache.SetCacheability(System.Web.HttpCacheability.[Public])
        Response.AppendHeader("Content-Type", "application/pdf")
        Response.AppendHeader("Content-Disposition", "attachment; filename=AttendanceReport.pdf")
        Response.OutputStream.Write(PDFData.GetBuffer(), 0, PDFData.GetBuffer().Length)
        Response.OutputStream.Flush()
        Response.OutputStream.Close()
    End Sub

    Protected Sub btnExport_Click(sender As Object, e As System.Web.UI.ImageClickEventArgs) Handles btnExport.Click
        ToPdf(ViewState("PDF"))
    End Sub
    Public Sub ddlLoc()
        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim con As New SqlConnection(conStr)
        Dim da As New SqlDataAdapter("select tid,fld1[Job Code],fld15[Period] from mmm_mst_master with(nolock) where eid='" & Session("EID") & "' and documenttype='Job Code Master'", con)
        Try
            Dim ds As New DataSet
            da.Fill(ds, "data")
            ddlLocation.DataSource = ds
            ddlLocation.DataTextField = "Job Code"
            ddlLocation.DataValueField = "Period"
            ddlLocation.DataBind()
            ' ddlLocation.Items.Insert(0, "--Please Select--")
        Catch ex As Exception
        Finally
            If Not con Is Nothing Then
                con.Close()
                da.Dispose()
                con.Dispose()
            End If
            If Not da Is Nothing Then
                da.Dispose()
            End If
        End Try
    End Sub
    

    Protected Sub ddlRtype_SelectedIndexChanged(sender As Object, e As System.EventArgs) Handles ddlRtype.SelectedIndexChanged
        If ddlRtype.SelectedItem.Text = "Attendance" Then
            ddlLocation.Visible = True
            '   lblLoc.Visible = True
            chkLoc.Visible = False
            ddlLoc()
        ElseIf ddlRtype.SelectedItem.Text = "Details" Then
            ddlLocation.Visible = False
            '  lblLoc.Visible = False
            chkLoc.Visible = True

        End If
    End Sub


    Public Overloads Overrides Sub VerifyRenderingInServerForm(ByVal control As Control)

    End Sub
End Class
