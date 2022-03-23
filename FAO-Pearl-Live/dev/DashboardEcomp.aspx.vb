Imports System.Web.Services
Imports System.Data
Imports System.Data.SqlClient
Imports Newtonsoft.Json
Imports System.Web.Script.Serialization
Imports Newtonsoft.Json.Converters

Partial Class DashboardEcomp
    Inherits System.Web.UI.Page



    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Shared Function GetCompChart(CompanyID As String, SMonth As String, SYear As String, TMonth As String, TYear As String) As ecompdashboard_1
        Dim objdashbrd = New ecompdashboard_1

        Dim ret As String = ""
        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim con As New SqlConnection(conStr)
        Dim Qry As String = "Get_EcompDashboard"
        Dim cmd As New SqlCommand(Qry, con)
        Dim da As New SqlDataAdapter(cmd)
        Dim dt As New DataTable()
        cmd.CommandText = Qry
        cmd.CommandType = CommandType.StoredProcedure
        cmd.Parameters.AddWithValue("@Type", "Company")
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

        Dim objseriesdelayed As New ecompseries_1
        objseriesdelayed.name = "Delayed"
        Dim delayeddata As New List(Of Double)

        Dim objseriesperformed As New ecompseries_1
        objseriesperformed.name = "Performed"
        Dim performeddata As New List(Of Double)
        Dim objseriesperfaftrdt As New ecompseries_1
        objseriesperfaftrdt.name = "Performed after due date"
        Dim performdaftrdtdata As New List(Of Double)
        For i As Integer = 0 To dt.Rows.Count - 1
            Dim delayedcount As Int32 = Convert.ToInt32(dt.Rows(i).Item("Delayed"))
            Dim performededcount As Int32 = Convert.ToInt32(dt.Rows(i).Item("Performed"))
            Dim peraftrdtdcount As Int32 = Convert.ToInt32(dt.Rows(i).Item("Performed After Due Date"))
            delayeddata.Add(Math.Round(((Convert.ToInt32(dt.Rows(i).Item("Delayed")) / (delayedcount + performededcount + peraftrdtdcount)) * 100), 2))
            performeddata.Add(Math.Round(((Convert.ToInt32(dt.Rows(i).Item("Performed")) / (delayedcount + performededcount + peraftrdtdcount)) * 100), 2))
            performdaftrdtdata.Add(Math.Round(((Convert.ToInt32(dt.Rows(i).Item("Performed After Due Date")) / (delayedcount + performededcount + peraftrdtdcount)) * 100), 2))
            lstcataxis.Add(dt.Rows(i).Item("Site").ToString() + ":," + dt.Rows(i).Item("SiteID").ToString())
        Next
        objseriesperformed.data = performeddata
        objseriesdelayed.data = delayeddata
        objseriesperfaftrdt.data = performdaftrdtdata
        Dim lstseries As New List(Of ecompseries_1)

        lstseries.Add(objseriesdelayed)
        lstseries.Add(objseriesperformed)
        lstseries.Add(objseriesperfaftrdt)
        objdashbrd.categoryAxis = lstcataxis
        objdashbrd.series1 = lstseries



        Return objdashbrd

    End Function

    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Shared Function GetSiteChart(CompanyID As String, SiteID As String, Status As String, SMonth As String, SYear As String, TMonth As String, TYear As String) As ecompdashboard_21
        Dim objdashbrd = New ecompdashboard_21
        Dim ret As String = ""
        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim con As New SqlConnection(conStr)
        Dim Qry As String = "Get_EcompDashboard"
        Dim cmd As New SqlCommand(Qry, con)
        Dim da As New SqlDataAdapter(cmd)
        Dim dt As New DataTable()
        cmd.CommandText = Qry
        cmd.CommandType = CommandType.StoredProcedure
        cmd.Parameters.AddWithValue("@Type", "SitewithStatus")
        cmd.Parameters.AddWithValue("@CompanyId", Convert.ToInt32(CompanyID))
        cmd.Parameters.AddWithValue("@Site", Convert.ToInt32(SiteID))
        cmd.Parameters.AddWithValue("@SMonth", Convert.ToInt32(SMonth))
        cmd.Parameters.AddWithValue("@SYear", Convert.ToInt32(SYear))
        cmd.Parameters.AddWithValue("@TMonth", Convert.ToInt32(TMonth))
        cmd.Parameters.AddWithValue("@TYear", Convert.ToInt32(TYear))
        cmd.Parameters.AddWithValue("@Status", Status)


        Using con
            Using da
                da.Fill(dt)
            End Using
        End Using

        Dim lstcataxis As New List(Of String)
        Dim objseriesdelayed As New ecompseries_2
        objseriesdelayed.name = "Delayed"
        Dim delayeddata As New List(Of Int32)

        Dim objseriesperformed As New ecompseries_2
        objseriesperformed.name = "Performed"
        Dim performeddata As New List(Of Int32)
        Dim objseriesperfaftrdt As New ecompseries_2
        objseriesperfaftrdt.name = "Performed after due date"
        Dim performdaftrdtdata As New List(Of Int32)
        For i As Integer = 0 To dt.Rows.Count - 1
            Dim delayedcount As Int32 = Convert.ToInt32(dt.Rows(i).Item("Delayed"))
            Dim performededcount As Int32 = Convert.ToInt32(dt.Rows(i).Item("Performed"))
            Dim peraftrdtdcount As Int32 = Convert.ToInt32(dt.Rows(i).Item("Performed After Due Date"))
            'delayeddata.Add(Math.Round((Convert.ToInt32(dt.Rows(i).Item("Delayed")) / (delayedcount + performededcount + peraftrdtdcount)) * 100, 2))
            'performeddata.Add(Math.Round((Convert.ToInt32(dt.Rows(i).Item("Performed")) / (delayedcount + performededcount + peraftrdtdcount)) * 100, 2))
            'performdaftrdtdata.Add(Math.Round((Convert.ToInt32(dt.Rows(i).Item("Performed After Due Date")) / (delayedcount + performededcount + peraftrdtdcount)) * 100, 2))
            delayeddata.Add(Convert.ToInt32(dt.Rows(i).Item("Delayed")))
            performeddata.Add(Convert.ToInt32(dt.Rows(i).Item("Performed")))
            performdaftrdtdata.Add(Convert.ToInt32(dt.Rows(i).Item("Performed After Due Date")))
            lstcataxis.Add(dt.Rows(i).Item("Act").ToString() + ":," + dt.Rows(i).Item("ActID").ToString())
        Next
        objseriesperformed.data = performeddata
        objseriesdelayed.data = delayeddata
        objseriesperfaftrdt.data = performdaftrdtdata
        Dim lstseries As New List(Of ecompseries_2)

        lstseries.Add(objseriesdelayed)
        lstseries.Add(objseriesperformed)
        lstseries.Add(objseriesperfaftrdt)
        objdashbrd.categoryAxis = lstcataxis
        objdashbrd.series1 = lstseries
        Return objdashbrd

    End Function


    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Shared Function GetdocGrid(CompanyID As String, SiteID As String, ActID As String, Status As String, SMonth As String, SYear As String, TMonth As String, TYear As String) As ecompReport_dash
        Dim jsonData As String = ""

        Dim res As New ecompReport_dash()
        Dim lstColumns As New List(Of ecompgrdcolumns_dash)
        Dim objColumn As ecompgrdcolumns_dash

        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim con As New SqlConnection(conStr)
        Dim Qry As String = "Get_EcompDashboard"
        Dim cmd As New SqlCommand(Qry, con)
        Dim da As New SqlDataAdapter(cmd)
        Dim dt As New DataTable()
        cmd.CommandText = Qry
        cmd.CommandType = CommandType.StoredProcedure
        cmd.Parameters.AddWithValue("@Type", "Grid")
        cmd.Parameters.AddWithValue("@CompanyId", Convert.ToInt32(CompanyID))
        cmd.Parameters.AddWithValue("@Site", Convert.ToInt32(SiteID))
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

            objColumn = New ecompgrdcolumns_dash()
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
Public Class ecompdashboard_1
    Public Property series1 As List(Of ecompseries_1)
    Public Property categoryAxis As List(Of String)

End Class
Public Class ecompdashboard_21
    Public Property series1 As List(Of ecompseries_2)
    Public Property categoryAxis As List(Of String)

End Class
Public Class ecompseries_2
    Public Property name As String
    Public Property data As List(Of Int32)

End Class
Public Class ecompseries_1
    Public Property name As String
    Public Property data As List(Of Double)


End Class
Public Class ecompReport_dash

    Public Property data As String
    Public Property columns As List(Of ecompgrdcolumns_dash)

End Class

Public Class ecompgrdcolumns_dash

    Public Property field As String
    Public Property title As String
    Public Property groupFooterTemplate As String = ""
    Public Property groupHeaderTemplate As String = ""
    Public Property aggregates As String = ""
    Public Property type As String = "string"
    '  Public Property template As String = ""

End Class