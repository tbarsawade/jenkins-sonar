Imports System.Data.SqlClient
Imports System.Data
Imports System.Web.Services
Imports Newtonsoft.Json
Imports System.Web.Script.Serialization
Imports Newtonsoft.Json.Converters
Partial Class ProjectDeatiledRpt
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



        End If

    End Sub

    <WebMethod> _
    Public Shared Function GetJSON(FirstGrid As Boolean, uid As String, urole As String) As eReportIndusmis1
        Dim jsonData As String = ""

        Dim res As New eReportIndusmis1()

        Try
            Dim ds As New DataSet()

            Dim Query As String = ""

            Dim constr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
            Dim con As New SqlConnection(constr)
            Dim dt As New DataTable
            Dim oda As New SqlDataAdapter
            Using con
                If (urole = "SU") Then
                    oda = New SqlDataAdapter("select tbl.[Project Name], [ProjectType],tbl.Total ,cast(cast(tbl.total as int )/8 as int )[ManHours], cast ((cast( tbl.Completed as numeric(18,2)) / cast(  tbl.Total  as numeric(18,2))) *100   as numeric(18,2)) [Completed] , " &
"case when tbl.completed =0 and tbl.Total =0 then 0 else  (100.00 - cast ((cast( tbl.Completed as numeric(18,2)) / cast(  tbl.Total  as numeric(18,2))) *100   as numeric(18,2))) end  [Remaining] ,[Team Members],tbl.fld1 [ProjectID],  [Project Status] ,[SPOCName] ,[ProjectOwner],[Status], [EDC] from ( select dms.udf_split('MASTER-Project Master-fld1', d.fld1) [Project Name],d.fld1, " &
 " sum(cast(d.fld4 as int))[Total],(select isnull( Sum(cast(fld4 as int)),0)  from mmm_mst_doc where  eid =100 and documenttype ='task document'   and curstatus ='Archive' and fld1= d.fld1 )[Completed],Count(distinct d.fld3)[Team Members] , (case when projmas.fld11 ='Live' then '    Live' when projmas.fld11 ='Development' then '   Development' when projmas.fld11='POC' then '  POC' " &
"  when projmas.fld11='Discarded' then ' Discarded' else projmas.fld11 end  )[Project Status] , dms.udf_split('MASTER-Project Type Master-fld1', ProjMas.fld9) [ProjectType],  ProjMas.fld3 [SPOCName],ProjMas.fld8[ProjectOwner],ProjMas.fld11 [Status], isnull(Projmas.fld12,'') [EDC]" &
     " from mmm_mst_doc d  inner join mmm_mst_master ProjMas on d.fld1= Projmas.tid	 where d.eid =100 and d.documenttype ='task document' and ProjMas.eid =100 and ProjMas.Documenttype='Project Master' and projmas.fld11<> ''   group by d.fld1,ProjMas.fld11,Projmas.fld9,Projmas.fld3,Projmas.fld8 ,Projmas.fld12 ) tbl order by [Project Status],[Project Name]", con)
                Else
                    oda = New SqlDataAdapter("select tbl.[Project Name], [ProjectType],tbl.Total ,cast(cast(tbl.total as int )/8 as int )[ManHours], cast ((cast( tbl.Completed as numeric(18,2)) / cast(  tbl.Total  as numeric(18,2))) *100   as numeric(18,2)) [Completed] , " &
"case when tbl.completed =0 and tbl.Total =0 then 0 else  (100.00 - cast ((cast( tbl.Completed as numeric(18,2)) / cast(  tbl.Total  as numeric(18,2))) *100   as numeric(18,2))) end  [Remaining] ,[Team Members],tbl.fld1 [ProjectID],  [Project Status] ,[SPOCName] ,[ProjectOwner],[Status], [EDC] from ( select dms.udf_split('MASTER-Project Master-fld1', d.fld1) [Project Name],d.fld1, " &
" sum(cast(d.fld4 as int))[Total],(select isnull( Sum(cast(fld4 as int)),0)  from mmm_mst_doc where  eid =100 and documenttype ='task document'   and curstatus ='Archive' and fld1= d.fld1 )[Completed],Count(distinct d.fld3)[Team Members] , (case when projmas.fld11 ='Live' then '    Live' when projmas.fld11 ='Development' then '   Development' when projmas.fld11='POC' then '  POC' " &
"  when projmas.fld11='Discarded' then ' Discarded' else projmas.fld11 end  )[Project Status] , dms.udf_split('MASTER-Project Type Master-fld1', ProjMas.fld9) [ProjectType],  ProjMas.fld3 [SPOCName],ProjMas.fld8[ProjectOwner],ProjMas.fld11 [Status], isnull(Projmas.fld12,'') [EDC]" &
  " from mmm_mst_doc d  inner join mmm_mst_master ProjMas on d.fld1= Projmas.tid	 where d.eid =100 and d.documenttype ='task document' and ProjMas.eid =100 and ProjMas.Documenttype='Project Master' and projmas.fld11<> '' " &
  " and ProjMas.tid in (select items from dbo.split((select fld1 from  mmm_ref_role_user where eid =100 and uid = " & uid & " and rolename ='" & urole & "'),',')) " &
  " group by d.fld1,ProjMas.fld11,Projmas.fld9,Projmas.fld3,Projmas.fld8 ,Projmas.fld12 ) tbl order by [Project Status],[Project Name]", con)
                End If
                'Dim oda As SqlDataAdapter = New SqlDataAdapter("select tbl.[Project Name], [ProjectType],tbl.Total ,cast(cast(tbl.total as int )/8 as int )[ManHours], cast ((cast( tbl.Completed as numeric(18,2)) / cast(  tbl.Total  as numeric(18,2))) *100   as numeric(18,2)) [Completed] , " &
                '"case when tbl.completed =0 and tbl.Total =0 then 0 else  (100.00 - cast ((cast( tbl.Completed as numeric(18,2)) / cast(  tbl.Total  as numeric(18,2))) *100   as numeric(18,2))) end  [Remaining] ,[Team Members],tbl.fld1 [ProjectID],  [Project Status] ,[SPOCName] ,[ProjectOwner],[Status], [EDC] from ( select dms.udf_split('MASTER-Project Master-fld1', d.fld1) [Project Name],d.fld1, " &
                ' " sum(cast(d.fld4 as int))[Total],(select isnull( Sum(cast(fld4 as int)),0)  from mmm_mst_doc where  eid =100 and documenttype ='task document'   and curstatus ='Archive' and fld1= d.fld1 )[Completed],Count(distinct d.fld3)[Team Members] , (case when projmas.fld11 ='Live' then '    Live' when projmas.fld11 ='Development' then '   Development' when projmas.fld11='POC' then '  POC' " &
                '"  when projmas.fld11='Discarded' then ' Discarded' else projmas.fld11 end  )[Project Status] , ProjMas.fld9 [ProjectType],  ProjMas.fld3 [SPOCName],ProjMas.fld8[ProjectOwner],ProjMas.fld11 [Status], isnull(Projmas.fld12,'') [EDC]" &
                '     " from mmm_mst_doc d  inner join mmm_mst_master ProjMas on d.fld1= Projmas.tid	 where d.eid =100 and d.documenttype ='task document' and ProjMas.eid =100 and ProjMas.Documenttype='Project Master' and projmas.fld11<> ''   group by d.fld1,ProjMas.fld11,Projmas.fld9,Projmas.fld3,Projmas.fld8 ,Projmas.fld12 ) tbl order by [Project Status],[Project Name]", con)
                oda.SelectCommand.CommandType = CommandType.Text
                oda.Fill(ds)
            End Using

            Dim serializerSettings As New JsonSerializerSettings()
            Dim json_serializer As New JavaScriptSerializer()

            serializerSettings.Converters.Add(New DataTableConverter())
            Dim lstColumns As New List(Of grdcolumns1indusmis1)
            Dim objColumn As grdcolumns1indusmis1
            If (FirstGrid = True) Then
                If (ds.Tables(0).Columns.Contains("no")) Then
                    ds.Tables(0).Columns.Remove("no")
                End If
                For i As Integer = 0 To ds.Tables(0).Columns.Count - 1
                    objColumn = New grdcolumns1indusmis1()
                    objColumn.field = Replace(ds.Tables(0).Columns(i).ColumnName, " ", "")
                    objColumn.title = ds.Tables(0).Columns(i).ColumnName
                    If (ds.Tables(0).Columns(i).ColumnName = "Status") Then
                        ' objColumn.width = 200
                    End If
                    lstColumns.Add(objColumn)
                    ds.Tables(0).Columns(i).ColumnName = Replace(ds.Tables(0).Columns(i).ColumnName, " ", "")
                Next

                jsonData = JsonConvert.SerializeObject(ds.Tables(0), Newtonsoft.Json.Formatting.None, serializerSettings)
            Else
                For i As Integer = 0 To ds.Tables(1).Columns.Count - 1
                    objColumn = New grdcolumns1indusmis1()
                    objColumn.field = Replace(ds.Tables(1).Columns(i).ColumnName, " ", "")

                    objColumn.title = ds.Tables(1).Columns(i).ColumnName
                    lstColumns.Add(objColumn)
                    ds.Tables(1).Columns(i).ColumnName = Replace(ds.Tables(1).Columns(i).ColumnName, " ", "")
                Next
                jsonData = JsonConvert.SerializeObject(ds.Tables(1), Newtonsoft.Json.Formatting.None, serializerSettings)
            End If

            res.data = jsonData
            res.columns = lstColumns

        Catch Ex As Exception
            Throw
        End Try
        Return res

    End Function

    <WebMethod> _
    Public Shared Function GetJSONModule(ProjectID As String, uid As String, urole As String) As eReportIndusmis1
        Dim jsonData As String = ""
        Dim res As New eReportIndusmis1()

        Try
            Dim ds As New DataSet()

            Dim Query As String = ""

            Dim constr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
            Dim con As New SqlConnection(constr)
            Dim dt As New DataTable
            Dim oda As SqlDataAdapter
            Using con
                oda = New SqlDataAdapter(" select tbl.[Module Name],tbl.Total,cast(cast(tbl.total as int )/8 as int )[ManHours] , cast ((cast( tbl.Completed as numeric(18,2)) / cast(  tbl.Total  as numeric(18,2))) *100   as numeric(18,2)) [Completed] , " &
" case when tbl.completed =0 and tbl.Total =0 then 0 else  (100.00 - cast ((cast( tbl.Completed as numeric(18,2)) / cast(  tbl.Total  as numeric(18,2))) *100   as numeric(18,2))) end [Remaining], tbl.fld1 [ProjectId],tbl.fld11 [ModuleId]  from (  select dms.udf_split('MASTER-Module-fld2', fld11) [Module Name],fld11,fld1, " &
" sum(cast(fld4 as int))[Total],(select isnull( Sum(cast(fld4 as int)),0)  from mmm_mst_doc where  eid =100 and documenttype ='task document'  and curstatus ='Archive' and fld1= d.fld1 and fld11 = d.fld11)[Completed],Count(distinct fld3)[Team Members] from mmm_mst_doc d where eid =100 and documenttype ='task document' and fld1 =" & ProjectID & "   group by fld1,fld11) tbl  order by [Module Name] ", con)






                oda.SelectCommand.CommandType = CommandType.Text
                oda.Fill(ds)
            End Using

            Dim serializerSettings As New JsonSerializerSettings()
            Dim json_serializer As New JavaScriptSerializer()

            serializerSettings.Converters.Add(New DataTableConverter())
            Dim lstColumns As New List(Of grdcolumns1indusmis1)
            Dim objColumn As grdcolumns1indusmis1

            For i As Integer = 0 To ds.Tables(0).Columns.Count - 1
                objColumn = New grdcolumns1indusmis1()
                objColumn.field = Replace(ds.Tables(0).Columns(i).ColumnName, " ", "")

                objColumn.title = ds.Tables(0).Columns(i).ColumnName
                lstColumns.Add(objColumn)
                ds.Tables(0).Columns(i).ColumnName = Replace(ds.Tables(0).Columns(i).ColumnName, " ", "")
            Next
            jsonData = JsonConvert.SerializeObject(ds.Tables(0), Newtonsoft.Json.Formatting.None, serializerSettings)


            res.data = jsonData
            res.columns = lstColumns

        Catch Ex As Exception
            Throw
        End Try
        Return res

    End Function

    <WebMethod> _
    Public Shared Function GetJSONTeamP(ProjectID As String, uid As String, urole As String) As eReportIndusmis1
        Dim jsonData As String = ""

        Dim res As New eReportIndusmis1()

        Try
            Dim ds As New DataSet()

            Dim Query As String = ""
            Dim constr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
            Dim con As New SqlConnection(constr)
            Dim dt As New DataTable

            Using con
                Dim oda As SqlDataAdapter = New SqlDataAdapter(" select tbl.[Team Member Name],tbl.Total,cast(cast(tbl.total as int )/8 as int )[ManHours] , cast ((cast( tbl.Completed as numeric(18,2)) / cast(  tbl.Total  as numeric(18,2))) *100   as numeric(18,2)) [Completed] , " &
                " case when tbl.completed =0 and tbl.Total =0 then 0 else  (100.00 - cast ((cast( tbl.Completed as numeric(18,2)) / cast(  tbl.Total  as numeric(18,2))) *100   as numeric(18,2))) end  [Remaining]  from ( select dms.udf_split('STATIC-USER-UserName', fld3) [Team Member Name], " &
                " isnull( sum(cast(fld4 as int)),0) [Total],(select isnull( Sum(cast(fld4 as int)),0)  from mmm_mst_doc where  eid =100 and documenttype ='task document'  and curstatus ='Archive' and fld1= d.fld1  and fld3=d.fld3)[Completed] from mmm_mst_doc d where eid =100 and documenttype ='task document' and fld1 =" & ProjectID & "   group by fld1,fld3 ) tbl order by [Team Member Name] ", con)
                oda.SelectCommand.CommandType = CommandType.Text
                oda.Fill(ds)
            End Using

            Dim serializerSettings As New JsonSerializerSettings()
            Dim json_serializer As New JavaScriptSerializer()

            serializerSettings.Converters.Add(New DataTableConverter())
            Dim lstColumns As New List(Of grdcolumns1indusmis1)
            Dim objColumn As grdcolumns1indusmis1

            For i As Integer = 0 To ds.Tables(0).Columns.Count - 1
                objColumn = New grdcolumns1indusmis1()
                objColumn.field = Replace(ds.Tables(0).Columns(i).ColumnName, " ", "")
                objColumn.title = ds.Tables(0).Columns(i).ColumnName
                lstColumns.Add(objColumn)
                ds.Tables(0).Columns(i).ColumnName = Replace(ds.Tables(0).Columns(i).ColumnName, " ", "")
            Next
            jsonData = JsonConvert.SerializeObject(ds.Tables(0), Newtonsoft.Json.Formatting.None, serializerSettings)
            res.data = jsonData
            res.columns = lstColumns

        Catch Ex As Exception
            Throw
        End Try
        Return res

    End Function

    <WebMethod> _
    Public Shared Function GetJSONTeam(ProjectID As String, Moduleid As String, uid As String, urole As String) As eReportIndusmis1
        Dim jsonData As String = ""

        Dim res As New eReportIndusmis1()

        Try
            Dim ds As New DataSet()

            Dim Query As String = ""

            Dim constr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
            Dim con As New SqlConnection(constr)
            Dim dt As New DataTable

            Using con

                Dim oda As SqlDataAdapter = New SqlDataAdapter(" select tbl.[Task],[UserName],CONVERT(VARCHAR(12),convert(datetime, [EDC],3),103) [EDC] ,CONVERT(VARCHAR(12),convert(datetime, [ADC],3),103) [ADC] ,tbl.Total,cast(cast(tbl.total as int )/8 as int )[ManHours] , cast ((cast( tbl.Completed as numeric(18,2)) / cast(  tbl.Total  as numeric(18,2))) *100   as numeric(18,2)) [Completed] , " &
     " case when tbl.completed =0 and tbl.Total =0 then 0 else  (100.00 - cast ((cast( tbl.Completed as numeric(18,2)) / cast(  tbl.Total  as numeric(18,2))) *100   as numeric(18,2)))  end  [Remaining]  from ( select  fld2 [Task], " &
      " sum(cast(fld4 as int))[Total],(select isnull( Sum(cast(fld4 as int)),0)  from mmm_mst_doc where  eid =100 and documenttype ='task document'  and curstatus ='Archive' and fld1= d.fld1 " &
      " and fld11 =d.fld11 and fld2=d.fld2  and fld3= d.fld3 and fld18 =d.fld18 and fld19 =d.fld19)[Completed], dms.udf_split('STATIC-USER-UserName',fld3)[UserName],isnull(fld18,'') [EDC],isnull(fld19,'') [ADC] from mmm_mst_doc d where eid =100 and documenttype ='task document' and fld1 =" & ProjectID & " and fld11=" & Moduleid & "  group by fld1,fld11,fld2,fld3,fld18,fld19 ) tbl order by [Task] ", con)

                oda.SelectCommand.CommandType = CommandType.Text
                oda.Fill(ds)

            End Using

            Dim serializerSettings As New JsonSerializerSettings()
            Dim json_serializer As New JavaScriptSerializer()
            serializerSettings.Converters.Add(New DataTableConverter())
            Dim lstColumns As New List(Of grdcolumns1indusmis1)
            Dim objColumn As grdcolumns1indusmis1

            For i As Integer = 0 To ds.Tables(0).Columns.Count - 1
                objColumn = New grdcolumns1indusmis1()
                objColumn.field = Replace(ds.Tables(0).Columns(i).ColumnName, " ", "")
                objColumn.title = ds.Tables(0).Columns(i).ColumnName
                lstColumns.Add(objColumn)
                ds.Tables(0).Columns(i).ColumnName = Replace(ds.Tables(0).Columns(i).ColumnName, " ", "")
            Next

            jsonData = JsonConvert.SerializeObject(ds.Tables(0), Newtonsoft.Json.Formatting.None, serializerSettings)
            res.data = jsonData
            res.columns = lstColumns

        Catch Ex As Exception
            Throw
        End Try
        Return res

    End Function

    End Class
Public Class eReportIndusmis1
    Public Property data As String
    Public Property columns As List(Of grdcolumns1indusmis1)
End Class

Public Class grdcolumns1indusmis1
    Public Property field As String
    Public Property title As String
    ' Public Property width As Integer = 100

End Class

