Imports System.Web.Services
Imports System.Data
Imports Newtonsoft.Json
Imports Newtonsoft.Json.Converters
Imports System.Data.SqlClient

Imports System.Web.Script.Serialization
Partial Class EcompCLDashboard
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
    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load

        If Not IsPostBack Then
            FillCompDDl()
            ddlCompany.SelectedValue = "1076419"
        End If

    End Sub
    Private Sub FillCompDDl()

        Dim jsonData As String = ""
        Dim conStr As String = ConfigurationManager.ConnectionStrings("ConStr").ConnectionString
        Dim con As New SqlConnection(conStr)
        Dim oda As SqlDataAdapter = New SqlDataAdapter("", con)
        '   Dim strquery As String = ""
        Dim urole As String = Session("USERROLE").ToString()
        Dim uid As String = Session("UID").ToString()

        If (urole = "SU") Then
            oda.SelectCommand.CommandText = "select tid, fld1 [CompanyName] from mmm_mst_master  where eid =98 and documenttype ='Company Master'  and isauth=1 order by fld1"
        Else
            oda.SelectCommand.CommandText = " if EXISTS (select docmapping from mmm_mst_forms where eid =98 and  formname ='company Master' and isroledef=1) " &
   " select tid,fld1 [CompanyName] from mmm_mst_master comp inner join   dbo.split((select fld1 from mmm_ref_role_user where eid =98 and uid =" & uid & " and rolename ='" & urole & "'),',') s on s.items = comp.tid " &
" where eid =98 and documenttype ='Company Master'  order by fld1 else select tid,fld1[CompanyName]  from mmm_mst_master comp where eid =98 and documenttype ='Company Master' and isauth=1   order by fld1"

        End If

        Try
            Dim dt As New DataTable()
            oda.Fill(dt)
            If (dt.Rows.Count = 0) Then
                hdnddlCompany.Value = 0
                ddlCompany.Visible = False
                lblcomp.Visible = False

            ElseIf (dt.Rows.Count = 1) Then
                hdnddlCompany.Value = dt.Rows(0)("tid")
                ddlCompany.Visible = False
                lblcomp.Visible = False

            Else : ddlCompany.Visible = True
                lblcomp.Visible = True
            End If
            ddlCompany.DataSource = dt
            ddlCompany.DataTextField = "CompanyName"
            ddlCompany.DataValueField = "tid"
            ddlCompany.DataBind()

        Catch ex As Exception
            Throw
        End Try

    End Sub


    <WebMethod()> _
    Public Shared Function GetSites(CompanyID As String, SMonth As String, SYear As String, TMonth As String, TYear As String) As String
        Dim ConStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim ret As String = ""
        Dim ds As New DataSet()
        Dim jsonData As String = ""
        Dim serializerSettings As New JsonSerializerSettings()
        serializerSettings.Converters.Add(New DataTableConverter())
        Try
            Using con = New SqlConnection(ConStr)
                Using da = New SqlDataAdapter("Get_EcompDashboardClient", con)
                    da.SelectCommand.CommandType = CommandType.StoredProcedure
                    da.SelectCommand.Parameters.AddWithValue("@Type", "Company")
                    da.SelectCommand.Parameters.AddWithValue("@CompanyId", CompanyID)
                    da.SelectCommand.Parameters.AddWithValue("@SMonth", Convert.ToInt32(SMonth))
                    da.SelectCommand.Parameters.AddWithValue("@SYear", Convert.ToInt32(SYear))
                    da.SelectCommand.Parameters.AddWithValue("@TMonth", Convert.ToInt32(TMonth))
                    da.SelectCommand.Parameters.AddWithValue("@TYear", Convert.ToInt32(TYear))
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
        Dim objdashbrd = New ecompdashboard1cl
        Dim jsonData As String = ""
        Dim ret As String = ""
        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim con As New SqlConnection(conStr)
        Dim Qry As String = "Get_EcompDashboardClient"
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
        cmd.Parameters.AddWithValue("@uid", Convert.ToInt32(Uid))
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
    Public Shared Function GetActwiseChart(CompanyID As String, SMonth As String, SYear As String, TMonth As String, TYear As String) As ecompdashboard1cl
        Dim objdashbrd = New ecompdashboard1cl

        Dim ret As String = ""
        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim con As New SqlConnection(conStr)
        Dim Qry As String = "Get_EcompDashboardClient"
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

        Dim objseriesperformed As New ecompseries1cl
        objseriesperformed.name = "Performed"
        objseriesperformed.color = "green"
        Dim performeddata As New List(Of Double)

        Dim objseriesperfaftrdt As New ecompseries1cl
        objseriesperfaftrdt.name = "Not Performed"
        objseriesperfaftrdt.color = "red"
        Dim performdaftrdtdata As New List(Of Double)

        Dim objserieInprocess As New ecompseries1cl
        objserieInprocess.name = "In Process"
        objserieInprocess.color = "#FF9900"
        Dim InProcessdata As New List(Of Double)


        For i As Integer = 0 To dt.Rows.Count - 1

            Dim performededcount As Int32 = Convert.ToInt32(dt.Rows(i).Item("Performed"))
            Dim peraftrdtdcount As Int32 = Convert.ToInt32(dt.Rows(i).Item("Not Performed"))
            Dim InProcesscount As Int32 = Convert.ToInt32(dt.Rows(i).Item("InProcess"))

            performeddata.Add(Convert.ToDouble(performededcount))
            performdaftrdtdata.Add(Convert.ToDouble(peraftrdtdcount))
            InProcessdata.Add(InProcesscount)
            Dim cattot As Integer = performededcount + peraftrdtdcount + InProcesscount

            'delayeddata.Add(Math.Round(((Convert.ToInt32(dt.Rows(i).Item("Delayed")) / (delayedcount + performededcount + peraftrdtdcount + InProcesscount)) * 100), 2))
            'performeddata.Add(Math.Round(((Convert.ToInt32(dt.Rows(i).Item("Performed")) / (delayedcount + performededcount + peraftrdtdcount + InProcesscount)) * 100), 2))
            'performdaftrdtdata.Add(Math.Round(((Convert.ToInt32(dt.Rows(i).Item("Performed After Due Date")) / (delayedcount + performededcount + peraftrdtdcount + InProcesscount)) * 100), 2))
            'InProcessdata.Add(Math.Round(((Convert.ToInt32(dt.Rows(i).Item("InProcess")) / (delayedcount + performededcount + peraftrdtdcount + InProcesscount)) * 100), 2))
            lstcataxis.Add(dt.Rows(i).Item("Act").ToString() + ":," + dt.Rows(i).Item("ActID").ToString() + ":," + cattot.ToString())
        Next


        objseriesperformed.data = performeddata

        objseriesperfaftrdt.data = performdaftrdtdata
        objserieInprocess.data = InProcessdata

        Dim lstseries As New List(Of ecompseries1cl)

        lstseries.Add(objseriesperformed)
        lstseries.Add(objseriesperfaftrdt)
        lstseries.Add(objserieInprocess)


        objdashbrd.categoryAxis = lstcataxis
        objdashbrd.series1 = lstseries

        Return objdashbrd

    End Function

    <WebMethod()> _
    <Script.Services.ScriptMethod()> _
    Public Shared Function GetPieChart(CompanyID As String, SMonth As String, SYear As String, TMonth As String, TYear As String) As List(Of ecomppiedata1cl)
        Dim listdata As New List(Of ecomppiedata1cl)

        Dim objdashbrd = New ecompdashboard1cl

        Dim ret As String = ""
        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim con As New SqlConnection(conStr)
        Dim Qry As String = "Get_EcompDashboardClient"
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

        Dim objperformed As New ecomppiedata1cl
        Dim objNotperformed As New ecomppiedata1cl
        Dim objInProcess As New ecomppiedata1cl

        If (dt.Rows.Count > 0) Then
            Dim performedcount = Convert.ToInt16(dt.Rows(0).Item("Performed"))
            Dim perfrmedduedtcount = Convert.ToInt16(dt.Rows(0).Item("Not Performed"))
            Dim InProcesscount = Convert.ToInt16(dt.Rows(0).Item("InProcess"))
            Dim totcount = performedcount + perfrmedduedtcount + InProcesscount

            objperformed.category = "Performed"
            objperformed.value = Math.Round(Convert.ToDouble((performedcount / totcount) * 100), 2)
            objperformed.color = "green"
            objperformed.count = performedcount
            listdata.Add(objperformed)
            objNotperformed.category = "Not Performed"
            objNotperformed.value = Math.Round(Convert.ToDouble((perfrmedduedtcount / totcount) * 100), 2)
            objNotperformed.color = "red"
            objNotperformed.count = perfrmedduedtcount
            listdata.Add(objNotperformed)
            objInProcess.category = "In Process"
            objInProcess.value = Math.Round(Convert.ToDouble((InProcesscount / totcount) * 100), 2)
            objInProcess.color = "#FF9900"
            objInProcess.count = InProcesscount
            listdata.Add(objInProcess)

        End If

        Return listdata

    End Function

    <WebMethod()> _
    <Script.Services.ScriptMethod()> _
    Public Shared Function GetActclickGrid(CompanyID As String, ActID As String, Status As String, SMonth As String, SYear As String, TMonth As String, TYear As String) As ecompReportcl
        Dim jsonData As String = ""

        Dim res As New ecompReportcl()
        Dim lstColumns As New List(Of ecompgrdcolumnscl)
        Dim objColumn As ecompgrdcolumnscl

        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim con As New SqlConnection(conStr)
        Dim Qry As String = "Get_EcompDashboardClient"
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

            objColumn = New ecompgrdcolumnscl()
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

    <WebMethod()> _
    <Script.Services.ScriptMethod()> _
    Public Shared Function GetGridHomeDet(CompanyID As String, SubType As String, uid As String) As ecompReportcl
        Dim jsonData As String = ""

        Dim res As New ecompReportcl()
        Dim lstColumns As New List(Of ecompgrdcolumnscl)
        Dim objColumn As ecompgrdcolumnscl

        Dim conStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
        Dim con As New SqlConnection(conStr)
        Dim Qry As String = "Get_EcompDashboardClient"
        Dim cmd As New SqlCommand(Qry, con)
        Dim da As New SqlDataAdapter(cmd)
        Dim dt As New DataTable()
        cmd.CommandText = Qry
        cmd.CommandType = CommandType.StoredProcedure
        cmd.Parameters.AddWithValue("@Type", "HomeDetsub")
        cmd.Parameters.AddWithValue("@CompanyId", Convert.ToInt32(CompanyID))
        cmd.Parameters.AddWithValue("@SubType", SubType)
        cmd.Parameters.AddWithValue("@uid", Convert.ToInt32(uid))
        Using con
            Using da
                da.Fill(dt)
            End Using
        End Using

        For i As Integer = 0 To dt.Columns.Count - 1

            objColumn = New ecompgrdcolumnscl()
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


Public Class ecompdashboard1cl
    Public Property series1 As List(Of ecompseries1cl)
    Public Property categoryAxis As List(Of String)
End Class
Public Class ecompdashboard21cl
    Public Property series1 As List(Of ecompseries2cl)
    Public Property categoryAxis As List(Of String)
End Class
Public Class ecompseries2cl
    Public Property name As String
    Public Property data As List(Of Int32)
    Public Property color As String
End Class
Public Class ecompseries1cl
    Public Property name As String
    Public Property data As List(Of Double)
    Public Property color As String
    Public Property total As Integer
End Class
Public Class ecompReportcl
    Public Property data As String
    Public Property columns As List(Of ecompgrdcolumnscl)
End Class
Public Class ecomppiedata1cl
    Public Property category As String
    Public Property value As Double
    Public Property color As String
    Public Property count As Integer
End Class

Public Class ecompgrdcolumnscl
    Public Property field As String
    Public Property title As String
    Public Property groupFooterTemplate As String = ""
    Public Property groupHeaderTemplate As String = ""
    Public Property aggregates As String = ""
    Public Property type As String = "string"
    '  Public Property template As String = ""
End Class
