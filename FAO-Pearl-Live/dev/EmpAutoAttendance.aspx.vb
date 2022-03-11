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
Imports System.Web.Hosting
Imports System.Globalization
Imports iTextSharp.text.pdf
Imports System.IO.Compression


Partial Class EmpAutoAttendance
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
            lblMsg.Visible = False
        End If
    End Sub

    Protected Sub btnSearch_Click(sender As Object, ByVal e As System.EventArgs) Handles btnSearch.Click
        If ddlRtype.SelectedItem.Text = "Send Attendance" Then
            sendAttendance()

        End If
    End Sub

    Protected Sub gvPending_DataBound(sender As Object, e As System.EventArgs) Handles gvPending.DataBound
        For rowIndex As Integer = gvPending.Rows.Count - 2 To 0 Step -1
            Dim gviewRow As GridViewRow = gvPending.Rows(rowIndex)
            Dim gviewPreviousRow As GridViewRow = gvPending.Rows(rowIndex + 1)
            If ddlcyc.SelectedItem.Text = "26 TO 25" Then

                If ddlMonth.SelectedItem.Text = "JAN" Or ddlMonth.SelectedItem.Text = "FEB" Or ddlMonth.SelectedItem.Text = "APR" Or ddlMonth.SelectedItem.Text = "JUN" Or ddlMonth.SelectedItem.Text = "AUG" Or ddlMonth.SelectedItem.Text = "SEP" Or ddlMonth.SelectedItem.Text = "NOV" Then
                    For cellCount As Integer = 0 To gviewRow.Cells.Count - 37
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
                ElseIf ddlMonth.SelectedItem.Text = "MAY" Or ddlMonth.SelectedItem.Text = "JUL" Or ddlMonth.SelectedItem.Text = "OCT" Or ddlMonth.SelectedItem.Text = "DEC" Then

                    For cellCount As Integer = 0 To gviewRow.Cells.Count - 35
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
                ElseIf ddlMonth.SelectedItem.Text = "MAR" Then
                    For cellCount As Integer = 0 To gviewRow.Cells.Count - 35
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
            If ddlcyc.SelectedItem.Text = "1 TO 31" Then

                For cellCount As Integer = 0 To gviewRow.Cells.Count - 37
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


            If ddlcyc.SelectedItem.Text = "21 TO 20" Then

                If ddlMonth.SelectedItem.Text = "JAN" Or ddlMonth.SelectedItem.Text = "FEB" Or ddlMonth.SelectedItem.Text = "APR" Or ddlMonth.SelectedItem.Text = "JUN" Or ddlMonth.SelectedItem.Text = "AUG" Or ddlMonth.SelectedItem.Text = "SEP" Or ddlMonth.SelectedItem.Text = "NOV" Then
                    For cellCount As Integer = 0 To gviewRow.Cells.Count - 36
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
                ElseIf ddlMonth.SelectedItem.Text = "MAY" Or ddlMonth.SelectedItem.Text = "JUL" Or ddlMonth.SelectedItem.Text = "OCT" Or ddlMonth.SelectedItem.Text = "DEC" Then

                    For cellCount As Integer = 0 To gviewRow.Cells.Count - 35
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
                ElseIf ddlMonth.SelectedItem.Text = "MAR" Then
                    For cellCount As Integer = 0 To gviewRow.Cells.Count - 35
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

    Public Overloads Overrides Sub VerifyRenderingInServerForm(ByVal control As Control)

    End Sub
    Private Sub sendAttendance()
        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim con As New SqlConnection(conStr)
        Dim oda As SqlDataAdapter = New SqlDataAdapter("", con)
        oda.SelectCommand.CommandType = CommandType.Text
        oda.SelectCommand.CommandText = ""
        Dim cnt As Integer = 0
        Dim prevloc As String = ""
        Dim prevEmail As String = ""
        Dim prevHREmail As String = ""
        Dim prevCoorEmail As String = ""
        'lblMsg.Text = ""
        Try

            Dim cycle As String = ""
            oda.SelectCommand.CommandText = "select d.fld1[Location],d.fld7[Email],d.fld9[HREMAIL],d.fld12[Coordinator],m.fld1[JobCode],m.fld15[Cycle] from mmm_mst_master d inner join mmm_mst_master m on d.tid=m.fld4 and d.eid=103 and m.documenttype='job code master' and d.documenttype='location master' and m.eid=103 and  m.fld15='" & ddlcyc.SelectedItem.Text & "' and d.fld7<>'' ORDER BY d.fld1"
            Dim ds As New DataSet
            oda.Fill(ds, "Cycle")
            ' progressbar.Visible = True
            For i = 0 To ds.Tables("Cycle").Rows.Count - 1
                Dim qry As String = ""
                cycle = ds.Tables("Cycle").Rows(i).Item("cycle").ToString
                ViewState("PERIOD") = cycle
                Dim jobcode As String = ds.Tables("Cycle").Rows(i).Item("Jobcode").ToString
                Dim emailid As String = ds.Tables("Cycle").Rows(i).Item("Email").ToString
                Dim HREMAIL As String = ds.Tables("Cycle").Rows(i).Item("HREMAIL").ToString
                Dim Coordinator As String = ds.Tables("Cycle").Rows(i).Item("Coordinator").ToString
                Dim loccode As String = ds.Tables("Cycle").Rows(i).Item("Location").ToString

                If i <> 0 Then
                    prevloc = ds.Tables("Cycle").Rows(i - 1).Item("Location").ToString
                    prevEmail = ds.Tables("Cycle").Rows(i - 1).Item("Email").ToString
                    prevHREmail = ds.Tables("Cycle").Rows(i - 1).Item("HREMAIL").ToString
                    prevCoorEmail = ds.Tables("Cycle").Rows(i - 1).Item("Coordinator").ToString
                End If

                If Trim(cycle).ToUpper = "26 TO 25" Then
                    If ddlMonth.SelectedItem.Text = "JAN" Or ddlMonth.SelectedItem.Text = "FEB" Or ddlMonth.SelectedItem.Text = "APR" Or ddlMonth.SelectedItem.Text = "JUN" Or ddlMonth.SelectedItem.Text = "AUG" Or ddlMonth.SelectedItem.Text = "SEP" Or ddlMonth.SelectedItem.Text = "NOV" Then

                        qry &= "select distinct m.fld4[Employee Code],m.fld1[Employee Name],dms.udf_split('MASTER-Position Master-fld1',m.FLD7)[Designation],"
                        qry &= "dms.udf_split('MASTER-PayCode Master-fld1',d.fld8)[Payroll Code],'Attd.'[ ],'P'[D26],'P'[D27],'P'[D28],'P'[D29],'P'[D30],'P'[D31],'P'[D1],'P'[D2],'P'[D3],'P'[D4],'P'[D5],'P'[D6],'P'[D7],'P'[D8],'P'[D9],'P'[D10],'P'[D11],'P'[D12],'P'[D13],'P'[D14], "
                        qry &= "'P'[D15],'P'[D16],'P'[D17],'P'[D18],'P'[D19],'P'[D20],'P'[D21],'P'[D22],'P'[D23],'P'[D24],'P'[D25],"
                        qry &= "''[CL],''[PL],''[Total],''[Others] from mmm_mst_master m with(nolock) inner join mmm_mst_master d with(nolock) on m.fld19=d.tid "
                        qry &= "where m.eid=103  "
                        qry &= "and m.documenttype='Employee Master' and m.isauth=1 and d.eid=103 and d.documenttype='job code master' and d.fld1='" & jobcode & "'"
                        qry &= " union all  select distinct m.fld4[Employee Code],m.fld1[Employee Name],dms.udf_split('MASTER-Position Master-fld1',m.FLD7)[Designation],"
                        qry &= "dms.udf_split('MASTER-PayCode Master-fld1',d.fld8)[Payroll Code],'OT'[Type],''[D26],''[D27],''[D28],''[D29],''[D30],''[D31],''[D1],''[D2],''[D3],''[D4],''[D5],''[D6],''[D7],''[D8],''[D9],''[D10],"
                        qry &= " ''[D11],''[D12],''[D13],''[D14],''[D15],''[D16],''[D17],''[D18],''[D19],''[D20],''[D21],''[D22],''[D23],''[D24],''[D25], "
                        qry &= "''[CL],''[PL],''[Total],''[Others] from mmm_mst_master m with(nolock) inner join mmm_mst_master d with(nolock) on m.fld19=d.tid "
                        qry &= "where m.eid=103 and m.isauth=1 "
                        qry &= "and m.documenttype='Employee Master' and d.eid=103 and d.documenttype='job code master' and d.fld1='" & jobcode & "'  order by m.fld1 DESC"

                    ElseIf ddlMonth.SelectedItem.Text = "MAY" Or ddlMonth.SelectedItem.Text = "JUL" Or ddlMonth.SelectedItem.Text = "OCT" Or ddlMonth.SelectedItem.Text = "DEC" Then

                        qry &= "select distinct m.fld4[Employee Code],m.fld1[Employee Name],dms.udf_split('MASTER-Position Master-fld1',m.FLD7)[Designation],"
                        qry &= "dms.udf_split('MASTER-PayCode Master-fld1',d.fld8)[Payroll Code],'Attd.'[ ],'P'[D26],'P'[D27],'P'[D28],'P'[D29],'P'[D30],'P'[D1],'P'[D2],'P'[D3],'P'[D4],'P'[D5],'P'[D6],'P'[D7],'P'[D8],'P'[D9],'P'[D10], "
                        qry &= "'P'[D11],'P'[D12],'P'[D13],'P'[D14],'P'[D15],'P'[D16],'P'[D17],'P'[D18],'P'[D19],'P'[D20],'P'[D21],'P'[D22],'P'[D23],'P'[D24],'P'[D25]"
                        qry &= "''[CL],''[PL],''[Total],''[Others]  from mmm_mst_master m with(nolock) inner join mmm_mst_master d with(nolock) on m.fld19=d.tid "
                        qry &= "where m.eid=103 and d.fld1='" & jobcode & "' and m.isauth=1  "
                        qry &= "and m.documenttype='Employee Master' and d.eid=103  and d.documenttype='job code master' "
                        qry &= " union all  select distinct m.fld4[Employee Code],m.fld1[Employee Name],dms.udf_split('MASTER-Position Master-fld1',m.FLD7)[Designation],"
                        qry &= "dms.udf_split('MASTER-PayCode Master-fld1',d.fld8)[Payroll Code],'OT'[Type],''[D26],''[D27],''[D28],''[D29],''[D30],''[D1],''[D2],''[D3],''[D4],''[D5],''[D6],''[D7],''[D8],''[D9],''[D10],"
                        qry &= " ''[D11],''[D12],''[D13],''[D14],''[D15],''[D16],''[D17],''[D18],''[D19],''[D20],''[D21],''[D22],''[D23],''[D24],''[D25], "
                        qry &= "''[CL],''[PL],''[Total],''[Others]  from mmm_mst_master m with(nolock) inner join mmm_mst_master d with(nolock) on m.fld19=d.tid "
                        qry &= "where m.eid=103 and m.isauth=1 "
                        qry &= "and m.documenttype='Employee Master' and d.eid=103 and d.documenttype='job code master' and d.fld1='" & jobcode & "'  order by m.fld1 DESC "

                    ElseIf ddlMonth.SelectedItem.Text = "MAR" Then

                        qry &= "select distinct m.fld4[Employee Code],m.fld1[Employee Name],dms.udf_split('MASTER-Position Master-fld1',m.FLD7)[Designation],"
                        qry &= "dms.udf_split('MASTER-PayCode Master-fld1',d.fld8)[Payroll Code],'Attd.'[ ],'P'[D26],'P'[D27],'P'[D28],'P'[D29],'P'[D1],'P'[D2],'P'[D3],'P'[D4],'P'[D5],'P'[D6],'P'[D7],'P'[D8],'P'[D9],'P'[D10], "
                        qry &= "'P'[D11],'P'[D12],'P'[D13],'P'[D14],'P'[D15],'P'[D16],'P'[D17],'P'[D18],'P'[D19],'P'[D20],'P'[D21],'P'[D22],'P'[D23],'P'[D24],'P'[D25],"
                        qry &= "''[CL],''[PL],''[Total],''[Others]  from mmm_mst_master m with(nolock) inner join mmm_mst_master d with(nolock) on m.fld19=d.tid "
                        qry &= "where m.eid=103 and m.isauth=1 "
                        qry &= "and m.documenttype='Employee Master' and d.eid=103  and d.documenttype='job code master' and d.fld1='" & jobcode & "' "
                        qry &= " union all  select distinct m.fld4[Employee Code],m.fld1[Employee Name],dms.udf_split('MASTER-Position Master-fld1',m.FLD7)[Designation],"
                        qry &= "dms.udf_split('MASTER-PayCode Master-fld1',d.fld8)[Payroll Code],'OT'[Type],''[D26],''[D27],''[D28],''[D29],''[D1],''[D2],''[D3],''[D4],''[D5],''[D6],''[D7],''[D8],''[D9],''[D10],"
                        qry &= " ''[D11],''[D12],''[D13],''[D14],''[D15],''[D16],''[D17],''[D18],''[D19],''[D20],''[D21],''[D22],''[D23],''[D24],''[D25], "
                        qry &= "''[CL],''[PL],''[Total],''[Others]  from mmm_mst_master m with(nolock) inner join mmm_mst_master d with(nolock) on m.fld19=d.tid "
                        qry &= "where m.eid=103 and m.isauth=1 "
                        qry &= "and m.documenttype='Employee Master' and d.eid=103 and d.documenttype='job code master' and d.fld1='" & jobcode & "'  order by m.fld1 DESC "


                    End If
                End If

                If Trim(cycle).ToUpper = "1 TO 31" Then
                    If ddlMonth.SelectedItem.Text = "JAN" Or ddlMonth.SelectedItem.Text = "FEB" Or ddlMonth.SelectedItem.Text = "APR" Or ddlMonth.SelectedItem.Text = "JUN" Or ddlMonth.SelectedItem.Text = "AUG" Or ddlMonth.SelectedItem.Text = "SEP" Or ddlMonth.SelectedItem.Text = "NOV" Then
                        qry &= "select distinct m.fld4[Employee Code],m.fld1[Employee Name],dms.udf_split('MASTER-Position Master-fld1',m.FLD7)[Designation],"
                        qry &= "dms.udf_split('MASTER-PayCode Master-fld1',d.fld8)[Payroll Code],'Attd.'[ ],'P'[D1],'P'[D2],'P'[D3],'P'[D4],'P'[D5],'P'[D6],'P'[D7],'P'[D8],'P'[D9],'P'[D10],'P'[D11],'P'[D12],'P'[D13],'P'[D14],'P'[D15], "
                        qry &= "'P'[D16],'P'[D17],'P'[D18],'P'[D19],'P'[D20],'P'[D21],'P'[D22],'P'[D23],'P'[D24],'P'[D25],'P'[D26],'P'[D27],'P'[D28],'P'[D29],'P'[D30],'P'[D31], "
                        qry &= "''[CL],''[PL],''[Total],''[Others] from mmm_mst_master m with(nolock) inner join mmm_mst_master d with(nolock) on m.fld19=d.tid "
                        qry &= "where m.eid=103 and m.isauth=1  "
                        qry &= "and m.documenttype='Employee Master' and d.eid=103  and d.documenttype='job code master' and d.fld1='" & jobcode & "' "
                        qry &= "union all select distinct m.fld4[Employee Code],m.fld1[Employee Name],dms.udf_split('MASTER-Position Master-fld1',m.FLD7)[Designation],"
                        qry &= "dms.udf_split('MASTER-PayCode Master-fld1',d.fld8)[Payroll Code],'OT',''[D1],''[D2],''[D3],''[D4],''[D5],''[D6],''[D7],''[D8],''[D9],''[D10],''[D11],''[D12],''[D13],''[D14],''[D15], "
                        qry &= "''[D16],''[D17],''[D18],''[D19],''[D20],''[D21],''[D22],''[D23],''[D24],''[D25],''[D26],''[D27],''[D28],''[D29],''[D30],''[D31], "
                        qry &= "''[CL],''[PL],''[Total],''[Others]  from mmm_mst_master m with(nolock) inner join mmm_mst_master d with(nolock) on m.fld19=d.tid "
                        qry &= "where m.eid=103 and m.isauth=1 "
                        qry &= "and m.documenttype='Employee Master' and d.eid=103  and d.documenttype='job code master' and d.fld1='" & jobcode & "'  order by m.fld1 DESC "

                    ElseIf ddlMonth.SelectedItem.Text = "MAY" Or ddlMonth.SelectedItem.Text = "JUL" Or ddlMonth.SelectedItem.Text = "OCT" Or ddlMonth.SelectedItem.Text = "DEC" Then

                        qry &= "select distinct m.fld4[Employee Code],m.fld1[Employee Name],dms.udf_split('MASTER-Position Master-fld1',m.FLD7)[Designation],"
                        qry &= "dms.udf_split('MASTER-PayCode Master-fld1',d.fld8)[Payroll Code],'Attd.'[ ],'P'[D1],'P'[D2],'P'[D3],'P'[D4],'P'[D5],'P'[D6],'P'[D7],'P'[D8],'P'[D9],'P'[D10],'P'[D11],'P'[D12],'P'[D13],'P'[D14],'P'[D15], "
                        qry &= "'P'[D16],'P'[D17],'P'[D18],'P'[D19],'P'[D20],'P'[D21],'P'[D22],'P'[D23],'P'[D24],'P'[D25],'P'[D26],'P'[D27],'P'[D28],'P'[D29],'P'[D30], "
                        qry &= "''[CL],''[PL],''[Total],''[Others]  from mmm_mst_master m with(nolock) inner join mmm_mst_master d with(nolock) on m.fld19=d.tid "
                        qry &= "where m.eid=103 and m.isauth=1  "
                        qry &= "and m.documenttype='Employee Master' and d.eid=103  and d.documenttype='job code master' and d.fld1='" & jobcode & "' "
                        qry &= "union all select distinct m.fld4[Employee Code],m.fld1[Employee Name],dms.udf_split('MASTER-Position Master-fld1',m.FLD7)[Designation],"
                        qry &= "dms.udf_split('MASTER-PayCode Master-fld1',d.fld8)[Payroll Code],'OT',''[D1],''[D2],''[D3],''[D4],''[D5],''[D6],''[D7],''[D8],''[D9],''[D10],''[D11],''[D12],''[D13],''[D14],''[D15], "
                        qry &= "''[D16],''[D17],''[D18],''[D19],''[D20],''[D21],''[D22],''[D23],''[D24],''[D25],''[D26],''[D27],''[D28],''[D29],''[D30],"
                        qry &= "''[CL],''[PL],''[Total],''[Others]  from mmm_mst_master m with(nolock) inner join mmm_mst_master d with(nolock) on m.fld19=d.tid "
                        qry &= "where m.eid=103 and m.isauth=1 "
                        qry &= "and m.documenttype='Employee Master' and d.eid=103  and d.documenttype='job code master' and d.fld1='" & jobcode & "'  order by m.fld1 DESC "

                    ElseIf ddlMonth.SelectedItem.Text = "MAR" Then

                        qry &= "select distinct m.fld4[Employee Code],m.fld1[Employee Name],dms.udf_split('MASTER-Position Master-fld1',m.FLD7)[Designation],"
                        qry &= "dms.udf_split('MASTER-PayCode Master-fld1',d.fld8)[Payroll Code],'Attd.'[ ],'P'[D1],'P'[D2],'P'[D3],'P'[D4],'P'[D5],'P'[D6],'P'[D7],'P'[D8],'P'[D9],'P'[D10],'P'[D11],'P'[D12],'P'[D13],'P'[D14],'P'[D15], "
                        qry &= "'P'[D16],'P'[D17],'P'[D18],'P'[D19],'P'[D20],'P'[D21],'P'[D22],'P'[D23],'P'[D24],'P'[D25],'P'[D26],'P'[D27],'P'[D28],'P'[D29], "
                        qry &= "''[CL],''[PL],''[Total],''[Others]  from mmm_mst_master m with(nolock) inner join mmm_mst_master d with(nolock) on m.fld19=d.tid "
                        qry &= "where m.eid=103 and m.isauth=1  "
                        qry &= "and m.documenttype='Employee Master' and d.eid=103  and d.documenttype='job code master' and d.fld1='" & jobcode & "' "
                        qry &= "union all select distinct m.fld4[Employee Code],m.fld1[Employee Name],dms.udf_split('MASTER-Position Master-fld1',m.FLD7)[Designation],"
                        qry &= "dms.udf_split('MASTER-PayCode Master-fld1',d.fld8)[Payroll Code],'OT',''[D1],''[D2],''[D3],''[D4],''[D5],''[D6],''[D7],''[D8],''[D9],''[D10],''[D11],''[D12],''[D13],''[D14],''[D15], "
                        qry &= "''[D16],''[D17],''[D18],''[D19],''[D20],''[D21],''[D22],''[D23],''[D24],''[D25],''[D26],''[D27],''[D28],''[D29],"
                        qry &= "''[CL],''[PL],''[Total],''[Others]  from mmm_mst_master m with(nolock) inner join mmm_mst_master d with(nolock) on m.fld19=d.tid "
                        qry &= "where m.eid=103 and m.isauth=1 "
                        qry &= "and m.documenttype='Employee Master' and d.eid=103  and d.documenttype='job code master' and d.fld1='" & jobcode & "'  order by m.fld1 DESC "

                    End If
                End If

                If Trim(cycle).ToUpper = "21 TO 20" Then

                    If ddlMonth.SelectedItem.Text = "JAN" Or ddlMonth.SelectedItem.Text = "FEB" Or ddlMonth.SelectedItem.Text = "APR" Or ddlMonth.SelectedItem.Text = "JUN" Or ddlMonth.SelectedItem.Text = "AUG" Or ddlMonth.SelectedItem.Text = "SEP" Or ddlMonth.SelectedItem.Text = "NOV" Then

                        qry &= "select distinct m.fld4[Employee Code],m.fld1[Employee Name],dms.udf_split('MASTER-Position Master-fld1',m.FLD7)[Designation],"
                        qry &= "dms.udf_split('MASTER-PayCode Master-fld1',d.fld8)[Payroll Code],'Attd.'[ ],'P'[D21],'P'[D22],'P'[D23],'P'[D24],'P'[D25],'P'[D26],'P'[D27],'P'[D28],'P'[D29],'P'[D30],'P'[D31],'P'[D1],'P'[D2],'P'[D3],'P'[D4],'P'[D5], "
                        qry &= "'P'[D6],'P'[D7],'P'[D8],'P'[D9],'P'[D10],'P'[D11],'P'[D12],'P'[D13],'P'[D14],'P'[D15],'P'[D16],'P'[D17],'P'[D18],'P'[D19],'P'[D20], "
                        qry &= "''[CL],''[PL],''[Total],''[Others]  from mmm_mst_master m with(nolock) inner join mmm_mst_master d with(nolock) on m.fld19=d.tid "
                        qry &= "where m.eid=103 and m.isauth=1  "
                        qry &= "and m.documenttype='Employee Master' and d.eid=103  and d.documenttype='job code master' and d.fld1='" & jobcode & "' "

                        qry &= "union all select distinct m.fld4[Employee Code],m.fld1[Employee Name],dms.udf_split('MASTER-Position Master-fld1',m.FLD7)[Designation],"
                        qry &= "dms.udf_split('MASTER-PayCode Master-fld1',d.fld8)[Payroll Code],'OT',''[D21],''[D22],''[D23],''[D24],''[D25],''[D26],''[D27],''[D28],''[D29],''[D30],''[D31],''[D1],''[D2],''[D3],''[D4],''[D5], "
                        qry &= "''[D6],''[D7],''[D8],''[D9],''[D10],''[D11],''[D12],''[D13],''[D14],''[D15],''[D16],''[D17],''[D18],''[D19],''[D20], "
                        qry &= "''[CL],''[PL],''[Total],''[Others] from mmm_mst_master m with(nolock) inner join mmm_mst_master d with(nolock) on m.fld19=d.tid "
                        qry &= "where m.eid=103 and m.isauth=1 "
                        qry &= "and m.documenttype='Employee Master' and d.eid=103  and d.documenttype='job code master' and d.fld1='" & jobcode & "'  order by m.fld1 DESC "

                    ElseIf ddlMonth.SelectedItem.Text = "MAY" Or ddlMonth.SelectedItem.Text = "JUL" Or ddlMonth.SelectedItem.Text = "OCT" Or ddlMonth.SelectedItem.Text = "DEC" Then

                        qry &= "select distinct m.fld4[Employee Code],m.fld1[Employee Name],dms.udf_split('MASTER-Position Master-fld1',m.FLD7)[Designation],"
                        qry &= "dms.udf_split('MASTER-PayCode Master-fld1',d.fld8)[Payroll Code],'Attd.'[ ],'P'[D21],'P'[D22],'P'[D23],'P'[D24],'P'[D25],'P'[D26],'P'[D27],'P'[D28],'P'[D29],'P'[D1],'P'[D2],'P'[D3],'P'[D4],'P'[D5], "
                        qry &= "'P'[D6],'P'[D7],'P'[D8],'P'[D9],'P'[D10],'P'[D11],'P'[D12],'P'[D13],'P'[D14],'P'[D15],'P'[D16],'P'[D17],'P'[D18],'P'[D19],'P'[D20], "
                        qry &= "''[CL],''[PL],''[Total],''[Others] rom mmm_mst_master m with(nolock) inner join mmm_mst_master d with(nolock) on m.fld19=d.tid "
                        qry &= "where m.eid=103 and m.isauth=1  "
                        qry &= "and m.documenttype='Employee Master' and d.eid=103  and d.documenttype='job code master' and d.fld1='" & jobcode & "' "

                        qry &= "union all select distinct m.fld4[Employee Code],m.fld1[Employee Name],dms.udf_split('MASTER-Position Master-fld1',m.FLD7)[Designation],"
                        qry &= "dms.udf_split('MASTER-PayCode Master-fld1',d.fld8)[Payroll Code],'OT',''[D21],''[D22],''[D23],''[D24],''[D25],''[D26],''[D27],''[D28],''[D29],''[D30],''[D1],''[D2],''[D3],''[D4],''[D5], "
                        qry &= "''[D6],''[D7],''[D8],''[D9],''[D10],''[D11],''[D12],''[D13],''[D14],''[D15],''[D16],''[D17],''[D18],''[D19],''[D20], "
                        qry &= "''[CL],''[PL],''[Total],''[Others] from mmm_mst_master m with(nolock) inner join mmm_mst_master d with(nolock) on m.fld19=d.tid "
                        qry &= "where m.eid=103 and m.isauth=1 "
                        qry &= "and m.documenttype='Employee Master' and d.eid=103  and d.documenttype='job code master' and d.fld1='" & jobcode & "'  order by m.fld1 DESC "

                    ElseIf ddlMonth.SelectedItem.Text = "MAR" Then

                        qry &= "select distinct m.fld4[Employee Code],m.fld1[Employee Name],dms.udf_split('MASTER-Position Master-fld1',m.FLD7)[Designation],"
                        qry &= "dms.udf_split('MASTER-PayCode Master-fld1',d.fld8)[Payroll Code],'Attd.'[ ],'P'[D21],'P'[D22],'P'[D23],'P'[D24],'P'[D25],'P'[D26],'P'[D27],'P'[D28],'P'[D29], "
                        qry &= "'P'[D6],'P'[D7],'P'[D8],'P'[D9],'P'[D10],'P'[D11],'P'[D12],'P'[D13],'P'[D14],'P'[D15],'P'[D16],'P'[D17],'P'[D18],'P'[D19],'P'[D20], "
                        qry &= "''[CL],''[PL],''[Total],''[Others]  from mmm_mst_master m with(nolock) inner join mmm_mst_master d with(nolock) on m.fld19=d.tid "
                        qry &= "where m.eid=103 and m.isauth=1  "
                        qry &= "and m.documenttype='Employee Master' and d.eid=103  and d.documenttype='job code master' and d.fld1='" & jobcode & "' "
                        qry &= "union all select distinct m.fld4[Employee Code],m.fld1[Employee Name],dms.udf_split('MASTER-Position Master-fld1',m.FLD7)[Designation],"
                        qry &= "dms.udf_split('MASTER-PayCode Master-fld1',d.fld8)[Payroll Code],'OT',''[D21],''[D22],''[D23],''[D24],''[D25],''[D26],''[D27],''[D28],''[D29],''[D1],''[D2],''[D3],''[D4],''[D5], "
                        qry &= "''[D6],''[D7],''[D8],''[D9],''[D10],''[D11],''[D12],''[D13],''[D14],''[D15],''[D16],''[D17],''[D18],''[D19],''[D20], "
                        qry &= "''[CL],''[PL],''[Total],''[Others]  from mmm_mst_master m with(nolock) inner join mmm_mst_master d with(nolock) on m.fld19=d.tid "
                        qry &= "where m.eid=103 and m.isauth=1 "
                        qry &= "and m.documenttype='Employee Master' and d.eid=103  and d.documenttype='job code master' and d.fld1='" & jobcode & "'  order by m.fld1 DESC "

                    End If
                End If


                Dim obj As New MailUtill(eid:=Session("EID"))
                Dim Filepath As String = HostingEnvironment.MapPath("~/MailAttach/")
                oda = New SqlDataAdapter("", con)
                oda.SelectCommand.CommandText = qry
                Dim ds1 As New DataSet
                oda.Fill(ds1, "data")
                Dim file As String = ""
                If prevloc <> loccode Then
                    Directory.CreateDirectory(Filepath & loccode)
                End If
                Dim mailto As String = prevEmail & "," & prevCoorEmail & "," & prevHREmail

                If ds1.Tables("data").Rows.Count > 0 Then
                    ' If i < 100 Then

                    Dim BCC As String = ""

                    file = CreateCSVR(ds1.Tables("data"), jobcode)
                    file = file.Replace(".xls", "")

                    oda.SelectCommand.Parameters.Clear()
                    oda.SelectCommand.CommandText = "INSERT_MAILLOG_G4S"
                    oda.SelectCommand.CommandType = CommandType.StoredProcedure
                    oda.SelectCommand.Parameters.AddWithValue("@MAILTO", prevEmail & "," & prevCoorEmail)
                    oda.SelectCommand.Parameters.AddWithValue("@Cc", "")
                    oda.SelectCommand.Parameters.AddWithValue("@Bcc", BCC)
                    oda.SelectCommand.Parameters.AddWithValue("@JobCode", jobcode)
                    oda.SelectCommand.Parameters.AddWithValue("@Cycle", ddlcyc.SelectedItem.Text)
                    oda.SelectCommand.Parameters.AddWithValue("@MYP", ddlMonth.SelectedItem.Text + "-" + ddlYear.SelectedItem.Text)
                    oda.SelectCommand.Parameters.AddWithValue("@EID", Session("EID"))
                    If con.State <> ConnectionState.Open Then
                        con.Open()

                    End If
                    oda.SelectCommand.ExecuteScalar()
                    '  End If

                    'End If
                End If
                If i <> 0 Then
                    If prevloc <> loccode Then
                        Dim filenames As String() = Directory.GetFiles(Filepath & prevloc)
                        Using zip As New ZipFile()
                            zip.AddFiles(filenames, "files")
                            zip.Save(Filepath & prevloc & ".zip")
                        End Using
                        oda.SelectCommand.CommandType = CommandType.Text
                        oda.SelectCommand.CommandText = "select msgbody from mmm_mst_reportscheduler where tid=479"
                        oda.Fill(ds, "mailbody")
                        Dim mailbody As String = ds.Tables("mailbody").Rows(0).Item(0).ToString
                        mailbody = Replace(mailbody, "@JC", jobcode)
                        obj.SendMail(ToMail:=mailto, Subject:=" Rota of " & prevEmail & " for the month of " & ddlMonth.SelectedItem.Text & "-" & ddlYear.SelectedItem.Text, MailBody:=mailbody, Attachments:=Filepath + prevloc & ".zip", BCC:="alokkr.myndsol@in.g4s.com,myndits@gmail.com")
                        If Directory.Exists(Filepath & prevloc) Then
                            IO.File.Delete(Filepath & prevloc + ".zip")
                            System.IO.Directory.Delete(Filepath & prevloc, True)
                        End If
                    ElseIf i = ds.Tables("Cycle").Rows.Count - 1 Then
                        Dim filenames As String() = Directory.GetFiles(Filepath & prevloc)
                        Using zip As New ZipFile()
                            zip.AddFiles(filenames, "files")
                            zip.Save(Filepath & prevloc & ".zip")
                        End Using
                        oda.SelectCommand.CommandType = CommandType.Text
                        oda.SelectCommand.CommandText = "select msgbody from mmm_mst_reportscheduler where tid=479"
                        oda.Fill(ds, "mailbody")
                        Dim mailbody As String = ds.Tables("mailbody").Rows(0).Item(0).ToString
                        mailbody = Replace(mailbody, "@JC", jobcode)
                        obj.SendMail(ToMail:=mailto, Subject:=" Rota of " & prevEmail & " for the month of " & ddlMonth.SelectedItem.Text & "-" & ddlYear.SelectedItem.Text, MailBody:=mailbody, Attachments:=Filepath + prevloc & ".zip", BCC:="alokkr.myndsol@in.g4s.com,myndits@gmail.com")
                        If Directory.Exists(Filepath & prevloc) Then
                            IO.File.Delete(Filepath & prevloc + ".zip")
                            ' IO.File.Delete(Filepath & prevloc)
                            System.IO.Directory.Delete(Filepath & prevloc, True)
                        End If
                    End If
                End If
            Next
            lblMsg.Visible = True
            lblMsg.Text = "Sent...!"
        Catch ex As Exception
            lblMsg.Text = ex.ToString
        Finally
            oda.Dispose()
            con.Close()
            con.Dispose()
        End Try
    End Sub
    Private Function CreateCSVR(ByVal dt As DataTable, ByVal jbcode As String) As String
        Dim gvPending As New GridView
        Dim Folder As New DirectoryInfo(HostingEnvironment.MapPath("~/MailAttach/"))
        Dim fname1 As String = ""
        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim con As New SqlConnection(conStr)
        Dim oda As SqlDataAdapter = New SqlDataAdapter("", con)
        Dim ds As New DataSet
        Try
            fname1 = Now.Year & Now.Month & Now.Day & Now.Hour & Now.Minute & Now.Second & Now.Millisecond
            Dim FPath As String = HostingEnvironment.MapPath("~/MailAttach/")

            oda.SelectCommand.CommandType = CommandType.Text
            Dim Jcode As String = ""
            Dim location As String = ""
            Dim loccode As String = ""
            Dim site As String = ""
            oda.SelectCommand.CommandText = " select top 1 fld1[JobCode],dms.udf_split('MASTER-Location Master-fld1',fld4)[Location],fld2[site] from MMM_MST_master where documenttype='Job Code Master'  and eid=" & Session("EID") & " and fld1='" & jbcode & "'"
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
            Dim Period As String = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(ddlMonth.SelectedItem.Text.ToLower & "-" & ddlYear.SelectedItem.Text.ToLower)


            'To Export all pages

            'To Export all pages
            gvPending.AllowPaging = False
            gvPending.DataSource = dt
            gvPending.DataBind()
            For rowIndex As Integer = gvPending.Rows.Count - 2 To 0 Step -1
                Dim gviewRow As GridViewRow = gvPending.Rows(rowIndex)
                Dim gviewPreviousRow As GridViewRow = gvPending.Rows(rowIndex + 1)
                If ddlcyc.SelectedItem.Text = "26 TO 25" Then

                    If ddlMonth.SelectedItem.Text = "JAN" Or ddlMonth.SelectedItem.Text = "FEB" Or ddlMonth.SelectedItem.Text = "APR" Or ddlMonth.SelectedItem.Text = "JUN" Or ddlMonth.SelectedItem.Text = "AUG" Or ddlMonth.SelectedItem.Text = "SEP" Or ddlMonth.SelectedItem.Text = "NOV" Then
                        For cellCount As Integer = 0 To gviewRow.Cells.Count - 37
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
                    ElseIf ddlMonth.SelectedItem.Text = "MAY" Or ddlMonth.SelectedItem.Text = "JUL" Or ddlMonth.SelectedItem.Text = "OCT" Or ddlMonth.SelectedItem.Text = "DEC" Then

                        For cellCount As Integer = 0 To gviewRow.Cells.Count - 35
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
                    ElseIf ddlMonth.SelectedItem.Text = "MAR" Then
                        For cellCount As Integer = 0 To gviewRow.Cells.Count - 35
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
                If ddlcyc.SelectedItem.Text = "1 TO 31" Then

                    For cellCount As Integer = 0 To gviewRow.Cells.Count - 37
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


                If ddlcyc.SelectedItem.Text = "21 TO 20" Then

                    If ddlMonth.SelectedItem.Text = "JAN" Or ddlMonth.SelectedItem.Text = "FEB" Or ddlMonth.SelectedItem.Text = "APR" Or ddlMonth.SelectedItem.Text = "JUN" Or ddlMonth.SelectedItem.Text = "AUG" Or ddlMonth.SelectedItem.Text = "SEP" Or ddlMonth.SelectedItem.Text = "NOV" Then
                        For cellCount As Integer = 0 To gviewRow.Cells.Count - 36
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
                    ElseIf ddlMonth.SelectedItem.Text = "MAY" Or ddlMonth.SelectedItem.Text = "JUL" Or ddlMonth.SelectedItem.Text = "OCT" Or ddlMonth.SelectedItem.Text = "DEC" Then

                        For cellCount As Integer = 0 To gviewRow.Cells.Count - 35
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
                    ElseIf ddlMonth.SelectedItem.Text = "MAR" Then
                        For cellCount As Integer = 0 To gviewRow.Cells.Count - 35
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
            Response.Clear()
            Response.Buffer = True
            '  Response.AddHeader("content-disposition", "attachment;filename=" & fname1)
            'Response.ContentType = "application/ms-excel"
            'Response.ContentEncoding = System.Text.Encoding.Unicode
            'Response.BinaryWrite(System.Text.Encoding.Unicode.GetPreamble())

            gvPending.HeaderRow.BackColor = Color.White

            For Each cell As TableCell In gvPending.HeaderRow.Cells
                cell.BackColor = gvPending.HeaderStyle.BackColor
            Next
            Dim datestr As String = location
            Using sw As New StringWriter()
                Using hw As New HtmlTextWriter(sw)



                    Dim writer As StreamWriter = File.AppendText(FPath & datestr & "\" & fname1 & ".xls")
                    gvPending.RenderControl(hw)
                    writer.Write("<br/><br/><div align=""left"" style=""border:1px solid black"" ><h3>For the month of: " & Period & "<br/>Location:" & location & "<br/>Location Code:" & loccode & "<br/>Job Code:" & Jcode & "<br/>Site Name:" & site & "<br/></h3></div>")
                    writer.WriteLine(sw.ToString())
                    writer.Write("<br/><br/><br/><br/><h5>SIGNATURE OF FIELD SUPERVISOR &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;SIGNATURE OF OPERATIONS MANAGER&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;SIGNATURE OF BRANCH MANGER&nbsp;&nbsp&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;SIGNATURE OF CLIENT WITH STAMP</h5>")
                    writer.Close()
                End Using
            End Using

            Return FPath & datestr & "/" & fname1 & ".xls"

            '   Zip(FPath, fname1)
        Catch ex As Exception
        Finally
            ds.Dispose()
            oda.Dispose()
            con.Close()
            con.Dispose()

        End Try

    End Function
    Protected Sub PrepareForExport(ByVal Gridview As GridView)
        Gridview.DataBind()

        'Change the Header Row back to white color
        Gridview.HeaderRow.Style.Add("background-color", "#FFFFFF")

        'Apply style to Individual Cells
        For k As Integer = 0 To Gridview.HeaderRow.Cells.Count - 1
            Gridview.HeaderRow.Cells(k).Style.Add("background-color", "green")
        Next

        For i As Integer = 0 To Gridview.Rows.Count - 1
            Dim row As GridViewRow = Gridview.Rows(i)

            'Change Color back to white
            row.BackColor = System.Drawing.Color.White

            'Apply text style to each Row
            row.Attributes.Add("class", "textmode")

            'Apply style to Individual Cells of Alternating Row
            If i Mod 2 <> 0 Then
                For j As Integer = 0 To Gridview.Rows(i).Cells.Count - 1
                    row.Cells(j).Style.Add("background-color", "#C2D69B")
                Next
            End If
        Next
    End Sub
    Sub Zip(ByVal path As String, ByVal filename As String)
        Dim filenames As String() = Directory.GetFiles(path)
        Using zip As New ZipFile()
            zip.AddFiles(filenames, "files")
            '  HostingEnvironment.MapPath("~/Templates/" & txtFolderName.Text)
            zip.Save(HostingEnvironment.MapPath("~/MailAttach/" & Now.Millisecond & "/" & Now.Millisecond & ".zip"))
        End Using
    End Sub
End Class
