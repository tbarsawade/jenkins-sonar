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
Partial Class SDReport
    Inherits System.Web.UI.Page
    Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
    Dim con As SqlConnection = New SqlConnection(conStr)
    Dim stradd As String = ""
    Dim actualval As String = ""
    Dim strOr As String = ""

    Public Shared Mydatatable As New DataTable()
    Protected Sub Page_Init(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Init
        Try
            If Not Session("CTheme") Is Nothing And Not Session("CTheme") = String.Empty Then
                Page.Theme = Convert.ToString(Session("CTheme"))
            Else
                Page.Theme = "Default"
            End If
        Catch ex As Exception
        End Try
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



    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Shared Function GetExport(sdate As String, edate As String, SYear As String, EYear As String) As String
        If SYear = "null" Then

        End If
        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim con As New SqlConnection(conStr)
        Dim ds As New DataSet
        Dim da As SqlDataAdapter = New SqlDataAdapter("", con)
        Dim dt As New DataTable

        Dim qry As String = ""
        Try
            If System.Web.HttpContext.Current.Session("USERROLE") = "HRSPOC" Then
                da.SelectCommand.CommandText = "select qryfield[cmd] from mmm_mst_report with(nolock) where reportid=2070"
            Else
                da.SelectCommand.CommandText = "select qryfieldrole[cmd] from mmm_mst_report with(nolock) where reportid=2070"
            End If
            da.Fill(ds, "qry")
            qry = ds.Tables("qry").Rows(0).Item(0).ToString()

            da.SelectCommand.CommandText = qry
            da.SelectCommand.CommandType = CommandType.StoredProcedure
            da.SelectCommand.Parameters.AddWithValue("@SYear", SYear)
            da.SelectCommand.Parameters.AddWithValue("@EYear", EYear)
            da.SelectCommand.Parameters.AddWithValue("@LeaseStartD", sdate)
            da.SelectCommand.Parameters.AddWithValue("@LeaseEndD", edate)
            da.SelectCommand.CommandTimeout = 900
            If con.State <> ConnectionState.Open Then
                con.Open()
            End If
            da.Fill(ds, "data")
            Dim obj As SDReport = New SDReport()
            obj.DynamicHeader(ds, SYear, EYear)
            obj.DynamicData(ds, SYear, EYear)
            Dim ds1 As New DataSet
            ds1 = ds
            ds1 = ds.Copy()
            ds1.Tables("data").Columns.RemoveAt(13)
            ds1.Tables("data").Columns.Remove("F_Contractual Receipt")
            ds1.Tables("data").Columns.Remove("F_Date of Deposit")
            dt = ds1.Tables("data")
            Dim fname As String = "SDReport.CSV"
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
            Return "/Mailattach/" & fname
        Catch ex As Exception
            da.SelectCommand.CommandText = "INSERT_ERRORLOG"
            da.SelectCommand.CommandType = CommandType.StoredProcedure
            da.SelectCommand.Parameters.Clear()
            da.SelectCommand.Parameters.AddWithValue("@ERRORMSG", ex.ToString() & " Error column number:")
            da.SelectCommand.Parameters.AddWithValue("@ALERTNAME", "DCMREPORT")
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

    <WebMethod()>
    Public Shared Function GetDataStore(sdate As String, edate As String, SYear As String, EYear As String) As DGrid
        Dim grid As New DGrid()
        If SYear = "null" Then
            grid.Message = "Please select Filter...!"
            Return grid
            grid.Success = False
        End If

        Dim jsonData As String = ""

        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim con As SqlConnection = New SqlConnection(conStr)
        Try
            Dim da As SqlDataAdapter = New SqlDataAdapter("", con)
            Dim ds As New DataSet()
            Dim qry As String = ""
            If System.Web.HttpContext.Current.Session("USERROLE") = "HRSPOC" Then
                da.SelectCommand.CommandText = "select qryfield[cmd] from mmm_mst_report with(nolock) where reportid=2070"
            Else
                da.SelectCommand.CommandText = "select qryfieldrole[cmd] from mmm_mst_report with(nolock) where reportid=2070"
            End If
            da.Fill(ds, "qry")
            qry = ds.Tables("qry").Rows(0).Item(0).ToString()
            da.SelectCommand.CommandText = qry
            da.SelectCommand.CommandType = CommandType.StoredProcedure
            da.SelectCommand.Parameters.AddWithValue("@SYear", SYear)
            da.SelectCommand.Parameters.AddWithValue("@EYear", EYear)
            da.SelectCommand.Parameters.AddWithValue("@LeaseStartD", sdate)
            da.SelectCommand.Parameters.AddWithValue("@LeaseEndD", edate)
            da.SelectCommand.CommandTimeout = 900
            If con.State <> ConnectionState.Open Then
                con.Open()
            End If
            da.Fill(ds, "data")
            Dim obj As SDReport = New SDReport()
            obj.DynamicHeader(ds, SYear, EYear)
            obj.DynamicData(ds, SYear, EYear)
            Dim ds1 As New DataSet
            ds1 = ds
            ds1.Tables("data").Columns.RemoveAt(13)
            ds1.Tables("data").Columns.Remove("F_Contractual Receipt")
            ds1.Tables("data").Columns.Remove("F_Date of Deposit")
            Dim strError = ""
            grid = DynamicGrid.GridData(ds1.Tables("data"), strError)
            If ds.Tables("data").Rows.Count = 0 Then
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
    Private Function DynamicData(ByVal newDatatable As DataSet, SYear As String, EYear As String) As DataSet
        Dim ds As New DataSet
        ds = newDatatable
        Dim FYear As Int32 = Convert.ToInt32(SYear)
        Dim TYear As Int32 = Convert.ToInt32(EYear)
        Dim fromdate As Date = New Date(FYear, 4, 1)
        Dim toDate As Date = New Date(TYear, 3, 31)
        Dim diff As Int32 = DateDiff(DateInterval.Day, fromdate, toDate)
        Dim diff1 As Decimal = diff / 365

        For i = 0 To ds.Tables("data").Rows.Count - 1
            Dim PrevDr As DataRow
            Dim NextDr As DataRow
            PrevDr = prevYearData(ds.Tables("data").Rows(i), SYear, EYear)
            NextDr = NextYearData(ds.Tables("data").Rows(i), SYear, EYear)
            YearDiffData(ds.Tables("data").Rows(i), PrevDr, NextDr, SYear, EYear)
        Next
        Return ds
    End Function

    Private Function prevYearData(ByVal newDatatable As DataRow, SYear As String, EYear As String) As DataRow
        Dim dr As DataRow
        dr = newDatatable
        Dim Rent As Decimal = 0
        Dim interest As Decimal = 0
        Dim Balance As Decimal = 0
        Dim Security As Decimal = 0
        Dim FYear As Int32 = Convert.ToInt32(SYear)
        Dim TYear As Int32 = Convert.ToInt32(EYear)
        Dim fromdate As Date = New Date(FYear, 3, 31)
        Dim toDate As Date = New Date(TYear, 3, 31)
        Dim PresentValue As Decimal

        Try
            Dim ContractualReceipt As Date
            If Not IsDBNull(dr.Item("F_Contractual Receipt")) Then
                ContractualReceipt = Convert.ToDateTime(dr.Item("F_Contractual Receipt"))
            End If
            Dim DateofDeoosity As Date
            If Not IsDBNull(dr.Item("F_Date of Deposit")) Then
                DateofDeoosity = Convert.ToDateTime(dr.Item("F_Date of Deposit"))
            End If
            Dim diff As Int32 = DateDiff(DateInterval.Day, DateofDeoosity, fromdate)
            Dim diff1 As Decimal = diff / 365

            If (fromdate > DateofDeoosity AndAlso DateofDeoosity <> DateTime.MinValue) AndAlso NotNull(dr.Item("Security Period"), 0) <> 0 Then
                Rent = (NotNull(dr.Item("Prepaid rent in form of Deposit"), 0) / NotNull(dr.Item("Security Period"), 0)) * (DateDiff(DateInterval.Day, DateofDeoosity, fromdate) / 365)
            End If
            ' Rent = dr.Item("Prepaid rent in form of Deposit") / (dr.Item("Security Period") * DateDiff(DateInterval.Day, DateofDeoosity, fromdate) / 365)

            dr.Item("Rent expenses March " & SYear) = RoundUp(Rent, 2)
            '     Dim dsa As Decimal = dr.Item("Security Period") - (DateDiff(DateInterval.Day, DateofDeoosity, fromdate) / 365)
            '    PresentValue = (dr.Item("Amount") / (1.08 ^ (dr.Item("Security Period") - (DateDiff(DateInterval.Day, DateofDeoosity, fromdate) / 365))))
            If fromdate > DateofDeoosity Then
                interest = (NotNull(dr.Item("Amount"), 0) / (1.08 ^ (NotNull(dr.Item("Security Period"), 0) - (DateDiff(DateInterval.Day, DateofDeoosity, fromdate) / 365)))) - NotNull(dr.Item("Present Value of Deposit"), 0)
            End If
            dr.Item("Interest Income March " & SYear) = RoundUp(interest, 2)

            Balance = NotNull(dr.Item("Prepaid rent in form of Deposit"), 0) - Rent

            dr.Item("Balance prepaid Rent March " & SYear) = RoundUp(Balance, 2)

            Security = NotNull(dr.Item("Present Value of Deposit"), 0) + interest

            dr.Item("Security deposit value March " & SYear) = RoundUp(Security, 2)

        Catch Ex As Exception
            Dim sd As String = Ex.ToString
        End Try
        Return dr
    End Function
    Public Shared Function NotNull(Of T)(ByVal Value As T, ByVal DefaultValue As T) As T
        If Value Is Nothing OrElse IsDBNull(Value) Then
            Return DefaultValue
        Else
            Return Value
        End If
    End Function
    Private Function NextYearData(ByVal newDatatable As DataRow, SYear As String, EYear As String) As DataRow
        Dim dr As DataRow
        dr = newDatatable
        Dim Rent As Decimal = 0
        Dim interest As Decimal = 0
        Dim Balance As Decimal = 0
        Dim Security As Decimal = 0
        Dim FYear As Int32 = Convert.ToInt32(SYear)
        Dim TYear As Int32 = Convert.ToInt32(EYear)
        Dim fromdate As Date = New Date(FYear, 3, 31)
        Dim toDate As Date = New Date(TYear, 3, 31)

        Try
            Dim ContractualReceipt As Date
            If Not IsDBNull(dr.Item("F_Contractual Receipt")) Then
                ContractualReceipt = Convert.ToDateTime(dr.Item("F_Contractual Receipt"))
            End If
            Dim DateofDeoosity As Date
            If Not IsDBNull(dr.Item("F_Date of Deposit")) Then
                DateofDeoosity = Convert.ToDateTime(dr.Item("F_Date of Deposit"))
            End If
            '     Dim diff As Int32 = DateDiff(DateInterval.Day, DateofDeoosity, toDate)
            Dim d As Decimal = NotNull(dr.Item("Prepaid rent in form of Deposit"), 0)
            Dim diff1 As Decimal = (NotNull(dr.Item("Security Period"), 0))
            Dim a As Decimal = (DateDiff(DateInterval.Day, DateofDeoosity, toDate) / 365)

            If (toDate > DateofDeoosity AndAlso DateofDeoosity <> DateTime.MinValue) AndAlso NotNull(dr.Item("Security Period"), 0) <> 0 Then
                Rent = (NotNull(dr.Item("Prepaid rent in form of Deposit"), 0) / NotNull(dr.Item("Security Period"), 0)) * (DateDiff(DateInterval.Day, DateofDeoosity, toDate) / 365)
            End If
            'dr.Item("Prepaid rent in form of Deposit") / (dr.Item("Security Period") * DateDiff(DateInterval.Day, DateofDeoosity, toDate) / 365)
            dr.Item("Rent expenses March " & EYear) = RoundUp(Rent, 2)

            If ContractualReceipt <> DateTime.MinValue AndAlso ContractualReceipt > toDate Then
                interest = (NotNull(dr.Item("Amount"), 0) / (1.08 ^ (DateDiff(DateInterval.Day, toDate, ContractualReceipt) / 365))) - NotNull(dr.Item("Present Value of Deposit"), 0)
            End If

            dr.Item("Interest Income March " & EYear) = RoundUp(interest, 2)

            Balance = NotNull(dr.Item("Prepaid rent in form of Deposit"), 0) - Rent

            dr.Item("Balance prepaid Rent March " & EYear) = RoundUp(Balance, 2)

            Security = NotNull(dr.Item("Present Value of Deposit"), 0) + interest

            dr.Item("Security deposit value March " & EYear) = RoundUp(Security, 2)
        Catch Ex As Exception
            Dim sd As String = Ex.ToString

        End Try
        Return dr
    End Function
    Private Function RoundUp(value As Decimal, decimals As Integer) As Decimal
        Return Math.Round(value, decimals)
    End Function
    Private Function YearDiffData(ByVal newdr As DataRow, Prevdr As DataRow, Nextdr As DataRow, SYear As String, EYear As String) As DataRow
        Dim dr As DataRow
        dr = newdr
        Dim Rent As Decimal = 0
        Dim interest As Decimal = 0
        Dim Balance As Decimal = 0
        Dim Security As Decimal = 0
        Dim SDCurrent As Decimal = 0
        Dim SDNoNCurrent As Decimal = 0
        Dim PRCurrent As Decimal = 0
        Dim PRNoNCurrent As Decimal = 0
        Dim PV As Decimal
        Dim COst As Decimal
        Dim FYear As Int32 = Convert.ToInt32(SYear)
        Dim TYear As Int32 = Convert.ToInt32(EYear)
        Dim fromdate As Date = New Date(FYear, 3, 31)
        Dim toDate As Date = New Date(TYear, 3, 31)
        Dim YearAgoDate As Date = New Date(TYear + 1, 3, 31)
        Dim ColumnData As String = SYear & "-" & EYear
        Try
            Dim ContractualReceipt As Date
            If Not IsDBNull(dr.Item("F_Contractual Receipt")) Then
                ContractualReceipt = Convert.ToDateTime(dr.Item("F_Contractual Receipt"))
            End If
            Dim DateofDeoosity As Date
            If Not IsDBNull(dr.Item("F_Date of Deposit")) Then
                DateofDeoosity = Convert.ToDateTime(dr.Item("F_Date of Deposit"))
            End If
            '    Dim diff As Int32 = DateDiff(DateInterval.Day, DateofDeoosity, toDate)
            '    Dim diff1 As Decimal = diff / 365

            Rent = Nextdr.Item("Rent expenses March " & EYear) - Prevdr.Item("Rent expenses March " & SYear)
            interest = Nextdr.Item("Interest Income March " & EYear) - Prevdr.Item("Interest Income March " & SYear)
            Balance = Nextdr.Item("Balance prepaid Rent March " & EYear)
            Security = Nextdr.Item("Security deposit value March " & EYear)
            newdr.Item("Rent expenses " & ColumnData) = Rent
            newdr.Item("Interest Income " & ColumnData) = interest
            newdr.Item("Balance prepaid Rent " & ColumnData) = Balance
            newdr.Item("Security deposit value " & ColumnData) = Security


            If ContractualReceipt > YearAgoDate Then
                SDNoNCurrent = Security
            Else
                SDCurrent = Security
            End If
            newdr.Item("Security deposit Current " & EYear) = SDCurrent
            newdr.Item("Security deposit Non Current " & EYear) = SDNoNCurrent
            ' this column name from database
            If NotNull(Nextdr.Item("Years Above " & EYear & "/03/31"), 0) > 1 Then
                PRCurrent = Nextdr.Item("Balance prepaid Rent March " & EYear) / Nextdr.Item("Years Above " & EYear & "/03/31")
            Else
                PRCurrent = Nextdr.Item("Balance prepaid Rent March " & EYear)
            End If
            newdr.Item("Prepaid Rent Current " & EYear) = RoundUp(PRCurrent, 2)
            PRNoNCurrent = NotNull(Nextdr.Item("Balance prepaid Rent March " & EYear), 0) - PRCurrent
            newdr.Item("Prepaid Rent Non Current " & EYear) = RoundUp(PRNoNCurrent, 2)

            newdr.Item("PV as at Mar 31 " & EYear) = Security
            newdr.Item("Cost as at Mar 31 " & EYear) = NotNull(newdr.Item("Amount"), 0)
        Catch Ex As Exception
            Dim sd As String = Ex.ToString
        End Try
        Return dr
    End Function
    Private Function DynamicHeader(ByVal newDatatable As DataSet, SYear As String, EYear As String) As DataSet
        Dim ds As New DataSet
        ds = newDatatable
        'Dim FromDate As Date = Convert.ToDateTime(sdate)
        ' Dim Year As Int32 = FromDate.Year
        Dim ColumnData As String = SYear & "-" & EYear
        FYDynamicHeader(ds, SYear, EYear)
        'ds.Tables("data").Columns.Add("Years upto " & ColumnData, GetType(String))
        'ds.Tables("data").Columns.Add("Years Above " & ColumnData, GetType(String))
        'ds.Tables("data").Columns.Add("Present Value " & ColumnData, GetType(String))
        ds.Tables("data").Columns.Add("Rent expenses " & ColumnData, GetType(String))
        ds.Tables("data").Columns.Add("Interest Income " & ColumnData, GetType(String))
        ds.Tables("data").Columns.Add("Balance prepaid Rent " & ColumnData, GetType(String))
        ds.Tables("data").Columns.Add("Security deposit value " & ColumnData, GetType(String))


        ds.Tables("data").Columns.Add("Security deposit Current " & EYear, GetType(String))
        ds.Tables("data").Columns.Add("Security deposit Non Current " & EYear, GetType(String))
        ds.Tables("data").Columns.Add("Prepaid Rent Current " & EYear, GetType(String))
        ds.Tables("data").Columns.Add("Prepaid Rent Non Current " & EYear, GetType(String))
        ds.Tables("data").Columns.Add("PV as at Mar 31 " & EYear, GetType(String))
        ds.Tables("data").Columns.Add("Cost as at Mar 31 " & EYear, GetType(String))


        Return ds
    End Function

    Private Function FYDynamicHeader(ByVal newDatatable As DataSet, SYear As String, EYear As String) As DataSet
        Dim ds As New DataSet
        Dim dt As New DataTable
        ds = newDatatable
        Dim FYear As Int32 = Convert.ToInt32(SYear)
        Dim TYear As Int32 = Convert.ToInt32(EYear)

        For j = FYear To TYear
            Dim ColumnData As String = "March " & j
            ds.Tables("data").Columns.Add("Rent expenses " & ColumnData, GetType(String))
            ds.Tables("data").Columns.Add("Interest Income " & ColumnData, GetType(String))
            ds.Tables("data").Columns.Add("Balance prepaid Rent " & ColumnData, GetType(String))
            ds.Tables("data").Columns.Add("Security deposit value " & ColumnData, GetType(String))
        Next
        Return ds
    End Function
    Private Function YearlyDynamicHeader(ByVal newDatatable As DataSet, year As Int32) As DataSet
        Dim ds As New DataSet
        Dim dt As New DataTable
        ds = newDatatable
        Dim dv As DataView = ds.Tables("data").DefaultView
        dv.Sort = "Contractual Receipt DESC"
        dt = dv.ToTable()
        Dim dtMax As DateTime = Convert.ToDateTime(dt.Rows(0)("F_Contractual Receipt"))
        Dim LastYear As Int32 = dtMax.Year
        'Dim dtMin As DateTime = Convert.ToDateTime(dt.Rows(dt.Rows.Count - 1)

        For j = year To LastYear
            Dim ColumnData As String = j & "-" & j + 1
            ds.Tables("data").Columns.Add("Rent expenses " & ColumnData, GetType(String))
            ds.Tables("data").Columns.Add("Interest Income " & ColumnData, GetType(String))
            ds.Tables("data").Columns.Add("Balance prepaid Rent " & ColumnData, GetType(String))
            ds.Tables("data").Columns.Add("Security deposit value " & ColumnData, GetType(String))
        Next
        Return ds
    End Function


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

    Protected Sub show()
        If ViewState("IsViewOn") = 1 Then                     ' Added By Komal on 15Nov213
            'ShowViewsOnReport()
        Else
            '      showViewOffReport()
        End If
    End Sub
    'Show Report from View
    Private Sub BindDdl()
        'Dim da As New SqlDataAdapter("select Tid [Value], fld1   [Text],fld2  from mmm_mst_master where documenttype='Location Master' and Eid=" & Session("Eid"), con)
        'Dim dt As New DataTable
        'da.Fill(dt)
        Dim currentYear As Integer = System.DateTime.Now.Year
        Dim startYear As Integer = 2010
        ddlFYear.DataValueField = "Value"
        ddlFYear.DataTextField = "Text"

        For i As Integer = startYear To currentYear
            'ddlFYear.Items.Add(New ListItem(Convert.ToString(i)))
            ddlFYear.Items.Add(Convert.ToString(i & "-" & i + 1))
        Next
        ' ddlFYear.DataSource = dt
        'ddlFYear.DataBind()

        ddlFYear.Items.Insert(0, "Select")
    End Sub

    Function getdate(ByVal dbt As String) As Date
        Dim dtArr() As String
        dtArr = Split(dbt, "/")
        If dtArr.GetUpperBound(0) = 2 Then
            Dim dd, mm, yy As String
            dd = dtArr(1)
            mm = dtArr(0)
            yy = dtArr(2)
            Dim dt As Date
            Try
                dt = mm & "/" & dd & "/" & yy
                Return dt
            Catch ex As Exception
                Return Now.Date
            End Try
        Else
            Return Now.Date
        End If
    End Function
    'Filteration on Button click












    Function CreateCSV(ByVal dt As DataTable, ByVal path As String) As String
        'Create CSV File here
        Try
            Dim fname As String = path & Now.Year & Now.Month & Now.Day & Now.Hour & Now.Minute & Now.Millisecond & ".CSV"
            Dim FPath As String = HostingEnvironment.MapPath("~/MailAttach/")
            If File.Exists(path & fname) Then
                File.Delete(path & fname)
            End If
            Dim sw As StreamWriter = New StreamWriter(FPath & fname, False)
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
            Return fname
        Catch ex As Exception
            Return ""
            Throw
        Finally
        End Try
    End Function



End Class
