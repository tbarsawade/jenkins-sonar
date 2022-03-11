Imports System.Net
Imports System.Net.Mail
Imports System.Drawing
Imports System.IO
Imports System.Data
Imports System.Data.SqlClient
Imports System.Diagnostics
Imports System.Web.Services
Imports System.Web.Hosting
Imports Microsoft.Office.Interop
Imports System.Globalization

<Assembly: DebuggerDisplay("{ToString}", Target:=GetType(Date))>

Partial Class MGRentProjectionReport
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
            If Request.QueryString("SC") Is Nothing Then
            Else
                Dim scrname As String = Request.QueryString("SC").ToString()
                ViewState("ReportName") = scrname
                Dim da As SqlDataAdapter = Nothing
                Dim da1 As SqlDataAdapter = Nothing
                Dim con As SqlConnection = Nothing

            End If
            BindDdl()
        End If
    End Sub
    Private Sub BindDdl()

        Dim currentYear As Integer = System.DateTime.Now.Year + 5
        Dim startYear As Integer = 2015
        ddlFYear.DataValueField = "Value"
        ddlFYear.DataTextField = "Text"

        For i As Integer = startYear To currentYear
            ddlFYear.Items.Add(Convert.ToString(i & "-" & i + 1))
        Next

        ddlFYear.Items.Insert(0, "Select")
    End Sub
    <WebMethod()>
    Public Shared Function GetDataStore(SYear As String, EYear As String) As DGrid
        Dim grid As New DGrid()
        If SYear = "null" Then
            grid.Message = "Please select Filter...!"
            Return grid
            grid.Success = False
        End If

        Dim jsonData As String = ""

        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim con As SqlConnection = New SqlConnection(conStr)
        Dim ds As New DataSet
        Dim da As New SqlDataAdapter("", con)
        Try
            da.SelectCommand.CommandText = ";With cte as(select  ROW_NUMBER() OVER(PARTITION BY fld2,fld11 ORDER BY tid DESC) rn,dms.udf_split('MASTER-Vendor Master-fld1',fld11) as [Vendor],dms.udf_split('MASTER-Vendor Master-fld2',fld11) as [Vendor Name],dms.udf_split('MASTER-Location Master-fld1',fld28) as [Site],dms.udf_split('MASTER-Location Master-fld2',fld28) as [Location],dms.udf_split('MASTER-Location Master-fld3',fld28) as [Location1],convert(varchar(20),DATEADD(DAY,convert(int,DATEDIFF(DD,CONVERT(DATE,fld49, 3),  CONVERT(DATE,fld50, 3)))+1, CONVERT(DATE,fld3, 3)),3) as[Rent Comm date],fld4 as [Rent End Date],dms.udf_split('MASTER-Rent Type-fld1',fld41) as [Rent Type],fld74 as [MGamt],fld22 as [RentEsc],datename(month,DATEADD(DAY,convert(int,DATEDIFF(DD,CONVERT(DATE,fld49, 3),  CONVERT(DATE,fld50, 3)))+1, CONVERT(DATE,fld3, 3))) as [RentEscMonth],convert(int,datename(year,DATEADD(DAY,convert(int,DATEDIFF(DD,CONVERT(DATE,fld49, 3),  CONVERT(DATE,fld50, 3)))+1, CONVERT(DATE,fld3, 3))))+1 as [RentEscYear] ,DATEADD(DAY,convert(int,DATEDIFF(DD,CONVERT(DATE,fld49, 3),  CONVERT(DATE,fld50, 3)))+1, CONVERT(DATE,fld3, 3)) as [RentCommdt],CONVERT(DATE,fld4, 3) as [RentEnddt],CONVERT(DATE,fld3, 3) as [RentStartdt],convert(int,datediff(YEAR, CONVERT(DATE,fld3, 3),'" & EYear + "-01-01" & "'))-1 as [Yeardiff]   from MMM_MST_Master  with(nolock) where eid=181 and Documenttype='Lease Master' and '" & SYear & "-'+datename(MONTH, convert(date,fld3, 3))+'-'+datename(DAY, convert(date,fld3, 3))   between   convert(date,fld3, 3) and  convert(date,fld4, 3) )	select * from cte where rn=1 "
            'and   convert(date,fld3, 3)>='" & SYear + "-01-01" & "' And  convert(Date, fld4, 3)<= '" & EYear + "-01-01" & "'   and dms.udf_split('MASTER-Location Master-fld1',fld28)='S049' and dms.udf_split('MASTER-Vendor Master-fld1',fld11)='6000001702'
            da.Fill(ds, "LeaseMasterData")
            Using con1 As New SqlConnection(conStr)
                Using cmd As New SqlCommand("Proc_FinMonths")
                    cmd.Connection = con1
                    cmd.CommandType = CommandType.StoredProcedure
                    cmd.Parameters.Add(New SqlParameter("@StartDate", SYear + "-03-16"))
                    cmd.Parameters.Add(New SqlParameter("@EndDate", EYear + "-04-16"))
                    Using sda As New SqlDataAdapter(cmd)
                        sda.Fill(ds, "FinMonthsData")
                    End Using
                End Using
            End Using
            Dim dt As System.Data.DataTable
            Dim Mgamount As Double = 0
            Dim MGAmtDT As Double = 0
            Dim MGAmtESCDT As Double = 0
            Dim ESCMgamount As Double = 0
            Dim RentEscpage As Integer = 0
            Dim AEscYear As Integer = 0
            Dim EscYear As Integer = 0
            Dim MonthName As String = ""
            Dim MonthNameArr As String()
            Dim RentCommdt As Date
            Dim RentEnddt As Date
            Dim RentStartdt As Date
            Dim YearDiff As Integer = 0
            dt = ds.Tables("LeaseMasterData")
            If dt.Rows.Count > 0 Then
                For i As Integer = 0 To dt.Rows.Count - 1
                    For j As Integer = 0 To ds.Tables("FinMonthsData").Columns.Count - 1
                        dt.Columns.Add(ds.Tables("FinMonthsData").Columns(j).ColumnName, GetType(Double))
                    Next
                    Exit For
                Next
                dt.Columns.Add("Total", GetType(Integer))
            End If
            Dim TotalMgAmt As Double = 0
            Dim RentInvDate As Date
            Dim RentEndInvAmt As Double = 0
            Dim objRental As New Rentaltool
            Dim decimalVar As Decimal

            If dt.Rows.Count > 0 Then
                For i As Integer = 0 To dt.Rows.Count - 1
                    TotalMgAmt = 0
                    Mgamount = Convert.ToDouble(dt.Rows(i).Item("MGamt"))
                    Dim result As String = ParseDateFn("1554654", RentCommdt, Mgamount)
                    Dim PDFnResultStr = result.Split("=")
                    RentEnddt = dt.Rows(i).Item("RentEnddt")
                    RentStartdt = dt.Rows(i).Item("RentStartdt")
                    YearDiff = dt.Rows(i).Item("YearDiff")
                    Dim RentEndresult As String = ParseDateFn("1554654", RentEnddt, Mgamount)
                    Dim RentEndResultStr = RentEndresult.Split("=")
                    For j As Integer = 0 To ds.Tables("FinMonthsData").Columns.Count - 1
                        RentEscpage = dt.Rows(i).Item("RentEsc")
                        AEscYear = dt.Rows(i).Item("RentEscYear")
                        MonthNameArr = (ds.Tables("FinMonthsData").Columns(j).ColumnName).Split("_")
                        MonthName = MonthNameArr(0)
                        EscYear = MonthNameArr(1)
                        RentCommdt = dt.Rows(i).Item("RentCommdt")
                        MGAmtDT = PDFnResultStr(1)
                        RentInvDate = PDFnResultStr(0)
                        RentEndInvAmt = RentEndResultStr(1)
                        Dim date1 As DateTime = New DateTime(CDate(RentEnddt).Year, CDate(RentEnddt).Month, 1)
                        Dim monthNumber = DateTime.ParseExact(MonthName, "MMMM", CultureInfo.CurrentCulture).Month
                        'calculating Esclation Year

                        If EscYear = (AEscYear + YearDiff - 1) Then
                            decimalVar = 0
                            If date1.ToString("MMMM") = MonthName And CDate(RentEnddt).Year = EscYear Then
                                decimalVar = Decimal.Round(RentEndInvAmt, 2, MidpointRounding.AwayFromZero)
                                decimalVar = Math.Round(decimalVar, 2)
                                dt.Rows(i)(ds.Tables("FinMonthsData").Columns(j).ColumnName) = decimalVar
                            ElseIf CDate(RentEnddt).Year < EscYear Then
                                For cnt As Integer = j To ds.Tables("FinMonthsData").Columns.Count - 1
                                    dt.Rows(i)(ds.Tables("FinMonthsData").Columns(j).ColumnName) = 0
                                    j = j + 1
                                Next
                                Exit For
                            ElseIf dt.Rows(i).Item("RentEscMonth") = MonthName And EscYear < AEscYear Then
                                decimalVar = Decimal.Round(Convert.ToDouble(dt.Rows(i).Item("MGamt")), 2, MidpointRounding.AwayFromZero)
                                decimalVar = Math.Round(decimalVar, 2)
                                dt.Rows(i)(ds.Tables("FinMonthsData").Columns(j).ColumnName) = decimalVar
                            ElseIf dt.Rows(i).Item("RentEscMonth") = MonthName Then

                                ESCMgamount = (Mgamount * RentEscpage) / 100
                                MGAmtESCDT = Mgamount + ESCMgamount
                                decimalVar = Decimal.Round(MGAmtESCDT, 2, MidpointRounding.AwayFromZero)
                                Mgamount = MGAmtESCDT
                                decimalVar = Math.Round(decimalVar, 2)
                                dt.Rows(i)(ds.Tables("FinMonthsData").Columns(j).ColumnName) = decimalVar

                            ElseIf (DateTime.ParseExact(dt.Rows(i).Item("RentEscMonth"), "MMMM", CultureInfo.CurrentCulture).Month > monthNumber And EscYear = (AEscYear + YearDiff - 1)) Then
                                decimalVar = 0
                                If YearDiff = 1 Or YearDiff = 0 Then
                                    decimalVar = Decimal.Round(Mgamount, 2, MidpointRounding.AwayFromZero)
                                    decimalVar = Math.Round(decimalVar, 2)
                                    dt.Rows(i)(ds.Tables("FinMonthsData").Columns(j).ColumnName) = decimalVar
                                Else
                                    For cnt As Integer = 0 To (YearDiff - 1) - 1
                                        If j = 0 Then
                                            ESCMgamount = (Mgamount * RentEscpage) / 100
                                            MGAmtESCDT = Mgamount + ESCMgamount
                                            Mgamount = MGAmtESCDT
                                        Else
                                            MGAmtESCDT = Mgamount
                                        End If

                                    Next
                                    decimalVar = Decimal.Round(MGAmtESCDT, 2, MidpointRounding.AwayFromZero)
                                    decimalVar = Math.Round(decimalVar, 2)
                                    dt.Rows(i)(ds.Tables("FinMonthsData").Columns(j).ColumnName) = decimalVar
                                End If
                            ElseIf (DateTime.ParseExact(dt.Rows(i).Item("RentEscMonth"), "MMMM", CultureInfo.CurrentCulture).Month < monthNumber And EscYear <> (AEscYear + YearDiff - 1)) Then
                                decimalVar = 0
                                ESCMgamount = (Mgamount * RentEscpage) / 100
                                MGAmtESCDT = Mgamount + ESCMgamount
                                decimalVar = Decimal.Round(MGAmtESCDT, 2, MidpointRounding.AwayFromZero)
                                decimalVar = Math.Round(decimalVar, 2)
                                dt.Rows(i)(ds.Tables("FinMonthsData").Columns(j).ColumnName) = decimalVar

                            ElseIf (DateTime.ParseExact(dt.Rows(i).Item("RentEscMonth"), "MMMM", CultureInfo.CurrentCulture).Month < monthNumber And EscYear = (AEscYear + YearDiff - 1)) Then
                                decimalVar = 0

                                decimalVar = Decimal.Round(Mgamount, 2, MidpointRounding.AwayFromZero)
                                decimalVar = Math.Round(decimalVar, 2)
                                dt.Rows(i)(ds.Tables("FinMonthsData").Columns(j).ColumnName) = decimalVar

                            End If
                        ElseIf (EscYear = AEscYear) Then
                            If date1.ToString("MMMM") = MonthName And CDate(RentEnddt).Year = EscYear Then
                                decimalVar = 0
                                decimalVar = Decimal.Round(RentEndInvAmt, 2, MidpointRounding.AwayFromZero)
                                decimalVar = Math.Round(decimalVar, 2)
                                dt.Rows(i)(ds.Tables("FinMonthsData").Columns(j).ColumnName) = decimalVar
                            ElseIf CDate(RentEnddt).Year < EscYear Then

                                For cnt As Integer = j To ds.Tables("FinMonthsData").Columns.Count - 1
                                    dt.Rows(i)(ds.Tables("FinMonthsData").Columns(j).ColumnName) = 0
                                    j = j + 1
                                Next
                                Exit For
                            ElseIf dt.Rows(i).Item("RentEscMonth") = MonthName Then
                                decimalVar = 0
                                ESCMgamount = (Mgamount * RentEscpage) / 100
                                MGAmtESCDT = Mgamount + ESCMgamount
                                decimalVar = Decimal.Round(MGAmtESCDT, 2, MidpointRounding.AwayFromZero)
                                decimalVar = Math.Round(decimalVar, 2)
                                dt.Rows(i)(ds.Tables("FinMonthsData").Columns(j).ColumnName) = decimalVar
                                Mgamount = MGAmtESCDT

                            ElseIf (DateTime.ParseExact(dt.Rows(i).Item("RentEscMonth"), "MMMM", CultureInfo.CurrentCulture).Month > monthNumber And EscYear = AEscYear) Then
                                decimalVar = 0
                                decimalVar = Decimal.Round(Mgamount, 2, MidpointRounding.AwayFromZero)
                                decimalVar = Math.Round(decimalVar, 2)
                                dt.Rows(i)(ds.Tables("FinMonthsData").Columns(j).ColumnName) = decimalVar
                            ElseIf (DateTime.ParseExact(dt.Rows(i).Item("RentEscMonth"), "MMMM", CultureInfo.CurrentCulture).Month < monthNumber And EscYear <> AEscYear) Then
                                decimalVar = 0
                                ESCMgamount = (Mgamount * RentEscpage) / 100
                                MGAmtESCDT = Mgamount + ESCMgamount
                                decimalVar = Decimal.Round(MGAmtESCDT, 2, MidpointRounding.AwayFromZero)
                                decimalVar = Math.Round(decimalVar, 2)
                                dt.Rows(i)(ds.Tables("FinMonthsData").Columns(j).ColumnName) = decimalVar
                                'Mgamount = MGAmtESCDT 

                                'Mgamount = ESCMgamount + Mgamount
                            ElseIf (DateTime.ParseExact(dt.Rows(i).Item("RentEscMonth"), "MMMM", CultureInfo.CurrentCulture).Month <= monthNumber And EscYear = AEscYear) Then
                                decimalVar = 0
                                decimalVar = Decimal.Round(Mgamount, 2, MidpointRounding.AwayFromZero)
                                decimalVar = Math.Round(decimalVar, 2)
                                dt.Rows(i)(ds.Tables("FinMonthsData").Columns(j).ColumnName) = decimalVar
                                ' Mgamount = ESCMgamount
                            Else
                                dt.Rows(i)(ds.Tables("FinMonthsData").Columns(j).ColumnName) = 0
                            End If

                        ElseIf (EscYear > AEscYear) Then
                            If date1.ToString("MMMM") = MonthName And CDate(RentEnddt).Year = EscYear Then
                                decimalVar = 0
                                decimalVar = Decimal.Round(RentEndInvAmt, 2, MidpointRounding.AwayFromZero)
                                decimalVar = Math.Round(decimalVar, 2)
                                dt.Rows(i)(ds.Tables("FinMonthsData").Columns(j).ColumnName) = decimalVar
                            ElseIf EscYear > CDate(RentEnddt).Year Then
                                For cnt As Integer = j To ds.Tables("FinMonthsData").Columns.Count - 1
                                    dt.Rows(i)(ds.Tables("FinMonthsData").Columns(j).ColumnName) = 0
                                    j = j + 1
                                Next
                                Exit For
                            ElseIf dt.Rows(i).Item("RentEscMonth") <> MonthName And CDate(RentEnddt).Year = EscYear Then
                                dt.Rows(i)(ds.Tables("FinMonthsData").Columns(j).ColumnName) = 0
                            ElseIf (DateTime.ParseExact(dt.Rows(i).Item("RentEscMonth"), "MMMM", CultureInfo.CurrentCulture).Month < monthNumber And EscYear > AEscYear) Then
                                If YearDiff = 1 Or YearDiff = 0 Then
                                    decimalVar = 0
                                    ESCMgamount = (Mgamount * RentEscpage) / 100
                                    MGAmtESCDT = Mgamount + ESCMgamount
                                    decimalVar = Decimal.Round(MGAmtESCDT, 2, MidpointRounding.AwayFromZero)
                                    decimalVar = Math.Round(decimalVar, 2)
                                    Mgamount = decimalVar
                                    dt.Rows(i)(ds.Tables("FinMonthsData").Columns(j).ColumnName) = decimalVar
                                Else
                                    For cnt As Integer = 0 To (YearDiff - 1) - 1
                                        If j = 0 Then
                                            ESCMgamount = (Mgamount * RentEscpage) / 100
                                            MGAmtESCDT = Mgamount + ESCMgamount
                                            Mgamount = MGAmtESCDT
                                        Else
                                            MGAmtESCDT = Mgamount
                                        End If

                                    Next
                                    decimalVar = 0
                                    decimalVar = Decimal.Round(MGAmtESCDT, 2, MidpointRounding.AwayFromZero)
                                    decimalVar = Math.Round(decimalVar, 2)
                                    dt.Rows(i)(ds.Tables("FinMonthsData").Columns(j).ColumnName) = decimalVar
                                End If

                            Else
                                decimalVar = 0
                                decimalVar = Decimal.Round(Mgamount, 2, MidpointRounding.AwayFromZero)
                                decimalVar = Math.Round(decimalVar, 2)
                                dt.Rows(i)(ds.Tables("FinMonthsData").Columns(j).ColumnName) = decimalVar
                            End If
                        Else
                            dt.Rows(i)(ds.Tables("FinMonthsData").Columns(j).ColumnName) = 0
                        End If

                        TotalMgAmt = TotalMgAmt + dt.Rows(i)(ds.Tables("FinMonthsData").Columns(j).ColumnName)

                    Next
                    decimalVar = 0
                    decimalVar = Decimal.Round(TotalMgAmt, 2, MidpointRounding.AwayFromZero)
                    decimalVar = Math.Round(decimalVar, 2)
                    dt.Rows(i)("Total") = decimalVar
                Next
            End If

            If dt.Rows.Count > 0 Then
                dt.Columns.Add("Remarks", GetType(String))
                dt.Columns.Remove("rn")
                dt.Columns.Remove("MGamt")
                dt.Columns.Remove("RentEsc")
                dt.Columns.Remove("RentEscMonth")
                dt.Columns.Remove("RentEscYear")
                dt.Columns.Remove("RentEnddt")
                dt.Columns.Remove("RentStartdt")
                dt.Columns.Remove("RentCommdt")
                dt.Columns.Remove("Yeardiff")
                HttpContext.Current.Session("LeaseMasterData") = dt
            End If

            Dim strError = ""
            grid = DynamicGrid.GridData(dt, strError)
            If ds.Tables("LeaseMasterData").Rows.Count = 0 Then
                grid.Message = "No data found...!"
                grid.Success = False
            End If
        Catch Ex As Exception
            grid.Success = False
            grid.Message = "No data found...!"
            grid.Count = 0
        End Try
        Return grid

    End Function
    Shared Function ParseDateFn(ByVal LRentPayment As String, ByVal date2 As Date, ByVal MGAmount As Double) As String


        Dim Result As String = String.Empty
        Dim FinalInvDate As String = ""
        Dim MGAmtDT As Double = 0
        Dim dat As Date

        If Date.TryParse(date2, dat) Then

            If LRentPayment = "1554654" Then '"Monthly"
                Dim startDt As New Date(dat.Year, dat.Month, dat.Day)
                Dim startDate As New Date(dat.Year, dat.Month, 1)
                Dim dss As String = ""
                Dim dtss As String = ""
                Dim DTS As Date
                Dim endDt As New Date(dat.Year, dat.Month, 1)
                Dim lastDay As Date
                lastDay = DateAndTime.DateSerial(Year(endDt), 13, 0)

                Dim diff2 As Int64 = (lastDay - startDate).TotalDays.ToString() + 1

                Dim tss As TimeSpan = startDt.Subtract(endDt)

                If Convert.ToInt32(tss.Days) >= 0 Then
                    dss = tss.Days + 1
                End If

                Dim FinalInvDt As New Date(DTS.Year, DTS.Month, 1)
                FinalInvDate = FinalInvDt
                ' calculating rent Amount
                If diff2 < dss Then
                    MGAmtDT = MGAmount
                    MGAmtDT = MGAmtDT / dss
                    MGAmtDT = MGAmtDT * diff2
                ElseIf diff2 = dss Then
                    MGAmtDT = MGAmount
                End If


            End If


        End If

        Dim decimalVar As Decimal
        decimalVar = Decimal.Round(MGAmtDT, 2, MidpointRounding.AwayFromZero)
        decimalVar = Math.Round(decimalVar, 2)
        Result = Convert.ToString(FinalInvDate) + "=" + Convert.ToString(decimalVar)

        Return Result

    End Function

    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Shared Function GetExport(SYear As String) As String
        Dim grid As New DGrid()
        If SYear = "null" Then
            Return "Please select Filter...!"

        End If

        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim con As New SqlConnection(conStr)
        Dim ds As New DataSet
        Dim da As SqlDataAdapter = New SqlDataAdapter("", con)
        Dim dt As New System.Data.DataTable

        Dim qry As String = ""
        Try
            dt = HttpContext.Current.Session("LeaseMasterData")
            Dim fname As String = "MGRentProjectionReport.CSV"
            Dim FPath As String = HostingEnvironment.MapPath("~\Mailattach\")
            FPath = FPath & fname

            Dim sw As StreamWriter = New StreamWriter(FPath, False)
            sw.Flush()
            'First we will write the headers.

            Dim iColCount As Integer = dt.Columns.Count
            For i As Integer = 0 To iColCount - 1
                sw.Write(dt.Columns(i))
                If (i < iColCount - 1) Then
                    sw.Write(",")
                End If

            Next
            sw.Write(sw.NewLine)
            ' Now write all the rows.
            Dim dr As DataRow
            For Each dr In dt.Rows
                For i As Integer = 0 To iColCount - 1
                    If Not Convert.IsDBNull(dr(i)) Then
                        sw.Write(dr(i).ToString)
                    End If
                    If (i < iColCount - 1) Then
                        sw.Write(",")
                    End If
                Next
                sw.Write(sw.NewLine)
            Next
            sw.Close()
            'FormatExcel("/Mailattach/" & fname)
            Return "/Mailattach/" & fname
        Catch ex As Exception
            da.SelectCommand.CommandText = "INSERT_ERRORLOG"
            da.SelectCommand.CommandType = CommandType.StoredProcedure
            da.SelectCommand.Parameters.Clear()
            da.SelectCommand.Parameters.AddWithValue("@ERRORMSG", ex.ToString() & " Error column number:")
            da.SelectCommand.Parameters.AddWithValue("@ALERTNAME", "MGRentProjectionREPORT")
            da.SelectCommand.Parameters.AddWithValue("@EID", 181)
            If con.State <> ConnectionState.Open Then
                con.Open()
            End If
            da.SelectCommand.ExecuteNonQuery()
        Finally
            da.Dispose()
            con.Close()
            con.Dispose()
        End Try

    End Function



End Class
