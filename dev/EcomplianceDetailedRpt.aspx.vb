Imports System.Data.SqlClient
Imports System.Data
Imports System.Web.Services
Imports Newtonsoft.Json
Imports System.Web.Script.Serialization
Imports Newtonsoft.Json.Converters

Partial Class EcomplianceDetailedRpt
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

            FillDDL()

        End If

    End Sub
    Sub FillDDL()

        Try

            For i As Integer = 2015 To DateTime.Now.Year
                ddlYear.Items.Add(New ListItem(i.ToString(), i.ToString()))
            Next
            ddlYear.SelectedValue = DateTime.Now.Year.ToString()
            ddlMonth.SelectedValue = DateTime.Now.Month.ToString()
        Catch ex As Exception

        End Try

    End Sub

    <WebMethod> _
    Public Shared Function GetJSON(Month As String, year As String, CompID As String) As eReport
        Dim jsonData As String = ""

        Dim res As New eReport()

        Try
            Dim ds As New DataSet()

            Dim Query As String = "uspGetEcomplianceDetailedRpt " & Month & "," & year & ", 'Scheduled'," & CompID
            Dim constr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
            Dim con As New SqlConnection(constr)
            Dim dt As New DataTable
            Dim oda As SqlDataAdapter = New SqlDataAdapter(Query, con)
            oda.SelectCommand.CommandTimeout = 900
            oda.Fill(dt)
            Dim lstColumns As New List(Of grdcolumns)
            Dim objColumn As grdcolumns

            For i As Integer = 0 To dt.Columns.Count - 1

                objColumn = New grdcolumns()
                objColumn.field = Replace(dt.Columns(i).ColumnName, " ", "_")
                objColumn.title = dt.Columns(i).ColumnName
                If (dt.Columns(i).ColumnName.ToString() = "Created" Or dt.Columns(i).ColumnName.ToString() = "To be created") Then
                    objColumn.type = "number"
                End If

                lstColumns.Add(objColumn)
                dt.Columns(i).ColumnName = Replace(dt.Columns(i).ColumnName, " ", "_")

            Next


            Dim serializerSettings As New JsonSerializerSettings()
            Dim json_serializer As New JavaScriptSerializer()

            serializerSettings.Converters.Add(New DataTableConverter())

            jsonData = JsonConvert.SerializeObject(dt, Newtonsoft.Json.Formatting.None, serializerSettings)
            res.data = jsonData
            res.columns = lstColumns



        Catch Ex As Exception
            Throw
        End Try
        Return res

    End Function

    <WebMethod> _
    Public Shared Function GetJSONcontractor(Month As String, year As String, CompID As String) As eReport
        Dim jsonData As String = ""

        Dim res As New eReport()

        Try
            Dim ds As New DataSet()

            Dim Query As String = "uspGetEcomplianceDetailedRpt " & Month & "," & year & ", 'As Needed'," & CompID
            Dim constr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
            Dim con As New SqlConnection(constr)
            Dim dt As New DataTable
            Dim oda As SqlDataAdapter = New SqlDataAdapter(Query, con)
            oda.Fill(dt)
            Dim lstColumns As New List(Of grdcolumns)
            Dim objColumn As grdcolumns

            For i As Integer = 0 To dt.Columns.Count - 1

                objColumn = New grdcolumns()
                objColumn.field = Replace(dt.Columns(i).ColumnName, " ", "_")
                objColumn.title = dt.Columns(i).ColumnName

                If (dt.Columns(i).ColumnName.ToString() = "Created" Or dt.Columns(i).ColumnName.ToString() = "To be created") Then
                    objColumn.type = "number"
                End If
                lstColumns.Add(objColumn)
                dt.Columns(i).ColumnName = Replace(dt.Columns(i).ColumnName, " ", "_")

            Next


            Dim serializerSettings As New JsonSerializerSettings()
            Dim json_serializer As New JavaScriptSerializer()

            serializerSettings.Converters.Add(New DataTableConverter())


            jsonData = JsonConvert.SerializeObject(dt, Newtonsoft.Json.Formatting.None, serializerSettings)
            res.data = jsonData
            res.columns = lstColumns



        Catch Ex As Exception
            Throw
        End Try
        Return res

    End Function

    <WebMethod> _
    Public Shared Function GetJSONCreated(CompanyID As String, SiteID As String, ActID As String, Type As String, Month As String, Year As String) As eReport
        Dim jsonData As String = ""

        Dim res As New eReport()

        Try
            Dim ds As New DataSet()

            Dim Query As String = "select (case when fld47 is null or fld47='' then fld46 else fld47 end )[Activity]," &
                " convert(varchar(12),convert(datetime,fld43,3))[DueDate] ,Convert(varchar(12), adate) [FirstAlertDate],(select  dms.udf_split('STATIC-USER-UserName',userid) from mmm_doc_dtl where docid = doc.tid and " &
 " tid = (select max(tid) from mmm_doc_dtl where docid= doc.tid))[In Bucket OF],fld29[Company],fld30[Site],fld25[Type],fld44[Contractor],Doc.Curstatus [Status],doc.Source [Source], " &
   "(select (case when curStatus ='Archive' then '' else cast( datediff(day,fdate,getdate()) as varchar) end)   from mmm_doc_dtl where docid = doc.tid and " &
  " tid = (select max(tid) from mmm_doc_dtl where docid= doc.tid))[PendingDays] from mmm_mst_doc doc where eid =98 and documenttype='act Document' " &
" and doc.fld5='" & CompanyID & "' and doc.fld7='" & SiteID & "' and doc.fld2 ='" & ActID & "' and Month(doc.adate) =" & Month & " and year(doc.adate) =" & Year
            If (Type = "As Needed") Then
                ' Query &= " and doc.source<>'Scheduler' "
            Else

            End If

            Dim constr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
            Dim con As New SqlConnection(constr)
            Dim dt As New DataTable
            Dim oda As SqlDataAdapter = New SqlDataAdapter(Query, con)
            oda.Fill(dt)
            Dim lstColumns As New List(Of grdcolumns)
            Dim objColumn As grdcolumns

            For i As Integer = 0 To dt.Columns.Count - 1

                objColumn = New grdcolumns()
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



        Catch Ex As Exception
            Throw
        End Try
        Return res

    End Function

    <WebMethod> _
    Public Shared Function GetJSONToBeCreated(CompanyID As String, SiteID As String, ActID As String, Type As String, Month As String, Year As String) As eReport

        Dim jsonData As String = ""

        Dim res As New eReport()

        Try
            Dim ds As New DataSet()

            Dim Query As String = "uspGetEcomplianceDetailedRpt_ToBeCreated " & CompanyID & " ," & SiteID & "," & ActID & "," & Month & "," & Year & ",'" & Type & "'"
            Dim constr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
            Dim con As New SqlConnection(constr)
            Dim dt As New DataTable
            Dim oda As SqlDataAdapter = New SqlDataAdapter(Query, con)
            oda.Fill(dt)
            Dim lstColumns As New List(Of grdcolumns)
            Dim objColumn As grdcolumns

            For i As Integer = 0 To dt.Columns.Count - 1

                objColumn = New grdcolumns()
                objColumn.field = Replace(dt.Columns(i).ColumnName, " ", "_")
                objColumn.title = dt.Columns(i).ColumnName
                '  objColumn.template = "<p>#=" & dt.Columns(i).ColumnName & "</p>"

                lstColumns.Add(objColumn)
                dt.Columns(i).ColumnName = Replace(dt.Columns(i).ColumnName, " ", "_")

            Next

            Dim serializerSettings As New JsonSerializerSettings()
            Dim json_serializer As New JavaScriptSerializer()
            serializerSettings.Converters.Add(New DataTableConverter())
            jsonData = JsonConvert.SerializeObject(dt, Newtonsoft.Json.Formatting.None, serializerSettings)
            res.data = jsonData
            res.columns = lstColumns



        Catch Ex As Exception
            Throw
        End Try
        Return res

    End Function

    <System.Web.Services.WebMethod()>
    Public Shared Function GetCompany(str As String, uid As String, urole As String) As String
        Dim jsonData As String = ""
        Dim conStr As String = ConfigurationManager.ConnectionStrings("ConStr").ConnectionString
        Dim con As New SqlConnection(conStr)
        Dim oda As SqlDataAdapter = New SqlDataAdapter("", con)
        If (urole = "SU") Then
            oda.SelectCommand.CommandText = "select tid, fld1 [CompanyName] from mmm_mst_master  where eid =98 and documenttype ='Company Master' and fld1 like '%" & str & "%' order by fld1"
        Else
            oda.SelectCommand.CommandText = " if EXISTS (select docmapping from mmm_mst_forms where eid =98 and  formname ='company Master' and isroledef=1) " &
   " select tid,fld1[CompanyName] from mmm_mst_master comp inner join   dbo.split((select fld1 from mmm_ref_role_user where eid =98 and uid =" & uid & " and rolename ='" & urole & "'),',') s on s.items = comp.tid " &
" where eid =98 and documenttype ='Company Master' and fld1 like '%" & str & "%' order by fld1 else select tid,fld1[CompanyName]  from mmm_mst_master comp where eid =98 and documenttype ='Company Master'  and fld1 like '%" & str & "%' order by fld1"

        End If
        ' oda.SelectCommand.CommandText = "select tid, fld1 [CompanyName] from mmm_mst_master  where eid =98 and documenttype ='Company Master' and fld1 like '%" & str & "%' order by fld1"
        Try
            Dim ds As New DataSet()
            oda.Fill(ds)
            jsonData = JsonConvert.SerializeObject(ds.Tables(0), Newtonsoft.Json.Formatting.None)
            Return jsonData
        Catch ex As Exception
            Throw
        End Try
    End Function

    Public Class eReport

        Public Property data As String
        Public Property columns As List(Of grdcolumns)

    End Class

    Public Class grdcolumns

        Public Property field As String
        Public Property title As String
        Public Property groupFooterTemplate As String = ""
        Public Property groupHeaderTemplate As String = ""
        Public Property aggregates As String = ""
        Public Property type As String = "string"
        '  Public Property template As String = ""

    End Class

End Class
