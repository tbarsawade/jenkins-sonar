Imports System.Collections.Generic
Imports System.Linq
Imports System.Web
Imports System.Web.Services
Imports System.Web.UI
Imports System.Web.UI.WebControls
Imports System.Data.SqlClient
Imports System.Data
Imports System.Configuration
Imports System.Web.UI.Adapters.ControlAdapter
Imports System.Drawing
Imports System.Threading
Imports System
Imports System.Collections.Specialized
Imports System.Text
Imports System.Net.Security
Imports System.IO
Imports Newtonsoft.Json.Converters
Imports Newtonsoft.Json
Imports System.Web.Script.Serialization
Imports System.Web.Script.Services
Imports iTextSharp.text.pdf
Imports Ionic.Zip
Imports Microsoft.Office.Interop
Imports System.Web.Hosting

Partial Class PearlDashboard
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
    Private Sub PearlDB_Load(sender As Object, e As EventArgs) Handles Me.Load
        'BindPRtoPO()
        'BindInvPaymentCycle()
        'BindTotalPurchaseReq()
        'BindTotalPurchaseOrder()
        'BindTotalInvoice()
        ' BindTopCategory()
        'BindTopSupplier()
    End Sub

    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Shared Function GetDataPRPOClosure() As String
        Dim jsonData As String = ""
        Try
            Dim ds As New DataSet()
            Dim UID As Integer = 0
            Dim URole As String = ""
            Dim qry As String = ""
            Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
            Dim con As New SqlConnection(conStr)
            Dim dt As New DataTable
            Dim oda As SqlDataAdapter = New SqlDataAdapter("", con)
            qry = "SELECT rootquery from mmm_mst_dashboard where tid=77"
            oda.SelectCommand.CommandType = CommandType.Text
            oda.SelectCommand.CommandText = qry
            oda.Fill(ds, "qry")
            Dim str = ds.Tables("qry").Rows(0).Item(0).ToString
            oda.SelectCommand.CommandType = CommandType.Text
            oda.SelectCommand.CommandText = str
            oda.Fill(ds, "data")
            oda.Fill(dt)
            con.Close()
            dt.Dispose()
            Dim lstColumns As New List(Of String)
            Dim serializerSettings As New JsonSerializerSettings()
            Dim json_serializer As New JavaScriptSerializer()
            serializerSettings.Converters.Add(New DataTableConverter())
            jsonData = JsonConvert.SerializeObject(dt, Newtonsoft.Json.Formatting.None, serializerSettings)
        Catch Ex As Exception
            Throw
        End Try
        Return jsonData
    End Function

    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Shared Function GetDataInvPaymentCycle() As String
        Dim jsonData As String = ""
        Try
            Dim ds As New DataSet()
            Dim UID As Integer = 0
            Dim URole As String = ""
            Dim qry As String = ""
            Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
            Dim con As New SqlConnection(conStr)
            Dim dt As New DataTable
            Dim oda As SqlDataAdapter = New SqlDataAdapter("", con)
            qry = "Select rootquery from mmm_mst_dashboard where tid=78"
            oda.SelectCommand.CommandType = CommandType.Text
            oda.SelectCommand.CommandText = qry
            oda.Fill(ds, "qry")
            Dim str = ds.Tables("qry").Rows(0).Item(0).ToString
            oda.SelectCommand.CommandType = CommandType.Text
            oda.SelectCommand.CommandText = str
            oda.Fill(ds, "data")
            oda.Fill(dt)
            con.Close()
            dt.Dispose()
            Dim lstColumns As New List(Of String)
            Dim serializerSettings As New JsonSerializerSettings()
            Dim json_serializer As New JavaScriptSerializer()
            serializerSettings.Converters.Add(New DataTableConverter())
            jsonData = JsonConvert.SerializeObject(dt, Newtonsoft.Json.Formatting.None, serializerSettings)
        Catch Ex As Exception
            Throw
        End Try
        Return jsonData
    End Function

    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Shared Function GetDataTotalInvoice() As vdashboard
        Dim ds As New DataSet
        Dim dt As New DataTable
        Dim vtm As New VehicleTKM
        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Try
            Using oda As New SqlDataAdapter("", conStr)
                Dim qry As String = ""
                qry = "SELECT rootquery from mmm_mst_dashboard where tid=81"
                oda.SelectCommand.CommandType = CommandType.Text
                oda.SelectCommand.CommandText = qry
                oda.Fill(dt)
                Dim str = dt.Rows(0).Item(0).ToString
                oda.SelectCommand.CommandType = CommandType.Text
                oda.SelectCommand.CommandText = str
                oda.Fill(ds, "data")
            End Using


            Dim CatList As New List(Of String)

            Dim objPending As New series()
            objPending.name = "Pending"
            Dim objApproved As New series()
            objApproved.color = "rgb(80, 180, 50)"
            objApproved.name = "Approved"
            objPending.color = "rgb(248, 161, 63)"

            Dim LstPendData As New List(Of Integer)
            Dim LstApprovedData As New List(Of Integer)
            For i = 0 To ds.Tables(0).Rows.Count - 1
                CatList.Add(ds.Tables(0).Rows(i).Item("category"))
                LstPendData.Add(ds.Tables(0).Rows(i).Item("Pending"))
                LstApprovedData.Add(ds.Tables(0).Rows(i).Item("Approved"))
            Next
            objPending.data = LstPendData
            objApproved.data = LstApprovedData
            Dim LstSeries As New List(Of series)
            Dim Res As New vdashboard()
            LstSeries.Add(objPending)
            LstSeries.Add(objApproved)
            Res.categoryAxis = CatList
            Res.series = LstSeries
            Return Res
        Catch ex As Exception
            Throw
        Finally
            ds.Dispose()
            dt.Dispose()
        End Try

    End Function

    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Shared Function GetDataTotalPurchaseRequest() As vdashboard
        Dim ds As New DataSet
        Dim dt As New DataTable
        Dim vtm As New VehicleTKM
        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Try
            Using oda As New SqlDataAdapter("", conStr)
                Dim qry As String = ""
                qry = "SELECT rootquery from mmm_mst_dashboard where tid=79"
                oda.SelectCommand.CommandType = CommandType.Text
                oda.SelectCommand.CommandText = qry
                oda.Fill(dt)
                Dim str = dt.Rows(0).Item(0).ToString
                oda.SelectCommand.CommandType = CommandType.Text
                oda.SelectCommand.CommandText = str
                'oda.SelectCommand.CommandText = "Select 'TCS'[category],'48000'[value] union select 'CGI','37000' union select 'IBM','26000' union select 'Infosys','25000' union select 'Wipro','25000'  "
                oda.Fill(ds, "data")
            End Using


            Dim CatList As New List(Of String)

            Dim objPending As New series()
            objPending.name = "Pending"
            Dim objApproved As New series()
            objApproved.color = "rgb(80, 180, 50)"
            objApproved.name = "Approved"
            objPending.color = "rgb(248, 161, 63)"
            Dim LstPendData As New List(Of Integer)
            Dim LstApprovedData As New List(Of Integer)
            For i = 0 To ds.Tables(0).Rows.Count - 1
                CatList.Add(ds.Tables(0).Rows(i).Item("category"))
                LstPendData.Add(ds.Tables(0).Rows(i).Item("Pending"))
                LstApprovedData.Add(ds.Tables(0).Rows(i).Item("Approved"))
            Next
            objPending.data = LstPendData
            objApproved.data = LstApprovedData
            Dim LstSeries As New List(Of series)
            Dim Res As New vdashboard()
            LstSeries.Add(objPending)
            LstSeries.Add(objApproved)
            Res.categoryAxis = CatList
            Res.series = LstSeries
            Return Res
        Catch ex As Exception
            Throw
        Finally
            ds.Dispose()
            dt.Dispose()

        End Try

    End Function

    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Shared Function GetDataTotalPurchaseOrder() As vdashboard
        Dim ds As New DataSet
        Dim dt As New DataTable
        Dim vtm As New VehicleTKM
        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Try
            Using oda As New SqlDataAdapter("", conStr)
                Dim qry As String = ""
                qry = "SELECT rootquery from mmm_mst_dashboard where tid=80"
                oda.SelectCommand.CommandType = CommandType.Text
                oda.SelectCommand.CommandText = qry
                oda.Fill(dt)
                Dim str = dt.Rows(0).Item(0).ToString
                oda.SelectCommand.CommandType = CommandType.Text
                oda.SelectCommand.CommandText = str
                'oda.SelectCommand.CommandText = "Select 'TCS'[category],'48000'[value] union select 'CGI','37000' union select 'IBM','26000' union select 'Infosys','25000' union select 'Wipro','25000'  "
                oda.Fill(ds, "data")
            End Using


            Dim CatList As New List(Of String)

            Dim objPending As New series()
            objPending.name = "Pending"
            Dim objApproved As New series()
            objApproved.color = "rgb(80, 180, 50)"
            objApproved.name = "Approved"
            objPending.color = "rgb(248, 161, 63)"
            Dim LstPendData As New List(Of Integer)
            Dim LstApprovedData As New List(Of Integer)
            For i = 0 To ds.Tables(0).Rows.Count - 1
                CatList.Add(ds.Tables(0).Rows(i).Item("category"))
                LstPendData.Add(ds.Tables(0).Rows(i).Item("Pending"))
                LstApprovedData.Add(ds.Tables(0).Rows(i).Item("Approved"))
            Next
            objPending.data = LstPendData
            objApproved.data = LstApprovedData
            Dim LstSeries As New List(Of series)
            Dim Res As New vdashboard()
            LstSeries.Add(objPending)
            LstSeries.Add(objApproved)
            Res.categoryAxis = CatList
            Res.series = LstSeries
            Return Res
        Catch ex As Exception
            Throw
        Finally
            ds.Dispose()
            dt.Dispose()
        End Try

    End Function

    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Shared Function POBreakup() As String
        Dim jsonData As String = ""
        Try
            Dim ds As New DataSet()
            Dim UID As Integer = 0
            Dim URole As String = ""
            Dim qry As String = ""
            Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
            Dim con As New SqlConnection(conStr)
            Dim dt As New DataTable
            Dim oda As SqlDataAdapter = New SqlDataAdapter("", con)
            qry = "SELECT rootquery from mmm_mst_dashboard where tid=82"
            oda.SelectCommand.CommandType = CommandType.Text
            oda.SelectCommand.CommandText = qry
            oda.Fill(ds, "qry")
            Dim str = ds.Tables("qry").Rows(0).Item(0).ToString
            oda.SelectCommand.CommandType = CommandType.Text
            oda.SelectCommand.CommandText = str
            oda.Fill(ds, "data")
            oda.Fill(dt)
            con.Close()
            dt.Dispose()
            Dim lstColumns As New List(Of String)
            Dim serializerSettings As New JsonSerializerSettings()
            Dim json_serializer As New JavaScriptSerializer()
            serializerSettings.Converters.Add(New DataTableConverter())
            jsonData = JsonConvert.SerializeObject(dt, Newtonsoft.Json.Formatting.None, serializerSettings)
        Catch Ex As Exception
            Throw
        End Try
        Return jsonData
    End Function

    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Shared Function GetDataTopCategory() As String
        Dim jsonData As String = ""
        Try
            Dim ds As New DataSet()
            Dim UID As Integer = 0
            Dim URole As String = ""
            Dim qry As String = ""
            Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
            Dim con As New SqlConnection(conStr)
            Dim dt As New DataTable
            Dim oda As SqlDataAdapter = New SqlDataAdapter("", con)
            qry = "SELECT rootquery from mmm_mst_dashboard where tid=83"
            oda.SelectCommand.CommandType = CommandType.Text
            oda.SelectCommand.CommandText = qry
            oda.Fill(ds, "qry")
            Dim str = ds.Tables("qry").Rows(0).Item(0).ToString
            oda.SelectCommand.CommandType = CommandType.Text
            oda.SelectCommand.CommandText = str
            oda.SelectCommand.CommandText = str
            oda.Fill(ds, "data")
            oda.Fill(dt)
            con.Close()
            dt.Dispose()
            Dim lstColumns As New List(Of String)
            Dim serializerSettings As New JsonSerializerSettings()
            Dim json_serializer As New JavaScriptSerializer()
            serializerSettings.Converters.Add(New DataTableConverter())
            jsonData = JsonConvert.SerializeObject(dt, Newtonsoft.Json.Formatting.None, serializerSettings)
        Catch Ex As Exception
            Throw
        End Try
        Return jsonData
    End Function

    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Shared Function GetDataTopSupplier() As String
        Dim jsonData As String = ""
        Try
            Dim ds As New DataSet()
            Dim UID As Integer = 0
            Dim URole As String = ""
            Dim qry As String = ""
            Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
            Dim con As New SqlConnection(conStr)
            Dim dt As New DataTable
            Dim oda As SqlDataAdapter = New SqlDataAdapter("", con)
            qry = "SELECT rootquery from mmm_mst_dashboard where tid=84"
            oda.SelectCommand.CommandType = CommandType.Text
            oda.SelectCommand.CommandText = qry
            oda.Fill(ds, "qry")
            Dim str = ds.Tables("qry").Rows(0).Item(0).ToString
            oda.SelectCommand.CommandType = CommandType.Text
            oda.SelectCommand.CommandText = str
            oda.SelectCommand.CommandText = str
            'oda.SelectCommand.CommandText = "Select 'TCS'[category],'48000'[value] union select 'CGI','37000' union select 'IBM','26000' union select 'Infosys','25000' union select 'Wipro','25000'  "
            oda.Fill(ds, "data")
            oda.Fill(dt)
            con.Close()
            dt.Dispose()
            Dim lstColumns As New List(Of String)
            Dim serializerSettings As New JsonSerializerSettings()
            Dim json_serializer As New JavaScriptSerializer()
            serializerSettings.Converters.Add(New DataTableConverter())
            jsonData = JsonConvert.SerializeObject(dt, Newtonsoft.Json.Formatting.None, serializerSettings)
        Catch Ex As Exception
            Throw
        End Try
        Return jsonData
    End Function

    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Shared Function getPRTOPOCLOSUREDtl(Month As String) As DGrid
        Dim jsonData As String = ""
        Dim grid As New DGrid()
        Try
            Dim ds As New DataSet()
            Dim Query As String = ""
            Dim dt As New DataTable
            Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
            Using con As New SqlConnection(conStr)
                Using oda As New SqlDataAdapter(Query, con)
                    Dim qry As String = ""
                    qry = "SELECT firstlevelquery from mmm_mst_dashboard where tid=77"
                    oda.SelectCommand.CommandType = CommandType.Text
                    oda.SelectCommand.CommandText = qry
                    oda.Fill(dt)
                    Dim str = dt.Rows(0).Item(0).ToString
                    str = str.Replace("@mnt", Month)
                    oda.SelectCommand.CommandType = CommandType.Text
                    oda.SelectCommand.CommandText = str
                    oda.Fill(ds)
                End Using
            End Using
            Dim strError = ""
            grid = DynamicGrid.GridData(ds.Tables(0), strError)
            If ds.Tables(0).Rows.Count = 0 Then
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

    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Shared Function getTotalPRDtl(Month As String, Status As String) As DGrid
        Dim jsonData As String = ""
        Dim grid As New DGrid()
        Try
            Dim ds As New DataSet()
            Dim Query As String = ""
            Dim dt As New DataTable
            Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
            Using con As New SqlConnection(conStr)
                Using oda As New SqlDataAdapter(Query, con)
                    Dim qry As String = ""
                    qry = "SELECT firstlevelquery from mmm_mst_dashboard where tid=79"
                    oda.SelectCommand.CommandType = CommandType.Text
                    oda.SelectCommand.CommandText = qry
                    oda.Fill(dt)
                    Dim str = dt.Rows(0).Item(0).ToString
                    If Status = "Pending" Then
                        str &= "and d.curstatus<>'ARCHIVE'"
                    Else
                        str &= "and d.curstatus='ARCHIVE'"
                    End If
                    If Month = "2018" Or Month = "2019" Or Month = "2020" Then
                        str &= "and DATENAME(Year, d.adate) ='" & Month & "'"
                    Else
                        str &= "and DATENAME(Month, d.adate) ='" & Month & "'"
                    End If

                    oda.SelectCommand.CommandType = CommandType.Text
                    oda.SelectCommand.CommandText = str
                    oda.Fill(ds)
                End Using
            End Using

            Dim strError = ""
            grid = DynamicGrid.GridData(ds.Tables(0), strError)
            If ds.Tables(0).Rows.Count = 0 Then
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

    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Shared Function getTotalPODtl(Month As String, Status As String) As DGrid
        Dim jsonData As String = ""
        Dim grid As New DGrid()
        Try
            Dim ds As New DataSet()
            Dim Query As String = ""
            Dim dt As New DataTable
            Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
            Using con As New SqlConnection(conStr)
                Using oda As New SqlDataAdapter(Query, con)
                    Dim qry As String = ""
                    qry = "SELECT firstlevelquery from mmm_mst_dashboard where tid=80"
                    oda.SelectCommand.CommandType = CommandType.Text
                    oda.SelectCommand.CommandText = qry
                    oda.Fill(dt)
                    Dim str = dt.Rows(0).Item(0).ToString
                    If Status = "Pending" Then
                        str &= "and d.curstatus<>'ARCHIVE'"
                    Else
                        str &= "and d.curstatus='ARCHIVE'"
                    End If
                    If Month = "2018" Or Month = "2019" Or Month = "2020" Then
                        str &= " and DATENAME(Year, d.adate) ='" & Month & "'"
                    Else
                        str &= " and DATENAME(Month, d.adate) ='" & Month & "'"
                    End If

                    oda.SelectCommand.CommandType = CommandType.Text
                    oda.SelectCommand.CommandText = str
                    oda.Fill(ds)
                End Using
            End Using

            Dim strError = ""
            grid = DynamicGrid.GridData(ds.Tables(0), strError)
            If ds.Tables(0).Rows.Count = 0 Then
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

    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Shared Function getTotalInvoice(Month As String, Status As String) As DGrid
        Dim jsonData As String = ""
        Dim grid As New DGrid()
        Try
            Dim ds As New DataSet()
            Dim Query As String = ""
            Dim dt As New DataTable
            Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
            Using con As New SqlConnection(conStr)
                Using oda As New SqlDataAdapter(Query, con)
                    Dim qry As String = ""
                    qry = "SELECT firstlevelquery from mmm_mst_dashboard where tid=81"
                    oda.SelectCommand.CommandType = CommandType.Text
                    oda.SelectCommand.CommandText = qry
                    oda.Fill(dt)
                    Dim str = dt.Rows(0).Item(0).ToString
                    If Status = "Pending" Then
                        str &= "and d.curstatus<>'ARCHIVE'"
                    Else
                        str &= "and d.curstatus='ARCHIVE'"
                    End If
                    If Month = "2018" Or Month = "2019" Or Month = "2020" Then
                        str &= " and DATENAME(Year, d.adate) ='" & Month & "'"
                    Else
                        str &= " and DATENAME(Month, d.adate) ='" & Month & "'"
                    End If

                    oda.SelectCommand.CommandType = CommandType.Text
                    oda.SelectCommand.CommandText = str
                    oda.Fill(ds)
                End Using
            End Using

            Dim strError = ""
            grid = DynamicGrid.GridData(ds.Tables(0), strError)
            If ds.Tables(0).Rows.Count = 0 Then
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

    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Shared Function getInvoicePayemntDtl(Month As String) As DGrid
        Dim jsonData As String = ""
        Dim grid As New DGrid()
        Try
            Dim ds As New DataSet()
            Dim Query As String = ""
            Dim dt As New DataTable
            Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
            Using con As New SqlConnection(conStr)
                Using oda As New SqlDataAdapter(Query, con)
                    Dim qry As String = ""
                    qry = "SELECT firstlevelquery from mmm_mst_dashboard where tid=77"
                    oda.SelectCommand.CommandType = CommandType.Text
                    oda.SelectCommand.CommandText = qry
                    oda.Fill(dt)
                    Dim str = dt.Rows(0).Item(0).ToString
                    str = str.Replace("@mnt", Month)
                    oda.SelectCommand.CommandType = CommandType.Text
                    oda.SelectCommand.CommandText = str
                    oda.Fill(ds)
                End Using
            End Using
            Dim strError = ""
            grid = DynamicGrid.GridData(ds.Tables(0), strError)
            If ds.Tables(0).Rows.Count = 0 Then
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

    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Shared Function getTopSupplierdtl(Vname As String) As DGrid
        Dim jsonData As String = ""
        Dim grid As New DGrid()
        Try
            Dim ds As New DataSet()
            Dim Query As String = ""
            Dim dt As New DataTable
            Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
            Using con As New SqlConnection(conStr)
                Using oda As New SqlDataAdapter(Query, con)
                    Dim qry As String = ""
                    qry = "SELECT firstlevelquery from mmm_mst_dashboard where tid=84"
                    oda.SelectCommand.CommandType = CommandType.Text
                    oda.SelectCommand.CommandText = qry
                    oda.Fill(dt)
                    Dim str = dt.Rows(0).Item(0).ToString
                    str = str.Replace("@vendor", Vname)
                    oda.SelectCommand.CommandType = CommandType.Text
                    oda.SelectCommand.CommandText = str
                    oda.Fill(ds)
                End Using
            End Using
            Dim strError = ""
            grid = DynamicGrid.GridData(ds.Tables(0), strError)
            If ds.Tables(0).Rows.Count = 0 Then
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

    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Shared Function getdtlPRPOBREAKUP(Type As String) As DGrid
        Dim jsonData As String = ""
        Dim grid As New DGrid()
        Try
            Dim ds As New DataSet()
            Dim Query As String = ""
            Dim dt As New DataTable
            Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
            Using con As New SqlConnection(conStr)
                Using da As New SqlDataAdapter("USP_GetPRPODTL", con)
                    da.SelectCommand.CommandType = CommandType.StoredProcedure
                    da.SelectCommand.Parameters.AddWithValue("@Type", Type)
                    da.SelectCommand.CommandTimeout = 120
                    da.Fill(ds)
                End Using
            End Using
            Dim strError = ""
            grid = DynamicGrid.GridData(ds.Tables(0), strError)
            If ds.Tables(0).Rows.Count = 0 Then
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
    Public Class vdashboard
        Public Property series As List(Of series)
        Public Property categoryAxis As List(Of String)
        Public Property HasSession As String
    End Class
    Public Class series
        Public Property name As String
        Public Property data As List(Of Integer)
        Public Property color As String
    End Class
    Public Class categoryAxisTkm
        Public Property categories As List(Of String)
    End Class
    Public Class VehicleTKM
        Public Property success As Boolean
        Public Property msg As String
        Public Property series As Object
        Public Property cata As Object
    End Class
    Public Class seriesTKm
        Public Property name As String
        Public Property data As String
    End Class

End Class
