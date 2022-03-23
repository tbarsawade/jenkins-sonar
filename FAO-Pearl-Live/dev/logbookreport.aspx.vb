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

Partial Class logbookreport
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Try

            hdnUID.Value = Session("UID")
            hdnurole.Value = Session("ROLES")
            hdnEid.Value = Session("Eid")

            Dim ConStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
            Dim da As New SqlDataAdapter("", ConStr)
            Dim dtMap As New DataTable
            da.SelectCommand.CommandText = "Select  maptype,APIKey from mmm_mst_Entity with(nolock) where Eid=" & Session("Eid")
            da.Fill(dtMap)
            If dtMap.Rows.Count > 0 Then
                hdnMap.Value = dtMap.Rows(0).Item("maptype").ToString()
            End If
            Dim dt As New DataTable
            da.SelectCommand.CommandText = "Select  * from mmm_mst_elogbooksettings with(nolock) where IsActive=1 and  Eid=" & Session("Eid")
            da.Fill(dt)
            If dt.Rows.Count > 0 Then
                AssignSettingValues(dt)
                ' GetMainGridQuery(txtd1.Text, txtd2.Text)
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

    '<WebMethod()> _
    'Public Shared Function GetLogBook(sdate As String, tdate As String, UID As Integer, UROLE As String, Eid As Integer) As DataSet
    '    Dim ds As New DataSet()
    '    Dim ConStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
    '    Using con As New SqlConnection(ConStr)
    '        'Using da As New SqlDataAdapter(GetMainGridQuery(sdate, tdate, UID, UROLE, Eid), con)
    '        'da.Fill(ds)
    '        ' End Using
    '    End Using
    '    Return ds
    'End Function

    Protected Sub gv_OnRowCreated(sender As Object, e As GridViewRowEventArgs)

        If e.Row.RowType = DataControlRowType.DataRow Then
            If e.Row.RowState = DataControlRowState.Alternate Then
                e.Row.Attributes.Add("onmouseover", "this.style.backgroundColor='#CAFF70';")
                e.Row.Attributes.Add("onmouseout", "this.style.backgroundColor='#ffffff';")
            Else
                e.Row.Attributes.Add("onmouseover", "this.style.backgroundColor='#CAFF70';")
                e.Row.Attributes.Add("onmouseout", "this.style.backgroundColor='#ffffff';")
            End If

        End If

    End Sub

    'Public Shared Function CreateCollection(dt As DataTable) As BPMGraph
    '    Dim CatName As New List(Of [String])()
    '    Dim Lst As New List(Of Series)()
    '    Dim i As Integer = 0
    '    Dim objS As Series
    '    Dim objG As New BPMGraph()
    '    For Each column As DataColumn In dt.Columns
    '        Dim LstData As New List(Of [String])()
    '        objS = New Series()
    '        objS.name = column.ColumnName
    '        For j As Integer = 0 To dt.Rows.Count - 1
    '            If i = 0 Then
    '                CatName.Add(dt.Rows(j)(column.ColumnName).ToString())
    '            Else
    '                LstData.Add(dt.Rows(j)(column.ColumnName).ToString())
    '            End If
    '        Next
    '        If i > 0 Then
    '            objS.data = LstData
    '            Lst.Add(objS)
    '        End If
    '        i = i + 1
    '    Next
    '    objG.Category = CatName
    '    objG.Series = Lst
    '    objG.Type = "Log Book Graph"
    '    Return objG
    'End Function


    <WebMethod()> _
    Public Shared Function GetJSON(d1 As String, d2 As String) As pichartCol
        Dim ret As New pichartCol()
        Try
            Dim ds As New DataSet()
            Dim Query As String = ""
            HttpContext.Current.Session("Sdate") = d1
            HttpContext.Current.Session("edate") = d2

            If HttpContext.Current.Session("EID") = 58 Then
                If HttpContext.Current.Session("USERROLE").ToString.ToUpper = "SU" Or HttpContext.Current.Session("USERROLE").ToString.ToUpper = "CORPORATEUSER" Then

                    If d1 = "" And d2 = "" Then
                        Query = "select  dms.udf_split('MASTER-Vehicle Type-fld1',m.fld14)[Vehicle Type], (select count(distinct e.vehicle_no) from MMM_MST_MASTER m1 inner join  MMM_MST_ELOGBOOK e on e.vehicle_no = m1.fld1 where m1.eid =58 and e.eid=58 and convert(date, Trip_start_datetime) >=convert(date,getdate()-1) and convert(date,Trip_end_datetime)<= convert(date,getdate()-1) and  m1.fld14 = m.fld14"
                        Query &= " ) [Total Vehicle Count],(select count(m3.fld14) from mmm_mst_elogbook e with (nolock) inner join mmm_mst_master m3 with (nolock) on e.vehicle_no=m3.fld1 "
                        Query &= "where sitevisiteid<>0 and m3.fld14=m.fld14 and Convert(date,trip_start_datetime)>=convert(date,getdate()-1)  and Convert(date,trip_End_datetime)<=convert(date,getdate()-1) )[Total Site Visit Count],sum(convert(int,round(convert(numeric(10,4),isnull(e.total_distance,0)),0)))[Total Km.] from mmm_mst_elogbook e with (nolock) join "
                        Query &= "mmm_mst_master m with (nolock) on m.fld1=e.vehicle_no where m.eid=58 and e.eid=58 and Convert(date,trip_start_datetime)>=convert(date,getdate()-1) and Convert(date,trip_End_datetime)<=convert(date,getdate()-1)  group by m.fld14"
                    Else
                        Query &= "select  dms.udf_split('MASTER-Vehicle Type-fld1',m.fld14)[Vehicle Type],(select count(distinct e.vehicle_no) from MMM_MST_MASTER m1 inner join  MMM_MST_ELOGBOOK e on e.vehicle_no = m1.fld1 where m1.eid = 58 And e.eid = 58 And Convert( Date, Trip_start_datetime)>= ' " & d1 & "' And Convert( Date, Trip_end_datetime) <= '" & d2 & "' And m1.fld14 = m.fld14"
                        Query &= ") [Total Vehicle Count],(select count(m3.fld14) from mmm_mst_elogbook e with (nolock) inner join mmm_mst_master m3 with (nolock) on e.vehicle_no=m3.fld1 "
                        Query &= "where sitevisiteid<>0 and m3.fld14=m.fld14 and Convert(date,trip_start_datetime)>='" & d1 & "'  and Convert(date,trip_End_datetime)<='" & d2 & "')[Total Site Visit Count],sum(convert(int,round(convert(numeric(10,4),isnull(e.total_distance,0)),0)))[Total Km.] from mmm_mst_elogbook e with (nolock) join "
                        Query &= "mmm_mst_master m with (nolock) on m.fld1=e.vehicle_no where m.eid=58 and e.eid=58 and Convert(date,trip_start_datetime)>='" & d1 & "' and Convert(date,trip_End_datetime)<='" & d2 & "' and Convert(date,trip_start_datetime)>='" & d1 & "' and Convert(date,trip_End_datetime)<='" & d2 & "'  group by m.fld14"
                    End If

                Else

                    If d1 = "" And d2 = "" Then
                        Query &= "select  dms.udf_split('MASTER-Vehicle Type-fld1',m.fld14)[Vehicle Type],(select count(distinct e.vehicle_no) from MMM_MST_MASTER m1 inner join  MMM_MST_ELOGBOOK e on e.vehicle_no = m1.fld1 where m1.eid =" & HttpContext.Current.Session("EID") & " and e.eid=" & HttpContext.Current.Session("EID") & " and convert(date, Trip_start_datetime) >=convert(date,getdate()-1) and convert(date,Trip_end_datetime)<=convert(date,getdate()-1) and  m1.fld14 = m.fld14"
                        Query &= ") [Total Vehicle Count],(select count(m3.fld14) from mmm_mst_elogbook e with (nolock) inner join mmm_mst_master m3 with (nolock) on e.vehicle_no=m3.fld1 "
                        Query &= "where sitevisiteid<>0 and e.compID=(Select fld12 from mmm_mst_user where uid=" & HttpContext.Current.Session("UID") & " and eid=58) and m3.fld14=m.fld14 and Convert(date,trip_start_datetime)>=convert(date,getdate()-1)  and Convert(date,trip_End_datetime)<=convert(date,getdate()-1) )[Total Site Visit Count],sum(convert(int,round(convert(numeric(10,4),isnull(e.total_distance,0)),0)))[Total Km.] from mmm_mst_elogbook e with (nolock) join "
                        Query &= "mmm_mst_master m with (nolock) on m.fld1=e.vehicle_no where m.eid=58 and e.eid=58 and Convert(date,trip_start_datetime)>=convert(date,getdate()-1) and Convert(date,trip_End_datetime)<=convert(date,getdate()-1)  and e.CompID in (select items from dbo.split((select fld1 from mmm_ref_role_user with (nolock) where eid=" & HttpContext.Current.Session("EID") & " and uid=" & HttpContext.Current.Session("UID") & " and rolename='" & HttpContext.Current.Session("USERROLE") & "' ), ',')) and triptype<>'proxycard' group by m.fld14"
                    Else
                        Query &= "select  dms.udf_split('MASTER-Vehicle Type-fld1',m.fld14)[Vehicle Type],  (select count(distinct e.vehicle_no) from MMM_MST_MASTER m1 inner join  MMM_MST_ELOGBOOK e on e.vehicle_no = m1.fld1 where m1.eid =" & HttpContext.Current.Session("EID") & " and e.eid=" & HttpContext.Current.Session("EID") & " and convert(date, Trip_start_datetime) >='" & d1 & "' and convert(date,Trip_end_datetime)<='" & d2 & "' and  m1.fld14 = m.fld14"
                        Query &= ") [Total Vehicle Count],(select count(m3.fld14) from mmm_mst_elogbook e with (nolock) inner join mmm_mst_master m3 with (nolock) on e.vehicle_no=m3.fld1 "
                        Query &= "where sitevisiteid<>0 and e.compID=(Select fld12 from mmm_mst_user where uid=" & HttpContext.Current.Session("UID") & " and eid=58) and m3.fld14=m.fld14 and Convert(date,trip_start_datetime)>='" & d1 & "'  and Convert(date,trip_End_datetime)<='" & d2 & "' )[Total Site Visit Count],sum(convert(int,round(convert(numeric(10,4),isnull(e.total_distance,0)),0)))[Total Km.] from mmm_mst_elogbook e with (nolock) join "
                        Query &= "mmm_mst_master m with (nolock) on m.fld1=e.vehicle_no where m.eid=58 and e.eid=58  and Convert(date,trip_start_datetime)>='" & d1 & "' and Convert(date,trip_End_datetime)<='" & d2 & "'  and e.CompID in (select items from dbo.split((select fld1 from mmm_ref_role_user with (nolock) where eid=" & HttpContext.Current.Session("EID") & " and uid=" & HttpContext.Current.Session("UID") & " and rolename='" & HttpContext.Current.Session("USERROLE") & "' ), ','))  and triptype <> 'proxycard' group by m.fld14"
                    End If

                End If

                ''Added  by Pallavi  on 22 Feb 15(also changed queries for 58 & 54 eids
                'ElseIf HttpContext.Current.Session("EID") = 60 Then
                '    If HttpContext.Current.Session("USERROLE").ToString.ToUpper = "SU" Or HttpContext.Current.Session("USERROLE").ToString.ToUpper = "CORPORATEUSER" Then
                '        If d1 = "" And d2 = "" Then
                '            Query = "select  dms.udf_split('MASTER-Vehicle Type-fld1',m.fld14)[Vehicle Type],(select count(distinct e.vehicle_no) from MMM_MST_MASTER m1 inner join  MMM_MST_ELOGBOOK e on e.vehicle_no = m1.fld1 where m1.eid =" & HttpContext.Current.Session("EID") & " and e.eid=" & HttpContext.Current.Session("EID") & " and convert(date, Trip_start_datetime) >=convert(date,getdate()-1) and convert(date,Trip_end_datetime)<=convert(date,getdate()-1) and  m1.fld14 = m.fld14"
                '            Query &= ") [Total Vehicle Count],(select count(m3.fld14) from mmm_mst_elogbook e with (nolock) inner join mmm_mst_master m3 with (nolock) on e.vehicle_no=m3.fld1 where sitevisiteid<>0 and m3.fld14=m.fld14 and Convert(date,trip_start_datetime)>=convert(date,getdate()-1) and Convert(date,trip_End_datetime)<=convert(date,getdate()-1) )[Total Site Visit Count],sum(convert(int,round(convert(numeric(10,4),isnull(e.total_distance,0)),0)))[Total Km.] from mmm_mst_elogbook e with (nolock) join mmm_mst_master m with (nolock) on m.fld1=e.vehicle_no  where m.eid=" & HttpContext.Current.Session("Eid") & " and e.eid=" & HttpContext.Current.Session("Eid") & " and Convert(date,trip_start_datetime)>=convert(date,getdate()-1) and Convert(date,trip_End_datetime)<=convert(date,getdate()-1)  group by m.fld14"
                '        Else
                '            Query = "select  dms.udf_split('MASTER-Vehicle Type-fld1',m.fld14)[Vehicle Type],(select count(distinct e.vehicle_no) from MMM_MST_MASTER m1 inner join  MMM_MST_ELOGBOOK e on e.vehicle_no = m1.fld1 where m1.eid =" & HttpContext.Current.Session("EID") & " and e.eid=" & HttpContext.Current.Session("EID") & " and convert(date, Trip_start_datetime) >='" & d1 & "' and convert(date,Trip_end_datetime)<='" & d2 & "' and  m1.fld14 = m.fld14"
                '            Query &= ") [Total Vehicle Count],(select count(m3.fld14) from mmm_mst_elogbook e with (nolock) inner join mmm_mst_master m3 with (nolock) on e.vehicle_no=m3.fld1 where sitevisiteid<>0 and m3.fld14=m.fld14 and Convert(date,trip_start_datetime)>='" & d1 & "' and Convert(date,trip_End_datetime)<='" & d2 & "' )[Total Site Visit Count],sum(convert(int,round(convert(numeric(10,4),isnull(e.total_distance,0)),0)))[Total Km.] from mmm_mst_elogbook e with (nolock) join mmm_mst_master m with (nolock) on m.fld1=e.vehicle_no  where m.eid=" & HttpContext.Current.Session("Eid") & " and e.eid=" & HttpContext.Current.Session("Eid") & " and Convert(date,trip_start_datetime)>='" & d1 & "' and Convert(date,trip_End_datetime)<='" & d2 & "'   group by m.fld14"
                '        End If
                '    Else
                '        If d1 = "" And d2 = "" Then
                '            Query = "select  dms.udf_split('MASTER-Vehicle Type-fld1',m.fld14)[Vehicle Type],(select count(distinct e.vehicle_no) from MMM_MST_MASTER m1 inner join  MMM_MST_ELOGBOOK e on e.vehicle_no = m1.fld1 where m1.eid =" & HttpContext.Current.Session("EID") & " and e.eid=" & HttpContext.Current.Session("EID") & " and convert(date, Trip_start_datetime) >=convert(date,getdate()-1) and convert(date,Trip_end_datetime)<=convert(date,getdate()-1) and  m1.fld14 = m.fld14"
                '            Query &= ") [Total Vehicle Count],(select count(m3.fld14) from mmm_mst_elogbook e with (nolock) inner join mmm_mst_master m3 with (nolock) on e.vehicle_no=m3.fld1 where sitevisiteid<>0 and m3.fld14=m.fld14 and Convert(date,trip_start_datetime)>='" & d1 & "' and Convert(date,trip_End_datetime)<='" & d2 & "' )[Total Site Visit Count],sum(convert(int,round(convert(numeric(10,4),isnull(e.total_distance,0)),0)))[Total Km.] from mmm_mst_elogbook e with (nolock) join mmm_mst_master m with (nolock) on m.fld1=e.vehicle_no inner join  dbo.split((select fld1 from mmm_ref_role_user where uid=" & HttpContext.Current.Session("UID") & " and rolename='" & HttpContext.Current.Session("USERROLE") & "' and eid=" & HttpContext.Current.Session("EID") & "), ',') s on s.items in (select items from dbo.split(m.fld16, ','))  where m.eid=" & HttpContext.Current.Session("Eid") & " and e.eid=" & HttpContext.Current.Session("Eid") & " and Convert(date,trip_start_datetime)>='" & d1 & "' and Convert(date,trip_End_datetime)<='" & d2 & "' group by m.fld14"
                '        Else
                '            Query = "select  dms.udf_split('MASTER-Vehicle Type-fld1',m.fld14)[Vehicle Type],(select count(distinct e.vehicle_no) from MMM_MST_MASTER m1 inner join  MMM_MST_ELOGBOOK e on e.vehicle_no = m1.fld1 where m1.eid =" & HttpContext.Current.Session("EID") & " and e.eid=" & HttpContext.Current.Session("EID") & " and convert(date, Trip_start_datetime) >='" & d1 & "' and convert(date,Trip_end_datetime)<='" & d2 & "' and  m1.fld14 = m.fld14"
                '            Query &= ") [Total Vehicle Count],(select count(m3.fld14) from mmm_mst_elogbook e with (nolock) inner join mmm_mst_master m3 with (nolock) on e.vehicle_no=m3.fld1 where sitevisiteid<>0 and m3.fld14=m.fld14 and Convert(date,trip_start_datetime)>='" & d1 & "' and Convert(date,trip_End_datetime)<='" & d2 & "' )[Total Site Visit Count],sum(convert(int,round(convert(numeric(10,4),isnull(e.total_distance,0)),0)))[Total Km.] from mmm_mst_elogbook e with (nolock) join mmm_mst_master m with (nolock) on m.fld1=e.vehicle_no inner join  dbo.split((select fld1 from mmm_ref_role_user where uid=" & HttpContext.Current.Session("UID") & " and rolename='" & HttpContext.Current.Session("USERROLE") & "' and eid=" & HttpContext.Current.Session("EID") & "), ',') s on s.items in (select items from dbo.split(m.fld16, ','))  where m.eid=" & HttpContext.Current.Session("Eid") & " and e.eid=" & HttpContext.Current.Session("Eid") & " and Convert(date,trip_start_datetime)>='" & d1 & "' and Convert(date,trip_End_datetime)<='" & d2 & "'   group by m.fld14"
                '        End If
                '    End If

            Else

                If HttpContext.Current.Session("USERROLE").ToString.ToUpper = "SU" Or HttpContext.Current.Session("USERROLE").ToString.ToUpper = "CORPORATEUSER" Then
                    If d1 = "" And d2 = "" Then
                        Query = "select  dms.udf_split('MASTER-Vehicle Type-fld1',m.fld14)[Vehicle Type],(select count(distinct e.vehicle_no) from MMM_MST_MASTER m1 inner join  MMM_MST_ELOGBOOK e on e.vehicle_no = m1.fld1 where m1.eid =" & HttpContext.Current.Session("EID") & " and e.eid=" & HttpContext.Current.Session("EID") & " and convert(date, Trip_start_datetime) >=convert(date,getdate()-1) and convert(date,Trip_end_datetime)<=convert(date,getdate()-1) and  m1.fld14 = m.fld14"
                        Query &= ") [Total Vehicle Count],(select count(m3.fld14) from mmm_mst_elogbook e with (nolock) inner join mmm_mst_master m3 with (nolock) on e.vehicle_no=m3.fld1 where sitevisiteid<>0 and m3.fld14=m.fld14 and Convert(date,trip_start_datetime)>=convert(date,getdate()-1) and Convert(date,trip_End_datetime)<=convert(date,getdate()-1) )[Total Site Visit Count],sum(convert(int,round(convert(numeric(10,4),isnull(e.total_distance,0)),0)))[Total Km.] from mmm_mst_elogbook e with (nolock) join mmm_mst_master m with (nolock) on m.fld1=e.vehicle_no  where m.eid=" & HttpContext.Current.Session("Eid") & " and e.eid=" & HttpContext.Current.Session("Eid") & " and Convert(date,trip_start_datetime)>=convert(date,getdate()-1) and Convert(date,trip_End_datetime)<=convert(date,getdate()-1) and e.triptype <>'proxycard' group by m.fld14"
                    Else
                        Query = "select  dms.udf_split('MASTER-Vehicle Type-fld1',m.fld14)[Vehicle Type],(select count(distinct e.vehicle_no) from MMM_MST_MASTER m1 inner join  MMM_MST_ELOGBOOK e on e.vehicle_no = m1.fld1 where m1.eid =" & HttpContext.Current.Session("EID") & " and e.eid=" & HttpContext.Current.Session("EID") & " and convert(date, Trip_start_datetime) >='" & d1 & "' and convert(date,Trip_end_datetime)<='" & d2 & "' and  m1.fld14 = m.fld14"
                        Query &= ") [Total Vehicle Count],(select count(m3.fld14) from mmm_mst_elogbook e with (nolock) inner join mmm_mst_master m3 with (nolock) on e.vehicle_no=m3.fld1 where sitevisiteid<>0 and m3.fld14=m.fld14 and Convert(date,trip_start_datetime)>='" & d1 & "' and Convert(date,trip_End_datetime)<='" & d2 & "' )[Total Site Visit Count],sum(convert(int,round(convert(numeric(10,4),isnull(e.total_distance,0)),0)))[Total Km.] from mmm_mst_elogbook e with (nolock) join mmm_mst_master m with (nolock) on m.fld1=e.vehicle_no  where m.eid=" & HttpContext.Current.Session("Eid") & " and e.eid=" & HttpContext.Current.Session("Eid") & " and Convert(date,trip_start_datetime)>='" & d1 & "' and Convert(date,trip_End_datetime)<='" & d2 & "' and e.triptype <>'proxycard'  group by m.fld14"
                    End If
                Else
                    If d1 = "" And d2 = "" Then
                        Query = "select  dms.udf_split('MASTER-Vehicle Type-fld1',m.fld14)[Vehicle Type],(select count(distinct e.vehicle_no) from MMM_MST_MASTER m1 inner join  MMM_MST_ELOGBOOK e on e.vehicle_no = m1.fld1 where m1.eid =" & HttpContext.Current.Session("EID") & " and e.eid=" & HttpContext.Current.Session("EID") & " and convert(date, Trip_start_datetime) >=convert(date,getdate()-1) and convert(date,Trip_end_datetime)<=convert(date,getdate()-1) and  m1.fld14 = m.fld14"
                        Query &= ") [Total Vehicle Count],(select count(m3.fld14) from mmm_mst_elogbook e with (nolock) inner join mmm_mst_master m3 with (nolock) on e.vehicle_no=m3.fld1 where sitevisiteid<>0 and m3.fld14=m.fld14 and Convert(date,trip_start_datetime)>=convert(date,getdate()-1) and Convert(date,trip_End_datetime)<=convert(date,getdate()-1) )[Total Site Visit Count],sum(convert(int,round(convert(numeric(10,4),isnull(e.total_distance,0)),0)))[Total Km.] from mmm_mst_elogbook e with (nolock) join mmm_mst_master m with (nolock) on m.fld1=e.vehicle_no inner join  dbo.split((select fld1 from mmm_ref_role_user where uid=" & HttpContext.Current.Session("UID") & " and rolename='" & HttpContext.Current.Session("USERROLE") & "' and eid=" & HttpContext.Current.Session("EID") & "), ',') s on s.items in (select items from dbo.split(m.fld16, ','))  where m.eid=" & HttpContext.Current.Session("Eid") & " and e.eid=" & HttpContext.Current.Session("Eid") & " and Convert(date,trip_start_datetime)>=convert(date,getdate()-1) and Convert(date,trip_End_datetime)<=convert(date,getdate()-1) and e.triptype <>'proxycard' group by m.fld14"
                    Else
                        Query = "select  dms.udf_split('MASTER-Vehicle Type-fld1',m.fld14)[Vehicle Type],(select count(distinct e.vehicle_no) from MMM_MST_MASTER m1 inner join  MMM_MST_ELOGBOOK e on e.vehicle_no = m1.fld1 where m1.eid =" & HttpContext.Current.Session("EID") & " and e.eid=" & HttpContext.Current.Session("EID") & " and convert(date, Trip_start_datetime) >='" & d1 & "' and convert(date,Trip_end_datetime)<='" & d2 & "' and  m1.fld14 = m.fld14"
                        Query &= ") [Total Vehicle Count],(select count(m3.fld14) from mmm_mst_elogbook e with (nolock) inner join mmm_mst_master m3 with (nolock) on e.vehicle_no=m3.fld1 where sitevisiteid<>0 and m3.fld14=m.fld14 and Convert(date,trip_start_datetime)>='" & d1 & "' and Convert(date,trip_End_datetime)<='" & d2 & "' )[Total Site Visit Count],sum(convert(int,round(convert(numeric(10,4),isnull(e.total_distance,0)),0)))[Total Km.] from mmm_mst_elogbook e with (nolock) join mmm_mst_master m with (nolock) on m.fld1=e.vehicle_no inner join  dbo.split((select fld1 from mmm_ref_role_user where uid=" & HttpContext.Current.Session("UID") & " and rolename='" & HttpContext.Current.Session("USERROLE") & "' and eid=" & HttpContext.Current.Session("EID") & "), ',') s on s.items in (select items from dbo.split(m.fld16, ','))  where m.eid=" & HttpContext.Current.Session("Eid") & " and e.eid=" & HttpContext.Current.Session("Eid") & " and Convert(date,trip_start_datetime)>='" & d1 & "' and Convert(date,trip_End_datetime)<='" & d2 & "' and e.triptype <>'proxycard'  group by m.fld14"
                    End If
                End If
            End If


            Dim ConStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
            Using con As New SqlConnection(ConStr)
                Using da As New SqlDataAdapter(Query, con)
                    da.Fill(ds)
                End Using
            End Using
            Try
                If ds.Tables(0).Rows.Count > 0 Then
                    'For Each column As DataColumn In ds.Tables(0).Columns
                    'Next
                    Dim obj As pichart
                    Dim o1 As New List(Of pichart)()
                    Dim o2 As New List(Of pichart)()
                    Dim o3 As New List(Of pichart)()
                    For i As Integer = 0 To ds.Tables(0).Rows.Count - 1
                        obj = New pichart()
                        obj.category = ds.Tables(0).Rows(i).Item("Vehicle Type")
                        obj.value = ds.Tables(0).Rows(i).Item("Total Vehicle Count")
                        If (ElogbookSettings.VehicleChartDisplay <> "0") Then
                            obj.Isvisible = True
                            obj.DisplayText = ElogbookSettings.VehicleChartDisplay
                        Else : obj.Isvisible = False
                        End If


                        o1.Add(obj)
                        obj = New pichart()
                        obj.category = ds.Tables(0).Rows(i).Item("Vehicle Type")
                        obj.value = ds.Tables(0).Rows(i).Item("Total Site Visit Count")
                        If (ElogbookSettings.SiteChartDisplay <> "0") Then
                            obj.Isvisible = True
                            obj.DisplayText = ElogbookSettings.SiteChartDisplay
                        Else : obj.Isvisible = False
                        End If

                        o2.Add(obj)
                        obj = New pichart()
                        obj.category = ds.Tables(0).Rows(i).Item("Vehicle Type")
                        obj.value = ds.Tables(0).Rows(i).Item("Total Km.")
                        If (ElogbookSettings.KmsChartDisplay <> "0") Then
                            obj.Isvisible = True
                            obj.DisplayText = ElogbookSettings.KmsChartDisplay
                        Else : obj.Isvisible = False
                        End If


                        o3.Add(obj)
                    Next
                    ret.data1 = o1
                    ret.data2 = o2
                    ret.data3 = o3

                End If
            Catch Ex As Exception
                Throw
            End Try
        Catch ex As Exception

        End Try
        Return ret
    End Function


    Protected Sub btnSearch_Click(sender As Object, e As EventArgs) Handles btnSearch.Click

        '  bindGrid()

    End Sub

    'Public Overloads Overrides Sub VerifyRenderingInServerForm(ByVal control As Control)
    'End Sub

    <WebMethod()> _
    Public Function GetlogBookJSON(d1 As String, d2 As String) As String
        Dim ds As New DataSet()
        Dim ret = ""

        Try
            If ds.Tables(0).Rows.Count > 0 Then
                For j As Integer = 0 To ds.Tables(0).Columns.Count - 1
                    ds.Tables(0).Columns(j).ColumnName = ds.Tables(0).Columns(j).ColumnName.Replace(" ", "_")
                Next
            End If
            Dim serializerSettings As New JsonSerializerSettings()
            Dim json_serializer As New JavaScriptSerializer()
            serializerSettings.Converters.Add(New DataTableConverter())
            Dim jsonData As [String] = JsonConvert.SerializeObject(ds.Tables(0), Formatting.None, serializerSettings)
            ret = jsonData
        Catch Ex As Exception
            Throw
        End Try

        Return ret
    End Function

    'Public Overloads Overrides Sub VerifyRenderingInServerForm(ByVal control As Control)
    'End Sub

    'Protected Sub btnExcelExport_Click(sender As Object, e As System.Web.UI.ImageClickEventArgs)
    '    Dim ds1 As New DataSet()
    '    ds1 = bindGrid()
    '    Dim grdTripData As New GridView
    '    grdTripData.DataSource = ds1
    '    grdTripData.DataBind()
    '    grdTripData.ShowHeader = True
    '    grdTripData.RowHeaderColumn = True
    '    grdTripData.AllowPaging = False
    '    Response.Clear()
    '    Response.Buffer = True
    '    Response.Write("<br/><div align=""center"" style=""border:1px solid red"" ><h3>VEHICLE LOG BOOK (Electronic)</h3></div> <br/>")
    '    Response.AddHeader("content-disposition", "attachment;filename=Trip Report.xls")
    '    Response.Charset = ""
    '    Response.ContentType = "application/vnd.ms-excel"
    '    Dim sw As New StringWriter()
    '    Dim hw As New HtmlTextWriter(sw)
    '    For i As Integer = 0 To grdTripData.Rows.Count - 1
    '        'Apply text style to each Row 
    '        grdTripData.Rows(i).Attributes.Add("class", "textmode")
    '    Next
    '    grdTripData.RenderControl(hw)
    '    'style to format numbers to string 
    '    Dim style As String = "<style> .textmode{mso-number-format:\@;}</style>"
    '    Response.Write(style)
    '    Response.Output.Write(sw.ToString())
    '    Response.Flush()
    '    Response.End()
    'End Sub


    '<WebMethod()> _
    'Public Shared Function FillUserDetails(ByVal vehno As String, ByVal start As String, ByVal enddate As String) As String
    '    Dim ds As New DataSet()
    '    Dim ret = ""
    '    Dim ConStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
    '    Using con As New SqlConnection(ConStr)
    '        Using da As New SqlDataAdapter("getLogBookDetailsP", con)
    '            da.SelectCommand.CommandType = CommandType.StoredProcedure
    '            da.SelectCommand.Parameters.AddWithValue("@eid", HttpContext.Current.Session("Eid"))
    '            da.SelectCommand.Parameters.AddWithValue("@vehno", vehno)
    '            da.SelectCommand.Parameters.AddWithValue("@d1", start)
    '            da.SelectCommand.Parameters.AddWithValue("@d2", enddate)
    '            da.SelectCommand.CommandTimeout = 120
    '            da.Fill(ds)
    '            If ds.Tables(0).Rows.Count > 0 Then
    '                For j As Integer = 0 To ds.Tables(0).Columns.Count - 1
    '                    ds.Tables(0).Columns(j).ColumnName = ds.Tables(0).Columns(j).ColumnName.Replace(" ", "_")
    '                Next
    '            End If
    '            Dim serializerSettings As New JsonSerializerSettings()
    '            Dim json_serializer As New JavaScriptSerializer()
    '            serializerSettings.Converters.Add(New DataTableConverter())
    '            Dim jsonData As [String] = JsonConvert.SerializeObject(ds.Tables(0), Formatting.None, serializerSettings)
    '            ret = jsonData
    '        End Using
    '    End Using
    '    Return ret
    'End Function

    Sub AssignSettingValues(dt As DataTable)
        Try

            If (dt.Select("SettingType ='Chart' AND SettingName='VehicleChart'").Length > 0) Then
                ElogbookSettings.VehicleChartDisplay = (dt.Select("SettingType ='Chart' AND SettingName='VehicleChart'"))(0).Item("DisplayText")
            End If
            If (dt.Select("SettingType ='Chart' AND SettingName='SiteChart'").Length > 0) Then
                ElogbookSettings.SiteChartDisplay = (dt.Select("SettingType ='Chart' AND SettingName='SiteChart'"))(0).Item("DisplayText")
            End If
            If (dt.Select("SettingType ='Chart' AND SettingName='KmsChart'").Length > 0) Then
                ElogbookSettings.KmsChartDisplay = (dt.Select("SettingType ='Chart' AND SettingName='KmsChart'"))(0).Item("DisplayText")
            End If
            Dim VehicleDoc = (dt.Select("SettingType ='MainGrid' AND SettingName='VehicleDoc'"))(0)
            ElogbookSettings.VehicleDoc = VehicleDoc.Item("Value")
            Dim SiteDoc = (dt.Select("SettingType ='Details' AND SettingName='SiteDoc'"))
            If (SiteDoc.Length > 0) Then
                ElogbookSettings.SiteDoc = SiteDoc(0).Item("Value")
                Dim SiteFromToOn = (dt.Select("SettingType ='Details' AND SettingName='SiteFromToOn'"))(0)
                If (SiteFromToOn.Item("Value") = "1") Then
                    ElogbookSettings.SiteFromToOn = True
                Else
                    ElogbookSettings.SiteFromToOn = False
                End If
            End If
            Dim DetailsOn = (dt.Select("SettingType ='MainGrid' AND SettingName='DetailSectionOn'"))(0)
            If (DetailsOn.Item("Value") = "1") Then
                ElogbookSettings.DetailsSectionOn = True
                ElogbookSettings.ElogbookdetFlds = dt.Select("SettingType ='Details' AND (SettingName='ElogbookFlds' or SettingName='VehicleFlds' or SettingName='SiteFlds')", " Sequence ASC")

            Else
                ElogbookSettings.DetailsSectionOn = False
            End If
            ElogbookSettings.ElogbookFlds = dt.Select("SettingType ='MainGrid' AND SettingName='ElogbookFlds'", "  Sequence ASC")
            Dim VehIMEI = dt.Select("SettingType ='MainGrid' AND SettingName='VehicleIMEIFld'")

            If (VehIMEI.Length > 0) Then
                ElogbookSettings.VehIMEIMapping = VehIMEI(0).Item("Value")
                ElogbookSettings.VehIMEIFieldNm = VehIMEI(0).Item("DisplayText")
            End If

        Catch ex As Exception

        End Try
    End Sub

    <WebMethod()> _
    Public Shared Function GetMainGridQuery(sdate As String, tdate As String, UID As Integer, UROLE As String, Eid As Integer) As Grid

        Dim objgrid = New Grid()

        Try
            Dim ds = New DataSet()
            Dim Columns As New List(Of GridCol)
            Dim Query As String = "Select "
            Dim Groupby As String = "group by e.vehicle_no, convert(varchar,e.Trip_Start_Datetime,103) , convert(varchar,e.Trip_end_Datetime, 103), "
            If (sdate = "" And tdate = "") Then
                sdate = DateTime.Now.Date.AddDays(-1).ToString("yyyy-MM-dd")
                tdate = DateTime.Now.Date.AddDays(-1).ToString("yyyy-MM-dd")
            End If

            For Each row As DataRow In ElogbookSettings.ElogbookFlds

                If (row.Item("Field").ToString().Contains("Total_Distance")) Then
                    Query &= " isnull( convert(varchar," & row.Item("Value").ToString() & "),'0')  " & "[" & row.Item("DisplayText").ToString() & "] ,"
                Else
                    If Not ((row.Item("Field").ToString().Contains("e.vehicle_no")) Or (row.Item("Field").ToString().Contains("Trip_Start_DateTime")) Or (row.Item("Field").ToString().Contains("Trip_end_DateTime"))) Then
                        Groupby &= row.Item("Value").ToString() + ","
                    End If
                    Query &= " isnull(" & row.Item("Value").ToString() & ",'NA')  " & "[" & row.Item("DisplayText").ToString() & "] ,"
                End If

            Next

            Query = Query.TrimEnd(",", "").TrimEnd(" ", String.Empty)
            Groupby = Groupby.TrimEnd(",", "").TrimEnd(" ", String.Empty)
            If (Eid = "58") Then
                If (UROLE = "SU") Then

                    Query &= " from mmm_mst_elogbook e inner join mmm_mst_master Vehicle on e.IMEI_No = Vehicle." & ElogbookSettings.VehIMEIMapping & " where Vehicle.eid= " & Eid & " and e.eid=" & Eid & "  and Convert(date,trip_start_datetime)>=convert(date,'" & sdate & "') " &
      " and Convert(date,trip_End_datetime)<=convert(date,'" & tdate & "')and triptype not in ('Night AutoTrip','proxycard')  "

                Else

                    Query &= " from mmm_mst_elogbook e inner join mmm_mst_master Vehicle on e.IMEI_No = Vehicle." & ElogbookSettings.VehIMEIMapping & " where Vehicle.eid= " & Eid & " and  e.eid=" & Eid & "  and Convert(date,trip_start_datetime)>=convert(date,'" & sdate & "') " &
      " and Convert(date,trip_End_datetime)<=convert(date,'" & tdate & "')and triptype not in ('Night AutoTrip','proxycard') " &
       "and CompID in (select items from dbo.split((select fld1 from mmm_ref_role_user with (nolock) where eid=" & Eid & "  and uid=" & UID & " and rolename='" & UROLE & "'), ',')) "

                End If
            ElseIf (Eid = "66") Then
                If (UROLE = "SU") Then


                    Query &= " from mmm_mst_elogbook e inner join mmm_mst_master Vehicle on e.IMEI_No = Vehicle." & ElogbookSettings.VehIMEIMapping & " where  Vehicle.eid= " & Eid & " and e.eid=" & Eid & " and Convert(date,trip_start_datetime)>=convert(date,'" & sdate & "') " &
      " and Convert(date,trip_End_datetime)<=convert(date,'" & tdate & "')and triptype not in ('Night AutoTrip','proxycard') "

                Else

                    Query &= " from mmm_mst_elogbook e inner join mmm_mst_master Vehicle on e.IMEI_No = Vehicle." & ElogbookSettings.VehIMEIMapping & " inner join   dbo.split((select fld4 from mmm_ref_role_user with (nolock) where eid=" & Eid & " and uid=" & UID &
                        " and rolename='" & UROLE & "'), ',') s on s.items in (select items from dbo.split(Vehicle.fld11, ','))   where Vehicle.eid= " & Eid & " and e.eid=" &
                        Eid & " and Convert(date,trip_start_datetime)>=convert(date,'" & sdate & "') " &
      " and Convert(date,trip_End_datetime)<=convert(date,'" & tdate & "')and triptype not in ('Night AutoTrip','proxycard')"
                End If
            Else

                If (UROLE = "SU") Then

                    Query &= " from mmm_mst_elogbook e inner join mmm_mst_master Vehicle on e.IMEI_No = Vehicle." & ElogbookSettings.VehIMEIMapping & " where  Vehicle.eid= " & Eid & " and e.eid=" & Eid & " and Convert(date,trip_start_datetime)>=convert(date,'" & sdate & "') " &
      " and Convert(date,trip_End_datetime)<=convert(date,'" & tdate & "')and triptype not in ('Night AutoTrip','proxycard') "

                Else

                    Query &= " from mmm_mst_elogbook e inner join mmm_mst_master Vehicle on e.IMEI_No = Vehicle." & ElogbookSettings.VehIMEIMapping & " inner join   dbo.split((select fld1 from mmm_ref_role_user with (nolock) where eid=" & Eid & " and uid=" & UID &
                        " and rolename='" & UROLE & "'), ',') s on s.items in (select items from dbo.split(Vehicle.fld16, ','))   where Vehicle.eid= " & Eid & " and e.eid=" &
                        Eid & " and Convert(date,trip_start_datetime)>=convert(date,'" & sdate & "') " &
      " and Convert(date,trip_End_datetime)<=convert(date,'" & tdate & "')and triptype not in ('Night AutoTrip','proxycard')"

                End If

            End If
            Dim ConStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
            Using con As New SqlConnection(ConStr)
                Using da As New SqlDataAdapter(Query + Groupby, con)
                    da.Fill(ds)
                End Using
            End Using

            Try
                For j As Integer = 0 To ds.Tables(0).Columns.Count - 1
                    Dim objgrodcol As New GridCol()
                    Dim colnm = ds.Tables(0).Columns(j).ColumnName.Replace(" ", "_").Replace(".", "").ToString()
                    objgrodcol.field = colnm
                    objgrodcol.title = ds.Tables(0).Columns(j).ColumnName
                    Columns.Add(objgrodcol)
                    ds.Tables(0).Columns(j).ColumnName = colnm
                Next

                Dim serializerSettings As New JsonSerializerSettings()
                Dim json_serializer As New JavaScriptSerializer()
                serializerSettings.Converters.Add(New DataTableConverter())
                Dim jsonData As String = JsonConvert.SerializeObject(ds.Tables(0), Newtonsoft.Json.Formatting.None, serializerSettings)
                objgrid.GridData = jsonData
                objgrid.Columns = Columns
                objgrid.IsDetailsOn = ElogbookSettings.DetailsSectionOn

            Catch Ex As Exception
                Throw
            End Try

        Catch ex As Exception

        End Try
        Return objgrid

    End Function

    <WebMethod()> _
    Public Shared Function GetDetailGridQuery(sdate As String, tdate As String, UID As Integer, UROLE As String, Eid As Integer, IMEI As String) As Grid
        Dim objgrid = New Grid()
        Try
            Dim ds = New DataSet()
            Dim Columns As New List(Of GridCol)
            Dim Query As String = "Select "
            If (ElogbookSettings.SiteFromToOn = False) Then
                If (sdate = "" And tdate = "") Then
                    sdate = DateTime.Now.Date.AddDays(-1).ToString("yyyy-MM-dd")
                    tdate = DateTime.Now.Date.AddDays(-1).ToString("yyyy-MM-dd")
                End If

                For Each row As DataRow In ElogbookSettings.ElogbookdetFlds
                    If Not (row.Item("Field").ToString().Contains("SiteVstFID") Or row.Item("Field").ToString().Contains("SiteVstTID")) Then
                        If (row.Item("Field").ToString().ToUpper().Contains("TRIP_START_DATETIME") Or row.Item("Field").ToString().ToUpper().Contains("TRIP_END_DATETIME")) Then
                            Query &= " isnull( convert(varchar," & row.Item("Value").ToString() & "),'')  " & "[" & row.Item("DisplayText").ToString() & "] ,"
                        Else
                            Query &= " isnull(convert(varchar," & row.Item("Value").ToString() & "),'NA')  " & "[" & row.Item("DisplayText").ToString() & "] ,"
                        End If
                    End If
                Next

                Query = Query.Remove(Query.LastIndexOf(","))

                If (Eid = "58") Then

                    If (UROLE = "SU") Then

                        Query &= " from mmm_mst_elogbook e inner join mmm_mst_master Vehicle on e.IMEI_No =Vehicle." & ElogbookSettings.VehIMEIMapping &
         " left outer join mmm_mst_master [Site] on e.SiteVisiteId = [Site].tid  where Vehicle.eid= " & Eid & " and e.eid =" & Eid & " and Convert(date,e.trip_start_datetime)>=convert(date, convert(datetime,'" & sdate & "',103)) " &
            " and Convert(date,e.trip_End_datetime)<=convert(date, convert(datetime,'" & tdate & "',103)) and e.triptype not in ('Night AutoTrip','proxycard') " &
         " and Vehicle.eid = " & Eid & "  and Vehicle.Documenttype ='" & ElogbookSettings.VehicleDoc & "' "


                    Else

                        Query &= " from mmm_mst_elogbook e  inner join mmm_mst_master Vehicle on e.IMEI_No =Vehicle." & ElogbookSettings.VehIMEIMapping &
                            "left outer join mmm_mst_master [Site] on e.SiteVisiteId = [Site].tid  where Vehicle.eid= " & Eid & " and e.eid=" &
                            Eid & "  and Convert(date,e.trip_start_datetime)>=convert(date, convert(datetime,'" & sdate & "',103)) " &
            " and Convert(date,trip_End_datetime)<=convert(date, convert(datetime,'" & tdate & "',103)) and e.triptype not in ('Night AutoTrip','proxycard') " &
            "and Vehicle.Documenttype ='" & ElogbookSettings.VehicleDoc & "' "


                    End If

                Else

                    If (UROLE = "SU") Then

                        Query &= " from mmm_mst_elogbook e  inner join mmm_mst_master Vehicle on e.IMEI_No =Vehicle." & ElogbookSettings.VehIMEIMapping &
         " left outer join mmm_mst_master [Site] on e.SiteVisiteId = [Site].tid where  e.eid=" & Eid & " and Convert(date,e.trip_start_datetime)>=convert(date, convert(datetime,'" & sdate & "',103)) " &
            " and Convert(date,e.trip_End_datetime)<=convert(date, convert(datetime,'" & tdate & "',103)) and triptype not in ('Night AutoTrip','proxycard') " &
             " and Vehicle.eid = " & Eid & "  and Vehicle.Documenttype ='" & ElogbookSettings.VehicleDoc & "' "

                    Else

                        Query &= " from mmm_mst_elogbook e inner join mmm_mst_master Vehicle on e.IMEI_No =Vehicle." & ElogbookSettings.VehIMEIMapping &
         " left outer join mmm_mst_master [Site] on e.SiteVisiteId = [Site].tid " &
         "  where e.eid=" & Eid & " and Convert(date,trip_start_datetime)>=convert(date, convert(datetime,'" & sdate & "',103)) " &
            " and Convert(date,trip_End_datetime)<=convert(date, convert(datetime,'" & tdate & "',103)) and triptype not in ('Night AutoTrip','proxycard')" &
             " and Vehicle.eid = " & Eid & "  and Vehicle.Documenttype ='" & ElogbookSettings.VehicleDoc & "' "

                    End If

                End If
                Query &= " and e.IMEI_No = '" & IMEI & "'"


            Else ''For From To 

                If (sdate = "" And tdate = "") Then
                    sdate = DateTime.Now.Date.AddDays(-1).ToString("yyyy-MM-dd")
                    tdate = DateTime.Now.Date.AddDays(-1).ToString("yyyy-MM-dd")
                End If

                For Each row As DataRow In ElogbookSettings.ElogbookdetFlds

                    If Not (row.Item("Field").ToString().ToUpper().Contains("SITEVISITEID")) Then

                        If (row.Item("SettingName").ToString().ToUpper().Contains("SITEFLDS")) Then

                            Query &= " isnull(convert(varchar," & row.Item("Value").ToString() & "),'NA')  " & "[" & row.Item("DisplayText").ToString() & " From] ,"
                            Query &= " isnull(convert(varchar," & row.Item("Value").ToString().Replace("Site.", "Site2.") & "),'NA')  " & "[" & row.Item("DisplayText").ToString() & " To] ,"

                        ElseIf (row.Item("Field").ToString().ToUpper().Contains("TRIP_START_DATETIME") Or row.Item("Field").ToString().ToUpper().Contains("TRIP_END_DATETIME")) Then
                            Query &= " isnull(convert(varchar," & row.Item("Value").ToString() & "),'')  " & "[" & row.Item("DisplayText").ToString() & "] ,"

                        Else : Query &= " isnull(convert(varchar," & row.Item("Value").ToString() & "),'NA')  " & "[" & row.Item("DisplayText").ToString() & "] ,"

                        End If

                    End If

                Next

                Query = Query.Remove(Query.LastIndexOf(","))

                If (Eid = "58") Then

                    If (UROLE = "SU") Then

                        Query &= " from mmm_mst_elogbook e inner join mmm_mst_master Vehicle on e.IMEI_No =Vehicle." & ElogbookSettings.VehIMEIMapping &
         " left outer join mmm_mst_master [Site] on e.SiteVstFId = [Site].tid left outer join mmm_mst_master [Site2] on e.SiteVstTId = [Site2].tid where e.eid =" & Eid & " and Convert(date,e.trip_start_datetime)>=convert(date, convert(datetime,'" & sdate & "',103)) " &
            "  and Convert(date,trip_End_datetime)<=convert(date, convert(datetime,'" & tdate & "',103)) " &
         " and e.triptype not in ('Night AutoTrip','proxycard') and Vehicle.eid = " & Eid & "  and Vehicle.Documenttype ='" & ElogbookSettings.VehicleDoc & "' "

                    Else

                        Query &= " from mmm_mst_elogbook e inner join mmm_mst_master Vehicle on e.IMEI_No =Vehicle. " & ElogbookSettings.VehIMEIMapping &
         " left outer join mmm_mst_master [Site] on e.SiteVstFId = [Site].tid left outer join mmm_mst_master [Site2] on e.SiteVstTId = [Site2].tid where e.eid=" & Eid &
         "  and Convert(date,e.trip_start_datetime)>=convert(date, convert(datetime,'" & sdate & "',103)) " &
            "  and Convert(date,trip_End_datetime)<=convert(date, convert(datetime,'" & tdate & "',103)) and e.triptype not in ('Night AutoTrip','proxycard') " &
            "and CompID in (select items from dbo.split((select fld1 from mmm_ref_role_user with (nolock) where eid=" & Eid & "  and uid=" & UID & " and rolename='" & UROLE & "'), ',')) "

                    End If

                Else

                    If (UROLE = "SU") Then

                        Query &= " from mmm_mst_elogbook e inner join mmm_mst_master Vehicle on e.IMEI_No =Vehicle." & ElogbookSettings.VehIMEIMapping &
         " left outer join mmm_mst_master [Site] on e.SiteVstFId = [Site].tid left outer join mmm_mst_master [Site2] on e.SiteVstTId = [Site2].tid " &
         " where e.eid=" & Eid & " and Convert(date,trip_start_datetime)>=convert(date, convert(datetime,'" & sdate & "',103)) " &
            " and Convert(date,trip_End_datetime)<=convert(date, convert(datetime,'" & tdate & "',103)) and triptype not in ('Night AutoTrip','proxycard') "

                    Else

                        Query &= " from mmm_mst_elogbook e inner join   dbo.split((select fld1 from mmm_ref_role_user with (nolock) where e.eid=" & Eid & " and uid=" & UID & " and rolename='" & UROLE & "'), ',')" &
                            " s on s.items in (select items from dbo.split(m.fld16, ',')) left outer join mmm_mst_master [Site] on e.SiteVstFId = [Site].tid left outer join mmm_mst_master [Site2] on e.SiteVstTId = [Site2].tid " &
                            "where eid=" & Eid & " and Convert(date,trip_start_datetime)>=convert(date, convert(datetime,'" & sdate & "',103)) " &
            " and Convert(date,trip_End_datetime)<=convert(date, convert(datetime,'" & tdate & "',103)) and triptype not in ('Night AutoTrip','proxycard')"

                    End If

                End If

                Query &= " and e.IMEI_no ='" & IMEI & "'"

            End If

            Dim ConStr As String = ConfigurationManager.ConnectionStrings("conStr").ConnectionString
            Using con As New SqlConnection(ConStr)
                Using da As New SqlDataAdapter(Query, con)
                    da.Fill(ds)
                End Using
            End Using

            Try
                For j As Integer = 0 To ds.Tables(0).Columns.Count - 1
                    Dim objgrodcol As New GridCol()
                    Dim colnm = ds.Tables(0).Columns(j).ColumnName.Replace(" ", "_").Replace(".", "").ToString()
                    objgrodcol.field = colnm
                    objgrodcol.title = ds.Tables(0).Columns(j).ColumnName
                    Columns.Add(objgrodcol)
                    ds.Tables(0).Columns(j).ColumnName = colnm
                Next

                Dim serializerSettings As New JsonSerializerSettings()
                Dim json_serializer As New JavaScriptSerializer()
                serializerSettings.Converters.Add(New DataTableConverter())
                Dim jsonData As String = JsonConvert.SerializeObject(ds.Tables(0), Newtonsoft.Json.Formatting.None, serializerSettings)
                objgrid.GridData = jsonData
                objgrid.Columns = Columns
                objgrid.IsDetailsOn = ElogbookSettings.DetailsSectionOn

            Catch Ex As Exception
                Throw
            End Try

        Catch ex As Exception

        End Try
        Return objgrid


    End Function

End Class
Public Class pichart
    Public category As String
    Public value As String
    Public Isvisible As Boolean
    Public DisplayText As String
    'Public color As String
End Class
Public Class pichartCol
    Public data1 As New List(Of pichart)()
    Public data2 As New List(Of pichart)()
    Public data3 As New List(Of pichart)()
End Class
Public Class Grid
    Public GridData As String
    Public Columns As List(Of GridCol)
    Public IsDetailsOn As Boolean
End Class
Public Class GridCol
    Public field As String
    Public title As String
    Public command As String
End Class
Public Class ElogbookSettings
    Public Shared VehicleChartDisplay As String = "0"
    Public Shared SiteChartDisplay As String = "0"
    Public Shared KmsChartDisplay As String = "0"
    Public Shared DetailsSectionOn As Boolean = False
    Public Shared SiteFromToOn As Boolean = False
    Public Shared VehicleDoc As String = "0"
    Public Shared SiteDoc As String = "0"
    Public Shared ElogbookFlds As DataRow()
    Public Shared ElogbookdetFlds As DataRow()
    Public Shared VehIMEIMapping As String = ""
    Public Shared VehIMEIFieldNm As String = ""
End Class
