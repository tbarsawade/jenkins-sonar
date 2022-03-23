Imports System.Web.Services
Imports System.Data
Imports Newtonsoft.Json
Imports Newtonsoft.Json.Converters
Imports System.Data.SqlClient

Imports System.Web.Script.Serialization


Partial Class Userdashboard
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

    <WebMethod()> _
    Public Shared Function GetSites() As String
        Dim ConStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim ret As String = ""
        Dim ds As New DataSet()
        Dim jsonData As String = ""
        Dim serializerSettings As New JsonSerializerSettings()
        serializerSettings.Converters.Add(New DataTableConverter())
        Try
            Using con = New SqlConnection(ConStr)
                Using da = New SqlDataAdapter("Get_EcompDashboard", con)
                    da.SelectCommand.CommandType = CommandType.StoredProcedure
                    da.SelectCommand.Parameters.AddWithValue("@Type", "Company")
                    da.SelectCommand.Parameters.AddWithValue("@CompanyId", 1076419)
                    da.SelectCommand.Parameters.AddWithValue("@SMonth", 10)
                    da.SelectCommand.Parameters.AddWithValue("@SYear", 2016)
                    da.SelectCommand.Parameters.AddWithValue("@TYear", 2016)
                    da.SelectCommand.Parameters.AddWithValue("@TMonth", 11)
                    da.Fill(ds)
                End Using
            End Using
            If ds.Tables(0).Rows.Count > 0 Then
                jsonData = JsonConvert.SerializeObject(ds.Tables(0), Newtonsoft.Json.Formatting.None, serializerSettings)
                ret = jsonData
            End If
            Return ret
        Catch ex As Exception
        End Try
        Return ret
    End Function

    <WebMethod()> _
       <Script.Services.ScriptMethod()> _
    Public Shared Function GetHomeDet(CompanyID As String, Uid As String, SMonth As String, SYear As String, TMonth As String, TYear As String) As String
        Dim objdashbrd = New ecompdashboard1
        Dim jsonData As String = ""
        Dim ret As String = ""
        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim con As New SqlConnection(conStr)
        Dim Qry As String = "Get_EcompDashboard"
        Dim cmd As New SqlCommand(Qry, con)
        Dim da As New SqlDataAdapter(cmd)
        Dim dt As New DataTable()
        cmd.CommandText = Qry
        cmd.CommandType = CommandType.StoredProcedure
        cmd.Parameters.AddWithValue("@Type", "HomeDet")
        cmd.Parameters.AddWithValue("@CompanyId", Convert.ToInt32(CompanyID))
        cmd.Parameters.AddWithValue("@SMonth", Convert.ToInt32(SMonth))
        cmd.Parameters.AddWithValue("@SYear", Convert.ToInt32(SYear))
        cmd.Parameters.AddWithValue("@TMonth", Convert.ToInt32(TMonth))
        cmd.Parameters.AddWithValue("@TYear", Convert.ToInt32(TYear))
        cmd.Parameters.AddWithValue("@uid", Convert.ToInt32(uid))
        Using con
            Using da
                da.Fill(dt)
            End Using
        End Using
        For i As Integer = 0 To dt.Columns.Count - 1
            dt.Columns(i).ColumnName = Replace(dt.Columns(i).ColumnName, " ", "_")
        Next

        Dim serializerSettings As New JsonSerializerSettings()
        Dim json_serializer As New JavaScriptSerializer()

        serializerSettings.Converters.Add(New DataTableConverter())

        jsonData = JsonConvert.SerializeObject(dt, Newtonsoft.Json.Formatting.None, serializerSettings)
        Return jsonData


    End Function

    <WebMethod()> _
    <Script.Services.ScriptMethod()> _
    Public Shared Function GetActwiseChart(CompanyID As String, SMonth As String, SYear As String, TMonth As String, TYear As String) As ecompdashboard1
        Dim objdashbrd = New ecompdashboard1

        Dim ret As String = ""
        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim con As New SqlConnection(conStr)
        Dim Qry As String = "Get_EcompDashboard"
        Dim cmd As New SqlCommand(Qry, con)
        Dim da As New SqlDataAdapter(cmd)
        Dim dt As New DataTable()
        cmd.CommandText = Qry
        cmd.CommandType = CommandType.StoredProcedure
        cmd.Parameters.AddWithValue("@Type", "Actwisetot")
        cmd.Parameters.AddWithValue("@CompanyId", Convert.ToInt32(CompanyID))
        cmd.Parameters.AddWithValue("@SMonth", Convert.ToInt32(SMonth))
        cmd.Parameters.AddWithValue("@SYear", Convert.ToInt32(SYear))
        cmd.Parameters.AddWithValue("@TMonth", Convert.ToInt32(TMonth))
        cmd.Parameters.AddWithValue("@TYear", Convert.ToInt32(TYear))
        Using con
            Using da
                da.Fill(dt)
            End Using
        End Using
        Dim lstcataxis As New List(Of String)

        Dim objseriesdelayed As New ecompseries1
        objseriesdelayed.name = "Delayed"
        objseriesdelayed.color = "#124ba0"
        Dim delayeddata As New List(Of Double)

        Dim objseriesperformed As New ecompseries1
        objseriesperformed.name = "Performed"
        objseriesperformed.color = "green"
        Dim performeddata As New List(Of Double)

        Dim objseriesperfaftrdt As New ecompseries1
        objseriesperfaftrdt.name = "Performed after due date"
        objseriesperfaftrdt.color = "red"
        Dim performdaftrdtdata As New List(Of Double)

        Dim objserieInprocess As New ecompseries1
        objserieInprocess.name = "In Process"
        objserieInprocess.color = "#FF9900"
        Dim InProcessdata As New List(Of Double)


        For i As Integer = 0 To dt.Rows.Count - 1
            Dim delayedcount As Int32 = Convert.ToInt32(dt.Rows(i).Item("Delayed"))
            Dim performededcount As Int32 = Convert.ToInt32(dt.Rows(i).Item("Performed"))
            Dim peraftrdtdcount As Int32 = Convert.ToInt32(dt.Rows(i).Item("Performed After Due Date"))
            Dim InProcesscount As Int32 = Convert.ToInt32(dt.Rows(i).Item("InProcess"))
            delayeddata.Add(Convert.ToDouble(delayedcount))
            performeddata.Add(Convert.ToDouble(performededcount))
            performdaftrdtdata.Add(Convert.ToDouble(peraftrdtdcount))
            InProcessdata.Add(InProcesscount)
            Dim cattot As Integer = delayedcount + performededcount + peraftrdtdcount + InProcesscount

            'delayeddata.Add(Math.Round(((Convert.ToInt32(dt.Rows(i).Item("Delayed")) / (delayedcount + performededcount + peraftrdtdcount + InProcesscount)) * 100), 2))
            'performeddata.Add(Math.Round(((Convert.ToInt32(dt.Rows(i).Item("Performed")) / (delayedcount + performededcount + peraftrdtdcount + InProcesscount)) * 100), 2))
            'performdaftrdtdata.Add(Math.Round(((Convert.ToInt32(dt.Rows(i).Item("Performed After Due Date")) / (delayedcount + performededcount + peraftrdtdcount + InProcesscount)) * 100), 2))
            'InProcessdata.Add(Math.Round(((Convert.ToInt32(dt.Rows(i).Item("InProcess")) / (delayedcount + performededcount + peraftrdtdcount + InProcesscount)) * 100), 2))
            lstcataxis.Add(dt.Rows(i).Item("Act").ToString() + ":," + dt.Rows(i).Item("ActID").ToString() + ":," + cattot.ToString())
        Next


        objseriesperformed.data = performeddata
        objseriesdelayed.data = delayeddata
        objseriesperfaftrdt.data = performdaftrdtdata
        objserieInprocess.data = InProcessdata

        Dim lstseries As New List(Of ecompseries1)

        lstseries.Add(objseriesperformed)
        lstseries.Add(objseriesperfaftrdt)
        lstseries.Add(objserieInprocess)
        lstseries.Add(objseriesdelayed)

        objdashbrd.categoryAxis = lstcataxis
        objdashbrd.series1 = lstseries

        Return objdashbrd

    End Function

    <WebMethod()> _
    <Script.Services.ScriptMethod()> _
    Public Shared Function GetPieChart(CompanyID As String, SMonth As String, SYear As String, TMonth As String, TYear As String) As List(Of ecomppiedata)
        Dim listdata As New List(Of ecomppiedata)

        Dim objdashbrd = New ecompdashboard1

        Dim ret As String = ""
        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim con As New SqlConnection(conStr)
        Dim Qry As String = "Get_EcompDashboard"
        Dim cmd As New SqlCommand(Qry, con)
        Dim da As New SqlDataAdapter(cmd)
        Dim dt As New DataTable()
        cmd.CommandText = Qry
        cmd.CommandType = CommandType.StoredProcedure
        cmd.Parameters.AddWithValue("@Type", "Companytot")
        cmd.Parameters.AddWithValue("@CompanyId", Convert.ToInt32(CompanyID))
        cmd.Parameters.AddWithValue("@SMonth", Convert.ToInt32(SMonth))
        cmd.Parameters.AddWithValue("@SYear", Convert.ToInt32(SYear))
        cmd.Parameters.AddWithValue("@TMonth", Convert.ToInt32(TMonth))
        cmd.Parameters.AddWithValue("@TYear", Convert.ToInt32(TYear))
        Using con
            Using da
                da.Fill(dt)
            End Using
        End Using

        Dim objperformed As New ecomppiedata
        Dim objNotperformed As New ecomppiedata
        Dim objDelayed As New ecomppiedata
        Dim objInProcess As New ecomppiedata

        If (dt.Rows.Count > 0) Then
            Dim performedcount = Convert.ToInt16(dt.Rows(0).Item("Performed"))
            Dim perfrmedduedtcount = Convert.ToInt16(dt.Rows(0).Item("Performed after due date"))
            Dim delayedcount = Convert.ToInt16(dt.Rows(0).Item("Delayed"))
            Dim InProcesscount = Convert.ToInt16(dt.Rows(0).Item("InProcess"))
            Dim totcount = performedcount + perfrmedduedtcount + delayedcount + InProcesscount

            objperformed.category = "Performed"
            objperformed.value = Math.Round(Convert.ToDouble((performedcount / totcount) * 100), 2)
            objperformed.color = "green"
            listdata.Add(objperformed)
            objNotperformed.category = "Performed after due date"
            objNotperformed.value = Math.Round(Convert.ToDouble((perfrmedduedtcount / totcount) * 100), 2)
            objNotperformed.color = "red"
            listdata.Add(objNotperformed)
            objInProcess.category = "In Process"
            objInProcess.value = Math.Round(Convert.ToDouble((InProcesscount / totcount) * 100), 2)
            objInProcess.color = "#FF9900"
            listdata.Add(objInProcess)
            objDelayed.category = "Delayed"
            objDelayed.value = Math.Round(Convert.ToDouble((delayedcount / totcount) * 100), 2)
            objDelayed.color = "#124ba0"
            listdata.Add(objDelayed)
        End If

        Return listdata

    End Function

    <WebMethod()> _
    <Script.Services.ScriptMethod()> _
    Public Shared Function GetActclickGrid(CompanyID As String, ActID As String, Status As String, SMonth As String, SYear As String, TMonth As String, TYear As String) As ecompReport
        Dim jsonData As String = ""

        Dim res As New ecompReport()
        Dim lstColumns As New List(Of ecompgrdcolumns)
        Dim objColumn As ecompgrdcolumns

        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim con As New SqlConnection(conStr)
        Dim Qry As String = "Get_EcompDashboard"
        Dim cmd As New SqlCommand(Qry, con)
        Dim da As New SqlDataAdapter(cmd)
        Dim dt As New DataTable()
        cmd.CommandText = Qry
        cmd.CommandType = CommandType.StoredProcedure
        cmd.Parameters.AddWithValue("@Type", "ActclickGrid")
        cmd.Parameters.AddWithValue("@CompanyId", Convert.ToInt32(CompanyID))
        cmd.Parameters.AddWithValue("@ActID", Convert.ToInt32(ActID))
        cmd.Parameters.AddWithValue("@Status", Status)
        cmd.Parameters.AddWithValue("@SMonth", Convert.ToInt32(SMonth))
        cmd.Parameters.AddWithValue("@SYear", Convert.ToInt32(SYear))
        cmd.Parameters.AddWithValue("@TMonth", Convert.ToInt32(TMonth))
        cmd.Parameters.AddWithValue("@TYear", Convert.ToInt32(TYear))
        Using con
            Using da
                da.Fill(dt)
            End Using
        End Using

        For i As Integer = 0 To dt.Columns.Count - 1

            objColumn = New ecompgrdcolumns()
            objColumn.field = Replace(dt.Columns(i).ColumnName, " ", "_")
            objColumn.title = dt.Columns(i).ColumnName
            lstColumns.Add(objColumn)
            dt.Columns(i).ColumnName = Replace(dt.Columns(i).ColumnName, " ", "_")

        Next


        Dim serializerSettings As New JsonSerializerSettings()
        Dim json_serializer As New JavaScriptSerializer()

        serializerSettings.Converters.Add(New DataTableConverter())


        jsonData = JsonConvert.SerializeObject(dt, Newtonsoft.Json.Formatting.None, serializerSettings)
        res.data = jsonData
        res.columns = lstColumns
        Return res
    End Function
End Class


Public Class ecompdashboard1
    Public Property series1 As List(Of ecompseries1)
    Public Property categoryAxis As List(Of String)
End Class
Public Class ecompdashboard21
    Public Property series1 As List(Of ecompseries2)
    Public Property categoryAxis As List(Of String)
End Class
Public Class ecompseries2
    Public Property name As String
    Public Property data As List(Of Int32)
    Public Property color As String
End Class
Public Class ecompseries1
    Public Property name As String
    Public Property data As List(Of Double)
    Public Property color As String
    Public Property total As Integer
End Class
Public Class ecompReport
    Public Property data As String
    Public Property columns As List(Of ecompgrdcolumns)
End Class
Public Class ecomppiedata
    Public Property category As String
    Public Property value As Double
    Public Property color As String
End Class

Public Class ecompgrdcolumns
    Public Property field As String
    Public Property title As String
    Public Property groupFooterTemplate As String = ""
    Public Property groupHeaderTemplate As String = ""
    Public Property aggregates As String = ""
    Public Property type As String = "string"
    '  Public Property template As String = ""
End Class
